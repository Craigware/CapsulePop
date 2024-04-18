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

        Capsule grabbed = null;

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
            root = GetTree().Root.GetChild<Node3D>(1);            
            Button readyButton = GetNode<Button>("/root/Node3D/Debug/Button3");

            nameDisplay.Text = "[center]" + PlayerID.ToString() + "[/center]";
            
            if (GetTree().GetMultiplayer().GetUniqueId() == PlayerID) {
                readyButton.ButtonDown += () => { Rpc(nameof(ToggleReady)); };
            }
        }


        public override void _Input(InputEvent @event) { 
            if (GetTree().GetMultiplayer().GetUniqueId() != PlayerID) return;
            if (@event is InputEventMouseMotion e) {
                Rpc(nameof(UpdateCursorPosition), e.Position);
            }

            if (Input.IsActionJustPressed("Select")) {

            }

            if (Input.IsActionJustPressed("Grab")) {
                Grab(GetViewport().GetMousePosition());
            }

            if (Input.IsActionJustReleased("Grab")) {
                Release();
            }

            if (Input.IsActionJustPressed("Escape")) {
                GetTree().Quit();
            }
        }



        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        public void UpdateCursorPosition(Vector2 newPos) {
            cursor.Position = newPos;            
        }

        public void Release() {
            GD.Print("Released");
            grabbed?.SetGrabbed(PlayerID);
            grabbed = null;
        }

        public void Grab(Vector2 mousePosition) {
            var target = CreateRayCast(mousePosition, 1000);
            
            try {
                Capsule capsule = (Capsule) target;
                if (capsule.IsGrabbed) return;
                capsule.SetGrabbed(PlayerID);
                grabbed = capsule; 
            }
            catch { 

            }   

       }

        //! there is a bug here that makes the cursor not pick up the
        //! capsule correctly, doesn't throw, doesnt happen a majority of the time
        //! confusion seems to happen when you grab stacked balls? has to do with velocity and force
        
        //* fixed by using impulse instead of force
        //! nvm 

        //* it is based on viewport positions what in gods naem why
        //* if multiple clients are open on the same computer in different locations ti
        //* acts weird

        //* dk if this will matter on multiple computers, need to test
        public Vector3 MousePositionToWorldSpace(int d) {
            Vector2 mousePosition = GetViewport().GetMousePosition(); 
            GD.Print(GetTree().GetMultiplayer().GetUniqueId(), d);

            var camera = GetViewport().GetCamera3D();
            var from = camera.ProjectRayOrigin(mousePosition);
            var to = from + camera.ProjectRayNormal(mousePosition) * d;

            GD.Print(from, camera.ProjectRayNormal(mousePosition), d);
            GD.Print(GetViewport().GetWindow().Size);
            return from;
        }

        public void Smack(Vector2 mousePosition) {
            // This is going to check an area 

        } 

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        public void ToggleReady() {
            StartReady = !StartReady;
        }

        public Variant? CreateRayCast(Vector2 mousePosition, int rayLen) {
            var worldSpace = GetTree().Root.GetChild<Node3D>(1).GetWorld3D().DirectSpaceState;
            
            var camera = GetViewport().GetCamera3D();
            var from = camera.ProjectRayOrigin(mousePosition);
            var to = from + camera.ProjectRayNormal(mousePosition) * rayLen;

            var query = PhysicsRayQueryParameters3D.Create(from, to);

            var res = worldSpace.IntersectRay(query);
            
            if (res.ContainsKey("collider")) {
                return res["collider"];
            }

            return null;
        }
    }
}