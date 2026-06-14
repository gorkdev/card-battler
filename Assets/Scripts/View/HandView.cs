using System.Collections.Generic;
using UnityEngine;

namespace CardBattler
{
    // Eldeki kartları yelpaze (fan) şeklinde dizer ve ekrana göre konumlar.
    // RESPONSIVE: elin merkezi viewport (0-1) koordinatından dünyaya çevrilir,
    // böylece farklı ekran/oran ve kamera boyutlarında alta yapışık kalır.
    public class HandView : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField] CardView cardPrefab;

        [Header("Yerleşim (responsive)")]
        [Tooltip("Elin merkezinin viewport konumu (0=sol/alt, 1=sağ/üst).")]
        [SerializeField] Vector2 handViewportCenter = new Vector2(0.5f, 0.12f);
        [Tooltip("Kartlar arası ideal mesafe (dünya birimi).")]
        [SerializeField] float cardSpacing = 1.6f;
        [Tooltip("Tüm elin aşamayacağı maksimum genişlik (taşarsa sıkışır).")]
        [SerializeField] float maxSpread = 8f;
        [Tooltip("Yelpazenin kavis yüksekliği.")]
        [SerializeField] float arcHeight = 0.3f;
        [Tooltip("Kenar kartların eğim açısı (derece).")]
        [SerializeField] float arcAngle = 6f;

        readonly List<CardView> cards = new List<CardView>();
        Camera cam;

        void Awake() => cam = Camera.main;

        // BattleManager her oyuncu turu başında çağırır.
        public void SetHand(IEnumerable<CardDefinition> definitions, BattleManager battle)
        {
            Clear();
            foreach (CardDefinition def in definitions)
            {
                CardView view = Instantiate(cardPrefab, transform);
                view.Setup(def, battle);
                cards.Add(view);
            }
            Layout();
        }

        public void RemoveCard(CardDefinition card)
        {
            CardView view = cards.Find(c => c != null && c.Card == card);
            if (view == null) return;
            cards.Remove(view);
            Destroy(view.gameObject);
            Layout();
        }

        public void Clear()
        {
            foreach (CardView c in cards)
                if (c != null) Destroy(c.gameObject);
            cards.Clear();
        }

        // Her kare yeniden konumla: pencere boyutu değişse de el doğru kalır.
        void LateUpdate() => Layout();

        void Layout()
        {
            if (cam == null) cam = Camera.main;
            if (cam == null || cards.Count == 0) return;

            // Elin merkezini viewport'tan dünyaya çevir (responsive temel).
            float depth = Mathf.Abs(cam.transform.position.z - transform.position.z);
            Vector3 center = cam.ViewportToWorldPoint(
                new Vector3(handViewportCenter.x, handViewportCenter.y, depth));
            center.z = transform.position.z;

            int n = cards.Count;
            float spacing = cardSpacing;
            float total = spacing * (n - 1);
            if (n > 1 && total > maxSpread)
                spacing = maxSpread / (n - 1);

            total = spacing * (n - 1);
            float startX = -total * 0.5f;

            for (int i = 0; i < n; i++)
            {
                if (cards[i] == null) continue;

                float t = n > 1 ? (float)i / (n - 1) - 0.5f : 0f; // -0.5..+0.5
                float x = startX + spacing * i;
                float y = -Mathf.Abs(t) * arcHeight * 2f; // merkez yukarıda
                float rotZ = -t * arcAngle * 2f;

                Vector3 worldPos = center + new Vector3(x, y, 0f);
                Vector3 localPos = transform.InverseTransformPoint(worldPos);
                localPos.z = -i * 0.01f; // soldan sağa öne doğru sıralama

                cards[i].SetHome(localPos, Quaternion.Euler(0f, 0f, rotZ));
            }
        }
    }
}
