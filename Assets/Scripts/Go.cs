using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Threading;

public class Go : GameMaster {

	public Text colour;
	public Text turn;
	public Text winlose;

	void Start () {
		if (GameData.playerIndex == 1) {
			turn.text = "Your turn";
			colour.text = "Selected colour: White";
			playerIndx = 1;
			playersTurn = true;
		} else {
			turn.text = "AIs turn";
			colour.text = "Selected colour: Black";
			playerIndx = 2;
			playersTurn = false;
		}

		tiles = new List<GameObject>();
		for (int i = 0; i < x; i++) {
			for (int j = 0; j < y; j++) {
				//create the game object 
				GameObject tile = (GameObject)Instantiate(preFabTile, new Vector3 ((i+(0.1f*i)), 0, j+(0.1f*j)), Quaternion.identity);
				tile.GetComponent<Tile>().setMaster(this);
				tile.GetComponent<Tile> ().setXY (i, j);
				tiles.Add( tile);
				tile.GetComponent<Tile>().preFabCounter = preFabCounter;
			}
		} 
		gameState = new GOState ();
		selectedColour = 1;
	}

	// Update is called once per frame
	void Update () {
		if (!playersTurn && gamePlaying) {
			turn.text = "AIs turn";
			thinkingPopup.SetActive (true);
			if (!brain.started) {
//				OCAIState preState = new OCAIState (gameState, playerIndx, null, 0);
			//	aiThread = new Thread (new ThreadStart (() => brain.runAI (preState)));
				aiThread.Start ();
			}
			if (brain.done) {
				OCAIState postState = (OCAIState)brain.next;
				if (postState == null) {
					Debug.Log ("ERROR: Null State.");
				}
				gameState = postState.state;
				brain.reset ();
				visualiseMove ();
				playersTurn = true;
				aiThread.Join ();
			}

		} else {
			thinkingPopup.SetActive (false);
			turn.text = "Your turn";
		}
	}

	public override void spawn(int x, int y)
	{
		playersTurn = false;
		GameObject counter = (GameObject)Instantiate(preFabCounter, new Vector3 ((x+(0.1f*x)), 0.1f, (y+(0.1f*y))), Quaternion.identity);
		counter.GetComponent<Collider> ().enabled = false;
		if (selectedColour == 1) {
			counter.GetComponent<Renderer> ().material.color = Color.white;
		} else {
			counter.GetComponent<Renderer> ().material.color = Color.black;
		}
		int[] playedPiece = new int[3]{ x, y, selectedColour};

		gameState.playPiece (playedPiece);
		checkState (playedPiece);
	}

	public void checkState (int[] playedPiece)
	{
//		if (gameState.checkGameEnd (playedPiece)) {
//			gamePlaying = false;
//			if (GameData.playerIndex == 1) {
//				winlose.text = "You Won!";
//			} else {
//				winlose.text = "You Lost!";
//			}
//			endGameMenu.SetActive (true);
//			return;
//		}
//		if (gameState.numbPiecesPlayed == 36) {
//			gamePlaying = false;
//			if (GameData.playerIndex == 1) {
//				winlose.text = "You Lost!";
//			} else {
//				winlose.text = "You Won!";
//			}
//			endGameMenu.SetActive (true);
//			return;
//		}
	}

	private void visualiseMove()
	{
		bool found = false;
		foreach(GameObject tile in tiles)
		{
			int pX = gameState.lastPiecePlayed [0];
			int pY = gameState.lastPiecePlayed [1];

			int tX = tile.GetComponent<Tile> ().x;
			int tY = tile.GetComponent<Tile> ().y;
			if (pX == tX && pY == tY) {
				tile.GetComponent<Tile>().aiPlayHere(gameState.lastPiecePlayed [2]);
				found = true;
			}
		}
		if (!found) {
			Debug.Log ("ERROR: Could not locate AI move.");
		}
		checkState (gameState.lastPiecePlayed);
	}
}

