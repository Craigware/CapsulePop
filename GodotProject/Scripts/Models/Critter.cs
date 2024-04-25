using System;
using System.Collections.Generic;
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
        [Export] public string CreatureName {get; set;}
        
        public Creature() : this("placeholder", null, Element.FIRE, new Stats()){}
        public Creature(string name, Texture2D sprite, Element element, Stats baseStats) {
            Sprite = sprite;
            Element = element;
            BaseStats = baseStats;
            CreatureName = name;
        }
	}

    public partial class PartyCreature : Resource {
        public Stats CurrentStats { get; set; }
        public long OwnerID { get; set; }
        public Rarity Rarity { get; set; }
        public Creature Creature { get; set; }

        public PartyCreature() : this(new Stats(), 0, null, 0){}
        public PartyCreature(Stats currentStats, Rarity rarity, Creature c, long ownerId) {
            CurrentStats = currentStats;
            Rarity = rarity;
            Creature = c;
            OwnerID = ownerId;
        }
    }

    public static class CreatureList {
        static Godot.Collections.Dictionary<string, Creature> all = new(){
            ["Protoghost"]=Ghost.ProtoGhost,
            ["Protofire"]=Fire.ProtoFire,
            ["Protograss"]=Grass.ProtoGrass,
            ["Protoelectric"]=Electric.ProtoElectric,
            ["Protowater"]=Water.ProtoWater
        };

        public static Godot.Collections.Dictionary<string, Creature> All { get { return all; } }
        
        public static class Ghost {
            private static Creature protoGhost = new(
                name: "Protoghost",
                sprite: GD.Load<Texture2D>("res://Assets/Images/Protoghost.png"),
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
                sprite: GD.Load<Texture2D>("res://Assets/Images/Protowater.png"),
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
                sprite: GD.Load<Texture2D>("res://Assets/Images/Protofire.png"),
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
                sprite: GD.Load<Texture2D>("res://Assets/Images/Protoelectric.png"),
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
            private static Creature protoGrass = new(
                name: "Protograss",
                sprite: GD.Load<Texture2D>("res://Assets/Images/Protograss.png"),
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
