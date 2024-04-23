using System.Reflection.Metadata.Ecma335;
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
        public string CreatureName {get; set;}
        
        public Creature() : this("placeholder", null, Element.FIRE, new Stats()){}
        public Creature(string name, Texture2D sprite, Element element, Stats baseStats) {
            Sprite = sprite;
            Element = element;
            BaseStats = baseStats;
            CreatureName = name;
        }

	}

    public partial class PartyCreature {
        public Stats CurrentStats { get; set; }
        public Rarity Rarity { get; set; }
        public Creature Creature { get; set; }

        public PartyCreature() : this(new Stats(), 0, null){}
        public PartyCreature(Stats currentStats, Rarity rarity, Creature c) {
            CurrentStats = currentStats;
            Rarity = rarity;
            Creature = c;
        }
    }

    public static class CreatureList {
        public static class Ghost {
            private static Creature protoGhost = new(
                name: "Protoghost",
                sprite: new Texture2D(),
                element: Element.GHOST,
                baseStats: new Stats(
                    attackSpeed: 1,
                    moveSpeed: 2,
                    range: 2,
                    health: 10
                )
            );


            public static Creature ProtoGhost { get { return protoGhost; } }
        }

        public static class Water { 
            private static Creature protoWater = new(
                name: "Protowater",
                sprite: new Texture2D(),
                element: Element.WATER,
                baseStats: new Stats(
                    attackSpeed: 1,
                    moveSpeed: 1,
                    range: 2,
                    health: 8
                )                
            );


            public static Creature ProtoWater { get { return protoWater; }} 
        }

        public static class Fire {
            private static Creature protoFire = new(
                name: "Protofire",
                sprite: new Texture2D(),
                element: Element.FIRE,
                baseStats: new Stats(
                    attackSpeed: 1,
                    moveSpeed: 1,
                    range: 1,
                    health: 12
                )
            );

            public static Creature ProtoFire { get { return protoFire; }}
        }
 
        public static class Electric {
            private static Creature protoElectric = new(
                name: "Protoelectric",
                sprite: new Texture2D(),
                element: Element.ELECTRIC,
                baseStats: new Stats(
                    attackSpeed: 2,
                    moveSpeed: 2,
                    range: 1,
                    health: 6
                )
            );

            public static Creature ProtoElectric { get { return protoElectric; }} 
        }
 
 
        public static class Grass {
            public static Creature protoGrass = new(
                name: "Protograss",
                sprite: new Texture2D(),
                element: Element.GRASS,
                baseStats: new Stats(
                    attackSpeed: 3,
                    moveSpeed: 1,
                    range: 3,
                    health: 4
                )
            );

            public static Creature ProtoGrass { get { return protoGrass; }}
        }
    }
}
