using UnityEngine;
using TMPro;
using LooterShooter.Weapons;
using LooterShooter.Player;

namespace LooterShooter.UI
{
    public class WeaponUI : MonoBehaviour
    {
        [SerializeField] private WeaponHandler weaponHandler;
        [SerializeField] private TextMeshProUGUI reloadText;
        [SerializeField] private TextMeshProUGUI ammoText;

        private Weapon _currentWeapon;

        private void Start()
        {
            if (weaponHandler == null)
            {
                weaponHandler = FindFirstObjectByType<WeaponHandler>();
            }

            if (weaponHandler != null)
            {
                weaponHandler.OnWeaponChanged += HandleWeaponChanged;

                if (weaponHandler.CurrentWeapon != null)
                {
                    HandleWeaponChanged(weaponHandler.CurrentWeapon);
                }
            }

            HideText();
        }

        private void OnDestroy()
        {
            if (weaponHandler != null)
            {
                weaponHandler.OnWeaponChanged -= HandleWeaponChanged;
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
            if (_currentWeapon == null) return;

            if (reloadText != null)
            {
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

            UpdateAmmoDisplay();
        }

        private void UpdateAmmoDisplay()
        {
            if (ammoText == null || _currentWeapon == null) return;

            ammoText.text = $"{_currentWeapon.CurrentAmmo} / {_currentWeapon.Data.magazineSize}";
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
