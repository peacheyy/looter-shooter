using UnityEngine;
using LooterShooter.Item;

namespace LooterShooter.Enemy
{
    public class Enemy : MonoBehaviour, IDamageable
    {
        [Header("Health")]
        [SerializeField] private float maxHealth = 50f;

        [Header("Behavior Tree Settings")]
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float chaseRange = 15f;
        [SerializeField] private float attackRange = 2f;
        [SerializeField] private float attackDamage = 10f;
        [SerializeField] private float attackCooldown = 1.5f;
        [SerializeField] private float patrolRadius = 5f;
        [SerializeField] private float patrolWaitTime = 2f;

        private float _currentHealth;
        private Renderer _renderer;
        private Color _originalColor;

        public Vector3 SpawnPosition { get; private set; }

        // Properties for behavior tree blackboard
        public float MoveSpeed => moveSpeed;
        public float ChaseRange => chaseRange;
        public float AttackRange => attackRange;
        public float AttackDamage => attackDamage;
        public float AttackCooldown => attackCooldown;
        public float PatrolRadius => patrolRadius;
        public float PatrolWaitTime => patrolWaitTime;

        private void Awake()
        {
            _currentHealth = maxHealth;
            SpawnPosition = transform.position;

            _renderer = GetComponent<Renderer>();
            if (_renderer != null)
            {
                _originalColor = _renderer.material.color;
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
            Item.Item.SpawnAt(transform.position);
            Destroy(gameObject);
        }
    }
}
