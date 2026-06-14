using UnityEngine;

namespace CardBattler
{
    // Tek bir kart etkisi. Bir kart birden fazla CardEffect taşıyabilir
    // (ör. "3 hasar + 2 zehir"). Inspector'dan düzenlenebilir.
    [System.Serializable]
    public class CardEffect
    {
        public EffectType type = EffectType.Attack;

        [Min(0)] public int amount = 1;

        // Attack ve Poison düşmana; Block ve Heal oyuncuya (sahibe) uygulanır.
        public bool TargetsEnemy => type == EffectType.Attack || type == EffectType.Poison;
    }
}
