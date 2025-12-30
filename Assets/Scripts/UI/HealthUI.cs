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
                UpdateHealthDisplay(playerHealth.CurrentHealth, playerHealth.MaxHealth);
            }
        }

        private void OnDestroy()
        {
            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged -= UpdateHealthDisplay;
                playerHealth.OnDeath -= OnPlayerDeath;
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
        }
    }
}
