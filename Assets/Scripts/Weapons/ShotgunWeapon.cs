using UnityEngine;

namespace LooterShooter.Weapons
{
    public class ShotgunWeapon : Weapon
    {
        protected override void Fire(Transform cameraTransform)
        {
            float damagePerPellet = weaponData.damage / weaponData.pelletsPerShot;

            for (int i = 0; i < weaponData.pelletsPerShot; i++)
            {
                Vector3 direction = ApplyPelletSpread(cameraTransform.forward);
                Ray ray = new Ray(cameraTransform.position, direction);

                if (Physics.Raycast(ray, out RaycastHit hit, weaponData.range))
                {
                    DealDamage(hit, 1f / weaponData.pelletsPerShot);
                    Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.yellow, 0.1f);
                }
                else
                {
                    Debug.DrawRay(ray.origin, ray.direction * weaponData.range, Color.red, 0.1f);
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
