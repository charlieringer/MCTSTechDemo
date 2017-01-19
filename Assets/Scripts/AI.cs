using System;
using System.Collections.Generic;
using UnityEngine;

public class AI
{
	double thinkingTime;
	float exploreWeight;
	public bool done;
	public bool started;
	public AIState next;
	int maxRollout = 36;
	System.Random randGen = new System.Random ();

	public AI ()
	{
		//Allow 4 seconds of thinking time
		thinkingTime = 4;
		exploreWeight = 0.5f;
	}

	public void reset()
	{
		//Resets the flags (for threading purposes)
		started = false;
		done = false;
		next = null;
	}

	public void runAI(AIState initalState)
	{
		//Set started to true
		started = true;
		//fine the next
		next = run (initalState);
		//Then set done to true
		done = true;
	}

	//Main MCTS algortim
	public AIState run(AIState initalState)
	{
		//Make the intial children
		List<AIState> children = initalState.generateChildren ();
		//Get the start time
		double startTime = (DateTime.Now.Ticks)/10000000;
		double latestTick = startTime;
		while (latestTick-startTime < thinkingTime) {
			//Update the latest tick
			latestTick = (DateTime.Now.Ticks)/10000000;
			//Set the best scores and index
			double bestScore = -1;
			int bestIndex = -1;
			//Loop through all of the children
			for(int i = 0; i < children.Count; i++){
				//Check is this node is terminal
				int isTerminalNode = children [i].terminal ();
				if (isTerminalNode == children [i].playerIndex) {
					//It it is and it is a winning move return it (and skip all the roll out)
					return children [i];
				} else if (isTerminalNode > 0 && isTerminalNode != children [i].playerIndex) {
					//If it is a losing move (so, someone else wins) just continue. 
					continue;
				}

				//Get the wins and games played
				double wins = children[i].wins;
				double games = children[i].totGames;

				//Work out the score for the wins
				double score = 1.0;
				if (games > 0) {
					score = wins / games;
				}

				//UBT (Upper Confidence Bound 1 applied to trees) function for determining
				//How much we want to explore vs exploit.
				//Because we want to change things the constant is configurable.
				double exploreRating = exploreWeight*Math.Sqrt(Math.Log(initalState.totGames+1)/(games+0.1));

				//Calcualte the total score
				double totalScore = score+exploreRating;
				//If this is our best score
				if (totalScore > bestScore){
					//Update the best score and best index.
					bestScore = totalScore;
					bestIndex = i;
				}
			}
			//Once done set the best child to this
			AIState bestChild = children[bestIndex];

			//And loop through it's child
			while(bestChild.children.Count > 0)
			{
				//Set the scores as abase line
				bestScore = -1;
				bestIndex = -1;

				for(int i = 0; i < bestChild.children.Count; i++){
					//Scores as per the previous part
					double wins = bestChild.children[i].wins;
					double games = bestChild.children[i].totGames;

					double score = 1.0;
					if (games > 0) {
						score = wins / games;
					}

					//UBT (Upper Confidence Bound 1 applied to trees) function for determining
					//How much we want to explore vs exploit.
					//Because we want to change things the constant is configurable.
					double exploreRating = exploreWeight*Math.Sqrt(Math.Log(initalState.totGames+1)/(games+0.1));

					double totalScore = score+exploreRating;
					//Again if the score is better updae
					if (totalScore > bestScore){
						bestScore = totalScore;
						bestIndex = i;
					}
				}
				//And set the best child for the next iteration
				bestChild = bestChild.children[bestIndex];
			}
			//Then roll out that child.
			rollout(bestChild);
		}

		//Once we get to this point we have worked out the best move
		//So just need to return it
		int mostGames = -1;
		int bestMove = -1;
		//Loop through all childern
		for(int i = 0; i < children.Count; i++)
		{
			//find the one that was played the most (this is the best move)
			int games = children[i].totGames;
			if(games >= mostGames)
			{
				mostGames = games;
				bestMove = i;
			}
		}
		//Return it.
		return children[bestMove];
	}

	//Rollout function (plays random moves till it hits a termination)
	void rollout(AIState rolloutStart)
	{
		bool terminalStateFound = false;
		//Get the children
		List<AIState> children = rolloutStart.generateChildren();

		if (children.Count == 0) {
			Debug.Log ("ERROR: No Children generated from inital state.");
			return;
		}

		int count = 0;
		while(!terminalStateFound)
		{
			//Loop through till a terminal state is found
			count++;
			if (count >= maxRollout) {
				//or maxroll out is hit
				rolloutStart.addDraw ();
				return;
			}
			//Get a random child index 
			int index = randGen.Next(children.Count);
			//and see if that node is terminal
			int endResult = children[index].terminal ();
			if(endResult > 0)
			{
				terminalStateFound = true;
				//If it is a win add a win
				if(endResult == rolloutStart.playerIndex) rolloutStart.addWin();
				//Else add a loss
				else rolloutStart.addLoss();
			} else {
				//Otherwise select that nodes as the childern and continue
				children = children [index].generateChildren();
				if (children.Count == 0) {
					break;
				}
			}
		}
		//Reset the children as these are not 'real' children but just ones for the roll out. 
		foreach( AIState child in rolloutStart.children)
		{
			child.children = new List<AIState>();
		}
	}
}




