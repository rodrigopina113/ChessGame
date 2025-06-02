using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    //SceneChanger do Level 1

    public void LoadScene1()
    {
        SceneManager.LoadScene("Level2"); // NEXT vai ao Level 2
    }

    public void LoadScene2()
    {
        SceneManager.LoadScene("LevelSelect"); //vai para o Level Select
    }

    public void LoadScene3()
    {
        SceneManager.LoadScene("Menu"); //vai para o Main Menu
    }
}