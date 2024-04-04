using Godot;

namespace Critter {
    public enum Element {
        GRASS,
        FIRE,
        WATER,
        ELECTRIC,
        GHOST
    }

    public enum Rarity {
        Common,
        Rare,
        Epic,
        Legendary
    }    

    [GlobalClass]
	public partial class Creature : Resource {
        [Export] public Texture2D Sprite { get; set; }
        [Export] public Element Element { get; set; }
        [Export] public Stats BaseStats { get; set; }
	}

    public partial class PartyCreature : Creature {
        public Stats CurrentStats { get; set; }
        public Rarity Rarity { get; set; }
    }    
}
