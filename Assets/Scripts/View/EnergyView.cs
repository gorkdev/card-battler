using TMPro;
using UnityEngine;

namespace CardBattler
{
    // Kalan / toplam enerjiyi gösterir (ör. "2/3").
    // TMP_Text hem world-space hem UGUI sürümüyle çalışır.
    public class EnergyView : MonoBehaviour
    {
        [SerializeField] TMP_Text energyText;

        public void Show(int current, int max)
        {
            if (energyText != null) energyText.text = $"{current}/{max}";
        }
    }
}
