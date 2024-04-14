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

        [Export] public RichTextLabel nameDisplay;
        
        private string playerName;
        public string PlayerName {
            get { return playerName; }
            set {
                playerName = value;     
                nameDisplay.Text = "[center]" + PlayerName.ToString() + "[/center]";
            }
        }

        public Texture2D PlayerIcon;

        public Color CursorColor {
            get { return cursor.Modulate; }
            set { cursor.Modulate = value; }  
        }
        public Party party = new();

        [Export] public Control cursor;
        

        private Node3D root;

        public Godot.Collections.Dictionary<string, Variant> ToDict(){
            Godot.Collections.Dictionary<string, Variant> player = new();
            
            player["PlayerName"] = PlayerName;
            player["PlayerIcon"] = PlayerIcon;
            player["PlayerID"] = PlayerID;
            player["Score"] = Score;
            player["StartReady"] = StartReady;
            player["CursorPosition"] = cursor.Position;
            player["CrsorColor"] = CursorColor;

            //Party will also have to be here eventually when created are made.

            return player;
        }

        public override void _Ready() {
            Input.MouseMode = Input.MouseModeEnum.Hidden;
            root = GetTree().Root.GetChild<Node3D>(0);            
            Button readyButton = GetNode<Button>("/root/Node3D/Debug/Button3");

            nameDisplay.Text = "[center]" + PlayerID.ToString() + "[/center]";
            readyButton.ButtonDown += () => { Rpc(nameof(ToggleReady)); };
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

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        public void ToggleReady() {
            StartReady = !StartReady;
            GD.Print("ready");
        }
    }
}