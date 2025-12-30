using UnityEngine;

namespace LooterShooter.Enemy
{
    public class GroundLocomotion : MonoBehaviour, ILocomotion
    {
        [SerializeField] private float speed = 4f;
        [SerializeField] private float stoppingDistance = 0.5f;

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
            direction.y = 0;
            float distance = direction.magnitude;

            if (distance <= stoppingDistance)
            {
                HasArrived = true;
                _destination = null;
                return;
            }

            Vector3 normalizedDir = direction.normalized;
            transform.position += normalizedDir * speed * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(normalizedDir);
        }
    }
}
