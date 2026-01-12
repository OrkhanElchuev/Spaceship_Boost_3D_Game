using System;
using NUnit.Framework;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] float sceneLoadDelay = 1f;
    [SerializeField] AudioClip crashSound;
    [SerializeField] AudioClip winSound;
    [SerializeField] ParticleSystem winParticles;
    [SerializeField] ParticleSystem crashParticles;

    AudioSource audioSource;

    bool isControllable = true;
    bool isCollidable = true;
    int currentScene = 0;

    private void Start()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        RespondToDebugKeys();
    }

    private void RespondToDebugKeys()
    {
        if (!isControllable) return;

        if (Keyboard.current.nKey.wasPressedThisFrame)
        {
            isControllable = false; // prevent double-trigger
            LoadNextLevel();
        }
        else if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            isCollidable = !isCollidable;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!isControllable || !isCollidable) { return; }

        switch (other.gameObject.tag)
        {
            case "Friendly":
                print("Friendly object");
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

    private void StartLevelFinishSequence()
    {
        PlaySound(winSound);
        DisableMovement();
        winParticles.Play();
        Invoke("LoadNextLevel", sceneLoadDelay);
    }

    private void StartCrashSequence()
    {
        PlaySound(crashSound);
        DisableMovement();
        crashParticles.Play();
        Invoke("ReloadLevel", sceneLoadDelay);
    }

    private void DisableMovement()
    {
        GetComponent<Movement>().enabled = false;
        isControllable = false;
    }

    private void PlaySound(AudioClip thisSound)
    {
        audioSource.Stop();
        audioSource.PlayOneShot(thisSound);
    }

    private void ReloadLevel()
    { 
        SceneManager.LoadScene(currentScene);
    }

    private void LoadNextLevel()
    {
        int nextScene = currentScene + 1;

        if (nextScene == SceneManager.sceneCountInBuildSettings)
        {
            nextScene = 0;
        }

        SceneManager.LoadScene(nextScene);
    }
}
