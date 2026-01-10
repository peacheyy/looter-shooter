using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Unity.Properties;
using LooterShooter;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Get Strafe Point", story: "[Agent] calculates strafe point at [StrafeDistance] within [StrafeAngleRange] degrees", category: "Action/Combat", id: "f7a8b9c0d1e2f3a4b5c6d7e8f9a0b1c2")]
public partial class GetStrafePointAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<float> StrafeDistance;
    [SerializeReference] public BlackboardVariable<float> StrafeAngleRange;
    [SerializeReference] public BlackboardVariable<Vector3> TargetPosition;

    private const float NavMeshSampleDistance = 2f;
    private const int MaxAttempts = 5;

    protected override Status OnStart()
    {
        if (Agent.Value == null) return Status.Failure;

        var playerRef = PlayerReference.Instance;
        if (playerRef == null || playerRef.Transform == null) return Status.Failure;

        Vector3 agentPos = Agent.Value.transform.position;
        Vector3 playerPos = playerRef.Transform.position;

        Vector3 toAgent = agentPos - playerPos;
        toAgent.y = 0;
        float currentAngle = Mathf.Atan2(toAgent.x, toAgent.z) * Mathf.Rad2Deg;

        for (int i = 0; i < MaxAttempts; i++)
        {
            float angleOffset = UnityEngine.Random.Range(-StrafeAngleRange.Value, StrafeAngleRange.Value);
            float newAngle = (currentAngle + angleOffset) * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(
                Mathf.Sin(newAngle) * StrafeDistance.Value,
                0f,
                Mathf.Cos(newAngle) * StrafeDistance.Value
            );

            Vector3 candidatePos = playerPos + offset;

            if (NavMesh.SamplePosition(candidatePos, out NavMeshHit hit, NavMeshSampleDistance, NavMesh.AllAreas))
            {
                TargetPosition.Value = hit.position;
                return Status.Success;
            }
        }

        TargetPosition.Value = agentPos;
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
