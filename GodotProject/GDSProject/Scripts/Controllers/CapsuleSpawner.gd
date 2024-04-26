extends Node3D

class_name CapsuleSpawner

@export var ball_spawn_rate: float = 0.25
@export var spawn_limit: int = 12
var ball_spawn_force: int = 1
var spawned_amount: int = 0
var capsule_scene = preload("res://Scenes/Components/Capsule.tscn")

var capsule_container: Node3D

var timer = Timer.new()

func _ready():
    if not get_tree().get_multiplayer().is_server():
        return
    timer.wait_time = ball_spawn_rate
    add_child(timer)
    timer.timeout.connect(_on_timer_timeout)

    if not capsule_container.is_inside_tree():
        get_tree().get_root().get_child(1).add_child(capsule_container)

    timer.start()

func _init(capsule_container_ref: Node3D = Node3D.new(), spawn_rate: float = 0.25, limit: int = 12):
    capsule_container = capsule_container_ref
    spawn_limit = limit
    ball_spawn_rate = spawn_rate

func _on_timer_timeout():
    if spawned_amount >= spawn_limit - 1:
        timer.paused = true

    var r_x = randf_range(-1, 1) * ball_spawn_force
    var r_y = randf_range(-1, 1) * ball_spawn_force

    var element = randi() % 5

    var args = [Vector3(r_x, 1, r_y), Vector3.ZERO, element]

    spawn_capsule.rpc(args[0], args[1], args[2])

@rpc("authority", "call_local")
func spawn_capsule(force: Vector3, point: Vector3, element: int):
    var capsule = capsule_scene.instantiate()
    if capsule is Capsule:
        capsule.element = element
        capsule.name = "Capsule " + str(spawned_amount)
        capsule_container.add_child(capsule)
        capsule.push(force)
        spawned_amount += 1