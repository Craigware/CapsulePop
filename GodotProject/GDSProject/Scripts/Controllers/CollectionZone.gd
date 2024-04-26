extends Area3D

signal capsule_collected

var associated_player
var time_to_collect = 1.0
var max_collects = 1
var collects = 0

var grabbed_capsule
var collecting = false
var collection_timer

func _ready():
	body_entered.connect(start_capsule_collection)
	body_exited.connect(cancel_capsule_collection)

@rpc("authority", "call_local")
func _on_collection_timer_timeout():
	var capsule_element = grabbed_capsule.element
	var creature = decide_creature(capsule_element)
	associated_player.party.append(creature)

	grabbed_capsule.queue_free()
	collection_timer.queue_free()
	grabbed_capsule = null
	collecting = false
	collects += 1

	capsule_collected.emit()

	print(collects)
	if collects == max_collects:
		queue_free()


func decide_creature(creature_element):
	var creature = null

	match creature_element:
		Creature.Element.GHOST:
			creature = CreatureList.Ghost.proto_ghost
		Creature.Element.FIRE:
			creature = CreatureList.Fire.proto_fire
		Creature.Element.WATER:
			creature = CreatureList.Water.proto_water
		Creature.Element.GRASS:
			creature = CreatureList.Grass.proto_grass
		Creature.Element.ELECTRIC:
			creature = CreatureList.Electric.proto_electric

	print(creature.element)
	return creature


func start_capsule_collection(capsule):
	if collecting:
		return
	
	var c = capsule as Capsule
	if c == null:
		return
	
	collection_timer = Timer.new()
	collection_timer.wait_time = time_to_collect
	collection_timer.autostart = true
	collection_timer.name = "CollectionTimer"
	collection_timer.timeout.connect(_on_collection_timer_timeout.rpc)

	c.linear_velocity = Vector3.ZERO
	c.angular_velocity = Vector3.ZERO
	c.freeze = true
	c.position = global_transform.origin + Vector3.UP

	collecting = true
	grabbed_capsule = c

	if get_tree().get_multiplayer().is_server():
		add_child(collection_timer)


func cancel_capsule_collection(capsule):
	if not collecting:
		return
	if capsule != grabbed_capsule:
		return

	collection_timer.queue_free()

	collecting = false
	grabbed_capsule = null
