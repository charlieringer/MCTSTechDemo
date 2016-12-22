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

	public abstract List<AIState> generateChildren ();
	public abstract int terminal ();
}

public class OCAIState : AIState
{
	public State state;

	public OCAIState(State _state, int pIndex, AIState _parent, int _depth) : base(pIndex, _parent, _depth)
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
					State childState = new State (newBoard, newNumbPieces, new int[3]{ x, y, 1});
					OCAIState childAIState = new OCAIState (childState, newPIndx, this, depth+1);
					children.Add (childAIState);

					int[,] newBoard2 = (int[,])state.getBoard().Clone ();
					newBoard2 [x, y] = 2;
					State childState2 = new State (newBoard2, newNumbPieces, new int[3]{ x, y, 2});
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
