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
		//Makes a blank game state (for the start of the game)
		numbPiecesPlayed = 0;
	}

	public GOState (int[,] _board, int _numbPiecesPlayed, int[] _lastPiecePlayed, int[] oldScores)
	{
		//For the AI. So the state is build from previous values (which are passed in during child generation. 
		whiteCaptureScore = oldScores [0];
		blackCaptureScore = oldScores [1];
		board = _board;
		numbPiecesPlayed = _numbPiecesPlayed;
		lastPiecePlayed = _lastPiecePlayed;
		//Check is any stones are captured.
		checkForCaptures (_lastPiecePlayed);
	}

	//Check to see if any pieces have been captured
	public void checkForCaptures (int[] _lastPiecePlayed)
	{
		//Get the colour of the opposing piece
		int colour;
		if (_lastPiecePlayed [2] == 1)
			colour = 2;
		else
			colour = 1;
		
		//These represent the adjecent pieces (and there colour - being the colour of the other side)
		//This colour is used to see if we need to check that piece (as adjecent peices
		//of the same colour will not be captured)
		int[] adjU = new int[] { _lastPiecePlayed [0], _lastPiecePlayed [1]+1, colour };
		int[] adjD = new int[] { _lastPiecePlayed [0], _lastPiecePlayed [1]-1, colour };
		int[] adjL = new int[] { _lastPiecePlayed [0]+1, _lastPiecePlayed [1], colour };
		int[] adjR = new int[] { _lastPiecePlayed [0]-1, _lastPiecePlayed [1], colour };

		//If the adjcent piece are on the board and have no liberties
		if((adjU[1] < 6) && (board[adjU[0],adjU[1]] != _lastPiecePlayed[2]) && !hasLiberty(adjU))
			//Remove it (and the group it is part of)
			remove(adjU);
		//Repeat for each direction. 
		if(adjD[1] >= 0 && (board[adjD[0],adjD[1]] != _lastPiecePlayed[2]) && !hasLiberty(adjD))
			remove(adjD);
		if(adjL[0] < 6 && (board[adjL[0],adjL[1]] != _lastPiecePlayed[2]) && !hasLiberty(adjL))
			remove(adjL);
		if(adjR[0] >= 0 && (board[adjR[0],adjR[1]] != _lastPiecePlayed[2]) && !hasLiberty(adjR))
			remove(adjR);
		//If this peice has no liberties after removing neighbours
		if (!hasLiberty (_lastPiecePlayed)) {
			//it is illegal so set this flag
			illegalState = true;
			return;
		}
	}

	//Checks to see if a group of stones starting at a given pieces has a liberty (a liberty is an empty space)
	//This uses breadth first graph search
	bool hasLiberty(int[] piece)
	{
		//Set up a frontier
		List<List<int>> frontier = new List<List<int>>();
		frontier.Add( new List<int>(){piece[0],piece[1]});
		//visted nodes
		List<List<int>> visited = new List<List<int>>();


		while (frontier.Count > 0)
		{
			//X and y from the Fronteir
			int x = frontier[0][0];
			int y = frontier[0][1];

			//if it is on the board
			if (x > 0) {
				//if the adjecent space is blank
				if (board [x - 1, y] == 0)
					//we have a liberty
					return true;
				else if (board [x - 1, y] == piece [2]) 
					//If it the same colour and not visted add to frontier
					if (!wasVisited(visited, x-1, y))
					frontier.Add (new List<int> (){x - 1, y});
			}
			//if it is on the board
			if (x < 5) {
				if (board [x + 1, y] == 0)
					//we have a liberty
					return true;
				else if (board [x + 1, y] == piece [2]) 
					//If it the same colour and not visted add to frontier
					if (!wasVisited(visited, x+1, y))
						frontier.Add (new List<int> (){x + 1, y});
			}
			//if it is on the board
			if (y > 0) {
				if (board [x, y - 1] == 0)
					//we have a liberty
					return true;
				else if (board [x, y - 1] == piece [2]) 
					//If it the same colour and not visted add to frontier
					if (!wasVisited(visited, x, y-1))
					frontier.Add (new List<int> (){x, y - 1});
			}
			//if it is on the board
			if (y < 5) {
				if (board [x, y+1] == 0)
					//we have a liberty
					return true;
				else if (board [x, y + 1] == piece [2]) 
					//If it the same colour and not visted add to frontier
					if (!wasVisited(visited, x, y+1))
					frontier.Add (new List<int> (){x, y + 1});
			}
			//Add node to vistied
			visited.Add (frontier [0]);
			//and remove from froniter
			frontier.RemoveAt (0);
		}
		//frontier is emtpy and no nodes to explore so no liberty, return false.
		return false;
	}

	void remove(int[] piece)
	{
		//Removes a group of stone starting from a given pieces
		List<List<int>> frontier = new List<List<int>>();
		frontier.Add( new List<int>(){piece[0],piece[1]});

		int captureIndx = piece [2];

		while (frontier.Count > 0)
		{
			//X and y from the Fronteir
			int x = frontier[0][0];
			int y = frontier[0][1];
			//If the pieces is white add to blacks score, else add to whites score
			if (board [x, y] == 1) {
				blackCaptureScore++;
			} else if (board [x, y] == 2) {
				whiteCaptureScore++;
			}
			//Empty this piece
			board [x, y] = 0;

			//if it is on the board
			if (x > 0) {
				//And the same colour
				if (board [x - 1, y] == captureIndx)
					//Add to frontier
					frontier.Add (new List<int> (){ x - 1, y });
			}
			//if it is on the board
			if (x < 5) {
				//And the same colour
				if (board [x + 1, y] == captureIndx) 
					//Add to frontier
					frontier.Add (new List<int> (){ x + 1, y });
			}
			//if it is on the board
			if (y > 0) {
				//And the same colour
				if (board [x, y - 1] == captureIndx)
					//Add to frontier
					frontier.Add (new List<int> (){ x, y - 1 });
			}
			//if it is on the board
			if (y < 5) {
				//And the same colour
				if (board [x, y + 1] == captureIndx)
					//Add to frontier
					frontier.Add (new List<int> (){ x, y + 1 });
			}
			//Remove from frontier
			frontier.RemoveAt (0);
		}
	}

	private bool wasVisited(List<List<int>> visited, int x, int y)
	{
		//If the x y appear is the list of list of ints we return true
		foreach (List<int> node in visited) {
			if (node [0] == x && node [1] == y)
				return true;
		}
		//Else return false
		return false;
	}

	public override bool checkGameEnd()
	{
		//Check the game end.
		return checkGameEnd(lastPiecePlayed);
	}

	public override bool checkGameEnd(int[] piecePlayed)
	{
		//check to see if the game is over
		if (whiteCaptureScore > (blackCaptureScore + 8)) {
			//White wins if it has captured 9 or more stones more than black (becuase first player advantage)
			winner = 1;
			return true;
		} else if (blackCaptureScore > (whiteCaptureScore + 4)) {
			//Black wins if it has captured 5 or more stones more than white
			winner = 2;
			return true;
		}
		return false;
	}

	public int[] getScores()
	{
		//Returns the scores of both players as an int array. 0 = white, 1 = black
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
		//If the game is over
		if (state.checkGameEnd ()) {
			//Return the indx of the player who won
			return state.winner;
		}
		//Otherwise still playing so return 0
		return 0;
	}
}

