extends Control

var server_script = load("res://Scripts/Controllers/Server.gd")
var client_script = load("res://Scripts/Controllers/Client.gd")

func host():
    var server = Server.new()
    add_child(server)

func join():
    var client = Client.new()
    add_child(client)