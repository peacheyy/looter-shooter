using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;
using LooterShooter;

[Serializable, GeneratePropertyBag]
[Condition(name: "Check Player Out Of Range", story: "[Agent] is beyond [Range] of player", category: "Conditions", id: "d4e5f6a7b8c9d0e1f2a3b4c5d6e7f8a9")]
public partial class CheckPlayerOutOfRange : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<float> Range;

    public override bool IsTrue()
    {
        if (Agent.Value == null) return true;

        var playerRef = PlayerReference.Instance;
        if (playerRef == null || playerRef.Transform == null) return true;

        float distance = Vector3.Distance(
            Agent.Value.transform.position,
            playerRef.Transform.position
        );

        return distance > Range.Value;
    }
}
