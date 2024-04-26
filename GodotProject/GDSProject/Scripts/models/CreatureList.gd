class_name CreatureList

static var all = {
	"Protoghost": Ghost.proto_ghost,
	"Protofire": Fire.proto_fire,
	"Protograss": Grass.proto_grass,
	"Protoelectric": Electric.proto_electric,
	"Protowater": Water.proto_water
}

static var All:
	get: return all

class Ghost:
	static var proto_ghost = Creature.new(
		"Protoghost",
		load("res://Assets/Images/Protoghost.png"),
		Creature.Element.GHOST,
		Stats.new(1, 2, 2, 10)
	)

class Water:
	static var proto_water = Creature.new(
		"Protowater",
		load("res://Assets/Images/Protowater.png"),
		Creature.Element.WATER,
		Stats.new(1, 1, 2, 8)
	)

class Fire:
	static var proto_fire = Creature.new(
		"Protofire",
		load("res://Assets/Images/Protofire.png"),
		Creature.Element.FIRE,
		Stats.new(1, 1, 1, 12)
	)

class Electric:
	static var proto_electric = Creature.new(
		"Protoelectric",
		load("res://Assets/Images/Protoelectric.png"),
		Creature.Element.ELECTRIC,
		Stats.new(2, 2, 1, 6)
	)

class Grass:
	static var proto_grass = Creature.new(
		"Protograss",
		load("res://Assets/Images/Protograss.png"),
		Creature.Element.GRASS,
		Stats.new(3, 1, 3, 4)
	)
