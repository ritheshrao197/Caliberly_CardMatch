using UnityEngine;

/// <summary>
/// Service to manage a simple timer for tracking elapsed time in the memory game.
/// Updates elapsed time when running and provides methods to control the timer state.
/// </summary>
public class TimerService : MonoBehaviour
{
    /// <summary>
    /// Indicates whether the timer is currently running
    /// </summary>
    public bool running = false;
    
    /// <summary>
    /// The total elapsed time in seconds
    /// </summary>
    public float elapsed { get; private set; }

    private void Update()
    {
        // Update elapsed time if the timer is running
        if (running) elapsed += Time.deltaTime;
    }

    /// <summary>
    /// Resets the elapsed time to zero
    /// </summary>
    public void ResetTimer() => elapsed = 0f;
    
    /// <summary>
    /// Starts the timer
    /// </summary>
    public void StartTimer() => running = true;
    
    /// <summary>
    /// Stops the timer
    /// </summary>
    public void StopTimer() => running = false;
}