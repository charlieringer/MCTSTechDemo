using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

	public Renderer renderer;
	private GameMaster master;
	public bool canPress;
	public int x;
	public int y;
	public GameObject preFabCounter;
	GameObject counter;

	public Texture tileTexture;

	// Use this for initialization
	void Start () {
		renderer.material.color = Color.grey;
		renderer.material.mainTexture =  tileTexture;
		counter = (GameObject)Instantiate(preFabCounter, new Vector3 ((x+(0.1f*x)), 0.1f, (y+(0.1f*y))), Quaternion.identity);
		counter.SetActive (false);
		counter.GetComponent<Collider> ().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnMouseOver()
	{
		renderer.material.color = Color.grey;
		renderer.material.mainTexture =  tileTexture;
		if (canPress) {
			renderer.material.color = Color.green;
			counter.SetActive (true);
			if (master.selectedPiece == 1) {
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
		renderer.material.color = Color.grey;
		renderer.material.mainTexture =  tileTexture;
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
