using Godot;
using System;
using System.Drawing;
using System.Reflection.Metadata.Ecma335;

namespace Critter 
{
    public partial class CapsuleSpawner : Node3D
    {
        [Export(PropertyHint.Range, "0,10")] float ballSpawnRate = 0.25f;
        [Export] int spawnLimit = 12;
        int ballSpawnForce = 1;
        int spawnedAmount = 0;
        PackedScene capsuleScene = GD.Load<PackedScene>("res://Scenes/Components/Capsule.tscn");

        Node3D CapsuleContainer;


        Timer timer = new() 
        {
            Autostart=true
        };

        public CapsuleSpawner() : this(new Node3D(){Name="CapsuleContainer"}, 0.25f, 12) {}
        public CapsuleSpawner(Node3D capsuleContainer, float spawnRate, int limit) {
            CapsuleContainer = capsuleContainer;
            spawnLimit = limit;
            ballSpawnRate = spawnRate;

            if (!CapsuleContainer.IsInsideTree()) {
                GetTree().Root.GetChild(1).AddChild(CapsuleContainer);
            }
        }

        public override void _Ready() {
            if (!GetTree().GetMultiplayer().IsServer()) return;
            timer.WaitTime = ballSpawnRate;
            AddChild(timer);
            timer.Timeout += () => {
                if (spawnedAmount >= spawnLimit-1) {
                    timer.Paused = true;
                }

                int rX = new Random().Next(-1, 1) * ballSpawnForce;
                int rY = new Random().Next(-1, 1) * ballSpawnForce;


                int element = new Random().Next(0,5);
                
                var args = new Variant[3] {
                    new Vector3(rX,1,rY),
                    new Vector3(0,0,0),
                    element
                };

                Rpc(nameof(SpawnCapsule), args);
            };
        }

        [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
        public void SpawnCapsule(Vector3 force, Vector3 point, Element element) {
            Capsule capsule = capsuleScene.Instantiate<Capsule>();
            
            capsule.element = element;
            capsule.Name = $"Capsule {spawnedAmount}";

            CapsuleContainer.AddChild(capsule);
            
             
            capsule.Push(force);
            spawnedAmount++;
        }
    }
}