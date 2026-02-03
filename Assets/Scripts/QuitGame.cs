using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitGame : MonoBehaviour
{

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Quit");
    }

public void NextScene()
{
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
}
void Start () {
        Cursor.visible = true;
    }
}
