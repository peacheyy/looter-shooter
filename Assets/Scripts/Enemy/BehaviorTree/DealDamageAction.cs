using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;
using Action = Unity.Behavior.Action;
using LooterShooter;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Deal Damage", story: "[Agent] deals [Damage] damage to player", category: "Action/Combat", id: "e5f6g7h8i9j0k1l2m3n4o5p6q7r8s9t0")]
public partial class DealDamageAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<float> Damage;

    protected override Status OnStart()
    {
        if (Agent.Value == null) return Status.Failure;

        var playerRef = PlayerReference.Instance;
        if (playerRef == null || playerRef.Transform == null)
            return Status.Failure;

        var damageable = playerRef.Transform.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(Damage.Value);
            Debug.Log($"{Agent.Value.name} dealt {Damage.Value} damage to player!");
        }

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
