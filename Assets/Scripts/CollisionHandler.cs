using System;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] float sceneLoadDelay = 1f;
    int currentScene = 0;

    void Awake()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
    }

    void OnCollisionEnter(Collision other)
    {
        switch (other.gameObject.tag)
        {
            case "Friendly":
                print("Friend");
                break;
            case "Finish":
                StartLevelFinishSequence();
                break;
            case "Fuel":
                print("Fuel");
                break;
            default:
                StartCrashSequence();
                break;
        }
    }

    private void DisableMovement()
    {
        GetComponent<Movement>().enabled = false;
    }

    private void StartLevelFinishSequence()
    {
        DisableMovement();
        Invoke("LoadNextLevel", sceneLoadDelay);
    }

    private void StartCrashSequence()
    {
        DisableMovement();
        Invoke("ReloadLevel", sceneLoadDelay);
    }

    void ReloadLevel()
    { 
        SceneManager.LoadScene(currentScene);
    }

    void LoadNextLevel()
    {
        int nextScene = currentScene + 1;

        if (nextScene == SceneManager.sceneCountInBuildSettings)
        {
            nextScene = 0;
        }

        SceneManager.LoadScene(nextScene);
    }
}
