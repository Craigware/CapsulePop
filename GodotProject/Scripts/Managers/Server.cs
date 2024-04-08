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
            multiplayer = GetTree().GetMultiplayer();
            var peer = new ENetMultiplayerPeer();
            peer.CreateServer(PORT, MAX_PLAYERS);
            multiplayer.MultiplayerPeer = peer;
            multiplayer.PeerConnected += PlayerJoin;

            GD.Print("Server Started");
        }

        public void PlayerJoin(long playerId) {  
            GD.Print("WAT!");           
        }

        public void PlayerLeave(long playerId) {
            PlayerIDs.Remove(playerId);
        } 
    }
}