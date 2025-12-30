using UnityEngine;

namespace LooterShooter.Enemy
{
    public class FlyingLocomotion : MonoBehaviour, ILocomotion
    {
        [SerializeField] private float speed = 6f;
        [SerializeField] private float stoppingDistance = 1f;
        [SerializeField] private float rotationSpeed = 5f;

        private Vector3? _destination;
        private Transform _target;

        public bool HasArrived { get; private set; }
        public bool IsMoving => _destination.HasValue || _target != null;

        public float Speed
        {
            get => speed;
            set => speed = value;
        }

        public float StoppingDistance
        {
            get => stoppingDistance;
            set => stoppingDistance = value;
        }

        public void SetDestination(Vector3 destination)
        {
            _destination = destination;
            _target = null;
            HasArrived = false;
        }

        public void SetTarget(Transform target)
        {
            _target = target;
            _destination = null;
            HasArrived = false;
        }

        public void Stop()
        {
            _destination = null;
            _target = null;
            HasArrived = true;
        }

        private void Update()
        {
            if (HasArrived) return;

            Vector3 targetPos;

            if (_target != null)
            {
                targetPos = _target.position;
            }
            else if (_destination.HasValue)
            {
                targetPos = _destination.Value;
            }
            else
            {
                return;
            }

            Vector3 direction = targetPos - transform.position;
            float distance = direction.magnitude;

            if (distance <= stoppingDistance)
            {
                HasArrived = true;
                _destination = null;
                return;
            }

            Vector3 normalizedDir = direction.normalized;
            transform.position += normalizedDir * speed * Time.deltaTime;

            Quaternion targetRotation = Quaternion.LookRotation(normalizedDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
