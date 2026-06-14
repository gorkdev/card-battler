# Mimari — 3 Katmanlı Yapı

Kod bilinçli olarak **üç katmana** ayrıldı. Amaç: savaş kurallarını oyun motorundan
bağımsız tutmak, böylece mantığı test etmek ve görseli değiştirmek birbirini bozmasın.

```
┌─────────────────────────────────────────────────────────────┐
│  GÖRSEL KATMAN (View) — MonoBehaviour                        │
│  CardView, HandView, CombatantView, IntentView, EnergyView,  │
│  EndTurnButton, BattleResultView, CameraFitter               │
│  Modeldeki olayları dinler, ekranı çizer, girdiyi alır.      │
└───────────────▲───────────────────────────┬─────────────────┘
                │ olaylar (events)           │ komutlar
                │                            ▼
┌───────────────┴─────────────────────────────────────────────┐
│  MANTIK KATMANI — MonoBehaviour                              │
│  BattleManager: tur döngüsü, enerji, kazan/kaybet            │
└───────────────▲───────────────────────────┬─────────────────┘
                │                            │
┌───────────────┴─────────────────────────────────────────────┐
│  MODEL KATMANI — saf C# (Unity'den bağımsız)                 │
│  Combatant (can/blok/zehir), BattleDeck (deste)             │
└───────────────▲─────────────────────────────────────────────┘
                │ okur
┌───────────────┴─────────────────────────────────────────────┐
│  VERİ KATMANI — ScriptableObject (asset)                     │
│  CardDefinition, EnemyDefinition + (CardEffect, EnemyIntent) │
└─────────────────────────────────────────────────────────────┘
```

## 1. Veri katmanı (`Assets/Scripts/Data/`)
Kartlar ve düşmanlar **kod değil, asset** olarak yaşar. `[CreateAssetMenu]` sayesinde
Unity'de sağ tık → Create ile yeni kart/düşman üretilir. Kod yazmadan içerik eklenir.

## 2. Model katmanı (`Assets/Scripts/Model/`)
**MonoBehaviour değil, saf C#.** `Combatant` ve `BattleDeck` burada. Can hesabı, blok,
zehir, deste karıştırma gibi kurallar motordan bağımsızdır → ileride birim testleri (Unit
Test) yazılabilir. Bu katman ekranı veya Unity API'sini bilmez.

## 3. Mantık katmanı (`Assets/Scripts/Combat/`)
`BattleManager` bir MonoBehaviour'dur ve **orkestra şefidir**: model nesnelerini
(Combatant, BattleDeck) yaratır, tur döngüsünü yürütür, görsel bileşenleri yönlendirir.

## 4. Görsel katman (`Assets/Scripts/View/`)
Ekranı çizen ve girdi alan her şey. Model değişince çalışan **olaylara (events)** abone
olur: `Combatant.Changed`, `Combatant.Damaged`, `Combatant.Died`. Böylece mantık,
görselin nasıl çizdiğini hiç bilmeden veri değişimini bildirir.

## Veri akışı örneği — "Vuruş kartını oyna"

1. Oyuncu kartı goblin'e sürükler → `CardView.OnEndDrag`
2. `CardView` → `BattleManager.PlayCard(card)`
3. `BattleManager` enerjiyi düşer, `card.effects` içindeki her etkiyi uygular:
   `Enemy.TakeDamage(6)` (model)
4. `Combatant` (Enemy) canını azaltır, `Changed` ve `Damaged` olaylarını tetikler
5. Goblin'in `CombatantView`'i bu olayları yakalar → can barını günceller, sarsıntı oynatır
6. Goblin canı 0 olursa `Died` tetiklenir → `BattleManager.EndBattle(true)` → "Kazandın!"

## Neden böyle?
- **Test edilebilirlik:** Kurallar Unity'siz çalışır.
- **Değiştirilebilirlik:** Kartın görselini değiştirmek mantığı bozmaz; tersi de geçerli.
- **Anlaşılırlık:** Her dosyanın tek bir işi var, kafada tutması kolay.
