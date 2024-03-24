# Capsule Pop
A game created with Godot with specific intention to work with Discord activities.

# What is Capsule Pop?
Capsule Pop is a game created for the Discord App Pitches 2024. It utilizes Godot and the Discord
embedded app SDK to run directly inside of a Discord client. This project serves a short term
learning project for Godot and it's multiplayer functionallity. I intend to use Godot for my future
long term game projects so this is a perfect way to learn about the engine.

### Discord App Pitches 2024 Category Target
This game will be targeting the compete section as well as they collect and create section.

---

# Game Loop
> [!TIP]
> If you are not interested in learning about the game loop and just want to play hop down to the
> [How to Play](#how-to-play) section.

The main gameplay loop currently has 4 states:
- [Game Start](#game-start)
- [Team Building](#team-building)
- [Battle](#battle)
- [Game End](#game-end)

## Game Start
This is where players will connect to the server, and sit in lobby until the game has been started.
The game will start when all players are connected and toggle a ready check.

## Team Building
Team building has all of the players interacting with a table, the table will have physics properties,
each player can tilt the board in any direction, and eventually the board will return to a neutral. 
The goal of moving the table around is to have CAPSULES fall onto your side of the board and collected
to your party.

**CAPSULES** are containers containing CREATURES to collect to your party, these elements can be used during the
battle phase. CAPSULES do not depict which specific CREATURE is in them but do indicate what type of element
the CREATURE is.

Once a player has collect a certain amount of creatures for the team building phase they cannot collect any more.
Although they can still control how the board is tilted.

## Battle
This is where points get determined. Each combat victory results in 1 point being awarded to the victor. At this
stage of the game you play your collected CREATURES onto the board. The player chooses how these CREATURES move
but selected a group of them and clicking around the board. The CREATURES then attack on an automated timer.

**CREATURES** are your gladiators, each CREATURE has a different element, attack style, attack speed, and movespeed.

## Game End
When a certain threshold of points is met, the player meeting that threshold is considered the winner and the game
is reset.

---

# Controls
The player will be only using their mouse to interact with the game. Potential keyboard shortcuts can be considered
for selected individual creatures in battle mode.

---
# How to Play
No current available way to play, please check back in the future.

---
