using UnityEngine;

namespace LooterShooter.Weapons
{
    public class ProjectileWeapon : Weapon
    {
        protected override void Fire(Transform cameraTransform)
        {
            if (weaponData.projectilePrefab == null)
            {
                Debug.LogWarning($"[{weaponData.weaponName}] No projectile prefab assigned!");
                return;
            }

            Vector3 spawnPosition = firePoint != null ? firePoint.position : cameraTransform.position;
            Vector3 direction = ApplySpread(cameraTransform.forward);

            GameObject projectile = Instantiate(
                weaponData.projectilePrefab,
                spawnPosition,
                Quaternion.LookRotation(direction)
            );

            if (projectile.TryGetComponent<Projectile>(out var proj))
            {
                proj.Initialize(direction, weaponData.projectileSpeed, weaponData.damage, weaponData.range);
            }
            else if (projectile.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.linearVelocity = direction * weaponData.projectileSpeed;
                Destroy(projectile, weaponData.range / weaponData.projectileSpeed);
            }
        }
    }
}
