using UnityEngine;
using LooterShooter.Player;

namespace LooterShooter.Item
{
    public class Item : MonoBehaviour, IInteractable
    {
        [SerializeField] private float glowIntensity = 2f;
        [SerializeField] private float bobSpeed = 2f;
        [SerializeField] private float bobHeight = 0.25f;
        [SerializeField] private float rotationSpeed = 90f;

        private Vector3 _startPosition;
        private Material _material;
        private ItemData _itemData;

        private void Awake()
        {
            _startPosition = transform.position;
        }

        private void Update()
        {
            float newY = _startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }

        public void Initialize(ItemData data)
        {
            _itemData = data;
            SetupGlow(data.glowColor);
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
            string itemName = _itemData?.itemName ?? "Item";
            return $"[E] Pick up {itemName}";
        }

        public void Interact()
        {
            if (_itemData != null && Inventory.Instance != null)
            {
                Inventory.Instance.AddItem(_itemData);
            }
            else
            {
                Debug.Log("Item picked up! (No inventory found)");
            }
            Destroy(gameObject);
        }

        public static GameObject SpawnAt(Vector3 position, ItemData itemData = null)
        {
            GameObject item = GameObject.CreatePrimitive(PrimitiveType.Cube);
            item.name = "DroppedItem";
            item.transform.position = position + Vector3.up * 0.5f;
            item.transform.localScale = Vector3.one * 0.5f;

            Item itemComponent = item.AddComponent<Item>();

            if (itemData == null)
            {
                itemData = GenerateRandomLoot();
            }
            itemComponent.Initialize(itemData);

            Rigidbody rb = item.AddComponent<Rigidbody>();
            rb.isKinematic = true;

            return item;
        }

        private static ItemData GenerateRandomLoot()
        {
            ItemType[] types = { ItemType.Ammo, ItemType.Health, ItemType.Armor };
            ItemType randomType = types[Random.Range(0, types.Length)];

            int quantity = randomType switch
            {
                ItemType.Ammo => Random.Range(10, 30),
                ItemType.Health => Random.Range(1, 3),
                ItemType.Armor => Random.Range(1, 2),
                _ => 1
            };

            string name = randomType switch
            {
                ItemType.Ammo => "Ammo Pack",
                ItemType.Health => "Med Kit",
                ItemType.Armor => "Armor Shard",
                _ => "Loot"
            };

            return new ItemData(name, randomType, quantity);
        }
    }
}
