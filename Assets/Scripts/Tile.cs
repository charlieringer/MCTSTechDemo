using UnityEngine;
using System.Collections;

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

	void OnMouseOver()
	{
		rend.material.mainTexture =  tileTexture;
		if (canPress) {
			rend.material.color = Color.green;
			counter.SetActive (true);
			if (master.selectedColour == 1) {
				counter.GetComponent<Renderer> ().material.color = Color.white;
			} else {
				counter.GetComponent<Renderer> ().material.color = Color.black;
			}
		}
	}

	void OnMouseDown()
	{
		if (master.playersTurn && canPress) {
			counter.SetActive (false);
			canPress = false;
			master.spawn (x, y);
		}
	}

	void OnMouseExit()
	{
		rend.material.mainTexture =  tileTexture;
		rend.material.color = Color.white;
		counter.SetActive (false);
	}

	public void setMaster(GameMaster _master)
	{
		master = _master;
	}

	public void setXY(int _x, int _y)
	{
		x = _x;
		y = _y;
	}

	public void aiPlayHere(int colourIndex)
	{
		GameObject counter = (GameObject)Instantiate(preFabCounter, new Vector3 ((x+(0.1f*x)), 0.1f, (y+(0.1f*y))), Quaternion.identity);
		counter.GetComponent<Collider> ().enabled = false;
		if (colourIndex == 1) {
			counter.GetComponent<Renderer> ().material.color = Color.white;
		} else {
			counter.GetComponent<Renderer> ().material.color = Color.black;
		}
		canPress = false;
	}
}
