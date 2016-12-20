﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class AI
{
	int maxIters;
	float exploreWeight;
	System.Random randGen = new System.Random ();

	public AI ()
	{
		maxIters = 4000;
		exploreWeight = 0.5f;
	}

	public AIState run(AIState initalState)
	{
		int numbIters = 0;
		int maxDepth = 0; 
		List<AIState> children = initalState.generateChildren ();
		//initalState.children = children;
		while (numbIters < maxIters) {
			//Debug.Log("Loop: " + numbIters);
			numbIters++;
			if (numbIters > 10) {
				numbIters += 0;
			}

			double bestScore = -1;
			int bestIndex = -1;
			for(int i = 0; i < children.Count; i++){
				Debug.Assert (children [i].parent != null);
				
				int wins = children[i].wins;
				int games = children[i].totGames;

				double score = 1.0;
				if (games > 0) score = wins/games;

				//UBT (Upper Confidence Bound 1 applied to trees) function for determining
				//How much we want to explore vs exploit.
				//Because we want to change things the constant is configurable.
				double exploreRating = exploreWeight*Math.Sqrt(2*(Math.Log(initalState.totGames+1)/(games+0.1)));

				double totalScore = score+exploreRating;
				//Debug.Log ("Child " + i + " has a score of " + totalScore);
				if (totalScore > bestScore){
					bestScore = totalScore;
					bestIndex = i;
				}
			}
			//Debug.Log ("Best Child was " + bestIndex);
			AIState bestChild = children[bestIndex];


			while(bestChild.children.Count > 0)
			{
				bestScore = -1;
				bestIndex = -1;

				for(int i = 0; i < bestChild.children.Count; i++){

					//Scores as per the previous part
					int wins = bestChild.children[i].wins;
					int games = bestChild.children[i].totGames;

					double score = 1.0;
					if (games > 0) score = wins/games;

					//UBT (Upper Confidence Bound 1 applied to trees) function for determining
					//How much we want to explore vs exploit.
					//Because we want to change things the constant is configurable.
					double exploreRating = exploreWeight*Math.Sqrt(2*(Math.Log(initalState.totGames+1)/(games+0.1)));

					double totalScore = score+exploreRating;

					if (totalScore > bestScore){
						bestScore = totalScore;
						bestIndex = i;
					}
				}
				bestChild = bestChild.children[bestIndex];
			}
			if (bestChild.depth > maxDepth)
				maxDepth = bestChild.depth;
			rollout(bestChild);
		}
			
		int mostGames = -1;
		int bestMove = -1;
		for(int i = 0; i < children.Count; i++)
		{
			int games = children[i].totGames;
			if(games >= mostGames)
			{
				mostGames = games;
				bestMove = i;
			}
			//Debug.Log ("Child " + i + " played " + games);
		}
		Debug.Log ("Wins: " + initalState.wins);
		Debug.Log ("Losses: " + initalState.losses);
		Debug.Log ("Max Depth reached: " + maxDepth);
		return children[bestMove];
	}

	void rollout(AIState rolloutStart)
	{
		bool terminalStateFound = false;
		List<AIState> children = rolloutStart.generateChildren();
		//rolloutStart.children = children;	

		if (children.Count == 0) {
			Debug.Log("Error: No Children generated from inital state.");
			return;
		}

		while(!terminalStateFound)
		{
			int index = randGen.Next(children.Count);
			int endResult = children[index].terminal ();
			if(endResult > 0)
			{
				terminalStateFound = true;
				if(endResult == rolloutStart.playerIndex) rolloutStart.addWin();
				else rolloutStart.addLoss();
			} else {
				children = children [index].generateChildren();
				if (children.Count == 0) {
					Debug.Log("Error: End State not recognised.");
					break;
				}
			}
		}
		//Reset the children as these are not 'real' children but just ones for the roll out. 
		//rolloutStart.children = null;
	}
}




