extends Grabbable

class_name Capsule

var element: Creature.Element
var rarity: PartyCreature.Rarity
var material: StandardMaterial3D

var element_color_pairs = {
    Creature.Element.FIRE: Color("#FF253D"),
    Creature.Element.WATER: Color("#889cc5"),
    Creature.Element.GRASS: Color("#1f9343"),
    Creature.Element.ELECTRIC: Color("#c3f246"),
    Creature.Element.GHOST: Color("#6f5475")
}

func _ready():
    var mi = $MeshInstance3D
    var sm = StandardMaterial3D.new()
    sm.albedo_color = element_color_pairs[element]
    mi.material_override = sm