extends Node3D

enum GameStates {
	START,
	TEAM_BUILDING,
	BATTLE,
	END
}

signal GameStateChange

static var CollectionZoneLocations = [
	Vector3(-7, -3, 4),
	Vector3(7, -3, 4),
	Vector3(7, -3, -4),
	Vector3(-7, -3, 4)
]

var playerScene = preload("res://Scenes/Components/Player.tscn")
var CollectionZoneScene = preload("res://Scenes/Components/CollectionZone.tscn")
var BoardCreatureScene = preload("res://Scenes/Components/BoardCreature.tscn")

var CurrentState = GameStates.START
var players = []
var battlePlayers = {}
var spectators = []

var ScoreToWin = 4
var CapsulesPerTB = 12
var CapsulesCollected = 0

var PlayersContainer
var BoardCreaturesContainer
var CapsulesContainer

var mp

func set_up():
	mp = get_tree().get_multiplayer()
	print("Game state initialized.")

	PlayersContainer = get_node("/root/Node3D/Players")
	CapsulesContainer = get_node("/root/Node3D/Capsules")
	BoardCreaturesContainer = get_node("/root/Node3D/Creatures")
	var startButton = get_node("/root/Node3D/Debug/Button4")

	startButton.button_down.connect(start_game)  


func to_dict():
	var gameState = {}
	var _players = []

	for player in players:
		_players.append(player.to_dict())

	gameState["CurrentState"] = CurrentState
	gameState["players"] = _players
	gameState["CapsulesCollected"] = CapsulesCollected

	return gameState


func connect_player(id):
	spawn_player.rpc(id)

	var args = to_dict()
	sync_gamestate.rpc_id(id, args)
	print("!!! PLAYER CONNECTED !!!", id)


@rpc("any_peer")
func sync_gamestate(gameState):
	var _players = gameState["players"]

	for i in range(_players.size() - 1):
		var player = playerScene.instantiate()
		player.name = str(_players[i]["PlayerID"])
		player.PlayerID = _players[i]["PlayerID"]
		
		PlayersContainer.add_child(player)
		player.cursor.position = _players[i]["CursorPosition"]

	CurrentState = gameState["CurrentState"]
	CapsulesCollected = gameState["CapsulesCollected"]


func disconnect_player(id):
	rpc("free_player", id)
	print("!!! PLAYER DISCONNECTED !!!", id)


@rpc("authority", "call_local")
func spawn_player(playerID):
	var newPlayer = playerScene.instantiate()
	newPlayer.PlayerID = playerID
	newPlayer.name = str(playerID)
	
	PlayersContainer.add_child(newPlayer)
	players.append(newPlayer) 
	print("New player spawned", playerID)


@rpc("authority", "call_local")
func free_player(id):
	var player = PlayersContainer.get_node(str(id))

	print("Player Freed", id) 
	players.erase(player)
	player.queue_free()


@rpc("authority", "call_local")
func switch_state(newState):
	emit_signal("GameStateChange")
	CurrentState = newState
	print("Gamestate swapped:", CurrentState)


func start_game():
	if CurrentState != GameStates.START:
		return
	if not get_tree().get_multiplayer().is_server():
		return 
		
	var readyCount = 0
	for p in players:
		if p.StartReady:
			readyCount += 1
	
	if readyCount != players.size():
		print("readyCount does not equal player count.")
		return

	print("===== Starting game =====")
	rpc("switch_state", GameStates.TEAM_BUILDING)
	start_team_building()


func start_team_building():
	print("===== Teambuilding Started =====")
	CapsulesCollected = 0
	spawn_capsule_spawner.rpc()
	
	var index = 0
	for p in players:
		var args = [p.PlayerID, index, players.size()]
		spawn_collection_zone.rpc(args[0], args[1], args[2])
		index += 1



@rpc("authority", "call_local")
func spawn_capsule_spawner():
	var capsuleSpawner = CapsuleSpawner.new(CapsulesContainer, 0.25, CapsulesPerTB)
	capsuleSpawner.name = "CapsuleSpawner"
	get_tree().root.get_child(1).add_child(capsuleSpawner)

@rpc("authority", "call_local")
func spawn_collection_zone(playerId, index, totalPlayers):
	var p = PlayersContainer.get_node(str(playerId))

	var collectionZone = CollectionZoneScene.instantiate()
	collectionZone.name = str(playerId)
	collectionZone.associated_player = p
	collectionZone.max_collects = CapsulesPerTB / totalPlayers
	collectionZone.capsule_collected.connect(capsule_collected)

	get_tree().root.get_child(1).add_child(collectionZone)

	collectionZone.position = CollectionZoneLocations[index] 

func capsule_collected():
	CapsulesCollected += 1
	if get_tree().get_multiplayer().is_server():
		if CapsulesCollected == CapsulesPerTB:
			end_team_building()


@rpc("authority", "call_local")
func remove_ball_spawner():
	get_tree().root.get_child(1).get_node("CapsuleSpawner").queue_free()


func end_team_building():
	rpc("remove_ball_spawner")
	rpc("switch_state", GameStates.BATTLE)
	start_battle()


func start_battle():
	if not get_tree().get_multiplayer().is_server():
		return
	print("===== Battle Started =====")
	var i = 0
	battlePlayers = {} 
	for p in players:
		var creatureIndex = 0
		battlePlayers[p] = p.party.size()
		for c in p.party:
			var args = [i, p.PlayerID, c.creature_name, creatureIndex]
			summon_creature.rpc(args[0], args[1], args[2], args[3])
			creatureIndex += 1
		i += 1


static var all = {
	"Protoghost": CreatureList.Ghost.proto_ghost,
	"Protofire": CreatureList.Fire.proto_fire,
	"Protograss": CreatureList.Grass.proto_grass,
	"Protoelectric": CreatureList.Electric.proto_electric,
	"Protowater": CreatureList.Water.proto_water
}

@rpc("authority", "call_local")
func summon_creature(playerIndex, playerId, creatureId, creatureIndex):
	if not BoardCreaturesContainer.has_node(str(playerId)):
		var playerBoardContainer = Node3D.new();
		playerBoardContainer.name = (str(playerId))
		BoardCreaturesContainer.add_child(playerBoardContainer)
	
	var c = all[creatureId]
	var pC = PartyCreature.new(c.base_stats.duplicate(), 0, c, playerId)
	var bC = BoardCreatureScene.instantiate()
	bC.PartyCreature = pC
	bC.name = str(creatureIndex)
	bC.Fient.connect(creature_feinted)
	BoardCreaturesContainer.get_node(str(playerId)).add_child(bC)
	bC.position = CollectionZoneLocations[playerIndex] + Vector3.UP * 3

func creature_feinted(c):
	if not get_tree().get_multiplayer().is_server():
		return

	var p = PlayersContainer.get_node(str(c.party_creature.OwnerID))
	battlePlayers[p] -= 1
	print(battlePlayers)
	
	if battlePlayers[p] <= 0:
		battlePlayers.erase(p)

	if battlePlayers.keys().size() == 1:
		var alive = battlePlayers.keys()
		print("One player remains.")
		var winner = alive[0]
		winner.Score += 1
		print(winner, winner.Score)
		end_battle()


func end_battle():
	rpc("clear_board_creatures")
	var winner = check_wincon()
	if winner != null:
		pass
	else:
		loop_game_state()


@rpc("authority", "call_local")
func clear_board_creatures():
	for c in BoardCreaturesContainer.get_children():
		c.queue_free()


func loop_game_state():
	for p in players:
		p.party.clear()

	print("===== Game state looping =====")
	rpc("switch_state", GameStates.TEAM_BUILDING)
	start_team_building()


func check_wincon():
	for player in players:
		if player.Score == ScoreToWin:
			return player
	return null
