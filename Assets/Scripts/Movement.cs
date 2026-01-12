using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField] InputAction thrust;
    [SerializeField] InputAction rotation;
    [SerializeField] float thrustPower = 100f;
    [SerializeField] float rotationPower = 100f;
    [SerializeField] AudioClip engineThrustSound;
    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem rightThrustParticles;
    [SerializeField] ParticleSystem leftThrustParticles;

    Rigidbody rb;
    AudioSource audioSource;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        thrust.Enable();
        rotation.Enable();
    }

    private void FixedUpdate()
    {
        Thrusting();
        Rotating();
    }

    private void Thrusting()
    {
        if (thrust.IsPressed())
        {
            rb.AddRelativeForce(Vector3.up * thrustPower * Time.fixedDeltaTime);
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(engineThrustSound);            
            }

            if (!mainEngineParticles.isPlaying)
            {
                mainEngineParticles.Play();
            }
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void Rotating()
    {
        float rotationInput = rotation.ReadValue<float>();
        
        if (rotationInput < 0)
        {
            ApplyRotation(rotationPower);
            if (!rightThrustParticles.isPlaying)
            {
                leftThrustParticles.Stop();
                rightThrustParticles.Play();
            }
        }
        else if (rotationInput > 0)
        {
            ApplyRotation(-rotationPower);
            if (!leftThrustParticles.isPlaying)
            {
                rightThrustParticles.Stop();
                leftThrustParticles.Play();
            }   
        }
        else
        {
            rightThrustParticles.Stop();
            leftThrustParticles.Stop();
        }
    }

    private void ApplyRotation(float rotationInThisFrame)
    {
        rb.freezeRotation = true;
        transform.Rotate(Vector3.forward * rotationInThisFrame * Time.fixedDeltaTime);
        rb.freezeRotation = false;
    }
}
