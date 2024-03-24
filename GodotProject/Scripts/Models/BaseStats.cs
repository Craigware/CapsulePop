using Godot;

namespace Critter {
    [GlobalClass]
    public partial class BaseStats : Resource {
        [Export] public int AttackSpeed { get; set; }
        [Export] public int MoveSpeed { get; set; }
        [Export] public int Range { get; set; }
        [Export] public int Health { get; set; }
    }
}