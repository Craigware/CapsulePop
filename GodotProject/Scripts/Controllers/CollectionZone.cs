using Critter;
using Godot;

public partial class CollectionZone : Area3D
{
    [Signal]
    public delegate void CapsuleCollectedEventHandler();

    public Player.Player associatedPlayer;
    float timeToCollect = 1f;
    public int maxCollects = 1;
    int collects = 0;

    Capsule grabbedCapsule; 
    bool collecting = false;
    Timer collectionTimer;

	public override void _Ready() { 
        BodyEntered += StartCapsuleCollection; 
        BodyExited += CancelCapsuleCollection;
	}

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    private void CaptureCapsule(Element capsuleElement) {
        Creature c = DecideCreature(capsuleElement);
        PartyCreature partyCreature = new((Stats) c.BaseStats.Duplicate(), 0, c);
        associatedPlayer.party.AddToParty(partyCreature);

        grabbedCapsule.QueueFree();
        collectionTimer.QueueFree();
        grabbedCapsule = null;
        collecting = false;
        collects++;

        if (collects == maxCollects) {
            QueueFree();
        }
    }

    private Creature DecideCreature(Element creatureElement) {
        Creature creature = CreatureList.Ghost.ProtoGhost;

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
        GD.Print(creature.Element);
        return creature;
    }


    private void StartCapsuleCollection(Node3D capsule) {
        if (collecting) return;
        GD.Print(associatedPlayer.PlayerID + " Started collecting a capsule");
        
        Capsule c = (Capsule) capsule;
        collectionTimer = new() {WaitTime=timeToCollect, Autostart=true, Name="CollectionTimer"};

        c.LinearVelocity = Vector3.Zero;
        c.AngularVelocity = Vector3.Zero;
        c.Freeze = true;
        
        c.Position = Position + Vector3.Up;
        
        collecting = true;
        grabbedCapsule = c;
        
        if (GetTree().GetMultiplayer().IsServer()){
            collectionTimer.Timeout += () => { Rpc(nameof(CaptureCapsule), (int) c.element); };
        }
          
        AddChild(collectionTimer);
    }

    private void CancelCapsuleCollection(Node3D capsule) {
        if (!collecting) return;
        if (capsule != grabbedCapsule) return;

        GD.Print(associatedPlayer.PlayerID + " stopped collecting a capsule");
        collectionTimer.Free();

        collecting = false;
        grabbedCapsule = null;
    }
}
