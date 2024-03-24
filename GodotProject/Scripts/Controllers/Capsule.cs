using System.Collections.Generic;
using Godot;

namespace Critter {
    public partial class Capsule : Sprite3D {
        private Element element;

        private static readonly Dictionary<Element, Color> elementColorPairs = new() {
            {Element.FIRE, new Color( )},
            {Element.WATER, new Color( )},
            {Element.GRASS, new Color( )},
            {Element.ELECTRIC, new Color( )},
            {Element.GHOST, new Color( )}
        };

        public Capsule(Element element) {
            this.element = element;
            Modulate = elementColorPairs[element];
        }
    }
}