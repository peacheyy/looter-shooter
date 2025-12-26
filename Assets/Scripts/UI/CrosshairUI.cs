using UnityEngine;
using UnityEngine.UI;

namespace LooterShooter.UI
{
    public class CrosshairUI : MonoBehaviour
    {
        [SerializeField] private Image crosshairImage;
        [SerializeField] private Sprite defaultCrosshair;
        [SerializeField] private Sprite scopedCrosshair;
        [SerializeField] private float defaultSize = 32f;
        [SerializeField] private float scopedSize = 16f;

        private RectTransform _rectTransform;

        private void Awake()
        {
            if (crosshairImage != null)
            {
                _rectTransform = crosshairImage.GetComponent<RectTransform>();
            }
        }

        public void SetScoped(bool isScoped)
        {
            if (crosshairImage == null) return;

            if (isScoped && scopedCrosshair != null)
            {
                crosshairImage.sprite = scopedCrosshair;
                if (_rectTransform != null)
                    _rectTransform.sizeDelta = new Vector2(scopedSize, scopedSize);
            }
            else if (defaultCrosshair != null)
            {
                crosshairImage.sprite = defaultCrosshair;
                if (_rectTransform != null)
                    _rectTransform.sizeDelta = new Vector2(defaultSize, defaultSize);
            }
        }

        public void Show()
        {
            if (crosshairImage != null)
                crosshairImage.gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (crosshairImage != null)
                crosshairImage.gameObject.SetActive(false);
        }
    }
}
