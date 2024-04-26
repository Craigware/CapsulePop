extends RigidBody3D

class_name Grabbable

var IsGrabbed = false
var Grabber = null

func fling(velocity):
	push.rpc(velocity)

@rpc("any_peer", "call_local")
func push(force):
	if not get_tree().get_multiplayer().is_server():
		return
	print("impulse")
	apply_impulse(force)

func _physics_process(delta):
	if IsGrabbed and get_tree().get_multiplayer().is_server():
		position = Grabber.mouse_position_to_world_space(12)

	if get_tree().get_multiplayer().is_server():
		update_position.rpc(position)

func set_grabbed(grabber_id = null):
	if grabber_id != null and not IsGrabbed:
		grabbed.rpc(grabber_id)
	else:
		print("release")
		released.rpc()

@rpc("any_peer", "call_local")
func grabbed(grabber_id):
	Grabber = get_node("/root/Node3D/Players/" + str(grabber_id))
	linear_velocity = Vector3.ZERO
	angular_velocity = Vector3.ZERO
	sleeping = true
	freeze = true
	IsGrabbed = true


@rpc("any_peer", "call_local")
func released():
	Grabber = null
	IsGrabbed = false
	freeze = false


@rpc("authority", "call_local")
func update_position(updated_position):
	position = updated_position


