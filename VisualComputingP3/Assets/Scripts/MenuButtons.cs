using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public string sceneToLoad = "VigScene"; 

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

