using System;
using System.Collections.Generic;

//Abstract base class for the AI state
public abstract class AIState
{
	//Tracks game data
	public int wins = 0;
	public int losses = 0;
	public int totGames = 0;
	public int playerIndex;
	public int depth = 0;
	public AIState parent;
	//List of child nodes
	public List<AIState> children = new List<AIState>();

	public AIState(int pIndex, AIState _parent, int _depth)
	{
		playerIndex = pIndex;
		parent = _parent;
		depth = _depth;
	}

	//For adding a win
	public void addWin(){
		wins++;
		totGames++;
		if (parent != null)
			parent.addLoss ();
	}

	//For adding a loss
	public void addLoss(){
		losses++;
		totGames++;
		if (parent != null)
			parent.addWin ();
	}

	//For adding a draw
	public void addDraw(){
		totGames++;
		if (parent != null)
			parent.addDraw ();
	}

	//These function are needed by the AI so MUST be implemented
	//For making child nodes
	public abstract List<AIState> generateChildren ();
	//For checking is a node is terminal (and which player won)ss
	public abstract int terminal ();
}

