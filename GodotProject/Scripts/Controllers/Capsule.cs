using System;
using System.Collections.Generic;
using Godot;

namespace Critter
{
    public partial class Capsule : RigidBody3D
    {
        public Element element;
        public Rarity rarity;
        private StandardMaterial3D material;

        PackedScene cap = GD.Load<PackedScene>("res://Scenes/Components/Capsule.tscn");

        private static readonly Dictionary<Element, Color> elementColorPairs = new()
        {
            {Element.FIRE, new Color("#ca6f42")},
            {Element.WATER, new Color("889cc5")},
            {Element.GRASS, new Color("#1f9343")},
            {Element.ELECTRIC, new Color("#c3f246")},
            {Element.GHOST, new Color("#6f5475")}
        };

        public override void _Ready() {
            material = new() {
                AlbedoColor = elementColorPairs[this.element]
            };
            GD.Print(elementColorPairs[this.element]);

            float initXForce = new Random().Next(-50,50);
            float initZForce = new Random().Next(-50,50);
            initXForce /= 100;
            initZForce /= 100;

            Vector3 initialForce = new(initXForce,1.25f,initZForce);
            ApplyForce(initialForce*100*5);

            GetChild<MeshInstance3D>(0).MaterialOverride = material;
        }
    }
}