
using System;
using System.Linq;
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
    private static Godot.Collections.Array<Vector3> CollectionZoneLocations = new(){
        new Vector3(-7,-3,4),
        new Vector3(7,-3,4),
        new Vector3(7,-3,-4),
        new Vector3(-7,-3,4)
    };
     
    private PackedScene playerScene = GD.Load<PackedScene>("res://Scenes/Components/Player.tscn");
    private PackedScene CollectionZoneScene = GD.Load<PackedScene>("res://Scenes/Components/CollectionZone.tscn");
    private PackedScene BoardCreatureScene = GD.Load<PackedScene>("res://Scenes/Components/BoardCreature.tscn");
    
    private GameStates CurrentState = GameStates.Start;
    private Godot.Collections.Array<Player.Player> players = new();
    private Godot.Collections.Dictionary<Player.Player, int> battlePlayers = new();
    private Godot.Collections.Array<Player.Player> spectators = new();

    public int ScoreToWin { get; } = 4;

    private int CapsulesPerTB = 12;
    private int CapsulesCollected = 0;
    
    public Control PlayersContainer;
    public Node3D BoardCreaturesContainer;
    public Node3D CapsulesContainer;

    private MultiplayerApi multiplayer;

    public override void _Ready() {
        multiplayer = GetTree().GetMultiplayer();
        GD.Print("Game state initialized.");

        PlayersContainer = GetNode<Control>("/root/Node3D/Players");
        CapsulesContainer = GetNode<Node3D>("/root/Node3D/Capsules");
        BoardCreaturesContainer = GetNode<Node3D>("/root/Node3D/Creatures");
        Button startButton = GetNode<Button>("/root/Node3D/Debug/Button4");

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
        
        if (readyCount != players.Count) {
            GD.Print("readyCount does not equal player count.");
            return;
        }
    
        GD.Print("===== Starting game =====");
        Rpc(nameof(SwitchState), (int) GameStates.TeamBuilding);
        StartTeamBuilding();
    }

    public void StartTeamBuilding() {
        GD.Print("===== Teambuilding Started =====");
        CapsulesCollected = 0;
        Rpc(nameof(SpawnCapsuleSpawner));
        
        int index = 0;
        foreach(var p in players) {
            Variant[] args = new Variant[3]{
                p.PlayerID,
                index,
                players.Count
            };

            Rpc(nameof(SpawnCollectionZone), args);
            index++;
        }
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    public void SpawnCapsuleSpawner() {        
        var capsuleSpawner = new CapsuleSpawner(CapsulesContainer, 1f, CapsulesPerTB)
        {
            Name = "CapsuleSpawner"
        };

        GetTree().Root.GetChild(1).AddChild(capsuleSpawner);        
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    public void SpawnCollectionZone(long playerId, int index, int totalPlayers) {
        Player.Player p = PlayersContainer.GetNode<Player.Player>(playerId.ToString());

        CollectionZone collectionZone = CollectionZoneScene.Instantiate<CollectionZone>();
        collectionZone.Name = playerId.ToString();
        collectionZone.associatedPlayer = p;
        collectionZone.maxCollects = CapsulesPerTB/totalPlayers;
        collectionZone.CapsuleCollected += CapsuleCollected;
        GD.Print(collectionZone.maxCollects);

        GetTree().Root.GetChild(1).AddChild(collectionZone);

        collectionZone.Position = CollectionZoneLocations[index]; 
    }

    public void CapsuleCollected() {
        CapsulesCollected++;
        if (GetTree().GetMultiplayer().IsServer()) {
            if (CapsulesCollected == CapsulesPerTB) EndTeamBuilding(); 
        }
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    public void RemoveBallSpawner() {
        GetTree().Root.GetChild(1).GetNode<CapsuleSpawner>("CapsuleSpawner").QueueFree();
    }

    public void EndTeamBuilding() {
       // await animation or timing
        Rpc(nameof(RemoveBallSpawner));
        Rpc(nameof(SwitchState), (int) GameStates.Battle);
        StartBattle();
    }

    public void StartBattle() {
        if (!GetTree().GetMultiplayer().IsServer()) return;
        GD.Print("===== Battle Started =====");
        int i = 0;
        battlePlayers = new(); 
        foreach (var p in players) {
            int creatureIndex = 0;
            battlePlayers.Add(p, p.party.Count());
            foreach (var c in p.party.Array) {
                Variant[] args = new Variant[]{
                    i,
                    p.PlayerID,
                    c.CreatureName,
                    creatureIndex
                };

                Rpc(nameof(SummonCreature), args);
                creatureIndex++; 
            }
            i++;
        }
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    public void SummonCreature(int playerIndex, long playerId, string creatureId, int creatureIndex) {
        if (BoardCreaturesContainer.GetNode<Node3D>(playerId.ToString()) == null) {
            BoardCreaturesContainer.AddChild(new Node3D(){Name=playerId.ToString()});
        }
        
        Creature c = CreatureList.All[creatureId];
        PartyCreature pC = new(
            (Stats) c.BaseStats.Duplicate(),
            0,
            c,
            playerId
        );
        BoardCreature bC = BoardCreatureScene.Instantiate<BoardCreature>();
        bC.PartyCreature = pC;
        bC.Name = creatureIndex.ToString();
        bC.Fient += CreatureFeinted;
        BoardCreaturesContainer.GetNode<Node3D>(playerId.ToString()).AddChild(bC);
        bC.Position = CollectionZoneLocations[playerIndex] + Vector3.Up*3;
    }

    public void CreatureFeinted(BoardCreature c) {
        if (!GetTree().GetMultiplayer().IsServer()) return;

        Player.Player p = PlayersContainer.GetNode<Player.Player>(c.PartyCreature.OwnerID.ToString());
        battlePlayers[p] -= 1;
        GD.Print(battlePlayers);
        
        if (battlePlayers[p] <= 0) {
            battlePlayers.Remove(p);
        }

        if (battlePlayers.Keys.Count() == 1) {
            var alive = battlePlayers.Keys;
            GD.Print("One player remains.");
            Player.Player winner = (Player.Player) alive.ToArray().GetValue(0);
            winner.Score++;
            GD.Print(winner, winner.Score);
            EndBattle();
        }
    }

    public void EndBattle() {
        Rpc(nameof(ClearBoardCreatures));
        var winner = CheckWincon();
        if (winner != null) {
            EndGame(winner);
        } else {
            LoopGameState();
        }
    }

    [ Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    public void ClearBoardCreatures() {
        foreach (var c in BoardCreaturesContainer.GetChildren()) {
            c.QueueFree();
        }
    }

    public void LoopGameState() {
        foreach (var p in players) {
            p.party.Clear();
        }

        GD.Print("===== Game state looping =====");
        Rpc(nameof(SwitchState), (int) GameStates.TeamBuilding);
        StartTeamBuilding();
    }

    public Player.Player CheckWincon() {
        foreach (var player in players) {
            if (player.Score == ScoreToWin) {
                return player;
            }
        }
        return null;
    }

    public void EndGame(Player.Player player) {
       GD.Print(player.PlayerName + " has won.");
    }
}