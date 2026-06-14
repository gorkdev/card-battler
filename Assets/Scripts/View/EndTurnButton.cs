using UnityEngine;
using UnityEngine.UI;

namespace CardBattler
{
    // "Turu Bitir" butonu. Responsive HUD için bir Canvas üzerindeki UI Button'a eklenir.
    [RequireComponent(typeof(Button))]
    public class EndTurnButton : MonoBehaviour
    {
        [SerializeField] BattleManager battle;
        Button button;

        void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);
        }

        void OnClick()
        {
            if (battle != null) battle.EndTurn();
        }

        // BattleManager turlar arasında butonu kilitler/açar.
        public void SetInteractable(bool value)
        {
            if (button != null) button.interactable = value;
        }
    }
}
