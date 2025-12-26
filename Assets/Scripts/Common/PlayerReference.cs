using UnityEngine;

namespace LooterShooter
{
    /// <summary>
    /// Provides a centralized reference to the player.
    /// Replaces tag-based lookups (FindGameObjectWithTag) with a reliable singleton pattern.
    /// The player registers itself on Awake, making it available to all other systems.
    /// </summary>
    public class PlayerReference : MonoBehaviour
    {
        public static PlayerReference Instance { get; private set; }

        public Transform Transform => transform;
        public GameObject GameObject => gameObject;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("[PlayerReference] Multiple player instances detected. Destroying duplicate.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}
