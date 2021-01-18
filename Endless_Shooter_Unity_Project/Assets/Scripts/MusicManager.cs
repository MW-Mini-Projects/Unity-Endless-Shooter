using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public AudioClip[] levelSongs;
    public AudioClip menuTheme;
    Spawner spawner;

    string sceneName;

    int oldWaveIndex;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string newSceneName = SceneManager.GetActiveScene().name;
        if(newSceneName != sceneName || newSceneName == "Game")
        {
            sceneName = newSceneName;
            //Delay so that we can delete the audiomanager in the scene we are in
            Invoke("PlayMusic", .2f);
        }
    }

    void OnSpawnerNewWave(int waveIndex)
    {
        if(oldWaveIndex == waveIndex)
        {
            return;
        }
        oldWaveIndex = waveIndex;
        AudioClip clipToPlay = levelSongs[waveIndex - 1];
        AudioManager.instance.PlayMusic(clipToPlay, 2);
        Invoke("PlayMusic", clipToPlay.length);
    }

    void PlayMusic()
    {
        AudioClip clipToPlay = null;

        if(sceneName == "Menu")
        {
            clipToPlay = menuTheme;
        }
        else if(sceneName == "Game")
        {
            print("I was called?");
            clipToPlay = levelSongs[0];
            spawner = GameObject.FindObjectOfType<Spawner>();
            spawner.OnNewWave += OnSpawnerNewWave;
        }

        if(clipToPlay != null)
        {
            AudioManager.instance.PlayMusic(clipToPlay, 2);
            CancelInvoke("PlayMusic");
            Invoke("PlayMusic", clipToPlay.length);
        }
    }
}
