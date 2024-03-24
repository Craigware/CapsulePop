using Godot;

namespace Player {
    public partial class Player : Control {
        [Export] public Color CursorColor { get; set; }
        [Export] public TextureRect playerCursor;
        [Export] public Camera3D cam;

        public const float CameraXZPos = 10;
        public const float CameraYPos = 5;

        private const float RayLength = 1000.0f;

        private Node3D root;

        public override void _Ready()
        {
            Input.MouseMode = Input.MouseModeEnum.Hidden;
            root = GetTree().Root.GetChild<Node3D>(0);
        }

        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseMotion e) 
            {
                playerCursor.Position = e.Position - playerCursor.Size/2; 
            }

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
                
            }
        }
    }
}