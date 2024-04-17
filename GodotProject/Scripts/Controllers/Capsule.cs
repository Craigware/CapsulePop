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

        private static readonly Dictionary<Element, Color> elementColorPairs = new() {
            {Element.FIRE, new Color("#ca6f42")},
            {Element.WATER, new Color("889cc5")},
            {Element.GRASS, new Color("#1f9343")},
            {Element.ELECTRIC, new Color("#c3f246")},
            {Element.GHOST, new Color("#6f5475")}
        };

        [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
        public void Push(Vector3 force, Vector3? forcePoint=null) {
            ApplyForce(force, forcePoint);
        }


        public override void _PhysicsProcess(double delta) {            
            if (GetTree().GetMultiplayer().IsServer()) {
                Rpc(nameof(UpdatePosition), Position);
            }
        }

        [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
        public void UpdatePosition(Vector3 updatedPosition) {
            Position = updatedPosition;
        }
    }  
}