extends Resource

class_name PartyCreature

enum Rarity {
    COMMON,
    RARE,
    EPIC,
    LEGENDARY
}

var current_stats : Stats
var owner_id : int
var rarity : Rarity
var creature : Creature

func _init(_current_stats, _rarity=Rarity.COMMON, _creature=null, _owner_id=0):
    self.current_stats = _current_stats
    self.rarity = _rarity
    self.creature = _creature
    self.owner_id = _owner_id