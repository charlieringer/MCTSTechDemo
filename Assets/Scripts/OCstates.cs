using System;
using System.Collections.Generic;
using UnityEngine;

public class OCState : State
{
	public OCState()
	{
		numbPiecesPlayed = 0;
	}

	public OCState (int[,] _board, int _numbPiecesPlayed, int[] _lastPiecePlayed)
	{
		board = _board;
		numbPiecesPlayed = _numbPiecesPlayed;
		lastPiecePlayed = _lastPiecePlayed;
		int localPieceCount = 0;
		for (int x = 0; x < 6; x++) {
			for (int y = 0; y < 6; y++)
			{
				if (board [x, y] > 0) {
					localPieceCount++;
				}
			}
		}
		if (localPieceCount != numbPiecesPlayed) {
			Debug.Log ("ERROR: Piece count mismatch");
		}
	}

	public override bool checkGameEnd()
	{
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
		if (countX >= 5 || countY >= 5 || countD1 >= 5 || countD2 >= 5) {
			return true;
		}
		return false;
	}
}


public class OCAIState : AIState
{
	public OCState state;

	public OCAIState(OCState _state, int pIndex, AIState _parent, int _depth) : base(pIndex, _parent, _depth)
	{
		state = _state;
	}

	public override List<AIState> generateChildren()
	{
		List<AIState> children = new List<AIState> ();

		int newPIndx = 0;
		if (playerIndex == 1)
			newPIndx = 2;
		else
			newPIndx = 1;
		int newNumbPieces = state.numbPiecesPlayed+1;

		for (int x = 0; x < 6; x++) {
			for (int y = 0; y < 6; y++) {
				int pieceAtPosition = state.getBoard () [x, y];
				if (pieceAtPosition == 0) {
					int[,] newBoard = (int[,])state.getBoard().Clone ();
					newBoard [x, y] = 1;
					OCState childState = new OCState (newBoard, newNumbPieces, new int[3]{ x, y, 1});
					OCAIState childAIState = new OCAIState (childState, newPIndx, this, depth+1);
					children.Add (childAIState);

					int[,] newBoard2 = (int[,])state.getBoard().Clone ();
					newBoard2 [x, y] = 2;
					OCState childState2 = new OCState (newBoard2, newNumbPieces, new int[3]{ x, y, 2});
					OCAIState childAIState2 = new OCAIState (childState2, newPIndx, this, depth+1);
					children.Add (childAIState2);
				}
			}
		}
		this.children = children;
		return children;
	}

	public override int terminal()
	{
		if (state.checkGameEnd ()) {
			return 1;
		}
		if (state.numbPiecesPlayed == 36) {
			return 2;
		}
		return 0;
	}
}