using Godot;
using Critter;
using System.Windows.Markup;

namespace Player
{
    public class Party
    {
        Creature[] party = new Creature[6];

        public bool AddToParty(Creature creature) {
            for (int i = 0; i < party.Length; i++) {
                if (party[i] == null) {
                    party[i] = creature;
                    return true;
                }
            }

            return false;
        }

        public int Count() {
            int count = 0;
            foreach (Creature c in party) {
                if (c == null) continue;
                count++;
            }
            return count;
        }
    }

    public partial class Player : Control
    {
        public const float CameraXZPos = 10;
        public const float CameraYPos = 5;
        private const float RayLength = 1000.0f;

        public int Score = 0;
        public bool StartReady = false;
        public long PlayerID = 1;

        public string PlayerName = "Null";
        public Texture2D PlayerIcon;

        public Color CursorColor {
            get { return cursor.Modulate; }
            set { cursor.Modulate = value; }  
        }
        public Party party = new();

        [Export] public Control cursor;
        [Export] public RichTextLabel nameDisplay;

        private Node3D root;

        public Player() : this(1, null, "Unnamed") {}
        public Player(long id, Texture2D playerIcon, string playerName) {
            PlayerID = id;
            PlayerName = playerName;
            PlayerIcon = playerIcon;
        }

        public override void _Ready() {
            Input.MouseMode = Input.MouseModeEnum.Hidden;
            root = GetTree().Root.GetChild<Node3D>(0);
            nameDisplay.Text = PlayerID.ToString();
        }
 
        public override void _Input(InputEvent @event) { 
            if (GetTree().GetMultiplayer().GetUniqueId() != PlayerID) return;
            if (@event is InputEventMouseMotion e) {
                Rpc(nameof(UpdateCursorPosition), e.Position);
            }

            if (Input.IsActionJustPressed("Click")) {
                Click(GetViewport().GetMousePosition());
            }

            if (Input.IsActionJustPressed("Escape")) {
                GetTree().Quit();
            }
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        public void UpdateCursorPosition(Vector2 newPos) {
            cursor.Position = newPos;
        }

        public void Click(Vector2 mousePosition) {
            
        }
    }
}