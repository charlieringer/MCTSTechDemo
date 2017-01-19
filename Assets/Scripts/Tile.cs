using UnityEngine;
using System.Collections;

//Class for the tile
public class Tile : MonoBehaviour {

	public Renderer rend;
	private GameMaster master;
	public bool canPress;
	public int x;
	public int y;
	public GameObject preFabCounter;
	public GameObject counter;

	public Texture tileTexture;

	// Use this for initialization
	void Start () {
		rend.material.mainTexture =  tileTexture;
		counter = (GameObject)Instantiate(preFabCounter, new Vector3 ((x+(0.1f*x)), 0.1f, (y+(0.1f*y))), Quaternion.identity);
		counter.SetActive (false);
		counter.GetComponent<Collider> ().enabled = false;
	}

	//When you mouse over 
	void OnMouseOver()
	{
		rend.material.mainTexture =  tileTexture;
		//If it can be pressed (therefore an empty space)
		if (canPress) {
			//Set the colour to green
			rend.material.color = Color.green;
			//And set the counter to active
			counter.SetActive (true);
			//And then set the colour of this counter based on the player colour
			if (master.selectedColour == 1) {
				counter.GetComponent<Renderer> ().material.color = Color.white;
			} else {
				counter.GetComponent<Renderer> ().material.color = Color.black;
			}
		}
	}

	void OnMouseDown()
	{
		//If it is the players turn and they can click this space
		if (master.playersTurn && canPress) {
			//Turn of the highlight counter (as a real counter will be spawned
			counter.SetActive (false);
			canPress = false;
			//And spawn the real counter
			master.spawn (x, y);
		}
	}

	void OnMouseExit()
	{
		//When we mouse off the tile we reset the highligh and hide the counter
		rend.material.mainTexture =  tileTexture;
		rend.material.color = Color.white;
		counter.SetActive (false);
	}

	public void setMaster(GameMaster _master)
	{
		//Sets which game master we are using.
		master = _master;
	}

	public void setXY(int _x, int _y)
	{
		//Sets the XY values of the tile
		x = _x;
		y = _y;
	}

	public void aiPlayHere(int colourIndex)
	{
		//Function to play the AI move here
		//Make a new counter
		GameObject counter = (GameObject)Instantiate(preFabCounter, new Vector3 ((x+(0.1f*x)), 0.1f, (y+(0.1f*y))), Quaternion.identity);
		counter.GetComponent<Collider> ().enabled = false;
		//Set it to the right colour
		if (colourIndex == 1) {
			counter.GetComponent<Renderer> ().material.color = Color.white;
		} else {
			counter.GetComponent<Renderer> ().material.color = Color.black;
		}
		//Player can no longer press this tile
		canPress = false;
	}
}
