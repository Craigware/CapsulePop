using System;
using System.Collections.Generic;
using Godot;

namespace Critter
{
    public partial class Capsule : RigidBody3D
    {
        public Element element;
        public Rarity rarity;
        public bool IsGrabbed = false;
        public Player.Player Grabber = null;
        private StandardMaterial3D material;

        PackedScene cap = GD.Load<PackedScene>("res://Scenes/Components/Capsule.tscn");

        private static readonly Dictionary<Element, Color> elementColorPairs = new() {
            {Element.FIRE, new Color("#ca6f42")},
            {Element.WATER, new Color("889cc5")},
            {Element.GRASS, new Color("#1f9343")},
            {Element.ELECTRIC, new Color("#c3f246")},
            {Element.GHOST, new Color("#6f5475")}
        };
 
        public void Fling(Vector3 velocity) {
            Rpc(nameof(Push), velocity);
        }
 
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        public void Push(Vector3 force) {
            if (!GetTree().GetMultiplayer().IsServer()) return;
            ApplyImpulse(force);
        }

        public override void _PhysicsProcess(double delta) {
            if (IsGrabbed && GetTree().GetMultiplayer().IsServer()) {
                Position = Grabber.MousePositionToWorldSpace(12);
            }

            if (GetTree().GetMultiplayer().IsServer()) {
                Rpc(nameof(UpdatePosition), Position);
            }
        }


        public void SetGrabbed(long? grabberId=null) { 
            if (grabberId != null && !IsGrabbed) {
                Rpc(nameof(Grabbed), (long) grabberId);
            } else {
                Rpc(nameof(Released));
            } 
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        public void Grabbed(long grabberId) {
            Grabber = GetNode<Player.Player>("/root/Node3D/Players/" + grabberId.ToString());
            LinearVelocity = Vector3.Zero;
            AngularVelocity = Vector3.Zero;
            Sleeping = true;
            Freeze = true;
            IsGrabbed = true;
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        public void Released() {
            Grabber = null;
            IsGrabbed = false;
            Freeze = false;
        } 

        [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
        public void UpdatePosition(Vector3 updatedPosition) {
            Position = updatedPosition;
        }
    }  
}