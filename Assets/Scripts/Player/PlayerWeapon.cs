using UnityEngine;

namespace LooterShooter.Player
{
    /// <summary>
    /// Player weapon component - provides access to WeaponHandler.
    /// WeaponHandler now handles input directly, so this is mainly for external access.
    /// </summary>
    [RequireComponent(typeof(WeaponHandler))]
    public class PlayerWeapon : MonoBehaviour
    {
        private WeaponHandler _weaponHandler;

        public WeaponHandler WeaponHandler => _weaponHandler;

        private void Awake()
        {
            _weaponHandler = GetComponent<WeaponHandler>();
        }
    }
}
