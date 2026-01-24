using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Quits the application when Escape is pressed.
/// </summary>

public class QuitGame : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Application.Quit();
            Debug.Log("Exit the game");
        }
    }
}
    