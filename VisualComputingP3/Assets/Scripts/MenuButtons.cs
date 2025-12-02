using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public string sceneToLoad = "TheOne"; 

    public void StartGame()
    {
        Debug.Log("START GAME PRESSED");
        SceneManager.LoadScene(sceneToLoad);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT BUTTON PRESSED");
    }
}

