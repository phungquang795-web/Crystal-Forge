using UnityEngine;
using CrystalForge.Economy;
using CrystalForge.Utils;

namespace CrystalForge.UI
{
    public class FloatingTextSpawner : MonoBehaviour
    {
        public static FloatingTextSpawner Instance { get; private set; }

        [Header("Prefab Reference")]
        [SerializeField] private FloatingTextItem floatingTextPrefab;

        [Header("Colors")]
        [SerializeField] private Color crystalGainColor = new Color(1.0f, 0.85f, 0.2f); // Vàng rực rỡ
        [SerializeField] private Color critDamageColor = new Color(1.0f, 0.25f, 0.25f);  // Đỏ rực
        [SerializeField] private Color healColor = new Color(0.25f, 1.0f, 0.4f);         // Xanh lá hồi máu

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            ClickSystem.OnCoreClicked += HandleCoreClicked;
        }

        private void OnDestroy()
        {
            ClickSystem.OnCoreClicked -= HandleCoreClicked;
        }

        private void HandleCoreClicked(Vector3 worldPos, double amount)
        {
            SpawnFloatingText(worldPos + Vector3.up * 0.5f, $"+{CrystalFormat.Format(amount)}", crystalGainColor);
        }

        public void SpawnFloatingText(Vector3 position, string text, Color color)
        {
            if (floatingTextPrefab != null)
            {
                var item = Instantiate(floatingTextPrefab, position, Quaternion.identity);
                item.Initialize(text, color);
            }
        }

        public void SpawnDamageText(Vector3 worldPosition, int damage, bool isCritical)
        {
            Color damageColor = isCritical ? critDamageColor : Color.white;
            string damageText = isCritical ? $"CRIT! -{damage}" : $"-{damage}";
            SpawnFloatingText(worldPosition, damageText, damageColor);
        }
    }
}
