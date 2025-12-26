using UnityEngine;

namespace LooterShooter.Weapons
{
    public enum WeaponType
    {
        Hitscan,
        Projectile,
        Shotgun
    }

    public enum FireMode
    {
        Single,
        Automatic,
        Burst
    }

    [CreateAssetMenu(fileName = "New Weapon", menuName = "Looter Shooter/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [Header("Basic Info")]
        public string weaponName;
        public WeaponType weaponType;
        public Sprite icon;
        public GameObject weaponPrefab;

        [Header("Damage")]
        public float damage = 10f;
        public float headshotMultiplier = 2f;

        [Header("Fire Settings")]
        public FireMode fireMode = FireMode.Automatic;
        public float fireRate = 10f;
        public int burstCount = 3;
        public float burstDelay = 0.05f;

        [Header("Range")]
        public float range = 100f;

        [Header("Ammo")]
        public int magazineSize = 30;
        public float reloadTime = 1.5f;

        [Header("Spread & Accuracy")]
        public float baseSpread = 0f;
        public float maxSpread = 5f;
        public float spreadIncreasePerShot = 0.5f;
        public float spreadRecoveryRate = 5f;

        [Header("Shotgun Settings")]
        public int pelletsPerShot = 8;
        public float pelletSpread = 5f;

        [Header("Projectile Settings")]
        public GameObject projectilePrefab;
        public float projectileSpeed = 50f;

        [Header("Audio/Visual")]
        public AudioClip fireSound;
        public AudioClip reloadSound;
        public AudioClip emptySound;
        public GameObject muzzleFlashPrefab;
    }
}
