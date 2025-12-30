using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LooterShooter.Player;

namespace LooterShooter.UI
{
    public class HealthUI : MonoBehaviour
    {
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private Image healthBarFill;
        [SerializeField] private GameObject deathOverlay;
        [SerializeField] private TextMeshProUGUI respawnCountdownText;

        private Coroutine _countdownCoroutine;

        private void Start()
        {
            if (playerHealth == null)
            {
                var playerRef = PlayerReference.Instance;
                if (playerRef != null)
                {
                    playerHealth = playerRef.GetComponent<PlayerHealth>();
                }
            }

            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged += UpdateHealthDisplay;
                playerHealth.OnDeath += OnPlayerDeath;
                playerHealth.OnRespawnStarted += OnRespawnStarted;
                playerHealth.OnRespawn += OnPlayerRespawn;
                UpdateHealthDisplay(playerHealth.CurrentHealth, playerHealth.MaxHealth);
            }

            // Ensure death overlay is hidden at start
            if (deathOverlay != null)
            {
                deathOverlay.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged -= UpdateHealthDisplay;
                playerHealth.OnDeath -= OnPlayerDeath;
                playerHealth.OnRespawnStarted -= OnRespawnStarted;
                playerHealth.OnRespawn -= OnPlayerRespawn;
            }
        }

        private void UpdateHealthDisplay(float currentHealth, float maxHealth)
        {
            if (healthText != null)
            {
                healthText.text = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)}";
            }

            if (healthBarFill != null)
            {
                healthBarFill.fillAmount = currentHealth / maxHealth;
            }
        }

        private void OnPlayerDeath()
        {
            if (healthText != null)
            {
                healthText.text = "DEAD";
            }

            if (deathOverlay != null)
            {
                deathOverlay.SetActive(true);
            }
        }

        private void OnRespawnStarted(float duration)
        {
            if (_countdownCoroutine != null)
            {
                StopCoroutine(_countdownCoroutine);
            }
            _countdownCoroutine = StartCoroutine(RespawnCountdownCoroutine(duration));
        }

        private IEnumerator RespawnCountdownCoroutine(float duration)
        {
            float remaining = duration;

            while (remaining > 0)
            {
                if (respawnCountdownText != null)
                {
                    respawnCountdownText.text = $"Respawning in {remaining:F1}s";
                }
                yield return null;
                remaining -= Time.deltaTime;
            }

            if (respawnCountdownText != null)
            {
                respawnCountdownText.text = "";
            }
        }

        private void OnPlayerRespawn()
        {
            if (_countdownCoroutine != null)
            {
                StopCoroutine(_countdownCoroutine);
                _countdownCoroutine = null;
            }

            if (deathOverlay != null)
            {
                deathOverlay.SetActive(false);
            }

            if (respawnCountdownText != null)
            {
                respawnCountdownText.text = "";
            }
        }
    }
}
