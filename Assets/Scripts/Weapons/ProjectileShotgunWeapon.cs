using UnityEngine;

namespace LooterShooter.Weapons
{
    public class ProjectileShotgunWeapon : Weapon
    {
        protected override void Fire(Transform cameraTransform)
        {
            if (weaponData.projectilePrefab == null)
            {
                Debug.LogWarning($"[{weaponData.weaponName}] No projectile prefab assigned!");
                return;
            }

            Vector3 spawnPosition = firePoint != null ? firePoint.position : cameraTransform.position;

            for (int i = 0; i < weaponData.pelletsPerShot; i++)
            {
                Vector3 direction = ApplyPelletSpread(cameraTransform.forward);

                GameObject projectile = Instantiate(
                    weaponData.projectilePrefab,
                    spawnPosition,
                    Quaternion.LookRotation(direction)
                );

                float damagePerPellet = weaponData.damage / weaponData.pelletsPerShot;

                if (projectile.TryGetComponent<Projectile>(out var proj))
                {
                    proj.Initialize(direction, weaponData.projectileSpeed, damagePerPellet, weaponData.range);
                }
                else if (projectile.TryGetComponent<Rigidbody>(out var rb))
                {
                    rb.linearVelocity = direction * weaponData.projectileSpeed;
                    Destroy(projectile, weaponData.range / weaponData.projectileSpeed);
                }
            }
        }

        private Vector3 ApplyPelletSpread(Vector3 direction)
        {
            float totalSpread = weaponData.pelletSpread + currentSpread;

            Vector2 randomCircle = Random.insideUnitCircle * Mathf.Tan(totalSpread * Mathf.Deg2Rad);
            Vector3 spreadOffset = new Vector3(randomCircle.x, randomCircle.y, 0f);

            Quaternion rotation = Quaternion.LookRotation(direction);
            Vector3 spreadDirection = rotation * spreadOffset;

            return (direction + spreadDirection).normalized;
        }
    }
}
