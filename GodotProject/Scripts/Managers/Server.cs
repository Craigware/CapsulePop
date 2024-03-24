using Godot;

namespace Server {    
    public partial class Server : Node3D
    {
        public enum GameStates {
            Start,
            TeamBuilding,
            Battle,
            End
        }

        const int PORT = 524;
        const int MAX_PLAYERS = 4;

        private MultiplayerApi multiplayer;

        private GameStates GameState = GameStates.Start;
        private Godot.Collections.Array<int> PlayerIDs = new();
        private int PlayerCount;

        public override void _Ready() {
            multiplayer = GetTree().GetMultiplayer();
            var peer = new ENetMultiplayerPeer();
            peer.CreateServer(PORT, MAX_PLAYERS);
            multiplayer.MultiplayerPeer = peer;
        }

        private void ChangeGameState(GameStates newState) {
            GameState = newState;
        }

        public void PlayerJoin(int playerId) {
            PlayerIDs.Add(playerId);
            PlayerCount++;
        }

        public void PlayerLeave(int playerId) {
            PlayerIDs.Remove(playerId);
            PlayerCount--;
        }

        public void GameStart() {
            ChangeGameState(GameStates.TeamBuilding);
        }
    }
}