namespace CardBattler
{
    // Bir kartın yapabileceği temel etki türleri.
    public enum EffectType
    {
        Attack, // Hedefe (düşmana) hasar verir
        Block,  // Sahibine geçici zırh (blok) kazandırır
        Heal,   // Sahibinin canını yeniler
        Poison  // Hedefe (düşmana) zehir yığını ekler
    }
}
