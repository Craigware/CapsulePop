using Godot;
using Critter;
using System.Windows.Markup;
using System.Security.Cryptography.X509Certificates;

namespace Player
{
    public class Party
    {
        Creature[] party = new Creature[6];
        public Creature[] Array { get { return party; } }

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

        public void Clear() {
            party = new Creature[6];
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
        public bool Stunned = false;

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

        Grabbable grabbed = null;


        Vector2 relative = Vector2.Zero;
        Vector2 velocity = Vector2.Zero;
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

            Input.UseAccumulatedInput = true;
        }


        public override void _Input(InputEvent @event) { 
            if (GetTree().GetMultiplayer().GetUniqueId() != PlayerID) return; 
            if (Stunned) return;
               
            //? Grab
            if (Input.IsActionJustPressed("Grab")) {
                Grab(GetViewport().GetMousePosition());
            }
 
            if (@event is InputEventMouseMotion e) {
                Rpc(nameof(UpdateCursorPosition), e.Position);
                relative = e.Relative;
                velocity = e.Velocity;
            }

            // ?Fling
            if (Input.IsActionJustReleased("Grab")) {              
                Fling(relative, velocity);
            }

            //? Smack
            if (Input.IsActionJustPressed("Select")) {
                Smack(GetViewport().GetMousePosition());
            }
    
            if (Input.IsActionJustPressed("Escape")) {
                GetTree().Quit();
            }
        }

        public override void _Process(double delta) {
            if (Input.IsActionJustReleased("Grab")) {
                Fling(relative, velocity);
            }
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        public void UpdateCursorPosition(Vector2 newPos) {
            cursor.Position = newPos;            
        }

        public void Fling(Vector2 relativePosition, Vector2 mouseVelocity) {
            if (grabbed == null) return;

            grabbed.SetGrabbed();

            Vector3 force = new(
                Mathf.Clamp(relativePosition.X/10, -1, 1) * 10,
                0,
                Mathf.Clamp(relativePosition.Y/10, -1, 1) * 10
            );

            grabbed.Fling(force);
            grabbed = null;
        }

        public void Grab(Vector2 mousePosition) {
            var target = CreateRayCast(mousePosition, 1000);
            
            try {
                Grabbable grabbable = (Grabbable) target;
                if (grabbable.IsGrabbed) return;
                grabbable.SetGrabbed(PlayerID);
                grabbed = grabbable;
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

            var camera = GetViewport().GetCamera3D();
            var from = camera.ProjectRayOrigin(mousePosition);
            var to = from + camera.ProjectRayNormal(mousePosition) * d;
 
            return to;
        }

        public void Smack(Vector2 mousePosition) {
            // This is going to check an area ccca

            var smackRegion = GetNode<Area2D>("Cursor/Area2D");
            var smackedAreas = smackRegion.GetOverlappingAreas();
            if ( smackedAreas.Count == 0 ) return;

            foreach (var area in smackedAreas) {
                var smackedPlayer = area.GetParent().GetParent<Player>();
                GD.Print(smackedPlayer.PlayerID, " Smacked.");
                smackedPlayer.SetStunned(1000);
            }
            
        } 


        public void SetStunned(int duration=1000) {
            if (!Stunned) {
                Rpc(nameof(Stun), duration);
            }
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        public void Stun(int duration) {
            Stunned = true;
            
            Timer timer = new() {
                WaitTime = duration/1000,
            };

            timer.Timeout += () => {
                timer.QueueFree();
                GD.Print("Unstunned");
                Stunned = false;
            };

            AddChild(timer);
            timer.Start();
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