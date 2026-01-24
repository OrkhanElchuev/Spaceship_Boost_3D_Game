using UnityEngine;

/// <summary>
/// Moves a GameObject back and forth between its start position and a target offset
/// using PingPong + Lerp.
/// </summary>

public class Oscillator : MonoBehaviour
{
    [Tooltip("Local offset from the start position to move towards.")]
    [SerializeField] Vector3 movementVector;
    [Tooltip("Speed multiplier for the ping-pong motion.")]
    [SerializeField] float speed;

    Vector3 startPosition;
    Vector3 endPosition;
    float movementFactor;

    void Start()
    {
        // Cahce endpoints once.
        startPosition = transform.position;        
        endPosition = startPosition + movementVector;
    }

    // Update is called once per frame
    void Update()
    {
        // Produces a value that goes 0 -> 1 -> 0 over time.
        movementFactor = Mathf.PingPong(Time.time * speed, 1f);
        transform.position = Vector3.Lerp(startPosition, endPosition, movementFactor);
    }
}
