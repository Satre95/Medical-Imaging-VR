using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DisplayBody : MonoBehaviour {

    public string DisplayId;

	public void displayBodyParts(string idName)
    {

        GameObject.DontDestroyOnLoad(this.gameObject);
        DisplayId = idName;
        SceneManager.LoadScene("Display Scene",LoadSceneMode.Single);
    }
}
