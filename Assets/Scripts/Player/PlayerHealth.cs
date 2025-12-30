using System;
using System.Collections;
using UnityEngine;

namespace LooterShooter.Player
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float respawnDelay = 3f;
        [SerializeField] private Transform spawnPoint;

        private float _currentHealth;
        private bool _isDead;
        private PlayerMovement _playerMovement;
        private CharacterController _characterController;
        private Vector3 _initialSpawnPosition;
        private Quaternion _initialSpawnRotation;

        public float CurrentHealth => _currentHealth;
        public float MaxHealth => maxHealth;
        public bool IsDead => _isDead;
        public float RespawnDelay => respawnDelay;

        public event Action<float, float> OnHealthChanged;
        public event Action OnDeath;
        public event Action<float> OnRespawnStarted; // passes remaining time
        public event Action OnRespawn;

        private void Awake()
        {
            _currentHealth = maxHealth;
            _playerMovement = GetComponent<PlayerMovement>();
            _characterController = GetComponent<CharacterController>();

            // Store initial position as fallback spawn point
            _initialSpawnPosition = transform.position;
            _initialSpawnRotation = transform.rotation;
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
            _isDead = true;
            Debug.Log("Player died!");

            // Disable player controls
            if (_playerMovement != null)
            {
                _playerMovement.SetControlsEnabled(false);
            }

            OnDeath?.Invoke();
            StartCoroutine(RespawnCoroutine());
        }

        private IEnumerator RespawnCoroutine()
        {
            OnRespawnStarted?.Invoke(respawnDelay);

            yield return new WaitForSeconds(respawnDelay);

            Respawn();
        }

        private void Respawn()
        {
            // Determine spawn position
            Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : _initialSpawnPosition;
            Quaternion spawnRot = spawnPoint != null ? spawnPoint.rotation : _initialSpawnRotation;

            // Disable CharacterController to allow position change
            if (_characterController != null)
            {
                _characterController.enabled = false;
            }

            transform.position = spawnPos;
            transform.rotation = spawnRot;

            // Re-enable CharacterController
            if (_characterController != null)
            {
                _characterController.enabled = true;
            }

            // Reset health
            _currentHealth = maxHealth;
            _isDead = false;

            // Re-enable controls
            if (_playerMovement != null)
            {
                _playerMovement.SetControlsEnabled(true);
            }

            Debug.Log("Player respawned!");
            OnHealthChanged?.Invoke(_currentHealth, maxHealth);
            OnRespawn?.Invoke();
        }

        /// <summary>
        /// Sets the spawn point for respawning. Can be called to update checkpoint.
        /// </summary>
        public void SetSpawnPoint(Transform newSpawnPoint)
        {
            spawnPoint = newSpawnPoint;
        }
    }
}
