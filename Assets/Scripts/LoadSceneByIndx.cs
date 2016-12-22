using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneByIndx : MonoBehaviour {

	public void loadSceneIndex(int indx)
	{
		SceneManager.LoadScene (indx);
	}
}
