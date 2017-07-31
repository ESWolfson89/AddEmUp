"AddEmUp"

By Eric Wolfson

About:
WFA application written in C# by Eric Wolfson 2016

![Alt text](/demo/screenshot.jpg?raw=true "Screenshot")

Rules:
1) Click number tiles such that the sum equals the goal value.
2) You must clear at least 50 percent of the level to advance.
3) If you fail and the timer reaches 0, the game ends. 
4) The number of tiles to clear increases with the level number.
5) The formula for scoring when goal value is matched is:
          NUM_SELECTED x GOAL_SCORE x COLOR_BONUS
6) Color bonus is 10 if colors match and 4 or more are selected.
7) Color bonus is 1 in any other case.
8) After clearing a level, you get a bonus of 
              50 x NUM_CLEARED x LEVEL_NUMBER
9) If all tiles are cleared, another bonus is awarded: 
                   10000 x LEVEL_NUMBER

This project is licensed under the terms of the MIT license.