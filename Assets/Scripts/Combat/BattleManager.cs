using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardBattler
{
    // Savaşın beynidir: tur döngüsünü, enerjiyi, desteyi ve kazan/kaybet
    // durumunu yönetir; görsel katmanı yönlendirir.
    public class BattleManager : MonoBehaviour
    {
        [Header("Kurulum")]
        [SerializeField] List<CardDefinition> startingDeck = new List<CardDefinition>();
        [SerializeField] EnemyDefinition enemyDefinition;
        [SerializeField] int playerMaxHealth = 50;
        [SerializeField] int energyPerTurn = 3;
        [SerializeField] int cardsPerTurn = 5;

        [Header("Görsel Referanslar")]
        [SerializeField] CombatantView playerView;
        [SerializeField] CombatantView enemyView;
        [SerializeField] HandView handView;
        [SerializeField] IntentView intentView;
        [SerializeField] EnergyView energyView;
        [SerializeField] EndTurnButton endTurnButton;
        [SerializeField] BattleResultView resultView; // opsiyonel

        [Header("Zamanlama")]
        [SerializeField] float enemyActionDelay = 0.6f;

        public Combatant Player { get; private set; }
        public Combatant Enemy { get; private set; }
        public int Energy { get; private set; }
        public bool CanAct => playerTurn && !battleOver;

        BattleDeck deck;
        int intentIndex;
        bool playerTurn;
        bool battleOver;

        void Start() => StartBattle();

        public void StartBattle()
        {
            if (!ValidateSetup()) return;

            battleOver = false;
            playerTurn = false;
            intentIndex = 0;

            Player = new Combatant("Oyuncu", playerMaxHealth);
            Enemy = new Combatant(enemyDefinition.enemyName, enemyDefinition.maxHealth);

            playerView.Bind(Player);
            enemyView.Bind(Enemy, enemyDefinition.artwork);

            Player.Died += () => EndBattle(false);
            Enemy.Died += () => EndBattle(true);

            deck = new BattleDeck(startingDeck);

            if (resultView != null) resultView.Hide();
            UpdateIntentDisplay();
            StartPlayerTurn();
        }

        void StartPlayerTurn()
        {
            if (battleOver) return;
            playerTurn = true;

            Player.ResetBlock();
            Player.TickPoison();
            if (battleOver) return; // zehir oyuncuyu öldürmüş olabilir

            Energy = energyPerTurn;
            energyView.Show(Energy, energyPerTurn);

            List<CardDefinition> drawn = deck.Draw(cardsPerTurn);
            handView.SetHand(drawn, this);
            endTurnButton.SetInteractable(true);
        }

        public bool CanPlay(CardDefinition card)
            => CanAct && card != null && card.energyCost <= Energy;

        // CardView sürükle-bırak tamamlandığında çağırır.
        public void PlayCard(CardDefinition card)
        {
            if (!CanPlay(card)) return;

            Energy -= card.energyCost;
            foreach (CardEffect effect in card.effects)
                ApplyEffect(effect);

            energyView.Show(Energy, energyPerTurn);

            deck.Discard(card);
            handView.RemoveCard(card);
            // Ölüm, Combatant.Died olayıyla otomatik ele alınır.
        }

        void ApplyEffect(CardEffect effect)
        {
            switch (effect.type)
            {
                case EffectType.Attack: Enemy.TakeDamage(effect.amount); break;
                case EffectType.Poison: Enemy.ApplyPoison(effect.amount); break;
                case EffectType.Block:  Player.GainBlock(effect.amount); break;
                case EffectType.Heal:   Player.Heal(effect.amount); break;
            }
        }

        // EndTurnButton çağırır.
        public void EndTurn()
        {
            if (!CanAct) return;
            playerTurn = false;
            endTurnButton.SetInteractable(false);
            deck.DiscardHand();
            handView.Clear();
            StartCoroutine(EnemyTurnRoutine());
        }

        IEnumerator EnemyTurnRoutine()
        {
            yield return new WaitForSeconds(enemyActionDelay);
            if (battleOver) yield break;

            Enemy.ResetBlock();
            Enemy.TickPoison();
            if (battleOver) yield break; // zehir düşmanı öldürmüş olabilir

            EnemyIntent intent = CurrentIntent();
            yield return new WaitForSeconds(enemyActionDelay);

            switch (intent.type)
            {
                case IntentType.Attack: Player.TakeDamage(intent.amount); break;
                case IntentType.Block:  Enemy.GainBlock(intent.amount); break;
            }
            if (battleOver) yield break;

            intentIndex++;
            UpdateIntentDisplay();

            yield return new WaitForSeconds(enemyActionDelay);
            StartPlayerTurn();
        }

        EnemyIntent CurrentIntent()
        {
            var pattern = enemyDefinition.intentPattern;
            if (pattern == null || pattern.Count == 0)
                return new EnemyIntent { type = IntentType.Attack, amount = 5 };
            return pattern[intentIndex % pattern.Count];
        }

        void UpdateIntentDisplay() => intentView.Show(CurrentIntent());

        void EndBattle(bool playerWon)
        {
            if (battleOver) return;
            battleOver = true;
            playerTurn = false;
            endTurnButton.SetInteractable(false);
            if (resultView != null) resultView.ShowResult(playerWon);
        }

        bool ValidateSetup()
        {
            var missing = new List<string>();
            if (enemyDefinition == null) missing.Add(nameof(enemyDefinition));
            if (playerView == null) missing.Add(nameof(playerView));
            if (enemyView == null) missing.Add(nameof(enemyView));
            if (handView == null) missing.Add(nameof(handView));
            if (intentView == null) missing.Add(nameof(intentView));
            if (energyView == null) missing.Add(nameof(energyView));
            if (endTurnButton == null) missing.Add(nameof(endTurnButton));
            if (startingDeck == null || startingDeck.Count == 0) missing.Add(nameof(startingDeck));

            if (missing.Count > 0)
            {
                Debug.LogError(
                    $"[BattleManager] Eksik referans/ayar: {string.Join(", ", missing)}. " +
                    "Inspector'dan doldurun.", this);
                return false;
            }
            return true;
        }
    }
}
