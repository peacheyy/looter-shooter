using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;
using LooterShooter;
using LooterShooter.Enemy;
using LooterShooter.Weapons;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Fire Projectile", story: "[Agent] fires projectile at player", category: "Action/Combat", id: "a1b2c3d4e5f6a7b8c9d0e1f2a3b4c5d6")]
public partial class FireProjectileAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    protected override Status OnStart()
    {
        if (Agent.Value == null) return Status.Failure;

        var playerRef = PlayerReference.Instance;
        if (playerRef == null || playerRef.Transform == null) return Status.Failure;

        var config = Agent.Value.GetComponent<EnemyRangedConfig>();
        if (config == null)
        {
            Debug.LogError($"[FireProjectileAction] No EnemyRangedConfig on {Agent.Value.name}");
            return Status.Failure;
        }

        GameObject prefab = config.ProjectilePrefab;
        if (prefab == null)
        {
            Debug.LogError($"[FireProjectileAction] No projectile prefab configured for {Agent.Value.name}");
            return Status.Failure;
        }

        Vector3 muzzlePos = config.GetMuzzlePosition();
        Vector3 aimTarget = config.GetAimTarget(playerRef.Transform);
        Vector3 direction = (aimTarget - muzzlePos).normalized;

        Projectile projectile = null;

        if (ProjectilePool.Instance != null)
        {
            projectile = ProjectilePool.Instance.Get(prefab);
            if (projectile != null)
            {
                projectile.transform.position = muzzlePos;
                projectile.transform.rotation = Quaternion.LookRotation(direction);
            }
        }

        if (projectile == null)
        {
            var projectileObj = UnityEngine.Object.Instantiate(
                prefab,
                muzzlePos,
                Quaternion.LookRotation(direction)
            );
            projectile = projectileObj.GetComponent<Projectile>();
        }

        if (projectile != null)
        {
            projectile.Initialize(
                direction,
                config.ProjectileSpeed,
                config.ProjectileDamage,
                config.ProjectileRange
            );
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
