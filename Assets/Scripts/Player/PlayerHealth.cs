using System;
using UnityEngine;

namespace LooterShooter.Player
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth = 100f;

        private float _currentHealth;

        public float CurrentHealth => _currentHealth;
        public float MaxHealth => maxHealth;

        public event Action<float, float> OnHealthChanged;
        public event Action OnDeath;

        private void Awake()
        {
            _currentHealth = maxHealth;
        }

        public void TakeDamage(float amount)
        {
            if (_currentHealth <= 0) return;

            _currentHealth -= amount;
            _currentHealth = Mathf.Max(_currentHealth, 0);

            Debug.Log($"Player took {amount} damage. Health: {_currentHealth}/{maxHealth}");

            OnHealthChanged?.Invoke(_currentHealth, maxHealth);

            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        public void Heal(float amount)
        {
            if (_currentHealth <= 0) return;

            _currentHealth += amount;
            _currentHealth = Mathf.Min(_currentHealth, maxHealth);

            OnHealthChanged?.Invoke(_currentHealth, maxHealth);
        }

        private void Die()
        {
            Debug.Log("Player died!");
            OnDeath?.Invoke();
        }
    }
}
