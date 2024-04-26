using System;
using Godot;

namespace Server {    
    public partial class Server : Node
    {
        const int PORT = 52401;
        const int MAX_PLAYERS = 4;

        private MultiplayerApi multiplayer;
        private Godot.Collections.Array<long> PlayerIDs = new();

        private GameSate gameSateManager;

        public override void _Ready() {
            gameSateManager = GetNode<GameSate>("/root/GameState");
            multiplayer = GetTree().GetMultiplayer();

            var peer = new ENetMultiplayerPeer();
            peer.CreateServer(PORT, MAX_PLAYERS);
            multiplayer.MultiplayerPeer = peer;

            multiplayer.PeerConnected += gameSateManager.ConnectPlayer;
            multiplayer.PeerDisconnected += gameSateManager.DisconnectPlayer;
            
            GD.Print("Server Started");
        } 
    }
}