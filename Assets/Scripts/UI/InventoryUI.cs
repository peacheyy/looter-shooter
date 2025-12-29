using UnityEngine;
using TMPro;
using LooterShooter.Player;
using LooterShooter.Item;

namespace LooterShooter.UI
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private TextMeshProUGUI headerText;
        [SerializeField] private TextMeshProUGUI itemsText;

        private Inventory _inventory;

        private void Start()
        {
            _inventory = Inventory.Instance;

            if (_inventory != null)
            {
                _inventory.OnInventoryChanged += UpdateUI;
            }

            UpdateUI();
        }

        private void OnDestroy()
        {
            if (_inventory != null)
            {
                _inventory.OnInventoryChanged -= UpdateUI;
            }
        }

        private void UpdateUI()
        {
            if (_inventory == null || itemsText == null) return;

            var items = _inventory.GetAllItems();

            if (items.Count == 0)
            {
                itemsText.text = "<color=#888888>Empty</color>";
            }
            else
            {
                var sb = new System.Text.StringBuilder();
                foreach (var kvp in items)
                {
                    Color itemColor = ItemData.GetDefaultColor(kvp.Key);
                    string hexColor = ColorUtility.ToHtmlStringRGB(itemColor);
                    sb.AppendLine($"<color=#{hexColor}>{kvp.Key}: {kvp.Value}</color>");
                }
                itemsText.text = sb.ToString().TrimEnd();
            }
        }
    }
}
