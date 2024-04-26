extends Resource

class_name Creature

enum Element {
    GRASS,
    FIRE,
    WATER,
    ELECTRIC,
    GHOST
}

@export var sprite : Texture2D
@export var element : Element
@export var base_stats : Stats
@export var creature_name : String
    
func _init(_name="placeholder", _sprite=null, _element=Element.FIRE, _base_stats=Stats.new()):
    self.sprite = _sprite
    self.element = _element
    self.base_stats = _base_stats
    self.creature_name = _name



