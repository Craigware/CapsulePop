extends Control

const CameraXZPos = 10
const CameraYPos = 5
const RayLength = 1000.0

var Score = 0
var StartReady = false
var PlayerID = 1
var Stunned = false

@export var nameDisplay : RichTextLabel

var playerName = ""
var PlayerName: String :
	set(value):
		set_player_name(value)
	get:
		return playerName 

@export var PlayerIcon : Texture2D

var CursorColor: Color :
	get:
		return cursor.modulate
	set(value):
		cursor.modulate = value

var party = []

@export var cursor : Control

var grabbed = null
var relative = Vector2.ZERO
var velocity = Vector2.ZERO
var root: Node3D

func set_player_name(value):
	playerName = value
	nameDisplay.text = "[center]" + str(PlayerName) + "[/center]"

func _ready():
	Input.mouse_mode = Input.MOUSE_MODE_HIDDEN
	root = get_tree().root.get_child(1)
	var readyButton = get_node("/root/Node3D/Debug/Button3")
	nameDisplay.text = "[center]" + str(PlayerID) + "[/center]"
	
	if get_tree().get_multiplayer().get_unique_id() == PlayerID:
		readyButton.button_down.connect(toggle_ready.rpc)

	Input.use_accumulated_input = true

func to_dict() -> Dictionary:
	var player = {}
    
	player["PlayerName"] = PlayerName
	player["PlayerIcon"] = PlayerIcon
	player["PlayerID"] = PlayerID
	player["Score"] = Score
	player["StartReady"] = StartReady
	player["CursorPosition"] = cursor.position
	player["CursorColor"] = CursorColor

	# Party will also have to be here eventually when created are made.

	return player

func _input(event):
	if get_tree().get_multiplayer().get_unique_id() != PlayerID:
		return
	if Stunned:
		return
	
	# Grab
	if Input.is_action_just_pressed("Grab"):
		grab(get_viewport().get_mouse_position())

	if event is InputEventMouseMotion:
		update_cursor_position.rpc(event.position)
		relative = event.relative
		velocity = event.velocity

	# Smack
	if Input.is_action_just_pressed("Select"):
		smack()

	if Input.is_action_just_pressed("Escape"):
		get_tree().quit()

func _process(delta):
	if Input.is_action_just_released("Grab"):
		fling(relative)

@rpc("any_peer", "call_local")
func update_cursor_position(new_pos):
	cursor.position = new_pos

func fling(relative_position):
	if grabbed == null:
		return

	grabbed.set_grabbed()

	var force = Vector3(
		clamp(relative_position.x / 10, -1, 1) * 10,
		0,
		clamp(relative_position.y / 10, -1, 1) * 10
	)

	
	grabbed.fling(force)
	grabbed = null

func grab(mouse_position):
	var target = create_ray_cast(mouse_position, 1000)

	var grabbable = target as Grabbable
	
	if grabbable == null:
		return;

	if grabbable.IsGrabbed:
		return

	grabbable.set_grabbed(PlayerID)
	grabbed = grabbable


func smack():
	var smack_region = get_node("Cursor/Area2D")
	var smacked_areas = smack_region.get_overlapping_areas()
	if smacked_areas.size() == 0:
		return

	for area in smacked_areas:
		var smacked_player = area.get_parent().get_parent()
		print(smacked_player.PlayerID, " Smacked.")
		smacked_player.set_stunned(1000)

func set_stunned(duration=1000):
	if not Stunned:
		stun(duration)

func stun(duration):
	Stunned = true

	var timer = Timer.new()
	timer.wait_time = duration / 1000

	timer.timeout.connect(self, "on_unstun_timeout")
	add_child(timer)
	timer.start()

func on_unstun_timeout():
	print("Unstunned")
	Stunned = false

@rpc("any_peer", "call_local")
func toggle_ready():
	StartReady = not StartReady

func create_ray_cast(mouse_position, ray_len):
	var world_space = get_tree().root.get_child(1).get_world_3d().direct_space_state

	var camera = get_viewport().get_camera_3d()
	var from = camera.project_ray_origin(mouse_position)
	var to = from + camera.project_ray_normal(mouse_position) * ray_len

	var query = PhysicsRayQueryParameters3D.create(from, to)

	var res = world_space.intersect_ray(query)

	if "collider" in res:
		return res["collider"]
	else:
		return null

@rpc("authority", "call_local")
func mouse_position_to_world_space(d):
	var mouse_position = get_viewport().get_mouse_position()
	var camera = get_viewport().get_camera_3d()
	var from = camera.project_ray_origin(mouse_position)
	var to = from + camera.project_ray_normal(mouse_position) * d
	return to