using System;
using System.Collections.Generic;
using UnityEngine;

public class GOState : State
{
	public int winner = -1;
	public int whiteCaptureScore = 0;
	public int blackCaptureScore = 0;
	public bool illegalState = false;

	public GOState()
	{
		numbPiecesPlayed = 0;
	}

	public GOState (int[,] _board, int _numbPiecesPlayed, int[] _lastPiecePlayed, int[] oldScores)
	{
		whiteCaptureScore = oldScores [0];
		blackCaptureScore = oldScores [1];
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
		//Debug.Log ("Checking captures.");
		checkForCaptures (_lastPiecePlayed);
		//Debug.Log ("Captures checked.");
		if (localPieceCount != numbPiecesPlayed) {
			//Debug.Log ("Error: Piece count mismatch");
		}
	}

	public void checkForCaptures (int[] _lastPiecePlayed)
	{
		int colour;
		if (_lastPiecePlayed [2] == 1)
			colour = 2;
		else
			colour = 1;
		int[] adjU = new int[] { _lastPiecePlayed [0], _lastPiecePlayed [1]+1, colour };
		int[] adjD = new int[] { _lastPiecePlayed [0], _lastPiecePlayed [1]-1, colour };
		int[] adjL = new int[] { _lastPiecePlayed [0]+1, _lastPiecePlayed [1], colour };
		int[] adjR = new int[] { _lastPiecePlayed [0]-1, _lastPiecePlayed [1], colour };

		//Debug.Log ("Checking liberties for adjU");
		if((adjU[1] < 6) && (board[adjU[0],adjU[1]] != _lastPiecePlayed[2]) && !hasLiberty(adjU))
			remove(adjU);
		//Debug.Log ("Checking liberties for adjD");
		if(adjD[1] > 0 && (board[adjD[0],adjD[1]] != _lastPiecePlayed[2]) && !hasLiberty(adjD))
			remove(adjD);
		//Debug.Log ("Checking liberties for  adjL");
		if(adjL[0] < 6 && (board[adjL[0],adjL[1]] != _lastPiecePlayed[2]) && !hasLiberty(adjL))
			remove(adjL);
		//Debug.Log ("Checking liberties for  adjR");
		if(adjR[0] > 0 && (board[adjR[0],adjR[1]] != _lastPiecePlayed[2]) && !hasLiberty(adjR))
			remove(adjR);
		//Debug.Log ("Checking liberties for self.");
		if (!hasLiberty (_lastPiecePlayed)) {
			illegalState = true;
			return;
		}
	}

	bool hasLiberty(int[] piece)
	{
		//Debug.Log ("Checking liberty");
		List<List<int>> frontier = new List<List<int>>();
		frontier.Add( new List<int>(){piece[0],piece[1]});

		List<List<int>> visited = new List<List<int>>();


		while (frontier.Count > 0)
		{
			//Debug.Log ("Looping...");
			int x = frontier[0][0];
			int y = frontier[0][1];
			//Debug.Log ("X = " + x + " Y = " + y);

			//THE BUG IS HERE SOMEWHERE
			if (x > 0) {
				if (board [x - 1, y] == 0)
					return true;
				else if (board [x - 1, y] == piece [2]) 
					if (!wasVisited(visited, x-1, y))
					frontier.Add (new List<int> (){x - 1, y});
			}

			if (x < 5) {
				if (board [x + 1, y] == 0)
					return true;
				else if (board [x + 1, y] == piece [2]) 
					if (!wasVisited(visited, x+1, y))
						frontier.Add (new List<int> (){x + 1, y});
			}

			if (y > 0) {
				if (board [x, y - 1] == 0)
					return true;
				else if (board [x, y - 1] == piece [2]) 
					if (!wasVisited(visited, x, y-1))
					frontier.Add (new List<int> (){x, y - 1});
			}

			if (y < 5) {
				if (board [x, y+1] == 0)
					return true;
				else if (board [x, y + 1] == piece [2]) 
					if (!wasVisited(visited, x, y+1))
					frontier.Add (new List<int> (){x, y + 1});
			}
			visited.Add (frontier [0]);
			frontier.RemoveAt (0);
		}
		//Debug.Log ("Finished Checking");
		return false;
	}

	void remove(int[] piece)
	{
		//Debug.Log ("Removing");
		List<List<int>> frontier = new List<List<int>>();
		frontier.Add( new List<int>(){piece[0],piece[1]});

		int captureIndx = piece [2];

		while (frontier.Count > 0)
		{
			int x = frontier[0][0];
			int y = frontier[0][1];
			if (board [x, y] == 1) {
				blackCaptureScore++;
			} else {
				whiteCaptureScore++;
			}
			board [x, y] = 0;


			if (x > 0) {
				if (board [x - 1, y] == captureIndx)
					frontier.Add (new List<int> (){ x - 1, y });
			}

			if (x < 5) {
				if (board [x + 1, y] == captureIndx) 
					frontier.Add (new List<int> (){ x + 1, y });
			}

			if (y > 0) {
				if (board [x, y - 1] == captureIndx) 
					frontier.Add (new List<int> (){ x, y - 1 });
			}

			if (y < 5) {
				if (board [x, y + 1] == captureIndx) 
					frontier.Add (new List<int> (){ x, y + 1 });
			}
			frontier.RemoveAt (0);
		}
	}

	private bool wasVisited(List<List<int>> visited, int x, int y)
	{
		//Debug.Log ("Checking if it has been visited");
		foreach (List<int> node in visited) {
			if (node [0] == x && node [1] == y)
				return true;
		}
		//Debug.Log ("Finished checking");
		return false;
	}

	public override bool checkGameEnd()
	{
		return checkGameEnd(lastPiecePlayed);
	}

	public override bool checkGameEnd(int[] piecePlayed)
	{
		int blackScore = 0;
		int whiteScore = 0;
		for (int x = 0; x < 6; x++) {
			for (int y = 0; y < 6; y++) {
				if (board [x, y] == 1) {
					whiteScore++;
				} else if (board [x, y] == 2) {
					blackScore++;
				}
			}
		}
		whiteScore += whiteCaptureScore;
		blackScore += blackCaptureScore;

		if (whiteScore > (blackScore + 10)) {
			winner = 1;
			return true;
		} else if (blackScore > (whiteScore + 10)) {
			winner = 2;
			return true;
		}
		return false;
	}

	public int[] getScores()
	{
		int[] returnList = new int[2];
		returnList [0] = whiteCaptureScore;
		returnList [1] = blackCaptureScore;
		return returnList;
	}
}


public class GOAIState : AIState
{
	public GOState state;

	public GOAIState(GOState _state, int pIndex, AIState _parent, int _depth) : base(pIndex, _parent, _depth)
	{
		state = _state;
	}

	public override List<AIState> generateChildren()
	{
		//Debug.Log ("Generating children.");
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
					newBoard [x, y] = newPIndx;
					int[] oldScores = state.getScores ();
					//Debug.Log ("Making new child.");
					GOState childState = new GOState (newBoard, newNumbPieces, new int[3]{ x, y, newPIndx}, oldScores);
					//Debug.Log ("Made new child.");
					if (childState.illegalState == false) {
						GOAIState childAIState = new GOAIState (childState, newPIndx, this, depth + 1);
						//Debug.Log ("Not illegal so adding.");
						children.Add (childAIState);
					}
				}

			}
		}
		this.children = children;
		return children;
	}

	public override int terminal()
	{
		if (state.checkGameEnd ()) {
			return state.winner;
		}
		return 0;
	}
}

