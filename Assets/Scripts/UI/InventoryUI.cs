using UnityEngine;
using UnityEngine.InputSystem;
using LooterShooter.Player;
using LooterShooter.Item;

namespace LooterShooter.UI
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private bool showInventory = true;

        private Inventory _inventory;

        private void Start()
        {
            _inventory = Inventory.Instance;
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
            {
                showInventory = !showInventory;
            }
        }

        private void OnGUI()
        {
            if (!showInventory || _inventory == null) return;

            float padding = 10f;
            float width = 200f;
            float lineHeight = 25f;

            var items = _inventory.GetAllItems();
            float height = Mathf.Max(60f, (items.Count + 1) * lineHeight + padding * 2);

            Rect boxRect = new Rect(padding, padding, width, height);

            GUI.Box(boxRect, "");

            GUIStyle headerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };

            GUIStyle itemStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                normal = { textColor = Color.white }
            };

            float y = padding + 5f;

            GUI.Label(new Rect(padding + 10f, y, width - 20f, lineHeight), "INVENTORY", headerStyle);
            y += lineHeight;

            if (items.Count == 0)
            {
                itemStyle.normal.textColor = Color.gray;
                GUI.Label(new Rect(padding + 10f, y, width - 20f, lineHeight), "Empty", itemStyle);
            }
            else
            {
                foreach (var kvp in items)
                {
                    Color itemColor = ItemData.GetDefaultColor(kvp.Key);
                    itemStyle.normal.textColor = itemColor;
                    GUI.Label(new Rect(padding + 10f, y, width - 20f, lineHeight), $"{kvp.Key}: {kvp.Value}", itemStyle);
                    y += lineHeight;
                }
            }
        }
    }
}
