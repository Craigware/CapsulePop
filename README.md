# Capsule Pop
*Created for Discord App Pitches 2024, Capsule Pop* is a game constructed with Godot, with
the intention of being run directly within the Discord client using the Discord Embedded App
SDK. Capsule Pop serves as a short term learning project for Godot and its multiplayer
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
The "Team Building" phase consists of players interacting with capsules that spawn over time. These **CAPSULES** can be flung around, you can try and capture your own **CREATURES** to build a perfect team
or sabotage enemy teams but flinging **CAPSULES** into their collection zone.

**CAPSULES** are containers containing **CREATURES**, which are added to your party. Each **CREATURE** contains
an associated element, such as fire or water. Each **CAPSULE** will indicate the **CREATURE**'s associated
element, but will not indicate the specific **CREATURE** within.

Once a player has collected a certain amount of **CAPSULES** during the "Team Building" phase, they cannot collect any more. However, they can still interact with the other **CAPSULES** and players.

## Battle
During the “Battle Stage,” players must play the **CREATURES** they have acquired onto the board. The player chooses how these **CREATURES** move by selecting a group of them and clicking around the board. The **CREATURES** then attack periodically through an automated timer.

**CREATURES** are your gladiators, with each **CREATURE** having a unique element, attack style, attack speed.

Points are awarded after each battle, with each combat victory resulting in 1 point being awarded to that player.

## Game End

When a player meets a certain threshold of points, that player will be deemed the winner and the game will end.

---

# Controls
Only a mouse is required to interact with the game.

LEFT MOUSE : Grab and release selectable objects. IE **CAPSULES** and **CREATURES**.
RIGHT MOUSE : Smack players within your cursors range.

---

# How to Play
Server Setup:


Client Setup:

---

# Out of time scope ideas
*Man this list got long..*
- [ ] Disconnect / Reconnect functionality
- [ ] Animations
- [ ] Overround Teambuilding
- [ ] Creature rarity
- [ ] Creature variety
- [ ] Touch controls
- [ ] Powerups
- [ ] Player Teaming
