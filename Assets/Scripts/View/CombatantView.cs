using TMPro;
using UnityEngine;

namespace CardBattler
{
    // Bir savaşçıyı (oyuncu veya düşman) görselleştirir:
    // can barı, blok ve zehir göstergeleri + basit hasar/ölüm geri bildirimi.
    public class CombatantView : MonoBehaviour
    {
        [Header("Görsel (opsiyonel)")]
        [SerializeField] SpriteRenderer artworkRenderer;

        [Header("Can Barı")]
        [Tooltip("Dolan parça. Pivotu SOLDA olmalı; x ölçeği 0..1 ile doldurulur.")]
        [SerializeField] Transform healthFill;
        [SerializeField] TMP_Text healthText;

        [Header("Durum Göstergeleri")]
        [SerializeField] GameObject blockRoot;   // blok 0 ise gizlenir
        [SerializeField] TMP_Text blockText;
        [SerializeField] GameObject poisonRoot;  // zehir 0 ise gizlenir
        [SerializeField] TMP_Text poisonText;

        [Header("Hasar Geri Bildirimi")]
        [SerializeField] float shakeDuration = 0.15f;
        [SerializeField] float shakeMagnitude = 0.1f;

        Combatant combatant;
        Vector3 baseLocalPos;
        float shakeTimer;

        public void Bind(Combatant c, Sprite artwork = null)
        {
            combatant = c;
            baseLocalPos = transform.localPosition;

            if (artwork != null && artworkRenderer != null)
                artworkRenderer.sprite = artwork;

            combatant.Changed += Refresh;
            combatant.Damaged += OnDamaged;
            combatant.Died += OnDied;
            Refresh();
        }

        void OnDestroy()
        {
            if (combatant == null) return;
            combatant.Changed -= Refresh;
            combatant.Damaged -= OnDamaged;
            combatant.Died -= OnDied;
        }

        void Refresh()
        {
            if (healthFill != null)
            {
                float pct = combatant.MaxHealth > 0
                    ? (float)combatant.Health / combatant.MaxHealth
                    : 0f;
                Vector3 s = healthFill.localScale;
                s.x = Mathf.Clamp01(pct);
                healthFill.localScale = s;
            }
            if (healthText != null)
                healthText.text = $"{combatant.Health}/{combatant.MaxHealth}";

            if (blockRoot != null) blockRoot.SetActive(combatant.Block > 0);
            if (blockText != null) blockText.text = combatant.Block.ToString();

            if (poisonRoot != null) poisonRoot.SetActive(combatant.Poison > 0);
            if (poisonText != null) poisonText.text = combatant.Poison.ToString();
        }

        void OnDamaged(int amount)
        {
            shakeTimer = shakeDuration; // basit sarsıntı
        }

        void OnDied()
        {
            if (artworkRenderer != null)
            {
                Color c = artworkRenderer.color;
                c.a = 0.3f;
                artworkRenderer.color = c;
            }
        }

        void Update()
        {
            if (shakeTimer > 0f)
            {
                shakeTimer -= Time.deltaTime;
                Vector2 offset = Random.insideUnitCircle * shakeMagnitude;
                transform.localPosition = baseLocalPos + (Vector3)offset;
                if (shakeTimer <= 0f) transform.localPosition = baseLocalPos;
            }
        }
    }
}
