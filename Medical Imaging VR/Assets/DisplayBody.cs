using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DisplayBody : MonoBehaviour {

    public string DisplayId;
    static DisplayBody displayBodyComp;

	public void displayBodyParts(string idName)
    {
        //coming back from the display scene
        if (displayBodyComp != null)
        {
            Destroy(this.gameObject);
            return;
        }
        displayBodyComp = this;
        GameObject.DontDestroyOnLoad(this.gameObject);
        DisplayId = idName;
        SceneManager.LoadScene("Display Scene",LoadSceneMode.Single);
    }
}
