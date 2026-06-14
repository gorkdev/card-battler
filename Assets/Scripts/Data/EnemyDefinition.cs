using System.Collections.Generic;
using UnityEngine;

namespace CardBattler
{
    // Bir düşmanın tanımı (ör. Goblin). Asset olarak yaşar:
    // Create -> Card Battler -> Enemy.
    [CreateAssetMenu(fileName = "NewEnemy", menuName = "Card Battler/Enemy")]
    public class EnemyDefinition : ScriptableObject
    {
        public string enemyName = "Goblin";

        [Min(1)]
        public int maxHealth = 30;

        public Sprite artwork;

        [Tooltip("Düşman bu niyetleri sırayla, döngüsel olarak uygular.")]
        public List<EnemyIntent> intentPattern = new List<EnemyIntent>();
    }
}
