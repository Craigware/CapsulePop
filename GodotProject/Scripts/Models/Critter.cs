using Godot;

namespace Critter {
    public enum Element {
        GRASS,
        FIRE,
        WATER,
        ELECTRIC,
        GHOST
    }
    

    [GlobalClass]
	public partial class Critter : Resource {
        [Export] public Texture2D Sprite { get; set; }
        [Export] public Element Element { get; set; }
        [Export] public BaseStats BaseStats { get; set; }
	}
}
