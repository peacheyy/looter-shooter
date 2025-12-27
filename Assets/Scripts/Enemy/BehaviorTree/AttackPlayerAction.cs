using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;
using Action = Unity.Behavior.Action;
using LooterShooter;
using LooterShooter.Enemy;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Attack Player", story: "[Agent] attacks player for [Damage] every [AttackCooldown] seconds", category: "Action/Enemy", id: "c3d4e5f6g7h8i9j0k1l2m3n4o5p6q7r8")]
public partial class AttackPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<float> Damage;
    [SerializeReference] public BlackboardVariable<float> AttackCooldown;

    private Transform _agentTransform;
    private float _lastAttackTime;

    protected override Status OnStart()
    {
        if (Agent.Value == null) return Status.Failure;

        _agentTransform = Agent.Value.transform;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        var playerRef = PlayerReference.Instance;
        if (playerRef == null || playerRef.Transform == null)
            return Status.Failure;

        // Face the player
        Vector3 direction = playerRef.Transform.position - _agentTransform.position;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            _agentTransform.rotation = Quaternion.LookRotation(direction);
        }

        // Check cooldown
        if (Time.time - _lastAttackTime < AttackCooldown.Value)
        {
            return Status.Running;
        }

        // Perform attack
        var damageable = playerRef.Transform.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(Damage.Value);
            Debug.Log($"{Agent.Value.name} attacked player for {Damage.Value} damage!");
        }

        _lastAttackTime = Time.time;
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}
