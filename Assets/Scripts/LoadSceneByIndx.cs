using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneByIndx : MonoBehaviour {

	public void loadSceneIndex(int indx)
	{
		//Loads the scene by the given index.
		SceneManager.LoadScene (indx);
	}
}
