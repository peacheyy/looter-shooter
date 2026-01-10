using UnityEngine;

namespace LooterShooter.Weapons
{
    public class Projectile : MonoBehaviour
    {
        private Vector3 _direction;
        private float _speed;
        private float _damage;
        private float _maxDistance;
        private float _distanceTraveled;
        private Vector3 _lastPosition;
        private bool _isPooled;

        public void Initialize(Vector3 direction, float speed, float damage, float maxDistance)
        {
            _direction = direction.normalized;
            _speed = speed;
            _damage = damage;
            _maxDistance = maxDistance;
            _distanceTraveled = 0f;
            _lastPosition = transform.position;
            _isPooled = ProjectilePool.Instance != null;
        }

        private void Update()
        {
            float moveDistance = _speed * Time.deltaTime;
            Vector3 movement = _direction * moveDistance;

            if (Physics.Raycast(transform.position, _direction, out RaycastHit hit, moveDistance))
            {
                OnHit(hit);
                return;
            }

            transform.position += movement;
            _distanceTraveled += moveDistance;

            if (_distanceTraveled >= _maxDistance)
            {
                ReturnToPool();
            }

            _lastPosition = transform.position;
        }

        private void OnHit(RaycastHit hit)
        {
            var damageable = hit.collider.GetComponent<IDamageable>();
            damageable?.TakeDamage(_damage);

            Debug.Log($"Projectile hit: {hit.collider.name}");
            ReturnToPool();
        }

        private void ReturnToPool()
        {
            if (_isPooled && ProjectilePool.Instance != null)
            {
                ProjectilePool.Instance.Release(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDisable()
        {
            _distanceTraveled = 0f;
            _direction = Vector3.zero;
            _speed = 0f;
            _damage = 0f;
            _maxDistance = 0f;
        }
    }
}
