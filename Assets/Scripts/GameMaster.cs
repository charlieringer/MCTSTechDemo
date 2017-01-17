using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Threading;

abstract public class GameMaster : MonoBehaviour {
	protected AI brain = new AI();
	protected bool gamePlaying = true;
	protected int playerIndx;
	protected State gameState;
	protected Thread aiThread;

	public int selectedColour;
	public GameObject preFabTile;
	public GameObject preFabCounter;
	public int x;
	public int y;
	public int tileSize;
	public int gap;
	public List<GameObject> tiles;
	public bool playersTurn = true;

	public GameObject thinkingPopup;
	public GameObject endGameMenu;

	public abstract void spawn (int x, int y);
}