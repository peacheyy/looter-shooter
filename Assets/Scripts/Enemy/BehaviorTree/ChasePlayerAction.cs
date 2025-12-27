using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;
using Action = Unity.Behavior.Action;
using LooterShooter;
using LooterShooter.Enemy;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Chase Player", story: "[Agent] chases player at [Speed] stopping at [StoppingDistance] giving up at [ChaseRange]", category: "Action/Enemy", id: "b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6q7")]
public partial class ChasePlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<float> Speed;
    [SerializeReference] public BlackboardVariable<float> StoppingDistance;
    [SerializeReference] public BlackboardVariable<float> ChaseRange;

    private Transform _agentTransform;
    private Transform _playerTransform;

    protected override Status OnStart()
    {
        if (Agent.Value == null) return Status.Failure;

        _agentTransform = Agent.Value.transform;

        var playerRef = PlayerReference.Instance;
        if (playerRef == null || playerRef.Transform == null) return Status.Failure;

        _playerTransform = playerRef.Transform;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_agentTransform == null || _playerTransform == null)
            return Status.Failure;

        Vector3 direction = _playerTransform.position - _agentTransform.position;
        direction.y = 0;
        float distance = direction.magnitude;

        if (distance <= StoppingDistance.Value)
        {
            return Status.Success;
        }

        // Give up if player escapes beyond chase range
        if (distance > ChaseRange.Value)
        {
            return Status.Success;
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
}
