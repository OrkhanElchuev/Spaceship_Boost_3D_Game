using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls player movement using the new Input System:
/// - Thrust applies an upward relative force
/// - Rotation rotates the rigidbody around Z
/// Also manages engine audio and particle effects for thrust/rotation.
/// </summary>

public class Movement : MonoBehaviour
{
    [Header("Input Actions")]
    [Tooltip("Input Action used for thrust.")]
    [SerializeField] InputAction thrust;
    [Tooltip("Input Action used for rotation. Negative=right, Positive=left.")]
    [SerializeField] InputAction rotation;

    [Header("Movement")]
    [Tooltip("Force applied when thrusting.")]
    [SerializeField] float thrustPower = 100f;
    [Tooltip("Rotation power (degrees per second-ish, scaled by FixedDeltaTime).")]
    [SerializeField] float rotationPower = 100f;

    [Header("Audio")]
    [Tooltip("Looping engine sound played while thrust is held.")]
    [SerializeField] AudioClip engineThrustSound;

    [Header("Particles")]
    [Tooltip("Main engine particles played while thrusting.")]
    [SerializeField] ParticleSystem mainEngineParticles;
    [Tooltip("Particles on the right thruster (used when rotating left).")]
    [SerializeField] ParticleSystem rightThrustParticles;
    [Tooltip("Particles on the left thruster (used when rotating right).")]
    [SerializeField] ParticleSystem leftThrustParticles;

    Rigidbody rb;
    AudioSource audioSource;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        // Configure engine audio once.
        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = true;
            audioSource.clip = engineThrustSound;
        }
    }

    private void OnEnable()
    {
        thrust.Enable();
        rotation.Enable();
    }

    private void OnDisable()
    {
        thrust.Disable();
        rotation.Disable();
    }

    private void FixedUpdate()
    {
        HandleThrust();
        HandleRotation();
    }

    private void HandleThrust()
    {
        if (thrust.IsPressed())
        {
            StartThrusting();
        }
        else
        {
            StopThrusting();
        }
    }

    private void StartThrusting()
    {
        // Apply upward relative force each physics step.
        rb.AddRelativeForce(Vector3.up * thrustPower * Time.fixedDeltaTime);

        // Start looping engine audio.
        if (audioSource != null && !audioSource.isPlaying && audioSource.clip != null)
        {
            audioSource.Play();
        }

        // Start engine particles.
        if (mainEngineParticles != null && !mainEngineParticles.isPlaying)
        {
            mainEngineParticles.Play();
        }
    }

    private void StopThrusting()
    {
        // Stop looping engine audio.
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        // Stop main engine partciles.
        if (mainEngineParticles != null && mainEngineParticles.isPlaying)
        {
            mainEngineParticles.Stop(); 
        }
    }

    private void HandleRotation()
    {
        float rotationInput = rotation.ReadValue<float>();
        
        if (rotationInput < 0)
        {
            RotateRight();
        }
        else if (rotationInput > 0)
        {
            RotateLeft();
        }
        else
        {
            StopRotatingPartciles();
        }
    }

    private void StopRotatingPartciles()
    {
        if (rightThrustParticles != null && rightThrustParticles.isPlaying)
            rightThrustParticles.Stop();

        if (leftThrustParticles != null && leftThrustParticles.isPlaying)
            leftThrustParticles.Stop();
    }

    private void RotateLeft()
    {
        ApplyRotation(-rotationPower);

        if (leftThrustParticles != null && !leftThrustParticles.isPlaying)
        {
            leftThrustParticles.Play();
        }

        if (rightThrustParticles != null && rightThrustParticles.isPlaying)
        {
            rightThrustParticles.Stop();
        }
    }

    private void RotateRight()
    {
        ApplyRotation(rotationPower);

        if (rightThrustParticles != null && !rightThrustParticles.isPlaying)
        {
            rightThrustParticles.Play();
        }

        if (leftThrustParticles != null && leftThrustParticles.isPlaying)
        {
            leftThrustParticles.Stop();
        }
    }

    private void ApplyRotation(float rotationInThisFrame)
    { 
        // Rigidbody rotation through physics for consistent behaviour.
        float rotationAmount = rotationInThisFrame * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, 0f, rotationAmount));
    }
}
