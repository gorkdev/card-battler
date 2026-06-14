using TMPro;
using UnityEngine;

namespace CardBattler
{
    // Düşmanın bir sonraki turda ne yapacağını (niyet) gösterir:
    // saldırı/savunma ikonu + miktar. Genelde düşmanın üstüne konur.
    public class IntentView : MonoBehaviour
    {
        [SerializeField] SpriteRenderer iconRenderer;
        [SerializeField] TMP_Text amountText;

        [Header("İkonlar")]
        [SerializeField] Sprite attackIcon;
        [SerializeField] Sprite blockIcon;

        public void Show(EnemyIntent intent)
        {
            gameObject.SetActive(true);

            if (iconRenderer != null)
                iconRenderer.sprite = intent.type == IntentType.Attack ? attackIcon : blockIcon;

            if (amountText != null)
                amountText.text = intent.amount > 0 ? intent.amount.ToString() : "";
        }

        public void Hide() => gameObject.SetActive(false);
    }
}
