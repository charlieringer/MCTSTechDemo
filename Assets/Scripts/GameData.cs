using UnityEngine;

public class GameData : MonoBehaviour {
	static public int playerIndex = 0;

	public void setPlayerIndex(int indx)
	{
		playerIndex = indx;
	}
}