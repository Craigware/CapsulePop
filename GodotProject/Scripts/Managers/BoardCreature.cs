using Godot;
using System;
using Critter;
using System.Globalization;

public partial class BoardCreature : Grabbable
{
    [Export] public PartyCreature PartyCreature;
    private Sprite3D gfx;
    private CollisionShape3D hitbox;
    private Timer attackTimer;
    private Area3D attackRange;
    bool Dead = false;

    [Signal]
    public delegate void FientEventHandler(BoardCreature c);

	public override void _Ready() {
        gfx = GetNode<Sprite3D>("./Gfx");
        hitbox = GetNode<CollisionShape3D>("./Hitbox");
        attackTimer = GetNode<Timer>("./AttackTimer");
        attackRange = GetNode<Area3D>("./Range");

        gfx.Texture = PartyCreature.Creature.Sprite;
        attackTimer.WaitTime = 1;
        var colShape = attackRange.GetChild<CollisionShape3D>(0);
        colShape.Shape = new CylinderShape3D() {
            Height = 2,
            Radius = PartyCreature.CurrentStats.Range
        };

        attackTimer.Timeout += Attack;
        
        attackTimer.Start();
    }

    public override void _Process(double delta) {
        if (GetTree().GetMultiplayer().GetUniqueId() == PartyCreature.OwnerID) {
            GetNode("Viewport").GetNode("Vbox").GetNode<RichTextLabel>("Text").Text = "[center]" + PartyCreature.CurrentStats.Health + "/" + PartyCreature.Creature.BaseStats.Health + "[/center]";
        }
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    public void Damage(int amount) {
        PartyCreature.CurrentStats.Health -= amount;

        if (PartyCreature.CurrentStats.Health <= 0 && Dead != true) {
            Dead = true;
            Rpc(nameof(Die));
        }
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    public void Heal(int amount) {
        PartyCreature.CurrentStats.Health += amount;
        if (PartyCreature.Creature.BaseStats.Health < PartyCreature.CurrentStats.Health) {
            PartyCreature.CurrentStats.Health = PartyCreature.Creature.BaseStats.Health;
        }
    }
    
    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    public void Die() {
        EmitSignal(SignalName.Fient, this);
        QueueFree();
    }

    public void Attack() {
        if (!GetTree().GetMultiplayer().IsServer()) return;

        var target = FindTarget();
        if (target == null) return;

        switch (PartyCreature.Creature.Element) {
            case Element.WATER:
                WaterAttack(target);
                break;
            case Element.GRASS:
                GrassAttack(target);
                break;
            case Element.FIRE:
                FireAttack(target);
                break;
            case Element.ELECTRIC:
                ElectricAttack(target);
                break;
            case Element.GHOST:
                GhostAttack(target);
                break;
        }
    }

    private BoardCreature FindTarget() {
        var bodies = attackRange.GetOverlappingBodies();
        bodies.Remove(this);

        Godot.Collections.Array<Node3D> toRemove = new();
        foreach(var body in bodies) {
            if (body is StaticBody3D) {
                toRemove.Add(body);
            }

            if (body is BoardCreature b) {
                if (b.PartyCreature.OwnerID == this.PartyCreature.OwnerID) {
                    toRemove.Add(body);
                }
            }
        }

        foreach(var body in toRemove) {
            bodies.Remove(body);
        }

        if (bodies.Count == 0) return null;
        if (bodies.Count == 1) return (BoardCreature) bodies[0];

        var randomIndex = new Random().Next(0,bodies.Count);
        return (BoardCreature) bodies[0];
    }

    private void WaterAttack(BoardCreature target) {
        // animation
        if (GetTree().GetMultiplayer().IsServer()) {
            target.Rpc(nameof(Damage), 1);
        }
    }

    private void ElectricAttack(BoardCreature target) {
        if (GetTree().GetMultiplayer().IsServer()) {
            target.Rpc(nameof(Damage), 1);
        }
    }

    private void FireAttack(BoardCreature target) {
        if (GetTree().GetMultiplayer().IsServer()) {
            target.Rpc(nameof(Damage), 1);
        }
    }

    private void GhostAttack(BoardCreature target) {
        if (GetTree().GetMultiplayer().IsServer()) {
            target.Rpc(nameof(Damage), 1);
        }
    }

    private void GrassAttack(BoardCreature target) {
        if (GetTree().GetMultiplayer().IsServer()) {
            target.Rpc(nameof(Damage), 1);
        }
    }
}
