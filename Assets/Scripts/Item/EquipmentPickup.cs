using UnityEngine;
using LooterShooter.Player;

namespace LooterShooter.Item
{
    /// <summary>
    /// World pickup for equipment items (weapons and armor).
    /// Adds item to backpack on interaction.
    /// </summary>
    public class EquipmentPickup : MonoBehaviour, IInteractable
    {
        [Header("Equipment")]
        [SerializeField] private EquipmentData equipmentData;

        [Header("Visual Effects")]
        [SerializeField] private float glowIntensity = 2f;
        [SerializeField] private float bobSpeed = 2f;
        [SerializeField] private float bobHeight = 0.25f;
        [SerializeField] private float rotationSpeed = 90f;

        private Vector3 _startPosition;
        private Material _material;
        private EquipmentInstance _instance;

        private void Awake()
        {
            _startPosition = transform.position;
        }

        private void Start()
        {
            if (equipmentData != null && _instance == null)
            {
                Initialize(equipmentData);
            }
        }

        private void Update()
        {
            // Bobbing animation
            float newY = _startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

            // Rotation
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Initializes the pickup with equipment data.
        /// </summary>
        public void Initialize(EquipmentData data)
        {
            equipmentData = data;
            _instance = new EquipmentInstance(data);
            SetupGlow(data.glowColor);
        }

        /// <summary>
        /// Initializes the pickup with an existing equipment instance.
        /// Used when dropping items from inventory.
        /// </summary>
        public void Initialize(EquipmentInstance instance)
        {
            _instance = instance;
            equipmentData = instance.Data;
            SetupGlow(equipmentData.glowColor);
        }

        private void SetupGlow(Color glowColor)
        {
            _material = GetComponent<Renderer>()?.material;
            if (_material != null)
            {
                _material.EnableKeyword("_EMISSION");
                _material.SetColor("_EmissionColor", glowColor * glowIntensity);
                _material.color = glowColor;
            }
        }

        public string GetInteractPrompt()
        {
            string itemName = equipmentData?.itemName ?? "Equipment";
            return $"[E] Pick up {itemName}";
        }

        public void Interact()
        {
            var em = EquipmentManager.Instance;
            if (em == null)
            {
                Debug.LogWarning("EquipmentManager not found!");
                return;
            }

            if (_instance == null)
            {
                Debug.LogWarning("No equipment instance to pick up!");
                return;
            }

            // Try to add to backpack
            if (em.Add(_instance))
            {
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Backpack is full!");
                // Item stays in world - player can come back for it
            }
        }

        /// <summary>
        /// Spawns an equipment pickup at a position.
        /// </summary>
        public static GameObject SpawnAt(Vector3 position, EquipmentData data)
        {
            // Create a simple cube visual (can be replaced with actual prefab)
            GameObject pickupObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pickupObj.name = $"EquipmentPickup_{data.itemName}";
            pickupObj.transform.position = position + Vector3.up * 0.5f;
            pickupObj.transform.localScale = Vector3.one * 0.5f;

            EquipmentPickup pickup = pickupObj.AddComponent<EquipmentPickup>();
            pickup.Initialize(data);

            Rigidbody rb = pickupObj.AddComponent<Rigidbody>();
            rb.isKinematic = true;

            return pickupObj;
        }

        /// <summary>
        /// Spawns an equipment pickup for an existing instance (for dropping items).
        /// </summary>
        public static GameObject SpawnAt(Vector3 position, EquipmentInstance instance)
        {
            GameObject pickupObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pickupObj.name = $"EquipmentPickup_{instance.Data.itemName}";
            pickupObj.transform.position = position + Vector3.up * 0.5f;
            pickupObj.transform.localScale = Vector3.one * 0.5f;

            EquipmentPickup pickup = pickupObj.AddComponent<EquipmentPickup>();
            pickup.Initialize(instance);

            Rigidbody rb = pickupObj.AddComponent<Rigidbody>();
            rb.isKinematic = true;

            return pickupObj;
        }
    }
}
