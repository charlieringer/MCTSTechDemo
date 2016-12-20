using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour {

	public OrderAndChaosGame ocGame; 
	public GameObject preFabTile;
	public GameObject preFabCounter;
	public int x;
	public int y;
	public int tileSize;
	public int gap;
	public List<GameObject> tiles;
	public int selectedPiece = 1;
	public bool playersTurn = true;

	void Start () {
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
		ocGame.start ();
	}

	// Update is called once per frame
	void Update () {
		ocGame.update (playersTurn);
	}

	public void spawn(int x, int y)
	{
		playersTurn = false;
		GameObject counter = (GameObject)Instantiate(preFabCounter, new Vector3 ((x+(0.1f*x)), 0.1f, (y+(0.1f*y))), Quaternion.identity);
		counter.GetComponent<Collider> ().enabled = false;
		if (selectedPiece == 1) {
			counter.GetComponent<Renderer> ().material.color = Color.white;
		} else {
			counter.GetComponent<Renderer> ().material.color = Color.black;
		}
		int[] playedPiece = new int[3]{ x, y, selectedPiece};

		ocGame.playAndCheck (playedPiece);
	}
}