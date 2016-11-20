using UnityEngine;
using System.Collections;

public class displayControl : MonoBehaviour {

	//sceneID corresponds to system buttons->body buttons->final display
	public void changeDisplay (int sceneID) {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneID);
	}
}
