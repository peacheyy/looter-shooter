using UnityEngine;

namespace LooterShooter.Weapons
{
    public class HitscanWeapon : Weapon
    {
        protected override void Fire(Transform cameraTransform)
        {
            Vector3 direction = ApplySpread(cameraTransform.forward);
            Ray ray = new Ray(cameraTransform.position, direction);

            if (Physics.Raycast(ray, out RaycastHit hit, weaponData.range))
            {
                DealDamage(hit);
                Debug.Log($"[{weaponData.weaponName}] Hit: {hit.collider.name}");
            }

            Debug.DrawRay(ray.origin, ray.direction * weaponData.range, Color.red, 0.1f);
        }
    }
}
