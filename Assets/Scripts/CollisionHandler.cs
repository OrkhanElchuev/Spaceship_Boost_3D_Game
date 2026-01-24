using System.Collections;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles collision outcomes for the player (win/finish, crash) and coordinates
/// audio/particle feedback plus scene transitions. Also includes simple debug controls:
/// N = load next level, C = toggle collision handling.
/// </summary>

public class CollisionHandler : MonoBehaviour
{
    [Tooltip("Delay (seconds) before reloading or loading the next scene after a crash/win.")]
    [SerializeField] float sceneLoadDelay = 1f;

    [Header("Audio")]
    [Tooltip("Played when the player crashes.")]
    [SerializeField] AudioClip crashSound;
    [Tooltip("Played when the player reaches the finish.")]
    [SerializeField] AudioClip winSound;

    [Header("Particles")]
    [Tooltip("Particles played on successful finish.")]
    [SerializeField] ParticleSystem winParticles;
    [Tooltip("Particles played on crash.")]
    [SerializeField] ParticleSystem crashParticles;

    AudioSource audioSource;
    Movement movement;
    Coroutine sceneCoroutine;
    WaitForSeconds sceneWait;

    bool isControllable = true;
    bool isCollidable = true;
    int currentSceneIndex;

    private void Awake()
    {
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        sceneWait = new WaitForSeconds(sceneLoadDelay);

        audioSource = GetComponent<AudioSource>();
        movement = GetComponent<Movement>();
    }

    private void Update()
    {
        RespondToDebugKeys();
    }

    private void RespondToDebugKeys()
    {
        if (!isControllable) return;

        // Debug: Load next scene.
        if (Keyboard.current?.nKey.wasPressedThisFrame == true)
        {
            isControllable = false; // prevent double-trigger
            LoadNextLevel();
            return;
        }

        // Debug: Toggle Collisions.
        if (Keyboard.current?.cKey.wasPressedThisFrame == true)
        {
            isCollidable = !isCollidable;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!isControllable || !isCollidable) return; 

        if (other.gameObject.CompareTag("Friendly")) return;

        if (other.gameObject.CompareTag("Finish"))
        {
            StartLevelFinishSequence();
            return;
        }

        StartCrashSequence();
    }

    private void StartLevelFinishSequence()
    {
        PlaySound(winSound);
        DisableMovement();

        if (winParticles != null) winParticles.Play();
        
        // Prevent multiple scene load coroutines.
        if (sceneCoroutine == null)
        {
            sceneCoroutine = StartCoroutine(LoadNextLevelAfterDelay());
        }
    }

    private void StartCrashSequence()
    {
        PlaySound(crashSound);
        DisableMovement();

        if (crashParticles != null) crashParticles.Play();

        if (sceneCoroutine == null)
        {
            sceneCoroutine = StartCoroutine(ReloadLevelAfterDelay());
        }
    }

    private void DisableMovement()
    {
        // If movement is disabled, input stops and physics handling stops.
        if (movement != null) movement.enabled = false;
        isControllable = false;
    }

    private void PlaySound(AudioClip thisSound)
    {
        if (audioSource == null || thisSound == null) return;

        // Stop any currently playing sound and play this one-shot.
        audioSource.Stop();
        audioSource.PlayOneShot(thisSound);
    }

    private void ReloadLevel()
    { 
        SceneManager.LoadScene(currentSceneIndex);
    }

    private void LoadNextLevel()
    {
        int nextScene = currentSceneIndex + 1;
        
        // Wrap around to the first scene.
        if (nextScene >= SceneManager.sceneCountInBuildSettings)
        {
            nextScene = 0;
        }

        SceneManager.LoadScene(nextScene);
    }

    IEnumerator LoadNextLevelAfterDelay()
    {
        yield return sceneWait;
        LoadNextLevel();
    }

    IEnumerator ReloadLevelAfterDelay()
    {
        yield return sceneWait;
        ReloadLevel();
    }
}
