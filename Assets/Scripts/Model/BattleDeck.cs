using System.Collections.Generic;

namespace CardBattler
{
    // Çekme yığını (drawPile), el (hand) ve atma yığınını (discardPile) yönetir.
    // Saf C#: karıştırma/çekme/yeniden karıştırma mantığı motordan bağımsız.
    public class BattleDeck
    {
        readonly List<CardDefinition> drawPile = new List<CardDefinition>();
        readonly List<CardDefinition> discardPile = new List<CardDefinition>();
        readonly List<CardDefinition> hand = new List<CardDefinition>();
        readonly System.Random rng;

        public IReadOnlyList<CardDefinition> Hand => hand;
        public int DrawCount => drawPile.Count;
        public int DiscardCount => discardPile.Count;

        // seed = 0 ise rastgele; testte sabit seed verilebilir.
        public BattleDeck(IEnumerable<CardDefinition> startingCards, int seed = 0)
        {
            rng = seed == 0 ? new System.Random() : new System.Random(seed);
            if (startingCards != null) drawPile.AddRange(startingCards);
            Shuffle(drawPile);
        }

        // Tek kart çeker. Çekme yığını boşsa atmayı karıştırıp geri koyar.
        public CardDefinition DrawOne()
        {
            if (drawPile.Count == 0) ReshuffleDiscardIntoDraw();
            if (drawPile.Count == 0) return null; // hiç kart kalmadı

            int last = drawPile.Count - 1;
            CardDefinition card = drawPile[last];
            drawPile.RemoveAt(last);
            hand.Add(card);
            return card;
        }

        public List<CardDefinition> Draw(int count)
        {
            var drawn = new List<CardDefinition>();
            for (int i = 0; i < count; i++)
            {
                CardDefinition c = DrawOne();
                if (c == null) break;
                drawn.Add(c);
            }
            return drawn;
        }

        // Tek kartı elden atma yığınına gönder.
        public void Discard(CardDefinition card)
        {
            if (hand.Remove(card)) discardPile.Add(card);
        }

        // Tur sonunda kalan eli atar.
        public void DiscardHand()
        {
            discardPile.AddRange(hand);
            hand.Clear();
        }

        void ReshuffleDiscardIntoDraw()
        {
            drawPile.AddRange(discardPile);
            discardPile.Clear();
            Shuffle(drawPile);
        }

        // Fisher–Yates karıştırma.
        void Shuffle(List<CardDefinition> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
