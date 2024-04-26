using Godot;

namespace Player {
    public partial class Client : Node
    {
        private MultiplayerApi multiplayer;
        private const string developmentIP = "127.0.0.1";
        private const int port = 52401;


        public override void _Ready() {
            multiplayer = GetTree().GetMultiplayer();
            var peer = new ENetMultiplayerPeer();
            peer.CreateClient(developmentIP, port);
            multiplayer.MultiplayerPeer = peer;
            
            GD.Print("Client created.");
        }
    }
}