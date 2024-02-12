# Group25Battleship
CISC 4900 group project repo

Battleship Game Demo

Goal: Create a demo of a match in the Battleship game that ani came up with. 

Walkthrough of Match: There will be 2 players, each with one battleship. While Under a Timer, each player will be required to make the descisions of (A) Choose which of the 3 areas on their side they would like to move to(or stay in) and (B) which of the 3 areas on the opponent’s side they would like to fire their cannon at. Once the timer on each turn is up, the locked in choices made will take place. This will repeat until one or both players have taken 3 hits, sinking that player’s ship. In the event that both players have taken 3 shots, there will be a draw. Otherwise the victor is clear. 

Objects to implement:
Battleship object
Will have health and position variables.
Will have 3 different sprites, a full health one, a slightly damaged one, and a very damaged one. The corresponding sprite will depend on hp.
Will make its move, then fire a cannonball at one of the 3 spots on the other side.
Sprites(bare bones): 6 sprites total. Right-facing ship with 3 different damage levels, and left-facing ship with 3 different damage levels.
Sprites(Advanced): sprites for the animations of turning downward and upward, and turning back to fighting position.

Cannonball
Will spawn when a ship is firing, and move to the position it was aimed at.
On impact with a ship, it will disappear and take one hp from the ship.
On a miss, it will either disappear or animate falling into the water.
Sprites(bare bones): just one cannonball
Sprites(Advanced): an explosion animation 

 Environment
The environment will be a body of water. Ideally animated to look like it is not still, and like it interacts with ships moving through it.
Sprites: ??????

UI/Game Controller
Will have and display a timer for each turn.
Will ask the player where they want to move/shoot(mouse option) or display the corresponding key to the options they have available.
Will then start the animations to show what the outcome of the turn was, and if the match is over, will declare the final outcome.
Will display a health bar for each player.

Problems that will need to be solved:
Need to learn how to spawn a cannonball
Need to learn how to animate a nonstatic ocean
Need to learn how to implement turn based combat, and how gameflow works in general
Need to learn how to animate a ship moving

Plan to get started:
Game Logic Portion
Create filler objects with the variables for battleships
Look up tutorials on turn-based 2D games, and implementing timers in them
Create a bare bones UI that displays hp and asks the players to make a decision
Look up tutorials on spawning objects and making them disappear
Art Portion
Create the 6 battleship sprites and one cannonball
Look up tutorials on implementing a 2d ocean environment.
Try to get that 
