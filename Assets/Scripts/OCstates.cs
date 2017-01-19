using System;
using System.Collections.Generic;
using UnityEngine;

public class OCState : State
{
	public OCState()
	{
		//Makes a blank game state (for the start of the game)
		numbPiecesPlayed = 0;
	}

	public OCState (int[,] _board, int _numbPiecesPlayed, int[] _lastPiecePlayed)
	{
		//For the AI. So the state is build from previous values (which are passed in during child generation. 
		board = _board;
		numbPiecesPlayed = _numbPiecesPlayed;
		lastPiecePlayed = _lastPiecePlayed;
	}

	public override bool checkGameEnd()
	{
		//If we just call check game end we used the last played piece.
		return checkGameEnd(lastPiecePlayed);
	}

	public override bool checkGameEnd(int[] piecePlayed)
	{
		int x = piecePlayed [0];
		int y = piecePlayed [1];
		int colourPlayed = piecePlayed [2];
		int countX = 0;
		int countY = 0; 
		int countD1 = 0;
		int countD2 = 0;
		int diagOffset = (x - y);
		for (int i = 0; i < 6; i++) {
			//Check orthognal directions
			if (board [x, i] == colourPlayed) //If we find a match
				countX++; //Increment the count
			else if (countX >= 1 && countX < 5) //If we find a break and have not completed the row
				countX = 0; //It is impossible to win so set count to 0
			if (board [i, y] == colourPlayed)  //If we find a match
				countY++; //Increment the count
			else if (countY >= 1 && countY < 5) //If we find a break and have not completed the row
				countY = 0; //It is impossible to win so set count to 0
			//check diagonal directions
			//Not all locations work for di
			if (!(diagOffset + i < 0 || diagOffset + i > 5) && board [diagOffset + i, i] == colourPlayed)
				countD1++;
			else if (countD1 >= 1 && countD1 < 5) //If we find a break and have not completed the row
				countD1 = 0; //It is impossible to win so set count to 0
			if (!(diagOffset + 2*y  - i < 0 || diagOffset + 2*y  - i > 5) && board [diagOffset + 2*y - i , i] == colourPlayed)
				countD2++;
			else if (countD2 >= 1 && countD2 < 5) //If we find a break and have not completed the row
				countD2 = 0; //It is impossible to win so set count to 0

		}
		//if either direction is at least 5
		if (countX >= 5 || countY >= 5 || countD1 >= 5 || countD2 >= 5) {
			//The game is over (Order wins)
			return true;
		}
		//Otherwise game is still going on.
		return false;
	}
}


public class OCAIState : AIState
{
	//Contains a game state
	public OCState state;

	public OCAIState(OCState _state, int pIndex, AIState _parent, int _depth) : base(pIndex, _parent, _depth)
	{
		state = _state;
	}

	public override List<AIState> generateChildren()
	{
		//Generates all possible child states from this state
		List<AIState> children = new List<AIState> ();
		//Swap the player
		int newPIndx = 0;
		if (playerIndex == 1)
			newPIndx = 2;
		else
			newPIndx = 1;
		//Increment the number of peices played
		int newNumbPieces = state.numbPiecesPlayed+1;
		//Loop through all of the board pieces
		for (int x = 0; x < 6; x++) {
			for (int y = 0; y < 6; y++) {
				//Get the piece
				int pieceAtPosition = state.getBoard () [x, y];
				//if it is 0 (therefore empty)
				if (pieceAtPosition == 0) {
					//We have a possible peice to play
					//So clone the board
					int[,] newBoard = (int[,])state.getBoard().Clone ();
					//and simululate playing a white piece
					newBoard [x, y] = 1;
					OCState childState = new OCState (newBoard, newNumbPieces, new int[3]{ x, y, 1});
					OCAIState childAIState = new OCAIState (childState, newPIndx, this, depth+1);
					//And add this state as a child
					children.Add (childAIState);
					//Then simululate playing a black piece
					int[,] newBoard2 = (int[,])state.getBoard().Clone ();
					newBoard2 [x, y] = 2;
					OCState childState2 = new OCState (newBoard2, newNumbPieces, new int[3]{ x, y, 2});
					OCAIState childAIState2 = new OCAIState (childState2, newPIndx, this, depth+1);
					//And add this state as a child
					children.Add (childAIState2);
				}
			}
		}
		//Set the children of this node
		this.children = children;
		//Also return it.
		return children;
	}

	public override int terminal()
	{
		//If the game is over
		if (state.checkGameEnd ()) {
			//1 has won
			return 1;
		}
		//If the board is full
		if (state.numbPiecesPlayed == 36) {
			//2 has won
			return 2;
		}
		//Other return 0 (game still going)
		return 0;
	}
}