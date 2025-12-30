using System;
using UnityEngine;

namespace LooterShooter.Objectives
{
    /// <summary>
    /// Capture zone states.
    /// </summary>
    public enum CaptureState
    {
        Idle,
        Capturing,
        Captured
    }

    /// <summary>
    /// A capture zone that the player must stand in for a duration to complete.
    /// Similar to Risk of Rain altar mechanics.
    /// </summary>
    [RequireComponent(typeof(SphereCollider))]
    public class CaptureZone : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private CaptureZoneSettings settings;

        [Header("State (Read Only)")]
        [SerializeField] private CaptureState currentState = CaptureState.Idle;
        [SerializeField] private float captureProgress = 0f;

        private SphereCollider _triggerCollider;
        private bool _playerInZone = false;

        /// <summary>Current state of the capture zone.</summary>
        public CaptureState CurrentState => currentState;

        /// <summary>Capture progress from 0 to 1.</summary>
        public float CaptureProgress => captureProgress;

        /// <summary>Whether the zone has been fully captured.</summary>
        public bool IsCaptured => currentState == CaptureState.Captured;

        /// <summary>Fired when player enters and capture begins.</summary>
        public event Action OnCaptureStarted;

        /// <summary>Fired each frame with current progress (0-1).</summary>
        public event Action<float> OnCaptureProgress;

        /// <summary>Fired when player exits before completion.</summary>
        public event Action OnCaptureInterrupted;

        /// <summary>Fired when capture reaches 100%.</summary>
        public event Action OnCaptureCompleted;

        private void Awake()
        {
            _triggerCollider = GetComponent<SphereCollider>();
            _triggerCollider.isTrigger = true;

            if (settings != null)
            {
                _triggerCollider.radius = settings.Radius;
            }
        }

        private void Update()
        {
            if (currentState == CaptureState.Captured) return;

            if (_playerInZone)
            {
                UpdateCapturing();
            }
            else if (captureProgress > 0f)
            {
                UpdateDecay();
            }
        }

        private void UpdateCapturing()
        {
            if (settings == null) return;

            float progressPerSecond = 1f / settings.CaptureTime;
            captureProgress += progressPerSecond * Time.deltaTime;
            captureProgress = Mathf.Clamp01(captureProgress);

            OnCaptureProgress?.Invoke(captureProgress);

            if (captureProgress >= 1f)
            {
                CompleteCapture();
            }
        }

        private void UpdateDecay()
        {
            if (settings == null) return;

            captureProgress -= settings.DecayRate * Time.deltaTime;
            captureProgress = Mathf.Max(captureProgress, 0f);

            OnCaptureProgress?.Invoke(captureProgress);

            if (captureProgress <= 0f && currentState == CaptureState.Capturing)
            {
                currentState = CaptureState.Idle;
            }
        }

        private void CompleteCapture()
        {
            currentState = CaptureState.Captured;
            Debug.Log($"[CaptureZone] {gameObject.name} captured!");
            OnCaptureCompleted?.Invoke();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (currentState == CaptureState.Captured) return;
            if (!other.CompareTag("Player")) return;

            _playerInZone = true;

            if (currentState == CaptureState.Idle)
            {
                currentState = CaptureState.Capturing;
                Debug.Log($"[CaptureZone] {gameObject.name} capture started");
                OnCaptureStarted?.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (currentState == CaptureState.Captured) return;
            if (!other.CompareTag("Player")) return;

            _playerInZone = false;

            if (currentState == CaptureState.Capturing)
            {
                Debug.Log($"[CaptureZone] {gameObject.name} capture interrupted");
                OnCaptureInterrupted?.Invoke();

                if (settings != null && settings.ResetOnExit)
                {
                    captureProgress = 0f;
                    currentState = CaptureState.Idle;
                    OnCaptureProgress?.Invoke(captureProgress);
                }
            }
        }

        /// <summary>
        /// Resets the capture zone to its initial state.
        /// </summary>
        public void ResetZone()
        {
            currentState = CaptureState.Idle;
            captureProgress = 0f;
            _playerInZone = false;
            OnCaptureProgress?.Invoke(captureProgress);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            float radius = settings != null ? settings.Radius : 5f;
            Color color = GetGizmoColor();
            color.a = 0.25f;

            Gizmos.color = color;
            Gizmos.DrawSphere(transform.position, radius);

            color.a = 1f;
            Gizmos.color = color;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        private Color GetGizmoColor()
        {
            if (settings == null) return Color.yellow;

            return currentState switch
            {
                CaptureState.Idle => settings.IdleColor,
                CaptureState.Capturing => settings.CapturingColor,
                CaptureState.Captured => settings.CapturedColor,
                _ => Color.yellow
            };
        }
#endif
    }
}
