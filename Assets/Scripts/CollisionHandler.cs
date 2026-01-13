using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] float sceneLoadDelay = 1f;

    [Header("Audio")]
    [SerializeField] AudioClip crashSound;
    [SerializeField] AudioClip winSound;

    [Header("Particles")]
    [SerializeField] ParticleSystem winParticles;
    [SerializeField] ParticleSystem crashParticles;

    AudioSource audioSource;
    Movement movement;
    Coroutine sceneCoroutine;


    bool isControllable = true;
    bool isCollidable = true;
    int currentScene = 0;

    private void Awake()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
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
        if (sceneCoroutine == null)
        {
            sceneCoroutine = StartCoroutine(LoadNextLevelAfterDelay());
        }
    }

    private void StartCrashSequence()
    {
        PlaySound(crashSound);
        DisableMovement();
        crashParticles.Play();
        if (sceneCoroutine == null)
        {
            sceneCoroutine = StartCoroutine(ReloadLevelAfterDelay());
        }
    }

    private void DisableMovement()
    {
        movement.enabled = false;
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

    IEnumerator LoadNextLevelAfterDelay()
    {
        yield return new WaitForSeconds(sceneLoadDelay);
        LoadNextLevel();
    }

    IEnumerator ReloadLevelAfterDelay()
    {
        yield return new WaitForSeconds(sceneLoadDelay);
        ReloadLevel();
    }
}
