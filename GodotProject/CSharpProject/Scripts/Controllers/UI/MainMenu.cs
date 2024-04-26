using Godot;

namespace Menus
{

    public partial class MainMenu : Control
    {
        public PackedScene playerScene = GD.Load<PackedScene>("res://Scenes/Components/Player.tscn");
        public Player.Player clientPlayerNode;


        public override void _Ready() {
            clientPlayerNode = playerScene.Instantiate<Player.Player>();
            
            // Grab players name
            JavaScriptBridge.Eval(@"");
            // Grab players icon
            JavaScriptBridge.Eval(@"");

        }
        

        public void onHostButtonDown() {
            GD.Print("TESt"); 
        }

        public void onJoinButtonDown() {
            GD.Print("Join");
        }
    } 

}