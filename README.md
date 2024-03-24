# Capsule Pop
*Created for Discord App Pitches 2024, Capsule Pop* is a game constructed with Godot, with
the intention of being run directly within the Discord client using the Discord Embedded App
SDK. This Project serves as a short term learning project for Godot and it's multiplayer
functionality. I intend to use Godot for my future long term game projects, so this event
is the perfect opportunity to learn more about the engine. 


### Discord App Pitches 2024 Category Target
Capsule Pop will be targeting the “Compete” and “Collect and Create” sections for Discord 
App Pitches 2024.

---

# Game Loop
> [!TIP]
> If you are not interested in learning about the game loop and just want to play, hop down to the
> [How to Play](#how-to-play) section.

The main gameplay loop contains 4 phases:
- [Game Start](#game-start)
- [Team Building](#team-building)
- [Battle](#battle)
- [Game End](#game-end)

## Game Start
During this phase, players will connect to a server, and sit in a lobby until the game has been
started. The game will start when all connected players toggle a ready check.

## Team Building
The "Team Building" phase consists of players interacting with a table. The table will have physics properties, allowing players to tilt the board in any direction. Players will attempt to have CAPSULES fall onto their side of the board by shifting around the table, adding to their party.

**CAPSULES** are containers containing **CREATURES**, which are added to your party. Each creature contains
an associated element, such as fire or water. Each **CAPSULE** will indicate the **CREATURE**'s associated
element, but will not indicate the specific **CREATURE** within.

Once a player has collected a certain amount of **CAPSULES** during the "Team Building" phase, they cannot collect any more. However, they can still continue to tilt the board.

## Battle
During the “Battle Stage,” players must play the **CREATURES** they have acquired onto the board. The player chooses how these **CREATURES** move by selecting a group of them and clicking around the board. The **CREATURES** then attack periodically through an automated timer.

**CREATURES** are your gladiators, with each **CREATURE** having a unique element, attack style, attack speed.

Points are awarded after each battle, with each combat victory resulting in 1 point being awarded to that player.

## Game End

When a player meets a certain threshold of points, that player will be deemed the winner and the game will reset.

---

# Controls
Only a mouse is required to interact with the game. Players may opt to use keyboard shortcuts if desired.

---

# How to Play
No current available way to play, please check back in the future.

---
