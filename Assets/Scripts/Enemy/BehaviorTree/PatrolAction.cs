using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;
using Action = Unity.Behavior.Action;
using LooterShooter.Enemy;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Patrol", story: "[Agent] patrols within [PatrolRadius] at [Speed]", category: "Action/Enemy", id: "d4e5f6g7h8i9j0k1l2m3n4o5p6q7r8s9")]
public partial class PatrolAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<float> PatrolRadius;
    [SerializeReference] public BlackboardVariable<float> Speed;
    [SerializeReference] public BlackboardVariable<float> WaitTime;

    private Transform _agentTransform;
    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private float _waitTimer;
    private bool _isWaiting;

    protected override Status OnStart()
    {
        if (Agent.Value == null) return Status.Failure;

        _agentTransform = Agent.Value.transform;

        var enemy = Agent.Value.GetComponent<Enemy>();
        _startPosition = enemy != null ? enemy.SpawnPosition : _agentTransform.position;

        PickNewPatrolPoint();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_agentTransform == null) return Status.Failure;

        if (_isWaiting)
        {
            _waitTimer -= Time.deltaTime;
            if (_waitTimer <= 0)
            {
                // Return Success to let the tree re-evaluate higher priority branches
                return Status.Success;
            }
            return Status.Running;
        }

        Vector3 direction = _targetPosition - _agentTransform.position;
        direction.y = 0;
        float distance = direction.magnitude;

        if (distance <= 0.5f)
        {
            _isWaiting = true;
            _waitTimer = WaitTime.Value;
            return Status.Running;
        }

        _agentTransform.position += direction.normalized * Speed.Value * Time.deltaTime;
        if (direction != Vector3.zero)
        {
            _agentTransform.rotation = Quaternion.LookRotation(direction);
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
    }

    private void PickNewPatrolPoint()
    {
        Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * PatrolRadius.Value;
        _targetPosition = _startPosition + new Vector3(randomCircle.x, 0, randomCircle.y);
    }
}
