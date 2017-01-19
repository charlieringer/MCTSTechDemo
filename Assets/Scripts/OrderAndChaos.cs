using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Threading;

public class OrderAndChaos : GameMaster {
	
	public Text role;
	public Text colour;
	public Text turn;
	public Text winlose;
	OCState gameState;

	void Start () {
		//Set up values based on which side the player chose
		if (GameData.playerIndex == 1) {
			role.text = "You are playing as Order";
			turn.text = "Your turn";
			playerIndx = 1;
			playersTurn = true;
		} else {
			role.text = "You are playing as Chaos";
			turn.text = "AIs turn";
			playerIndx = 2;
			playersTurn = false;
		}
		//Loop over all of the grid spaces and make a tile
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
		//Make a new game state
		gameState = new OCState ();
		//And set the selected colour
		selectedColour = 1;
		colour.text = "Selected playing piece: White";
	}

	// Update is called once per frame
	void Update () {
		//If the game is running and it is time for the AI to play
		if (!playersTurn && gamePlaying) {
			//Set the text to display this
			turn.text = "AIs turn";
			//Active the thinking popup
			thinkingPopup.SetActive (true);
			//If we have not started the AI brain thinking
			if (!brain.started) {
				//We make an AI State to represent the current state
				OCAIState preState = new OCAIState (gameState, playerIndx, null, 0);
				//Make a new AI thread with this state
				aiThread = new Thread (new ThreadStart (() => brain.runAI (preState)));
				//And start it.
				aiThread.Start ();
			}
			//We check is the brain is done
			if (brain.done) {
				//And get the next state
				OCAIState postState = (OCAIState)brain.next;
				//If we could not get a state we have a problem
				if (postState == null) {
					Debug.Log ("ERROR: Null State.");
				}
				//Unpack the state
				gameState = postState.state;
				//Reset the brain
				brain.reset ();
				//Draw the move
				visualiseMove ();
				//It is non the players turn
				playersTurn = true;
				//Join the finished thread
				aiThread.Join ();
			}
		} else {
			//It is the players turn so tell them so
			thinkingPopup.SetActive (false);
			turn.text = "Your turn";
		}
		//Check to see if the colour has been changed
		if (Input.GetKeyDown ("space")) {
			if (selectedColour == 1) {
				colour.text = "Selected playing piece: Black";
				selectedColour = 2;
			} else {
				colour.text = "Selected playing piece: White";
				selectedColour = 1;
			}
		}
	}

	public override void spawn(int x, int y)
	{
		//This is for when the Human player plays a move
		playersTurn = false;
		//Make a counter
		GameObject counter = (GameObject)Instantiate(preFabCounter, new Vector3 ((x+(0.1f*x)), 0.1f, (y+(0.1f*y))), Quaternion.identity);
		counter.GetComponent<Collider> ().enabled = false;
		//Colour it correctly
		if (selectedColour == 1) {
			counter.GetComponent<Renderer> ().material.color = Color.white;
		} else {
			counter.GetComponent<Renderer> ().material.color = Color.black;
		}
		//And then make it as in int array
		int[] playedPiece = new int[3]{ x, y, selectedColour};
		//And use that to check the game start after the move
		gameState.playPiece (playedPiece);
		checkState (playedPiece);
	}

	public void checkState (int[] playedPiece)
	{
		//If the game is over
		if (gameState.checkGameEnd (playedPiece)) {
			//Order has won
			gamePlaying = false;
			if (GameData.playerIndex == 1) {
				winlose.text = "You Won!";
			} else {
				winlose.text = "You Lost!";
			}
			//Turn on the end of game screen
			endGameMenu.SetActive (true);
			return;
		}
		//If the board is full
		if (gameState.numbPiecesPlayed == 36) {
			//Chaos has won
			gamePlaying = false;
			if (GameData.playerIndex == 1) {
				winlose.text = "You Lost!";
			} else {
				winlose.text = "You Won!";
			}
			//Turn on the end of game screen
			endGameMenu.SetActive (true);
			return;
		}
	}

	private void visualiseMove()
	{
		//For visualising the AIs moves
		bool found = false;
		foreach(GameObject tile in tiles)
		{
			//Find the corresponding tile 
			int pX = gameState.lastPiecePlayed [0];
			int pY = gameState.lastPiecePlayed [1];

			int tX = tile.GetComponent<Tile> ().x;
			int tY = tile.GetComponent<Tile> ().y;
			//and play here
			if (pX == tX && pY == tY) {
				tile.GetComponent<Tile>().aiPlayHere(gameState.lastPiecePlayed [2]);
				found = true;
			}
		}
		if (!found) {
			Debug.Log ("ERROR: Could not locate AI move.");
		}
		//When check the end game. 
		checkState (gameState.lastPiecePlayed);
	}
}