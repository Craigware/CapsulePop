extends Resource

class_name Stats

@export var attack_speed : int
@export var move_speed : int
@export var range : int
@export var health : int


func _init(attackSpeed=1, moveSpeed=1, _range=1, _health=1):
	self.attack_speed = attackSpeed
	self.move_speed = moveSpeed
	self.range = _range
	self.health = _health
