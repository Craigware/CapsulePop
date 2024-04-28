extends Control

var ipLine
var portLine

# Called when the node enters the scene tree for the first time.
func _ready():
    ipLine = get_node("./Ip") as LineEdit
    portLine = get_node("./Port") as LineEdit
	
func attempt_connect(ip, port):
    pass