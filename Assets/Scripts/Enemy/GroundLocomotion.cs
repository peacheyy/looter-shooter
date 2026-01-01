using UnityEngine;
using UnityEngine.AI;

namespace LooterShooter.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class GroundLocomotion : MonoBehaviour, ILocomotion
    {
        [SerializeField] private float speed = 4f;
        [SerializeField] private float stoppingDistance = 0.5f;

        private NavMeshAgent _agent;
        private Transform _target;

        public bool HasArrived => !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance;
        public bool IsMoving => _agent.hasPath && _agent.remainingDistance > _agent.stoppingDistance;

        public float Speed
        {
            get => speed;
            set
            {
                speed = value;
                if (_agent != null) _agent.speed = value;
            }
        }

        public float StoppingDistance
        {
            get => stoppingDistance;
            set
            {
                stoppingDistance = value;
                if (_agent != null) _agent.stoppingDistance = value;
            }
        }

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = speed;
            _agent.stoppingDistance = stoppingDistance;
        }

        public void SetDestination(Vector3 destination)
        {
            _target = null;
            _agent.SetDestination(destination);
        }

        public void SetTarget(Transform target)
        {
            _target = target;
            if (_target != null)
            {
                _agent.SetDestination(_target.position);
            }
        }

        public void Stop()
        {
            _target = null;
            _agent.ResetPath();
        }

        private void Update()
        {
            if (_target != null)
            {
                _agent.SetDestination(_target.position);
            }
        }
    }
}
