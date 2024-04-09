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
    
    [Export] public Control PlayersContainer;

    private MultiplayerApi multiplayer;

    public override void _Ready() {
        multiplayer = GetTree().GetMultiplayer();
        GD.Print("Game state initialized.");
    }

    public void ConnectPlayer(long id) {
        // instantiate new instance of player scene 
        Rpc(nameof(SpawnPlayer));
        GD.Print("!!! PLAYER CONNTECTED !!!");
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void SpawnPlayer(long playerID) {
        var newPlayer = playerScene.Instantiate<Player.Player>();
        newPlayer.PlayerID = playerID;
        PlayersContainer.AddChild(newPlayer);
    }


    public void DisconnectPlayer(long id) {
        // Remove player from players list
        // some sort of event to notify players someone left
    }

    public void SwitchState(GameStates newState) {
        EmitSignal(SignalName.GameStateChange);
        CurrentState = newState;
    }

    [Rpc()]
    public void StartGame() {
        if (!multiplayer.IsServer()) return; 
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

    public void StartTeamBuilding() {
        // Reset Teambuilding
        CapsulesCollected = 0;
        // spawn ball spawner
        
        // listen to signal that all balls have been collected
        // signal += CapsuleCollected;
    }

    public void CapsuleCollected(Player.Player player, Capsule capsule) {
        // get a random creature based on the elements type;
        // give to the player

        CapsulesCollected++;
        if (CapsulesCollected == CapsulesPerTB) 
        {
            EndTeamBuilding();
        }            
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
                Victory(player);
                return;
            }
        }
    }

    public void Victory(Player.Player player) {
        SwitchState(GameStates.End);
       // await animation

        SwitchState(GameStates.Start); 
    }
}