extends Node

class_name Client

var mp : MultiplayerAPI
const development_ip = "127.0.0.1"
const port = 52401

func _ready():
    mp = get_tree().get_multiplayer()
    var peer = ENetMultiplayerPeer.new()
    peer.create_client(development_ip, port)
    mp.multiplayer_peer = peer
    
    print("Client created.")