using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Threading;

public class Go : GameMaster {

	public Text colour;
	public Text turn;
	public Text winlose;
	public Text whiteScore;
	public Text blackScore;
	GOState gameState;
	private List<GameObject> counters = new List<GameObject>();

	void Start () {
		if (GameData.playerIndex == 1) {
			turn.text = "Your turn";
			colour.text = "Selected colour: White";
			playerIndx = 1;
			selectedColour = 1;
			playersTurn = true;
		} else {
			turn.text = "AIs turn";
			colour.text = "Selected colour: Black";
			playerIndx = 2;
			selectedColour = 2;
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
	}

	// Update is called once per frame
	void Update () {
		if (!playersTurn && gamePlaying) {
			turn.text = "AIs turn";
			thinkingPopup.SetActive (true);
			if (!brain.started) {
				GOAIState preState = new GOAIState (gameState, playerIndx, null, 0);
				aiThread = new Thread (new ThreadStart (() => brain.runAI (preState)));
				aiThread.Start ();
			}
			if (brain.done) {
				GOAIState postState = (GOAIState)brain.next;
				if (postState == null) {
					Debug.Log ("ERROR: Null State.");
				}
				gameState = postState.state;
				brain.reset ();
				//visualiseMove ();
				checkState(gameState.lastPiecePlayed);
				playersTurn = true;
				aiThread.Join ();
				redrawState ();
			}
		} else {
			thinkingPopup.SetActive (false);
			turn.text = "Your turn";
		}
		whiteScore.text = "White captures: " + gameState.lastWhiteScore;
		blackScore.text = "Black captures: " + gameState.lastBlackScore;
	}

	public override void spawn(int x, int y)
	{
		int[] playedPiece = new int[3]{ x, y, selectedColour };
		gameState.playPiece (playedPiece);
		gameState.checkForCaptures (playedPiece);
		checkState (playedPiece);

		if (gameState.illegalState) {
			int[] resetPiece = new int[3]{ x, y, 0 };
			gameState.playPiece (resetPiece);
			gameState.illegalState = false;
		} else {
			playersTurn = false;
			redrawState ();
		}
	}

	public void checkState (int[] playedPiece)
	{
		if (gameState.checkGameEnd (playedPiece)) {
			gamePlaying = false;
			Debug.Log ("Winner: " + gameState.winner);
			if (gameState.winner == playerIndx) {
				winlose.text = "You Won!";
			} else {
				winlose.text = "You Lost!";
			}
			endGameMenu.SetActive (true);
			return;
		}
	}

	private void redrawState()
	{
		foreach(GameObject tile in tiles)
		{
			if (tile != null) {
				tile.GetComponent<Tile> ().counter.SetActive (false);
				Destroy (tile);
			}
		}
		foreach(GameObject counter in counters)
		{
			Destroy(counter);
		}
		for (int i = 0; i < 6; i++) {
			for (int j = 0; j < 6; j++) {

				//create the game object 
				GameObject tile = (GameObject)Instantiate (preFabTile, new Vector3 ((i + (0.1f * i)), 0, j + (0.1f * j)), Quaternion.identity);
				tile.GetComponent<Tile> ().setMaster (this);
				tile.GetComponent<Tile> ().setXY (i, j);
				tiles.Add (tile);
				tile.GetComponent<Tile> ().preFabCounter = preFabCounter;

				if (gameState.board [i, j] == 1) {
					GameObject counter = (GameObject)Instantiate (preFabCounter, new Vector3 ((i + (0.1f * i)), 0.1f, (j + (0.1f * j))), Quaternion.identity);

					counter.GetComponent<Renderer> ().material.color = Color.white;
					counter.GetComponent<Collider> ().enabled = false;
					counters.Add (counter);
					tile.GetComponent<Tile> ().canPress = false;
				} else if (gameState.board [i, j] == 2) {
					GameObject counter = (GameObject)Instantiate (preFabCounter, new Vector3 ((i + (0.1f * i)), 0.1f, (j + (0.1f * j))), Quaternion.identity);
					counter.GetComponent<Renderer> ().material.color = Color.black;
					counter.GetComponent<Collider> ().enabled = false;
					counters.Add (counter);
					tile.GetComponent<Tile> ().canPress = false;
				}

			}
		} 
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

