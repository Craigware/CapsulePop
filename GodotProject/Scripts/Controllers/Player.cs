using Godot;
using Critter;

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
        // public Color CursorColor { get; set; }
        // public TextureRect playerCursor;
        public Camera3D cam;
        public Party party = new();

        public const float CameraXZPos = 10;
        public const float CameraYPos = 5;

        private const float RayLength = 1000.0f;

        private Node3D root;

        public override void _Ready() {
            Input.MouseMode = Input.MouseModeEnum.Hidden;
            root = GetTree().Root.GetChild<Node3D>(0);
        }

        public override void _Input(InputEvent @event) {
            // if (@event is InputEventMouseMotion e) 
            // {
            //     playerCursor.Position = e.Position - playerCursor.Size/2; 
            // }

            if (Input.IsActionJustPressed("Click")) {
                Click(GetViewport().GetMousePosition());
            }

            if (Input.IsActionJustPressed("Escape")) {
                GetTree().Quit();
            }
        }

        public void Click(Vector2 mousePosition) {
            var from = cam.ProjectRayOrigin(mousePosition);
            var to = from + cam.ProjectRayNormal(mousePosition) * RayLength;

            var spaceState = root.GetWorld3D().DirectSpaceState;
            var query = PhysicsRayQueryParameters3D.Create(from, to);

            var res = spaceState.IntersectRay(query);
            if (res.ContainsKey("collider"))
            {
                CollisionObject3D collider = (CollisionObject3D) res["collider"];
                GD.Print(collider);
            }
        }
    }
}