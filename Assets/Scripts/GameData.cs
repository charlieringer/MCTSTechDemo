using UnityEngine;

public class GameData : MonoBehaviour {
	static public int playerIndex = 0;

	//This just stores the player index so it can be passed from the front end to the game
	public void setPlayerIndex(int indx)
	{
		playerIndex = indx;
	}
}