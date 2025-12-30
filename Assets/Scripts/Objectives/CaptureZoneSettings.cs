using UnityEngine;

namespace LooterShooter.Objectives
{
    /// <summary>
    /// Configuration data for capture zones. Allows balancing without modifying code.
    /// </summary>
    [CreateAssetMenu(fileName = "New Capture Zone Settings", menuName = "Looter Shooter/Capture Zone Settings")]
    public class CaptureZoneSettings : ScriptableObject
    {
        [Header("Capture")]
        [Tooltip("Time in seconds required to complete capture")]
        [SerializeField] private float captureTime = 10f;

        [Tooltip("Rate at which progress decays when player leaves (per second)")]
        [SerializeField] private float decayRate = 0.5f;

        [Tooltip("If true, progress resets instantly when player leaves")]
        [SerializeField] private bool resetOnExit = false;

        [Header("Zone")]
        [Tooltip("Radius of the capture zone")]
        [SerializeField] private float radius = 5f;

        [Header("Visual")]
        [SerializeField] private Color idleColor = Color.yellow;
        [SerializeField] private Color capturingColor = Color.green;
        [SerializeField] private Color capturedColor = Color.blue;

        public float CaptureTime => captureTime;
        public float DecayRate => decayRate;
        public bool ResetOnExit => resetOnExit;
        public float Radius => radius;
        public Color IdleColor => idleColor;
        public Color CapturingColor => capturingColor;
        public Color CapturedColor => capturedColor;
    }
}
