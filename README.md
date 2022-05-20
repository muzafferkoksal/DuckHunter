# DuckHunter
1)When the Unity Project is opened, press "Testing" game object in the game objects tab.
2)Public property named "Mode" can be changed to test different algorithms:
  0 - Depth First Search
  1 - Markov Decision Process
  2 - Adversarial Search(Minimax)
  3 - Breadth First Search
  4 - Uniform Cost Search
  5 - Q Learning
  6 - A* Search

For search algorithms(modes 0-3-4-6), pressing "space" initiates the search algorithm and the character finds the ducks by itself.
Pressing "WASD" buttons or directional buttons allows the player to control the character manually. 

For adversarial search, pressing the left mouse key causes the character to move 1 tile. The second click causes the duck to move and so on. This allows the user to see the decision making process of the character in each step.

Pressing right click on the mouse puts a wall where the pointer is located.

For mode 1 and 5 which are MDP and Q-learning respectively. 
You need to select the mode from testing object and then, you need to select the duck and bomb counts default is 1 for both in the loaded version
Note that the MDP and Q learning mode should be activated using space key for once after running the game. Also MDP and Q-learning cna only be run once for each runtime soi you need to restrat game for running the alogirthm for second time. You can change the grid size from testing.cs file in the start functions else part. 
