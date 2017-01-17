using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
	public int[,] board = new int[6,6];
	public int numbPiecesPlayed;
	public int[] lastPiecePlayed;

	public int[,] getBoard() {
		return board;
	}

	public void playPiece(int[] moveData)
	{
		board [moveData [0], moveData [1]] = moveData [2];
		numbPiecesPlayed++;
	}

	public abstract bool checkGameEnd ();
	public abstract bool checkGameEnd(int[] piecePlayed);
}