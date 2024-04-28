extends Node

class_name Client

var mp : MultiplayerAPI
var development_ip = "127.0.0.1"
var port = 52401
var peer

func _ready():
    mp = get_tree().get_multiplayer()
    peer = ENetMultiplayerPeer.new()
    
    
    attempt_connect(development_ip, port)
    print("Client created.")

func attempt_connect(ip, p):
    peer.create_client(ip, p)
    
    mp.multiplayer_peer = peer
    print("Client connected.")