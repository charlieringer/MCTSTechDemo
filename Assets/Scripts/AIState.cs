using System;
using System.Collections.Generic;

public abstract class AIState
{
	public int wins = 0;
	public int losses = 0;
	public int totGames = 0;
	public int playerIndex;
	public int depth = 0;
	public AIState parent;
	public List<AIState> children = new List<AIState>();

	public AIState(int pIndex, AIState _parent, int _depth)
	{
		playerIndex = pIndex;
		parent = _parent;
		depth = _depth;
	}


	public void addWin(){
		wins++;
		totGames++;
		if (parent != null)
			parent.addLoss ();
	}

	public void addLoss(){
		losses++;
		totGames++;
		if (parent != null)
			parent.addWin ();
	}

	public void addDraw(){
		totGames++;
		if (parent != null)
			parent.addDraw ();
	}

	public abstract List<AIState> generateChildren ();
	public abstract int terminal ();
}

