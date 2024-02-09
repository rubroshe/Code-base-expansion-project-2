using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game_Manager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void quit()
    {
        Application.Quit();
    }

    public void loadLevel(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void restart()
    {
        int thisOne = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(thisOne);
    }

    public void nextLevel()
    {
        int nextOne = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextOne);
    }
}
