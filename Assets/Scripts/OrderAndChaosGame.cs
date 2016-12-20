using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class OrderAndChaosGame : MonoBehaviour {
	public GameMaster gm;
	public int selectedPiece;
	public Text role;
	public Text colour;
	public Text gameOver;
	private State gameState;
	private AI brain = new AI();


	private bool gamePlaying = true;
	private int aiPlayerIndx = 2;

	// Use this for initialization
	public void start () {
		gameState = new State ();
		selectedPiece = 1;
		role.text = "You are playing as Order";
		colour.text = "Selected playing piece: White";
		gameOver.text = "";
	}

	// Update is called once per frame
	public void update (bool playersTurn) {
		if (!playersTurn && gamePlaying) {
			OCAIState preState = new OCAIState (gameState, aiPlayerIndx, null, 0);
			OCAIState postState = (OCAIState)brain.run (preState);
			gameState = postState.state;
			visualiseMove ();
			gm.playersTurn = true;
		}
		if (Input.GetKeyDown ("space")) {
			if (selectedPiece == 1) {
				colour.text = "Selected playing piece: Black";
				selectedPiece = 2;
				gm.selectedPiece = 2;
			} else {
				colour.text = "Selected playing piece: White";
				selectedPiece = 1;
				gm.selectedPiece = 1;
			}
		}
	}
		

	public void checkState (int[] playedPiece)
	{
		if (gameState.checkGameEnd (playedPiece)) {
			gamePlaying = false;
			gameOver.text = "GAME OVER!\nOrder Wins";
			return;
		}
		if (gameState.numbPiecesPlayed == 36) {
			gamePlaying = false;
			gameOver.text = "GAME OVER!\nChaos Wins";
			return;
		}
	}

	private void visualiseMove()
	{
		List<GameObject> tiles = gm.tiles;
		foreach(GameObject tile in tiles)
		{
			int pX = gameState.lastPiecePlayed [0];
			int pY = gameState.lastPiecePlayed [1];

			int tX = tile.GetComponent<Tile> ().x;
			int tY = tile.GetComponent<Tile> ().y;
			if (pX == tX && pY == tY) {
				tile.GetComponent<Tile>().aiPlayHere(gameState.lastPiecePlayed [2]);
			}
		}
	}

	public void playAndCheck(int[] playedPiece)
	{
		gameState.playPiece (playedPiece);
		checkState (playedPiece);
	}
}