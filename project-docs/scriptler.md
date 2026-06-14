# Script Referansı

Her script'in ne yaptığı ve Inspector'da göreceğin önemli alanlar. Yol: `Assets/Scripts/...`

---

## Veri katmanı (`Data/`)

### `EffectType.cs`
Kart etkisi türleri (enum): `Attack` (0), `Block` (1), `Heal` (2), `Poison` (3).

### `CardEffect.cs`
Tek bir kart etkisi. `type` + `amount`. Bir kart birden fazla etki taşıyabilir.
`TargetsEnemy` özelliği: Attack/Poison düşmana, Block/Heal sahibe (oyuncuya) gider.

### `CardDefinition.cs`  *(ScriptableObject — Create → Card Battler → Card)*
Bir kartın tanımı.
- `cardName`, `description`, `energyCost` (oynamak için gereken enerji)
- `artwork` (illüstrasyon sprite'ı)
- `effects` — `CardEffect` listesi (sırayla uygulanır)

### `IntentType.cs`
Düşman niyet türleri (enum): `Attack` (0), `Block` (1).
`EnemyIntent` (struct): `type` + `amount` — düşmanın tek bir gelecek eylemi.

### `EnemyDefinition.cs`  *(ScriptableObject — Create → Card Battler → Enemy)*
Bir düşmanın tanımı.
- `enemyName`, `maxHealth`, `artwork`
- `intentPattern` — `EnemyIntent` listesi. Düşman bu niyetleri **sırayla, döngüsel**
  uygular (ör. Attack 6 → Attack 8 → Block 5 → baştan).

---

## Model katmanı (`Model/`) — saf C#

### `Combatant.cs`
Hem oyuncu hem düşman için ortak savaşçı modeli. **MonoBehaviour değil.**
- Durum: `Health`, `MaxHealth`, `Block`, `Poison`, `IsDead`
- Olaylar: `Changed` (UI yenile), `Damaged(int)` (cana işleyen hasar), `Died`
- Metotlar:
  - `GainBlock(n)` — blok ekler (geçici zırh)
  - `Heal(n)` — canı yeniler (maks. aşılmaz)
  - `ApplyPoison(n)` — zehir yığını ekler
  - `TakeDamage(n, ignoreBlock=false)` — önce blok emer, kalan cana işler
  - `TickPoison()` — zehir hasarını (bloğu yok sayarak) uygular, sonra 1 azaltır
  - `ResetBlock()` — bloğu sıfırlar (tur başında)

### `BattleDeck.cs`
Deste yönetimi: çekme yığını (drawPile), el (hand), atma yığını (discardPile).
- `Draw(count)` / `DrawOne()` — kart çeker; çekme yığını boşsa atmayı karıştırıp geri koyar
- `Discard(card)` — kartı elden atmaya gönderir
- `DiscardHand()` — kalan eli atar (tur sonu)
- Karıştırma: Fisher–Yates. Testte sabit sonuç için `seed` verilebilir.

---

## Mantık katmanı (`Combat/`)

### `BattleManager.cs`  *(savaşın beyni)*
Tur döngüsünü, enerjiyi, desteyi ve kazan/kaybet durumunu yönetir.
- **Kurulum alanları:** `startingDeck` (kart listesi), `enemyDefinition`,
  `playerMaxHealth` (50), `energyPerTurn` (3), `cardsPerTurn` (5)
- **Görsel referanslar:** `playerView`, `enemyView`, `handView`, `intentView`,
  `energyView`, `endTurnButton`, `resultView` (opsiyonel)
- **Zamanlama:** `enemyActionDelay` (0.6 sn — düşman eylemleri arası bekleme)
- Akış: `Start` → `StartBattle` → `StartPlayerTurn` → (kart oyna) → `EndTurn` →
  `EnemyTurnRoutine` → tekrar. Detay: [savas-kurallari.md](savas-kurallari.md)
- `ValidateSetup()`: eksik referans varsa Console'a hata yazar (oyun başlamadan uyarır).

---

## Görsel katman (`View/`)

### `CardView.cs`  *(Collider2D ister)*
Tek bir kartın görseli + **sürükle-bırak**. Kart world-space sprite'tır.
- Alanlar: `nameText`, `descriptionText`, `costText`, `illustration`,
  `playThreshold` (0.45 — kart bu viewport-Y oranı üstüne bırakılırsa oynanır),
  `dragScaleMultiplier` (1.1 — sürüklerken büyüme)
- **Gerekli sahne kurulumu:** EventSystem + kamerada Physics 2D Raycaster.

### `HandView.cs`  *(responsive)*
Eldeki kartları **yelpaze** dizer ve ekrana göre konumlar (viewport tabanlı, her ekran
oranında alta yapışık kalır).
- `cardPrefab`, `handViewportCenter` (0.5, 0.12), `cardSpacing` (1.6),
  `maxSpread` (8 — taşarsa sıkışır), `arcHeight` (0.3), `arcAngle` (6)
- `SetHand(...)`, `RemoveCard(...)`, `Clear()` — BattleManager çağırır.

### `CombatantView.cs`
Bir savaşçıyı (oyuncu/düşman) görselleştirir: can barı + blok/zehir göstergeleri +
hasar sarsıntısı + ölünce solma.
- `artworkRenderer`, `healthFill` (pivotu **solda**; x ölçeği 0..1 doldurur),
  `healthText`, `blockRoot`/`blockText`, `poisonRoot`/`poisonText`,
  `shakeDuration` (0.15), `shakeMagnitude` (0.1)
- `Bind(combatant, artwork)` — BattleManager savaş başında çağırır.

### `IntentView.cs`
Düşmanın bir sonraki niyetini gösterir (saldırı/savunma ikonu + miktar).
- `iconRenderer`, `amountText`, `attackIcon`, `blockIcon`.

### `EnergyView.cs`
Kalan/toplam enerjiyi gösterir (ör. "2/3"). Alan: `energyText`.

### `EndTurnButton.cs`  *(UI Button ister)*
"Turu Bitir" butonu. Responsive HUD için bir Canvas üzerindeki Button'a eklenir.
`battle` referansı + `SetInteractable(bool)` (tur arası kilitlenir).

### `BattleResultView.cs`
Savaş bitince "Kazandın / Kaybettin" panelini gösterir.
- `panel`, `messageText`, `winMessage`, `loseMessage`, `restartButton` (opsiyonel —
  basılınca sahneyi yeniden yükler).

### `CameraFitter.cs`  *(Camera ister)*
**Responsive kamera.** `targetWidth` kadar yatay dünya genişliği her ekran oranında
görünür kalsın diye `orthographicSize`'ı otomatik ayarlar. Dar ekranda uzaklaşır.
