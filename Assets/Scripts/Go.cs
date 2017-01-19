using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Threading;

//Game master object for Go specifically
public class Go : GameMaster {

	public Text colour;
	public Text turn;
	public Text winlose;
	public Text whiteScore;
	public Text blackScore;
	GOState gameState;
	private List<GameObject> counters = new List<GameObject>();

	void Start () {
		//Sets up the game
		//By seting values based on what side you have selected
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

		//Creates the new tiles
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
		//Create a new game state.
		gameState = new GOState ();
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
				GOAIState preState = new GOAIState (gameState, playerIndx, null, 0);
				//Make a new AI thread with this state
				aiThread = new Thread (new ThreadStart (() => brain.runAI (preState)));
				//And start it.
				aiThread.Start ();
			}
			//We check is the brain is done
			if (brain.done) {
				//And get the next state
				GOAIState postState = (GOAIState)brain.next;
				//If we could not get a state we have a problem
				if (postState == null) {
					Debug.Log ("ERROR: Null State.");
				}
				//Unpack the state
				gameState = postState.state;
				//Reset the brain
				brain.reset ();
				//Check the status of the game
				checkState(gameState.lastPiecePlayed);
				//It is non the players turn
				playersTurn = true;
				//Join the finished thread
				aiThread.Join ();
				//And redraw the whole game state
				redrawState ();
			}
		} else {
			//It is the players turn so tell them so
			thinkingPopup.SetActive (false);
			turn.text = "Your turn";
		}
		//Show scores
		whiteScore.text = "White scores: " + gameState.whiteCaptureScore;
		blackScore.text = "Black scores: " + gameState.blackCaptureScore;
	}

	public override void spawn(int x, int y)
	{
		//This is for when the Human player plays a move
		//Make a played peice
		int[] playedPiece = new int[3]{ x, y, selectedColour };
		//Play it
		gameState.playPiece (playedPiece);
		//Check to see if it caputured anything
		gameState.checkForCaptures (playedPiece);
		//Check the end of the game
		checkState (playedPiece);

		//If this state is illegal
		if (gameState.illegalState) {
			//Reset the move
			int[] resetPiece = new int[3]{ x, y, 0 };
			gameState.playPiece (resetPiece);
			gameState.illegalState = false;
		} else {
			//Otherwise it is now the AIs turn
			playersTurn = false;
			redrawState ();
		}
	}

	public void checkState (int[] playedPiece)
	{
		//Check to see if the game has ended
		if (gameState.checkGameEnd (playedPiece)) { 
			//And update the text if this is the case
			gamePlaying = false;
			if (gameState.winner == playerIndx) {
				winlose.text = "You Won!";
			} else {
				winlose.text = "You Lost!";
			}
			//And bring up the end of game menu
			endGameMenu.SetActive (true);
			return;
		}
	}

	private void redrawState()
	{
		//REdraws the state (more complicated for Go because peices get removed as well as added).

		//Destroy all the tiles
		foreach(GameObject tile in tiles)
		{
			if (tile != null) {
				//Ensuring the count is set to not active
				tile.GetComponent<Tile> ().counter.SetActive (false);
				Destroy (tile);
			}
		}
		//Destroy all of the counters
		foreach(GameObject counter in counters)
		{
			Destroy(counter);
		}
		//Loop through the board
		for (int i = 0; i < 6; i++) {
			for (int j = 0; j < 6; j++) {

				//create the game object 
				GameObject tile = (GameObject)Instantiate (preFabTile, new Vector3 ((i + (0.1f * i)), 0, j + (0.1f * j)), Quaternion.identity);
				tile.GetComponent<Tile> ().setMaster (this);
				tile.GetComponent<Tile> ().setXY (i, j);
				tiles.Add (tile);
				tile.GetComponent<Tile> ().preFabCounter = preFabCounter;
				//If it is a white piece
				if (gameState.board [i, j] == 1) {
					//Instantiate the counter
					GameObject counter = (GameObject)Instantiate (preFabCounter, new Vector3 ((i + (0.1f * i)), 0.1f, (j + (0.1f * j))), Quaternion.identity);
					//Set it to white
					counter.GetComponent<Renderer> ().material.color = Color.white;
					counter.GetComponent<Collider> ().enabled = false;
					counters.Add (counter);
					//And stop it being pressed
					tile.GetComponent<Tile> ().canPress = false;
				//if it is a black piece
				} else if (gameState.board [i, j] == 2) {
					//Instantiate the counter
					GameObject counter = (GameObject)Instantiate (preFabCounter, new Vector3 ((i + (0.1f * i)), 0.1f, (j + (0.1f * j))), Quaternion.identity);
					counter.GetComponent<Renderer> ().material.color = Color.black;
					//Set it to black
					counter.GetComponent<Collider> ().enabled = false;
					counters.Add (counter);
					//And stop it being pressed
					tile.GetComponent<Tile> ().canPress = false;
				}
			}
		} 
	}
}

