
using System;
using Critter;
using Godot;


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
    public Node3D CapsulesContainer;

    private MultiplayerApi multiplayer;

    public override void _Ready() {
        multiplayer = GetTree().GetMultiplayer();
        GD.Print("Game state initialized.");

        PlayersContainer = GetNode<Control>("/root/Node3D/Players");
        CapsulesContainer = GetNode<Node3D>("/root/Node3D/Capsules");
        Button startButton = GetNode<Button>("/root/Node3D/Debug/Button4");

        GD.Print(CapsulesContainer);

        startButton.ButtonDown += StartGame;   
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

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    public void SwitchState(GameStates newState) {
        EmitSignal(SignalName.GameStateChange);
        CurrentState = newState;
        GD.Print("Gamestate swapped: " + CurrentState);
    }

    public void StartGame() {
        if (CurrentState != GameStates.Start) return;
        if (!GetTree().GetMultiplayer().IsServer()) return; 
        
        int readyCount = 0;
        foreach (var p in players) {
            if (p.StartReady) readyCount++;
        }
        GD.Print(players.Count, readyCount);
        if (readyCount != players.Count) {
            GD.Print("readyCount does not equal player count.");
            return;
        }
    
        GD.Print("===== Starting game =====");
        Rpc(nameof(SwitchState), (int) GameStates.TeamBuilding);
        Rpc(nameof(StartTeamBuilding));
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    public void StartTeamBuilding() {
        GD.Print("===== Teambuilding Started =====");
        CapsulesCollected = 0;

        var capsuleSpawner = new CapsuleSpawner(CapsulesContainer, 0.25f, 12);
        {
            Name = "CapsuleSpawner";
        }

        foreach (var player in players) {
            var collectionZone = new CollectionZone(player);
            GetTree().Root.GetChild(0).AddChild(collectionZone); 
        }

        GetTree().Root.GetChild(0).AddChild(capsuleSpawner);
    }

    public void CapsuleCollected(Player.Player player, Capsule capsule) {
        // get a random creature based on the elements type;
        // give to the player

        CapsulesCollected++;
        if (CapsulesCollected == CapsulesPerTB) EndTeamBuilding(); 
    }

    public void EndTeamBuilding() {
       // await animation or timing
       SwitchState(GameStates.Battle); 
    }

    public void StartBattle() {

    }

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

    public void EndGame(Player.Player player) {
        SwitchState(GameStates.End);
        SwitchState(GameStates.Start); 
    }
}