using System.Collections.Generic;
using UnityEngine;

namespace CardBattler
{
    // Bir kartın tüm tanımı. Kod değil, asset olarak yaşar:
    // Project penceresinde sağ tık -> Create -> Card Battler -> Card.
    [CreateAssetMenu(fileName = "NewCard", menuName = "Card Battler/Card")]
    public class CardDefinition : ScriptableObject
    {
        public string cardName = "Yeni Kart";

        [TextArea]
        public string description = "";

        [Min(0)]
        [Tooltip("Bu kartı oynamak için gereken enerji.")]
        public int energyCost = 1;

        [Tooltip("Kartın illüstrasyon görseli.")]
        public Sprite artwork;

        [Tooltip("Kart oynanınca uygulanacak etkiler (sırayla).")]
        public List<CardEffect> effects = new List<CardEffect>();
    }
}
