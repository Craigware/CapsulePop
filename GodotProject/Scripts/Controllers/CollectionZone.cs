using Critter;
using Godot;
using System;

public partial class CollectionZone : Area3D
{
    [Signal]
    public delegate void CapsuleCollectedEventHandler();

    Player.Player associatedPlayer;
    float timeToCollect = 1f;


    public CollectionZone() : this(null) {}
    public CollectionZone(Player.Player player) {
        associatedPlayer = player;
    } 
    
	public override void _Ready() {
        BodyEntered += StopCapsuleVeloctiy; 
	}

    private void StopCapsuleVeloctiy(Node3D capsule) {
        // Start a timer
        Capsule rCapsule = (Capsule) capsule;
        rCapsule.Freeze = true;

    }
}
