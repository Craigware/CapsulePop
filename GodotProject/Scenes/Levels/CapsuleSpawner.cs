using Critter;
using Godot;
using System;

public partial class CapsuleSpawner : Node3D
{
    [Export(PropertyHint.Range, "0,10")] float ballSpawnRate = 0.25f;
    [Export] int spawnLimit = 12;
    int spawnedAmount = 0;
    PackedScene capsuleScene = GD.Load<PackedScene>("res://Scenes/Components/Capsule.tscn");

    Timer timer = new() {
        Autostart=true
    };

	public override void _Ready()
	{
        timer.WaitTime = ballSpawnRate;
        AddChild(timer);
        timer.Timeout += SpawnCapsule;
	}

    public void SpawnCapsule() {
        if (spawnedAmount >= spawnLimit-1) {
            timer.Paused = true;
        }

        Capsule capsule = capsuleScene.Instantiate<Capsule>();
        Element element = (Element) new Random().Next(0,5);
        capsule.element = element;
        GetTree().Root.AddChild(capsule);
        capsule.GlobalPosition = new Vector3(0,-2,0);
        spawnedAmount++;
    }
}
