using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;
using Action = Unity.Behavior.Action;
using LooterShooter;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Set Target To Player", story: "Set [TargetPosition] to player position", category: "Action/Targeting", id: "f6g7h8i9j0k1l2m3n4o5p6q7r8s9t0u1")]
public partial class SetTargetToPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<Vector3> TargetPosition;

    protected override Status OnStart()
    {
        var playerRef = PlayerReference.Instance;
        if (playerRef == null || playerRef.Transform == null)
            return Status.Failure;

        TargetPosition.Value = playerRef.Transform.position;
        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}
