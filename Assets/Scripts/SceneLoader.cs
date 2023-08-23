using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }
    public enum SceneIndex {MainMenu=0,Gameplay=1,Winning=2}
    private void Awake()
    {
        if(Instance != null)
        {
            Debug.Log("[Singleton] Trying to instantiate a seccond instance of a singleton class.");
            return;
        }
        else
        {
            Instance = this;
        }
    }

    public delegate void OnSceneLoad(SceneIndex sceneIndex);
    public static event OnSceneLoad sceneLoaded;

    public void LoadScene(SceneIndex sceneIndex)
    {
        sceneLoaded?.Invoke(sceneIndex);
        SceneManager.LoadScene((int)sceneIndex);
    }
}
