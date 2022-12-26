using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame ()
    {
        SceneLoader.Instance.LoadLevel(SceneNames.GAME_SCENE);
    }

    public void QuitGame ()
    {
        Application.Quit();
    }
}
