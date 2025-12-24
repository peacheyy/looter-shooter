using UnityEngine;
using LooterShooter.Player;

namespace LooterShooter.Enemy
{
    public class Enemy : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth = 50f;
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float stoppingDistance = 1.5f;

        private float _currentHealth;
        private Renderer _renderer;
        private Color _originalColor;
        private Transform _player;

        private void Awake()
        {
            _currentHealth = maxHealth;
            _renderer = GetComponent<Renderer>();
            if (_renderer != null)
            {
                _originalColor = _renderer.material.color;
            }
        }

        private void Start()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                _player = playerObj.transform;
            }
        }

        private void Update()
        {
            if (_player == null) return;

            Vector3 direction = _player.position - transform.position;
            direction.y = 0;
            float distance = direction.magnitude;

            if (distance > stoppingDistance)
            {
                transform.position += direction.normalized * moveSpeed * Time.deltaTime;
                transform.rotation = Quaternion.LookRotation(direction);
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
            Destroy(gameObject);
        }
    }
}
