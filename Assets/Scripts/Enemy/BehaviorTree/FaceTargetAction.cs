using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Face Target", story: "[Agent] faces [TargetPosition]", category: "Action/Movement", id: "b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6q7")]
public partial class FaceTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Vector3> TargetPosition;

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

        if (direction != Vector3.zero)
        {
            _agentTransform.rotation = Quaternion.LookRotation(direction);
        }

        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}
