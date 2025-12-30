using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LooterShooter.Objectives
{
    /// <summary>
    /// UI component for displaying capture zone progress.
    /// Can be used as world-space UI above the zone or screen-space HUD element.
    /// </summary>
    public class CaptureZoneUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CaptureZone captureZone;

        [Header("UI Elements")]
        [SerializeField] private GameObject progressContainer;
        [SerializeField] private Image progressFill;
        [SerializeField] private TextMeshProUGUI statusText;

        [Header("Settings")]
        [SerializeField] private bool hideWhenIdle = true;
        [SerializeField] private bool hideWhenCaptured = false;

        private void Start()
        {
            if (captureZone == null)
            {
                captureZone = GetComponentInParent<CaptureZone>();
            }

            if (captureZone != null)
            {
                captureZone.OnCaptureStarted += OnCaptureStarted;
                captureZone.OnCaptureProgress += OnCaptureProgress;
                captureZone.OnCaptureInterrupted += OnCaptureInterrupted;
                captureZone.OnCaptureCompleted += OnCaptureCompleted;

                UpdateVisibility();
                UpdateProgress(captureZone.CaptureProgress);
            }
        }

        private void OnDestroy()
        {
            if (captureZone != null)
            {
                captureZone.OnCaptureStarted -= OnCaptureStarted;
                captureZone.OnCaptureProgress -= OnCaptureProgress;
                captureZone.OnCaptureInterrupted -= OnCaptureInterrupted;
                captureZone.OnCaptureCompleted -= OnCaptureCompleted;
            }
        }

        private void OnCaptureStarted()
        {
            SetContainerActive(true);
            UpdateStatusText("Capturing...");
        }

        private void OnCaptureProgress(float progress)
        {
            UpdateProgress(progress);
        }

        private void OnCaptureInterrupted()
        {
            UpdateStatusText("Interrupted");
        }

        private void OnCaptureCompleted()
        {
            UpdateStatusText("Captured!");

            if (hideWhenCaptured)
            {
                SetContainerActive(false);
            }
        }

        private void UpdateProgress(float progress)
        {
            if (progressFill != null)
            {
                progressFill.fillAmount = progress;
            }
        }

        private void UpdateStatusText(string text)
        {
            if (statusText != null)
            {
                statusText.text = text;
            }
        }

        private void UpdateVisibility()
        {
            if (captureZone == null) return;

            bool shouldShow = captureZone.CurrentState switch
            {
                CaptureState.Idle => !hideWhenIdle,
                CaptureState.Capturing => true,
                CaptureState.Captured => !hideWhenCaptured,
                _ => true
            };

            SetContainerActive(shouldShow);
        }

        private void SetContainerActive(bool active)
        {
            if (progressContainer != null)
            {
                progressContainer.SetActive(active);
            }
        }
    }
}
