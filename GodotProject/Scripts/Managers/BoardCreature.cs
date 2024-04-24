using Godot;
using System;
using Critter;

public partial class BoardCreature : Grabbable
{
    [Export] public PartyCreature PartyCreature;
    private Sprite3D gfx;
    private Timer attackTimer;
    private Area3D attackRange;

	public override void _Ready() {
        gfx = GetNode<Sprite3D>("./Gfx");
        attackTimer = GetNode<Timer>("./AttackTimer");
        attackRange = GetNode<Area3D>("./Range");

        gfx.Texture = PartyCreature.Creature.Sprite;

        switch (PartyCreature.Creature.Element) {
            case Element.WATER:
                attackTimer.Timeout += WaterAttack;
                break;
            case Element.GRASS:
                break;
            case Element.FIRE:
                break;
            case Element.ELECTRIC:
                break;
            case Element.GHOST:
                break;
        }
        attackTimer.Start();
    }

    public void Damage(int amount) {

    }
    
    public void Heal(int amount) {

    }

    private BoardCreature FindTarget() {
        var bodies = attackRange.GetOverlappingBodies();
        GD.Print(bodies);
        return new BoardCreature();
    }

    private void WaterAttack() {
        FindTarget();
        GD.Print("WAHTA ATTAK");
    }

    private void ElectricAttack() {

    }

    private void FireAttack() {

    }

    private void GhostAttack() {

    }

    private void GrassAttack() {

    }
}
