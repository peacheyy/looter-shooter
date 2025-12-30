using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;
using LooterShooter;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move To Position", story: "[Agent] moves toward [TargetPosition]", category: "Action/Movement", id: "a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6")]
public partial class MoveToPositionAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Vector3> TargetPosition;

    private ILocomotion _locomotion;

    protected override Status OnStart()
    {
        if (Agent.Value == null) return Status.Failure;

        _locomotion = Agent.Value.GetComponent<ILocomotion>();
        if (_locomotion == null) return Status.Failure;

        _locomotion.SetDestination(TargetPosition.Value);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_locomotion == null) return Status.Failure;

        if (_locomotion.HasArrived)
        {
            return Status.Success;
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
        _locomotion?.Stop();
    }
}
