﻿using System;
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
		thinkingTime = 4;
		exploreWeight = 0.5f;
	}

	public void reset()
	{
		started = false;
		done = false;
		next = null;
	}

	public void runAI(AIState initalState)
	{
		started = true;
		next = run (initalState);
		done = true;
	}

	public AIState run(AIState initalState)
	{
		int numbIters = 0;
		int maxDepth = 0; 
		List<AIState> children = initalState.generateChildren ();
		//initalState.children = children;
		double startTime = (DateTime.Now.Ticks)/10000000;
		double latestTick = startTime;
		while (latestTick-startTime < thinkingTime) {
			latestTick = (DateTime.Now.Ticks)/10000000;

			numbIters++;
			if (numbIters > 100) {
				numbIters += 0;
			}

			double bestScore = -1;
			int bestIndex = -1;

			for(int i = 0; i < children.Count; i++){
				int isTerminalNode = children [i].terminal ();
				if (isTerminalNode == children [i].playerIndex) {
					return children [i];
				} else if (isTerminalNode > 0 && isTerminalNode != children [i].playerIndex) {
					continue;
				}
				//Debug.Assert (children [i].parent != null);
				
				double wins = children[i].wins;
				double games = children[i].totGames;

				double score = 1.0;
				if (games > 0) {
					score = wins / games;
				}

				//UBT (Upper Confidence Bound 1 applied to trees) function for determining
				//How much we want to explore vs exploit.
				//Because we want to change things the constant is configurable.
				double exploreRating = exploreWeight*Math.Sqrt(Math.Log(initalState.totGames+1)/(games+0.1));

				double totalScore = score+exploreRating;
				if (totalScore > bestScore){
					bestScore = totalScore;
					bestIndex = i;
				}
			}
			AIState bestChild = children[bestIndex];

			while(bestChild.children.Count > 0)
			{
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
		}
		return children[bestMove];
	}

	void rollout(AIState rolloutStart)
	{
		bool terminalStateFound = false;
		List<AIState> children = rolloutStart.generateChildren();

		if (children.Count == 0) {
			Debug.Log ("ERROR: No Children generated from inital state.");
			return;
		}
		int count = 0;
		while(!terminalStateFound)
		{
			count++;
			if (count >= maxRollout) {
				rolloutStart.addDraw ();
				return;
			}
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




