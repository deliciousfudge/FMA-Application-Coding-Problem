# Yay, Tic Tac Toe!  

The following is a console based version of Tic Tac Toe that allows two human players to play the game on a 3 x 3 board.

It's really simple. The first player will be the X, the second player will be the O. You keep playing the game until there is a winner, a draw, or someone gives up.

### Running the Program
Included in this repository is a file called TicTacToe.zip. Unzip this to a local folder of your choice and open TicTacToe.exe to run the program.

### Assumptions Made
* As no style guide was provided with the project brief, I have tried to keep to the official C# coding conventions except where mentioned otherwise below: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions.
* A non-developer may decide to change some values after the program is shipped, so I have designed it to be used with a config file (explained further below).
* The scale (ie 3 x 3 means scale of 3) of the grid can be changed but will never go below 3 x 3 or above 255 x 255. Hence I've chosen to store values as bytes where applicable to minimise memory usage.
* The user must enter the quit character by itself (with no other characters) to signal their intention to quit.
* The players may not want to exit the program immediately after a game, and instead play another.
* The program may be modified in future, so I have structured the code in a way that makes it easily extendable.

### Config File
Included with the TicTacToe.exe is a file called TicTacToe.exe.config. This can be modified with any text editor and contains a number of custom settings in the <appSettings> section.

* VacantSymbol, Player1Symbol, and Player2Symbol contain chars used to represent each of the pieces (empty cell, player 1, player 2) on the board. Only values between 1 and 5 characters are accepted.
* BoardScale represents the scale of the board. Only values between 3 and 255 are accepted.

### Game Play

* Two players are required for a game.  
* Each player will assume either an “X” or “O”.  
* Players take turn to play till a player wins, or the end of the game (whichever happens first).  
* Player X always starts the game.  

### Condition for a win

* A player wins when all fields in a column are taken by the player.
* A player wins when all fields in a row are taken by the player.
* A player wins when all fields in a diagonal are taken by the player.

### Conditions for a draw

The game is drawn when all fields are taken on the board.  

### Example Game

An example run through of a game console would be...

~~~
Welcome to Tic Tac Toe!

Here's the current board:

. . .
. . .
. . .

Player 1 enter a coord x,y to place your X or enter 'q' to give up: 1,1

Move accepted, here's the current board:

X . .  
. . . 
. . .

Player 2 enter a coord x,y to place your O or enter 'q' to give up: 1,1

Oh no, a piece is already at this place! Try again...

Player 2 enter a coord x,y to place your O or enter 'q' to give up: 1,3

Move accepted, here's the current board:

X . O  
. . . 
. . .

Player 1 enter a coord x,y to place your X or enter 'q' to give up: 2,1

Move accepted, here's the current board:

X . O  
X . . 
. . .

Player 2 enter a coord x,y to place your O or enter 'q' to give up: 2,2

Move accepted, here's the current board:

X . O  
X O . 
. . .

Player 1 enter a coord x,y to place your X or enter 'q' to give up: 3,1

Move accepted, well done you've won the game!  

X . O  
X O . 
X . .
~~~