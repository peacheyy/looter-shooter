using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move To Position", story: "[Agent] moves toward [TargetPosition] at [Speed]", category: "Action/Movement", id: "a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6")]
public partial class MoveToPositionAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Vector3> TargetPosition;
    [SerializeReference] public BlackboardVariable<float> Speed;
    [SerializeReference] public BlackboardVariable<float> StoppingDistance;

    private Transform _agentTransform;

    protected override Status OnStart()
    {
        if (Agent.Value == null) return Status.Failure;

        _agentTransform = Agent.Value.transform;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_agentTransform == null) return Status.Failure;

        Vector3 direction = TargetPosition.Value - _agentTransform.position;
        direction.y = 0;
        float distance = direction.magnitude;

        if (distance <= StoppingDistance.Value)
        {
            return Status.Success;
        }

        _agentTransform.position += direction.normalized * Speed.Value * Time.deltaTime;
        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}
