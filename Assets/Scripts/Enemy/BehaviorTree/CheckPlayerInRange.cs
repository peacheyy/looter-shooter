using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;
using LooterShooter;
using LooterShooter.Enemy;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Check Player In Range", story: "[Agent] is within [Range] of player", category: "Conditions", id: "a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6")]
public partial class CheckPlayerInRange : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<float> Range;

    public override bool IsTrue()
    {
        if (Agent.Value == null) return false;

        var playerRef = PlayerReference.Instance;
        if (playerRef == null || playerRef.Transform == null) return false;

        float distance = Vector3.Distance(
            Agent.Value.transform.position,
            playerRef.Transform.position
        );

        return distance <= Range.Value;
    }
}
