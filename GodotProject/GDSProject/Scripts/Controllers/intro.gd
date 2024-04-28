extends Control

var index = 0

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _input(event):
	if Input.is_action_just_pressed("Grab"):
		index += 1
		swap_visisble(index)

func swap_visisble(i):
	if (i >= get_child_count() - 1):
		get_tree().change_scene_to_file("res://Scenes/Levels/Client.tscn")
		print("?")
		return
	else:
		print("!")
		var vis = get_child(i+1) as TextureRect # +1 accounts for background color
		vis.visible = true
		var hid = get_child(i) as TextureRect
		hid.visible = false
