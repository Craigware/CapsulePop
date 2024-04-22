using Critter;
using Godot;

public partial class CollectionZone : Area3D
{
    [Signal]
    public delegate void CapsuleCollectedEventHandler();

    public Player.Player associatedPlayer;
    float timeToCollect = 1f;

    Capsule grabbedCapsule; 
    bool collecting = false;
    
	public override void _Ready() { 
        BodyEntered += StartCapsuleCollection; 
        BodyExited += CancelCapsuleCollection;
	}

    private void CaptureCapsule() {
        Element creatureElement = grabbedCapsule.element;
        Creature creature = CreatureList.Fire.ProtoFire;

        switch (creatureElement) {
            case Element.GHOST:
                creature = CreatureList.Ghost.ProtoGhost;
                break;
            case Element.FIRE:
                creature = CreatureList.Fire.ProtoFire;
                break;
            case Element.WATER:
                creature = CreatureList.Water.ProtoWater;
                break;
            case Element.GRASS:
                creature = CreatureList.Grass.ProtoGrass;
                break;
            case Element.ELECTRIC:
                creature = CreatureList.Electric.ProtoElectric;
                break;
        }

        associatedPlayer.party.AddToParty(creature);
        grabbedCapsule.QueueFree();
        grabbedCapsule = null;
    }

    private void StartCapsuleCollection(Node3D capsule) {
        GD.Print(associatedPlayer.PlayerID + " Started collecting a capsule");
        
        Capsule c = (Capsule) capsule;
        Timer collectionTimer = new() {WaitTime=timeToCollect, Autostart=true, Name="CollectionTimer"};
        collecting = true;
        grabbedCapsule = c;
        
        collectionTimer.Timeout += CaptureCapsule;
        
        AddChild(collectionTimer);
        
    }

    private void CancelCapsuleCollection(Node3D capsule) {
        collecting = false;
    }
}
