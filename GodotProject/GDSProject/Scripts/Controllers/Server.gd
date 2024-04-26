extends Node

class_name Server

const PORT = 52401
const MAX_PLAYERS = 4

var mp : MultiplayerAPI
var player_ids = []

var game_state_manager

func _ready():
    game_state_manager = get_node("/root/GameState")
    mp = get_tree().get_multiplayer()

    var peer = ENetMultiplayerPeer.new()
    peer.create_server(PORT, MAX_PLAYERS)
    mp.multiplayer_peer = peer

    mp.peer_connected.connect(game_state_manager.connect_player)
    mp.peer_disconnected.connect(game_state_manager.disconnect_player)
    
    print("Server Started")