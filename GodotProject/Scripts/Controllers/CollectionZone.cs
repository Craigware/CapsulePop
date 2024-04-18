using Critter;
using Godot;
using System;

public partial class CollectionZone : Area3D
{
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
        // After x times pass if capsule is still in beam open it and add it to team
        // 
    }
}
