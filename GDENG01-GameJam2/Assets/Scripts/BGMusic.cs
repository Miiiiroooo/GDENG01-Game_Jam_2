using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMusic : MonoBehaviour
{
    private static BGMusic Instance;
    [SerializeField] private AudioSource audioScouce;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            this.audioScouce = this.GetComponent<AudioSource>();
            GameObject.DontDestroyOnLoad(this.gameObject);
            SceneManager.activeSceneChanged += OnChangeScene;
        }
    }

    void OnDisable()
    {
        if (Instance != null && Instance == this)
        {
            SceneManager.activeSceneChanged -= OnChangeScene;
        }
    }

    private void OnChangeScene(Scene currentScene, Scene nextScene)
    {
        if (nextScene.name == SceneNames.MAIN_MENU_SCENE)
        {
            this.audioScouce.volume = 0.5f;
        }
        else if (nextScene.name == SceneNames.GAME_SCENE)
        {
            this.audioScouce.volume = 0.03f;
        }
    }
}
