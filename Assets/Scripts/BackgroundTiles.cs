using UnityEngine;
using System.Collections.Generic;

public class BackgroundTiles : MonoBehaviour {
	List<GameObject> tiles = new List<GameObject>();
	public GameObject preFabTile;

	// Use this for initialization
	void Start () {
		tiles = new List<GameObject>();
		for (int i = 0; i < 6; i++) {
			for (int j = 0; j < 6; j++) {
				//create the game object 
				GameObject tile = (GameObject)Instantiate (preFabTile, new Vector3 ((i + (0.1f * i)), 0, j + (0.1f * j)), Quaternion.identity);
				//tile.GetComponent<Tile> ().setMaster (this);
				tile.GetComponent<Tile> ().setXY (i, j);
				tiles.Add (tile);
			}
		} 
	}
}
