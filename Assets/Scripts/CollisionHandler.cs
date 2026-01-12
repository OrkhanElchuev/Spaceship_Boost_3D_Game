using System;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] float sceneLoadDelay = 1f;
    [SerializeField] AudioClip crashSound;
    [SerializeField] AudioClip winSound;

    AudioSource audioSource;

    bool isControllable = true;

    int currentScene = 0;

    private void Start()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!isControllable) { return; }

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
        isControllable = false;
    }

    private void StartLevelFinishSequence()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(winSound);
        DisableMovement();
        Invoke("LoadNextLevel", sceneLoadDelay);
    }

    private void StartCrashSequence()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(crashSound);
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
