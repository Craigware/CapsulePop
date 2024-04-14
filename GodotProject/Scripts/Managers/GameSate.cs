using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;
using Critter;
using Godot;
using Microsoft.VisualBasic;

public enum GameStates {
    Start,
    TeamBuilding,
    Battle,
    End
}

public partial class GameSate : Node3D
{
    [Signal] public delegate void GameStateChangeEventHandler();

    private PackedScene playerScene = GD.Load<PackedScene>("res://Scenes/Components/Player.tscn");

    private GameStates CurrentState = GameStates.Start;
    private Godot.Collections.Array<Player.Player> players = new();
    private Godot.Collections.Array<Player.Player> spectators = new();

    public int ScoreToWin { get; } = 4;

    private int CapsulesPerTB;
    private int CapsulesCollected;
    
    public Control PlayersContainer;

    private MultiplayerApi multiplayer;

    public override void _Ready() {
        multiplayer = GetTree().GetMultiplayer();
        GD.Print("Game state initialized.");
        PlayersContainer = GetNode<Control>("/root/Node3D/Players");
        Button startButton = GetNode<Button>("/root/Node3D/Debug/Button4");

        startButton.ButtonDown += () => { Rpc(nameof(StartGame)); };    
    }

    public Godot.Collections.Dictionary<string, Variant> ToDict(){
        Godot.Collections.Dictionary<string, Variant> gameState = new();
        Godot.Collections.Array<Godot.Collections.Dictionary<string, Variant>> _players = new();
        
        foreach (var player in players) {
           _players.Add(player.ToDict());
        }

        gameState["CurrentState"] = (int) CurrentState;
        gameState["players"] = _players;
        gameState["CapsulesCollected"] = CapsulesCollected;

        return gameState;
    } 

    public void ConnectPlayer(long id) {
        Rpc(nameof(SpawnPlayer), id);

        var args = ToDict();
        RpcId(id, nameof(SyncGamestate), args);
        GD.Print("!!! PLAYER CONNTECTED !!!" + id);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void SyncGamestate(Godot.Collections.Dictionary<string, Variant> gameState) {  
        var _players = gameState["players"].As<Godot.Collections.Array<Godot.Collections.Dictionary<string, Variant>>>(); 

        //* Spawn the existing players
        for (int i = 0; i < _players.Count - 1; i++) {
            var player = playerScene.Instantiate<Player.Player>();
            player.Name = _players[i]["PlayerID"].As<long>().ToString(); 
            player.PlayerID = _players[i]["PlayerID"].As<long>();
            
            PlayersContainer.AddChild(player);
            player.cursor.Position = _players[i]["CursorPosition"].As<Vector2>();
        }

        CurrentState = gameState["CurrentState"].As<GameStates>();
        CapsulesCollected = gameState["CapsulesCollected"].As<int>();
    }

    public void DisconnectPlayer(long id) {
        Rpc(nameof(FreePlayer), id);
        GD.Print("!!! PLAYER DISCONNECTED !!! " + id);
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    public void SpawnPlayer(long playerID) {
        var newPlayer = playerScene.Instantiate<Player.Player>();
        newPlayer.PlayerID = playerID;
        newPlayer.Name = playerID.ToString();
        GD.Print(GetTree().GetMultiplayer().GetUniqueId());        
        
        PlayersContainer.AddChild(newPlayer);
        players.Add(newPlayer);        
        GD.Print("New player spawned " + playerID);
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    public void FreePlayer(long id) {
        var player = PlayersContainer.GetNode<Player.Player>(id.ToString());

        GD.Print("Player Freed " + id); 
        players.Remove(player);
        player.QueueFree();
    }

    public void SwitchState(GameStates newState) {
        EmitSignal(SignalName.GameStateChange);
        CurrentState = newState;
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    public void StartGame() {  
        GD.Print(players);

        GD.Print(GetTree().GetMultiplayer().GetUniqueId());
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    public void StartTeamBuilding() {
        CapsulesCollected = 0;
        // spawn ball spawner
        
        // listen to signal that all balls have been collected
        // signal += CapsuleCollected;
    }

    public void CapsuleCollected(Player.Player player, Capsule capsule) {
        // get a random creature based on the elements type;
        // give to the player

        CapsulesCollected++;
        if (CapsulesCollected == CapsulesPerTB) EndTeamBuilding(); 
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    public void EndTeamBuilding() {
       // await animation or timing
       SwitchState(GameStates.Battle); 
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    public void StartBattle() {

    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    public void EndBattle() {
 
    }

    public void CheckWincon() {
        foreach (var player in players) {
            if (player.Score == ScoreToWin) {
                EndGame(player);
                return;
            }
        }
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    public void EndGame(Player.Player player) {
        SwitchState(GameStates.End);
        SwitchState(GameStates.Start); 
    }
}