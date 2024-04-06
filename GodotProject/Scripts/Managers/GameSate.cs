using Godot;
using System;

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
    private Player.Player[] players = new Player.Player[4];
    private int ScoreToWin = 4;

    [Export] public Control PlayersContainer;

    public void ConnectPlayer(int id) {
        // Add player to players list
        
        // Get discord information life profile picture and name
        
        // Add a player instance to the scene
        Player.Player newPlayer = playerScene.Instantiate<Player.Player>();
        PlayersContainer.AddChild(newPlayer);
    }

    public void DisconnectPlayer(int id) {
        // Remove player from players list
        // some sort of event to notify players someone left
    }

    public void SwitchState(GameStates newState) {
        EmitSignal(SignalName.GameStateChange);
        CurrentState = newState;
    }
    
    public void StartGame() {
        if (CurrentState != GameStates.Start) return; 
        
        int readyPlayers = 0;
        foreach (var player in players) {
            if (player.StartReady == true) readyPlayers++; 
        }

        if (readyPlayers == players.Length) {
           // Start the game
           SwitchState(GameStates.TeamBuilding); 
        }
    }

    public void StartTeamBuilding() {
        // spawn ball spawner
        // listen to signal that all balls have been collected
    }

    public void StartBattle() {

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