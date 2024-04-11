using System;
using Critter;
using Godot;

public enum GameStates {
    Start,
    TeamBuilding,
    Battle,
    End
}

public partial class GameSate : Node 
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
    }

    public void ConnectPlayer(long id) {
        Rpc(nameof(SpawnPlayer), id);
        GD.Print("!!! PLAYER CONNTECTED !!!");
    }
    
    public void DisconnectPlayer(long id) {
        Rpc(nameof(FreePlayer), id);
        GD.Print("!!! PLAYER DISCONNECTED !!!");
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    public void SpawnPlayer(long playerID) {
        var newPlayer = playerScene.Instantiate<Player.Player>();
        newPlayer.PlayerID = playerID;
        newPlayer.Name = playerID.ToString();
        PlayersContainer.AddChild(newPlayer);

        players.Add(newPlayer);
        GD.Print("New player spawned");
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    public void FreePlayer(long id) {
        var player = PlayersContainer.GetNode<Player.Player>(id.ToString());
        GD.Print(player);
        players.Remove(player);
        player.QueueFree();
    }

    public void SwitchState(GameStates newState) {
        EmitSignal(SignalName.GameStateChange);
        CurrentState = newState;
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    public void StartGame() {  
        if (CurrentState != GameStates.Start) return;  
        
        int readyPlayers = 0;
        foreach (var player in players) {
            if (player.StartReady == true) readyPlayers++; 
        }

        if (readyPlayers == players.Count) {
           // Start the game
           SwitchState(GameStates.TeamBuilding); 
        }
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