using System.Collections.Generic;
using UnityEngine;

public class State
{
	public int[,] board = new int[6,6];
	public int numbPiecesPlayed;
	public int[] lastPiecePlayed;

	public State()
	{
		numbPiecesPlayed = 0;
	}

	public State (int[,] _board, int _numbPiecesPlayed, int[] _lastPiecePlayed)
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
			Debug.Log ("Error: Piece count mismatch");
		}
	}

	public bool checkGameEnd()
	{
		return checkGameEnd(lastPiecePlayed);
	}

	public bool checkGameEnd(int[] piecePlayed)
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

	public int[,] getBoard() {
		return board;
	}

	public void playPiece(int[] moveData)
	{
		board [moveData [0], moveData [1]] = moveData [2];
		numbPiecesPlayed++;
	}
}

