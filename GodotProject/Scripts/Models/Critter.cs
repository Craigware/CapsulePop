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
        
        public Creature() : this(null, Element.FIRE, new Stats()){}
        public Creature(Texture2D sprite, Element element, Stats baseStats) {
            Sprite = sprite;
            Element = element;
            BaseStats = baseStats;
        }

	}

    public partial class PartyCreature : Creature {
        public Stats CurrentStats { get; set; }
        public Rarity Rarity { get; set; }
    }

    public static class CreatureList {
        public static class Ghost {
            private static Creature protoGhost = new(
                sprite: new Texture2D(),
                element: Element.GHOST,
                baseStats: new Stats(
                    attackSpeed: 1,
                    moveSpeed: 2,
                    range: 2,
                    health: 10
                )
            );


            public static Creature ProtoGhost { get { return (Creature) protoGhost.Duplicate(); } }
        }

        public static class Water { 
            private static Creature protoWater = new(
                sprite: new Texture2D(),
                element: Element.WATER,
                baseStats: new Stats(
                    attackSpeed: 1,
                    moveSpeed: 1,
                    range: 2,
                    health: 8
                )                
            );


            public static Creature ProtoWater { get { return (Creature) protoWater.Duplicate(); }} 
        }

        public static class Fire {
            private static Creature protoFire = new(
                sprite: new Texture2D(),
                element: Element.FIRE,
                baseStats: new Stats(
                    attackSpeed: 1,
                    moveSpeed: 1,
                    range: 1,
                    health: 12
                )
            );

            public static Creature ProtoFire { get { return (Creature) protoFire.Duplicate(); }}
        }
 
        public static class Electric {
            private static Creature protoElectric = new(
                sprite: new Texture2D(),
                element: Element.ELECTRIC,
                baseStats: new Stats(
                    attackSpeed: 2,
                    moveSpeed: 2,
                    range: 1,
                    health: 6
                )
            );

            public static Creature ProtoElectric { get { return (Creature) protoElectric.Duplicate(); }} 
        }
 
 
        public static class Grass {
            public static Creature protoGrass = new(
                sprite: new Texture2D(),
                element: Element.GRASS,
                baseStats: new Stats(
                    attackSpeed: 3,
                    moveSpeed: 1,
                    range: 3,
                    health: 4
                )
            );

            public static Creature ProtoGrass { get { return (Creature) protoGrass.Duplicate(); }}
        }
    }
}
