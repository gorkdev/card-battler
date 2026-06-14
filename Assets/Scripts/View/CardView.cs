using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CardBattler
{
    // Tek bir kartın görseli + sürükle-bırak davranışı.
    // World-space sprite olarak çalışır; Collider2D ister.
    // Sürükleme için sahnede EventSystem ve kamerada Physics 2D Raycaster olmalı.
    [RequireComponent(typeof(Collider2D))]
    public class CardView : MonoBehaviour,
        IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("Kart Görselleri (prefab içinden bağla)")]
        [SerializeField] TMP_Text nameText;
        [SerializeField] TMP_Text descriptionText;
        [SerializeField] TMP_Text costText;
        [SerializeField] SpriteRenderer illustration;

        [Header("Oynama")]
        [Tooltip("Kart bu viewport Y oranının üstüne bırakılırsa oynanır (0-1).")]
        [SerializeField] float playThreshold = 0.45f;
        [Tooltip("Sürüklerken kart bu oranda büyür.")]
        [SerializeField] float dragScaleMultiplier = 1.1f;

        CardDefinition card;
        BattleManager battle;

        Vector3 homePosition;
        Quaternion homeRotation = Quaternion.identity;
        Vector3 homeScale = Vector3.one;
        bool dragging;
        Camera cam;

        public CardDefinition Card => card;

        // HandView kart oluşturduğunda çağırır.
        public void Setup(CardDefinition definition, BattleManager battleManager)
        {
            card = definition;
            battle = battleManager;
            cam = Camera.main;
            homeScale = transform.localScale;
            Refresh();
        }

        void Refresh()
        {
            if (nameText != null) nameText.text = card.cardName;
            if (descriptionText != null) descriptionText.text = card.description;
            if (costText != null) costText.text = card.energyCost.ToString();
            if (illustration != null && card.artwork != null) illustration.sprite = card.artwork;
        }

        // HandView her düzenlemede kartın hedef ("ev") konumunu verir.
        public void SetHome(Vector3 localPos, Quaternion localRot)
        {
            homePosition = localPos;
            homeRotation = localRot;
            if (!dragging)
            {
                transform.localPosition = localPos;
                transform.localRotation = localRot;
                transform.localScale = homeScale;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (battle == null || !battle.CanPlay(card)) return;
            dragging = true;
            transform.localRotation = Quaternion.identity;
            transform.localScale = homeScale * dragScaleMultiplier;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!dragging) return;
            if (cam == null) cam = Camera.main;
            Vector3 world = cam.ScreenToWorldPoint(eventData.position);
            world.z = transform.position.z;
            transform.position = world;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!dragging) return;
            dragging = false;

            float viewportY = cam.ScreenToViewportPoint(eventData.position).y;
            bool playedHere = viewportY >= playThreshold && battle.CanPlay(card);

            if (playedHere)
                battle.PlayCard(card); // bu kart yok edilir
            else
                ReturnHome();
        }

        void ReturnHome()
        {
            transform.localPosition = homePosition;
            transform.localRotation = homeRotation;
            transform.localScale = homeScale;
        }
    }
}
