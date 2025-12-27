using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;
using Action = Unity.Behavior.Action;
using LooterShooter.Enemy;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Pick Random Patrol Point", story: "[Agent] picks random point within [PatrolRadius] storing in [TargetPosition]", category: "Action/Patrol", id: "c3d4e5f6g7h8i9j0k1l2m3n4o5p6q7r8")]
public partial class PickRandomPatrolPointAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<float> PatrolRadius;
    [SerializeReference] public BlackboardVariable<Vector3> TargetPosition;

    protected override Status OnStart()
    {
        if (Agent.Value == null) return Status.Failure;

        var enemy = Agent.Value.GetComponent<Enemy>();
        Vector3 centerPosition = enemy != null ? enemy.SpawnPosition : Agent.Value.transform.position;

        Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * PatrolRadius.Value;
        TargetPosition.Value = centerPosition + new Vector3(randomCircle.x, 0, randomCircle.y);

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
