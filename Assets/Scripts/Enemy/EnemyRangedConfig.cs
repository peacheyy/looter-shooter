using UnityEngine;

namespace LooterShooter.Enemy
{
    public class EnemyRangedConfig : MonoBehaviour
    {
        [Header("Muzzle")]
        [SerializeField] private Transform muzzleTransform;
        [SerializeField] private Vector3 muzzleOffset = new Vector3(0f, 1.5f, 0.5f);

        [Header("Projectile")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private float projectileSpeed = 30f;
        [SerializeField] private float projectileDamage = 10f;
        [SerializeField] private float projectileRange = 50f;

        [Header("Aim")]
        [SerializeField] private float aimHeightOffset = 1.2f;

        public Transform MuzzleTransform => muzzleTransform;
        public Vector3 MuzzleOffset => muzzleOffset;
        public GameObject ProjectilePrefab => projectilePrefab;
        public float ProjectileSpeed => projectileSpeed;
        public float ProjectileDamage => projectileDamage;
        public float ProjectileRange => projectileRange;
        public float AimHeightOffset => aimHeightOffset;

        public Vector3 GetMuzzlePosition()
        {
            if (muzzleTransform != null)
            {
                return muzzleTransform.position;
            }
            return transform.position + transform.TransformDirection(muzzleOffset);
        }

        public Vector3 GetAimTarget(Transform target)
        {
            return target.position + Vector3.up * aimHeightOffset;
        }
    }
}
