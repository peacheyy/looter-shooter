using UnityEngine;
using TMPro;
using LooterShooter.Weapons;

namespace LooterShooter.UI
{
    public class WeaponUI : MonoBehaviour
    {
        [SerializeField] private WeaponManager weaponManager;
        [SerializeField] private TextMeshProUGUI reloadText;

        private Weapon _currentWeapon;

        private void Start()
        {
            if (weaponManager == null)
            {
                weaponManager = FindFirstObjectByType<WeaponManager>();
            }

            if (weaponManager != null)
            {
                weaponManager.OnWeaponChanged += HandleWeaponChanged;

                if (weaponManager.CurrentWeapon != null)
                {
                    HandleWeaponChanged(weaponManager.CurrentWeapon);
                }
            }

            HideText();
        }

        private void OnDestroy()
        {
            if (weaponManager != null)
            {
                weaponManager.OnWeaponChanged -= HandleWeaponChanged;
            }

            UnsubscribeFromWeapon();
        }

        private void HandleWeaponChanged(Weapon weapon)
        {
            UnsubscribeFromWeapon();

            _currentWeapon = weapon;

            if (_currentWeapon != null)
            {
                _currentWeapon.OnAmmoChanged += UpdateUI;
                _currentWeapon.OnReload += OnReloadStarted;
                UpdateUI();
            }
        }

        private void UnsubscribeFromWeapon()
        {
            if (_currentWeapon != null)
            {
                _currentWeapon.OnAmmoChanged -= UpdateUI;
                _currentWeapon.OnReload -= OnReloadStarted;
            }
        }

        private void UpdateUI()
        {
            if (_currentWeapon == null || reloadText == null) return;

            if (_currentWeapon.IsReloading)
            {
                ShowText("Reloading...");
            }
            else if (_currentWeapon.CurrentAmmo <= 0)
            {
                ShowText("Reload");
            }
            else
            {
                HideText();
            }
        }

        private void OnReloadStarted()
        {
            ShowText("Reloading...");
        }

        private void ShowText(string message)
        {
            if (reloadText != null)
            {
                reloadText.text = message;
                reloadText.gameObject.SetActive(true);
            }
        }

        private void HideText()
        {
            if (reloadText != null)
            {
                reloadText.gameObject.SetActive(false);
            }
        }
    }
}
