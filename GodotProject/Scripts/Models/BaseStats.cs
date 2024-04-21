using Godot;

namespace Critter {
    [GlobalClass]
    public partial class Stats : Resource {
        [Export] public int AttackSpeed { get; set; }
        [Export] public int MoveSpeed { get; set; }
        [Export] public int Range { get; set; }
        [Export] public int Health { get; set; }

        public Stats() : this(1,1,1,1){}
        public Stats(int attackSpeed, int moveSpeed, int range, int health) {
            Health = health;
            MoveSpeed = moveSpeed;
            Range = range;
            AttackSpeed = attackSpeed;
        }
    }
}