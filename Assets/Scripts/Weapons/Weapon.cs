using System;
using UnityEngine;
using LooterShooter.Player;

namespace LooterShooter.Weapons
{
    public abstract class Weapon : MonoBehaviour
    {
        [SerializeField] protected WeaponData weaponData;
        [SerializeField] protected Transform firePoint;

        protected int currentAmmo;
        protected float currentSpread;
        protected float nextFireTime;
        protected bool isReloading;

        public WeaponData Data => weaponData;
        public int CurrentAmmo => currentAmmo;
        public bool IsReloading => isReloading;
        public bool CanFire => !isReloading && currentAmmo > 0 && Time.time >= nextFireTime;

        public event Action OnFire;
        public event Action OnReload;
        public event Action OnAmmoChanged;

        protected virtual void Awake()
        {
            if (weaponData != null)
            {
                currentAmmo = weaponData.magazineSize;
            }
        }

        protected virtual void Update()
        {
            RecoverSpread();
        }

        public virtual void Initialize(WeaponData data)
        {
            weaponData = data;
            currentAmmo = data.magazineSize;
            currentSpread = data.baseSpread;
        }

        public bool TryFire(Transform cameraTransform)
        {
            if (!CanFire) return false;

            Fire(cameraTransform);
            currentAmmo--;
            nextFireTime = Time.time + 1f / weaponData.fireRate;
            currentSpread = Mathf.Min(currentSpread + weaponData.spreadIncreasePerShot, weaponData.maxSpread);

            OnFire?.Invoke();
            OnAmmoChanged?.Invoke();

            PlayFireEffects();

            return true;
        }

        protected abstract void Fire(Transform cameraTransform);

        public virtual void StartReload()
        {
            if (isReloading || currentAmmo >= weaponData.magazineSize)
                return;

            isReloading = true;
            OnReload?.Invoke();
            StartCoroutine(ReloadCoroutine());
        }

        protected virtual System.Collections.IEnumerator ReloadCoroutine()
        {
            if (weaponData.reloadSound != null)
            {
                AudioSource.PlayClipAtPoint(weaponData.reloadSound, transform.position);
            }

            yield return new WaitForSeconds(weaponData.reloadTime);

            currentAmmo = weaponData.magazineSize;
            isReloading = false;
            OnAmmoChanged?.Invoke();
        }

        protected virtual void RecoverSpread()
        {
            if (weaponData == null) return;

            currentSpread = Mathf.Max(
                currentSpread - weaponData.spreadRecoveryRate * Time.deltaTime,
                weaponData.baseSpread
            );
        }

        protected Vector3 ApplySpread(Vector3 direction)
        {
            if (currentSpread <= 0f) return direction;

            float spreadAngle = currentSpread;
            Vector3 spreadOffset = UnityEngine.Random.insideUnitSphere * Mathf.Tan(spreadAngle * Mathf.Deg2Rad);
            spreadOffset.z = 0;

            Quaternion rotation = Quaternion.LookRotation(direction);
            Vector3 spreadDirection = rotation * spreadOffset;

            return (direction + spreadDirection).normalized;
        }

        protected virtual void PlayFireEffects()
        {
            if (weaponData.fireSound != null)
            {
                AudioSource.PlayClipAtPoint(weaponData.fireSound, transform.position);
            }

            if (weaponData.muzzleFlashPrefab != null && firePoint != null)
            {
                GameObject flash = Instantiate(weaponData.muzzleFlashPrefab, firePoint.position, firePoint.rotation);
                Destroy(flash, 0.1f);
            }
        }

        protected void DealDamage(RaycastHit hit, float damageMultiplier = 1f)
        {
            var damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                float finalDamage = weaponData.damage * damageMultiplier;

                if (hit.collider.CompareTag("Head"))
                {
                    finalDamage *= weaponData.headshotMultiplier;
                }

                damageable.TakeDamage(finalDamage);
            }
        }

        public virtual void OnEquip()
        {
            gameObject.SetActive(true);
        }

        public virtual void OnUnequip()
        {
            StopAllCoroutines();
            isReloading = false;
            gameObject.SetActive(false);
        }
    }
}
