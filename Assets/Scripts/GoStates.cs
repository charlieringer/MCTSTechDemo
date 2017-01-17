using System;
using System.Collections.Generic;
using UnityEngine;

public class GOState : State
{
	public int winner = -1;
	int whiteCaptureScore = 0;
	int blackCaptureScore = 0;
	public bool illegalState = false;

	public GOState()
	{
		numbPiecesPlayed = 0;
	}

	public GOState (int[,] _board, int _numbPiecesPlayed, int[] _lastPiecePlayed, int[] oldScores)
	{
		whiteCaptureScore = oldScores [0];
		blackCaptureScore = oldScores [1];
		checkForCaptures (_lastPiecePlayed);
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

	private void checkForCaptures (int[] _lastPiecePlayed)
	{
		if (!hasLiberty (_lastPiecePlayed)) {
			illegalState = true;
			return;
		}
		int colour;
		if (_lastPiecePlayed [2] == 1)
			colour = 2;
		else
			colour = 1;
		int[] adjU = new int[] { _lastPiecePlayed [0], _lastPiecePlayed [1]+1, colour };
		int[] adjD = new int[] { _lastPiecePlayed [0], _lastPiecePlayed [1]-1, colour };
		int[] adjL = new int[] { _lastPiecePlayed [0]+1, _lastPiecePlayed [1], colour };
		int[] adjR = new int[] { _lastPiecePlayed [0]-1, _lastPiecePlayed [1], colour };

		if(adjU[1] < 6 && !hasLiberty(adjU))
			remove(adjU);

		if(adjU[1] > 0 && !hasLiberty(adjD))
			remove(adjD);

		if(adjU[0] < 6 && !hasLiberty(adjL))
			remove(adjL);

		if(adjU[0] > 0 && !hasLiberty(adjR))
			remove(adjR);
	}

	bool hasLiberty(int[] piece)
	{
		List<List<int>> frontier = new List<List<int>>();
		frontier.Add( new List<int>(){piece[0],piece[1]});

		List<List<int>> visited = new List<List<int>>();


		while (frontier.Count > 0)
		{
			int x = frontier[0][0];
			int y = frontier[0][1];

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

			if (x > 0) {
				if (board [x - 1, y] == 0)
					return true;
				else if (board [x, y + 1] == piece [2]) 
					if (!wasVisited(visited, x, y+1))
					frontier.Add (new List<int> (){x, y + 1});
			}
			visited.Add (frontier [0]);
			frontier.RemoveAt (0);
		}
		return false;
	}

	void remove(int[] piece)
	{
		List<List<int>> frontier = new List<List<int>>();
		frontier.Add( new List<int>(){piece[0],piece[1]});

		while (frontier.Count > 0)
		{
			int x = frontier[0][0];
			int y = frontier[0][1];
			board [x, y] = 0;

			if (x > 0) {
				if (board [x - 1, y] == piece [2]) 
					frontier.Add (new List<int> (){x - 1, y});
			}

			if (x < 5) {
				if (board [x + 1, y] == piece [2]) 
					frontier.Add (new List<int> (){x + 1, y});
			}

			if (y > 0) {
				if (board [x, y - 1] == piece [2]) 
					frontier.Add (new List<int> (){x, y - 1});
			}

			if (x > 0) {
				if (board [x, y + 1] == piece [2]) 
					frontier.Add (new List<int> (){x, y + 1});
			}
			frontier.RemoveAt (0);
		}
	}

	private bool wasVisited(List<List<int>> visited, int x, int y)
	{
		foreach (List<int> node in visited) {
			if (node [0] == x && node [1] == y)
				return true;
		}
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
			for (int y = 0; y < 6; x++) {
				if (board [x, y] == 1) {
					whiteScore++;
				} else if (board [x, y] == 1) {
					blackScore++;
				}
			}
		}
		whiteScore += whiteCaptureScore;
		blackScore += blackCaptureScore;

		if (whiteScore > blackScore + 10) {
			winner = 1;
			return true;
		} else if (blackScore > whiteScore + 10) {
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
					GOState childState = new GOState (newBoard, newNumbPieces, new int[3]{ x, y, newPIndx}, oldScores);
					if (childState.illegalState == false) {
						GOAIState childAIState = new GOAIState (childState, newPIndx, this, depth + 1);
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

