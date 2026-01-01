using UnityEngine;
using LooterShooter.Item;

namespace LooterShooter.Enemy
{
    public class Enemy : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth = 50f;
        [SerializeField] private LootTable lootTable;

        private float _currentHealth;
        private Renderer _renderer;
        private Color _originalColor;

        public Vector3 SpawnPosition { get; private set; }

        private void Awake()
        {
            _currentHealth = maxHealth;
            SpawnPosition = transform.position;

            _renderer = GetComponent<Renderer>();
            if (_renderer != null)
            {
                _originalColor = _renderer.material.color;
            }

            if (GetComponent<ILocomotion>() == null)
            {
                Debug.LogWarning($"{gameObject.name}: No ILocomotion component found. Add GroundLocomotion or FlyingLocomotion.");
            }
        }

        public void TakeDamage(float amount)
        {
            _currentHealth -= amount;
            Debug.Log($"{gameObject.name} took {amount} damage. Health: {_currentHealth}/{maxHealth}");

            StartCoroutine(FlashRed());

            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        private System.Collections.IEnumerator FlashRed()
        {
            if (_renderer != null)
            {
                _renderer.material.color = Color.red;
                yield return new WaitForSeconds(0.1f);
                _renderer.material.color = _originalColor;
            }
        }

        private void Die()
        {
            Debug.Log($"{gameObject.name} died!");

            if (lootTable != null)
                lootTable.DropLoot(transform.position);
            else
                Item.Item.SpawnAt(transform.position);

            Destroy(gameObject);
        }
    }
}
