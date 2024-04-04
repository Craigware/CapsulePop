using Godot;
using System;

public partial class Table : RigidBody3D
{
	public override void _Ready() {
	}

	public override void _Process(double delta) {
	}

	public void Push(Vector3 position) {
		ApplyForce(Vector3.Down*20, position);
	}
}
