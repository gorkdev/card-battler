namespace CardBattler
{
    // Düşmanın bir turda yapabileceği eylem türleri.
    public enum IntentType
    {
        Attack, // Oyuncuya hasar verir
        Block   // Kendine blok kazandırır
    }

    // Düşmanın tek bir niyeti (gelecek eylemi). Inspector'dan desen oluşturulur.
    [System.Serializable]
    public struct EnemyIntent
    {
        public IntentType type;
        public int amount;
    }
}
