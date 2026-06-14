using System;
using UnityEngine;

namespace CardBattler
{
    // Hem oyuncu hem düşman için ortak savaşçı modeli.
    // MonoBehaviour DEĞİL: oyun mantığı motordan bağımsız, kolay test edilebilir.
    // Görsel katman (CombatantView) bu olayları dinleyip ekranı günceller.
    public class Combatant
    {
        public string Name { get; }
        public int MaxHealth { get; }
        public int Health { get; private set; }
        public int Block { get; private set; }
        public int Poison { get; private set; }

        public bool IsDead => Health <= 0;

        public event Action Changed;      // Herhangi bir değer değişti (UI yenile)
        public event Action<int> Damaged; // Cana işleyen hasar miktarı (blok sonrası)
        public event Action Died;         // Can 0'a düştü

        public Combatant(string name, int maxHealth)
        {
            Name = name;
            MaxHealth = Mathf.Max(1, maxHealth);
            Health = MaxHealth;
        }

        public void GainBlock(int amount)
        {
            if (amount <= 0) return;
            Block += amount;
            Changed?.Invoke();
        }

        public void Heal(int amount)
        {
            if (amount <= 0 || IsDead) return;
            Health = Mathf.Min(MaxHealth, Health + amount);
            Changed?.Invoke();
        }

        public void ApplyPoison(int amount)
        {
            if (amount <= 0) return;
            Poison += amount;
            Changed?.Invoke();
        }

        // Hasar uygula. Önce blok emer, kalan cana işler.
        // ignoreBlock=true ise blok yok sayılır (zehir gibi).
        public void TakeDamage(int amount, bool ignoreBlock = false)
        {
            if (amount <= 0 || IsDead) return;

            int remaining = amount;
            if (!ignoreBlock)
            {
                int absorbed = Mathf.Min(Block, remaining);
                Block -= absorbed;
                remaining -= absorbed;
            }

            if (remaining > 0)
            {
                Health = Mathf.Max(0, Health - remaining);
                Damaged?.Invoke(remaining);
            }

            Changed?.Invoke();
            if (IsDead) Died?.Invoke();
        }

        // Tur başında çağrılır: zehir hasarını (bloğu yok sayarak) uygula, sonra 1 azalt.
        public void TickPoison()
        {
            if (Poison <= 0) return;
            TakeDamage(Poison, ignoreBlock: true);
            Poison = Mathf.Max(0, Poison - 1);
            Changed?.Invoke();
        }

        // Tur başında blok sıfırlanır (geçicidir).
        public void ResetBlock()
        {
            if (Block == 0) return;
            Block = 0;
            Changed?.Invoke();
        }
    }
}
