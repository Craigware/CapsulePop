extends Grabbable

class_name BoardCreature

var PartyCreature : PartyCreature

var gfx: Sprite3D
var hitbox: CollisionShape3D
var attackTimer: Timer
var attackRange: Area3D
var Dead = false

signal Fient

func _ready():
	gfx = $Gfx
	hitbox = $Hitbox
	attackTimer = $AttackTimer
	attackRange = $Range

	gfx.texture = PartyCreature.creature.sprite
	attackTimer.wait_time = 1
	var colShape = attackRange.get_child(0)
	colShape.shape = CylinderShape3D.new()
	colShape.shape.height = 2
	colShape.shape.radius = PartyCreature.current_stats.range

	attackTimer.timeout.connect(attack)
	attackTimer.start()

func _process(delta):
	if get_tree().get_multiplayer().get_unique_id() == PartyCreature.owner_id:
		$Viewport/Vbox/Text.text = "[center]" + str(PartyCreature.current_stats.health) + "/" + str(PartyCreature.creature.base_stats.health) + "[/center]"


@rpc("authority", "call_local")
func damage(amount):
	PartyCreature.current_stats.health -= amount

	if PartyCreature.current_stats.health <= 0 and not Dead:
		Dead = true
		die.rpc()

@rpc("authority", "call_local")
func heal(amount):
	PartyCreature.current_stats.health += amount
	if PartyCreature.creature.base_stats.health < PartyCreature.current_stats.health:
		PartyCreature.current_stats.health = PartyCreature.creature.base_stats.health


@rpc("authority", "call_local")
func die():
	Fient.emit()
	queue_free()


func attack():
	if not get_tree().get_multiplayer().is_server():
		return

	var target = find_target()
	if not target:
		return

	match PartyCreature.creature.element:
		Creature.Element.WATER:
			water_attack(target)
		Creature.Element.GRASS:
			grass_attack(target)
		Creature.Element.FIRE:
			fire_attack(target)
		Creature.Element.ELECTRIC:
			electric_attack(target)
		Creature.Element.GHOST:
			ghost_attack(target)

func find_target():
	var bodies = attackRange.get_overlapping_bodies()
	bodies.erase(self)

	var toRemove = []
	for body in bodies:
		if body is StaticBody3D:
			toRemove.append(body)

		if body is BoardCreature and body.PartyCreature.owner_id == PartyCreature.owner_id:
			toRemove.append(body)

	for body in toRemove:
		bodies.erase(body)

	if bodies.size() == 0:
		return null
	if bodies.size() == 1:
		return bodies[0]

	var randomIndex = randi() % bodies.size()
	return bodies[randomIndex]

func water_attack(target):
	if get_tree().get_multiplayer().is_server():
		target.damage.rpc(1)

func electric_attack(target):
	if get_tree().get_multiplayer().is_server():
		target.damage.rpc(1)

func fire_attack(target):
	if get_tree().get_multiplayer().is_server():
		target.damage.rpc(1)
    	

func ghost_attack(target):
	if get_tree().get_multiplayer().is_server():
		target.damage.rpc(1)

func grass_attack(target):
	if get_tree().get_multiplayer().is_server():
		target.damage.rpc(1)
