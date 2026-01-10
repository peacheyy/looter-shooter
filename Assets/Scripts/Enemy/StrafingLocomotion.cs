using UnityEngine;
using UnityEngine.AI;

namespace LooterShooter.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class StrafingLocomotion : MonoBehaviour, ILocomotion
    {
        [Header("Movement")]
        [SerializeField] private float speed = 4f;
        [SerializeField] private float stoppingDistance = 0.5f;

        [Header("Rotation")]
        [SerializeField] private float rotationSpeed = 8f;

        [Header("Avoidance")]
        [SerializeField] private ObstacleAvoidanceType avoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        [SerializeField] private float avoidanceRadius = 0.5f;
        [SerializeField] [Range(0, 99)] private int avoidancePriority = 50;

        private NavMeshAgent _agent;
        private Transform _lookTarget;
        private Vector3? _lookPosition;

        public bool HasArrived => !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance;
        public bool IsMoving => _agent.hasPath && _agent.remainingDistance > _agent.stoppingDistance;
        public bool HasLookTarget => _lookTarget != null || _lookPosition.HasValue;

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

        public float RotationSpeed
        {
            get => rotationSpeed;
            set => rotationSpeed = value;
        }

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = speed;
            _agent.stoppingDistance = stoppingDistance;
            _agent.updateRotation = false;

            _agent.obstacleAvoidanceType = avoidanceType;
            _agent.radius = avoidanceRadius;
            _agent.avoidancePriority = avoidancePriority;
        }

        private void Update()
        {
            UpdateRotation();
        }

        private void UpdateRotation()
        {
            Vector3? targetPos = null;

            if (_lookTarget != null)
            {
                targetPos = _lookTarget.position;
            }
            else if (_lookPosition.HasValue)
            {
                targetPos = _lookPosition.Value;
            }

            if (!targetPos.HasValue) return;

            Vector3 direction = targetPos.Value - transform.position;
            direction.y = 0;

            if (direction.sqrMagnitude < 0.001f) return;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        public void SetDestination(Vector3 destination)
        {
            _agent.SetDestination(destination);
        }

        public void SetTarget(Transform target)
        {
            _lookTarget = target;
            _lookPosition = null;
        }

        public void Stop()
        {
            _agent.ResetPath();
        }

        public void SetLookTarget(Transform target)
        {
            _lookTarget = target;
            _lookPosition = null;
        }

        public void SetLookPosition(Vector3 position)
        {
            _lookTarget = null;
            _lookPosition = position;
        }

        public void ClearLookTarget()
        {
            _lookTarget = null;
            _lookPosition = null;
        }

        public void StopAll()
        {
            Stop();
            ClearLookTarget();
        }
    }
}
