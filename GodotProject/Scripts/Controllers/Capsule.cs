using System;
using System.Collections.Generic;
using Godot;

namespace Critter
{
    public partial class Capsule : Grabbable
    {
        public Element element;
        public Rarity rarity;
        private StandardMaterial3D material;

        PackedScene cap = GD.Load<PackedScene>("res://Scenes/Components/Capsule.tscn");

        private static readonly Dictionary<Element, Color> elementColorPairs = new() {
            {Element.FIRE, new Color("#ca6f42")},
            {Element.WATER, new Color("889cc5")},
            {Element.GRASS, new Color("#1f9343")},
            {Element.ELECTRIC, new Color("#c3f246")},
            {Element.GHOST, new Color("#6f5475")}
        };
    }  
}