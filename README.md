Game AI Mini Project Report

Purpose

The intended purpose of this project is to demonstrate that Monte Carlo Tree Search (MCTS) is a general game playing algorithm and is therefore capable of playing many different games. This has been achieved by implementing two different games which can both be played by the MCTS Agent and in which the MCTS Agent could be considered a strong opponent to a human player. 

The two games implemented for this project are “Order and Chaos”, an asymmetric game in the Tic-Tak-Toe family of games, and a simplified and modified version of “Go” of my own design. Full rules for these games can be found below.  These were chosen because as they share some similarities – they are both turn based, two-player, perfect information, deterministic, zero-sum games. These styles of games meant that it was possible to develop strong Agents with only a simple MCTS algorithm and there was no need to development methods for calculating the probabilities of actions (which require a certain number of samples to be taken to work well, a number that was larger than the times a tree node would be visited during the execution of the algorithm due to purposely giving the AI only a few seconds to consider a move). 

Additionally, this project provides a method of interfacing between an MCTS algorithm and a given game. This is achieved using an abstract AIState class which is implemented differently by each game but provides a method of generating child states from a game state and a method for determining if a game has finished and who has won. These two functions are all that is required for MCTS.  

Order and Chaos

Order and Chaos is played on a 6x6 Grid. It is an asymmetric game in which one player plays as “Order” and one player plays as “Chaos”. Each player, starting with the Order player, takes it in turns to play one stone on one of the 36 grid squares, this stone can either Black or White and any stone is not ‘owned’ by a player once placed. Additionally, once a stone has been placed on a grid square it will not move or be removed for the rest of the game and no more stones can be placed on that grid square. 

The Order player wins the game if there exists an unbroken row of 5 stones of the same colour in any direction (orthogonal or diagonal). The Chaos player simply wins by stopping the Order player from achieving their victory condition and instead forcing the board to become full, after 36 moves (or 18 plys). 

This game is a good test for a MCTS Agent because it asymmetric and thus provides. It is also interesting because it has higher branching factor, there are 64! ways to arrange two colours of stones on a 6x6 board, but a low maximum number of moves, the game is guaranteed to end after 36 moves. This high branching factor means that often the agent can perform poorly, especially when playing as Chaos, in the early game. However, if it can survive until the mid-game the branching factor reduces to the point that the agent becomes proficient at the game. 

Simple Go
The “Simple Go” game presented in the project is a game of my own design based heavily on the ancient game Go. It is played on a 6x6 board and unlike it’s ancestor is played on the grid squared rather than the intersections.  To play, first the player must pick to play with either the white or black stones (unlike in Order and Chaos where either can be chosen). On a player’s turn they choose one of the grid squares in which to place one of their stones. 

Then if this move caused a single stone or group of connected stones of the agent’s colour to be surrounded orthogonally by the players pieces these stones become ‘captured’, are removed from the board, and added to the capturing players score. However, a player is unable to place a stone in a square which would cause the automatic capture of that stone (although, if this placement allows the capture of opposing stones first this is allowable as the placed stone will no longer be surrounded. 

Players continue taking turns placing and capturing stones until one player has 15 more stones in play captured than the opposing player. That player is the winner. 

Simple Go provides a Go like game in which concepts from the original game are retained such as Ladders, Ko and Eyes but pushed players into a much more aggressive play style due to the small board. This smaller board is also why territory is only counted as stones places, it is much harder to determine exactly which areas of the board a player has control of. This small board means the branching factor of a game is much smaller than in the original game as is in fact much smaller than in Order and Chaos although games may go longer due to captures. It should be noted, as observable in the tech demo, the first player has a large advantage with the current rules and as such will often win. 

Tech

The main AI tech used for this project is MCTS (although some amount of graph search was used when determining which stones belonged to a set and if that set was surrounded). Monte Carlo Tree Search is  …

Using this Project

To use this project please down load the source code and load in Unity. There you can run the MainMenu scene which will give you the option to choose between playing Go and Order and Chaos. You will then be asked to select a side (Black/White and Order/Chaos respectively). Upon selection, the game will be loaded and, if the AI is to play first, the AI will select a move. To play a move, hover your mouse over a grid square, it will be highlighted green if it is selectable. Then click to place your stone. When playing Order and Chaos pressing the space bar will switch between colours.
