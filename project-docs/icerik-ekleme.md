# İçerik Ekleme (Kod Yazmadan)

Kartlar ve düşmanlar ScriptableObject olduğu için yeni içerik **Unity Editor'den** eklenir.

## Yeni kart ekleme
1. `Assets/Data` klasöründe sağ tık → **Create → Card Battler → Card**.
2. İsim ver (ör. `Kritik Vuruş`).
3. Inspector'da doldur:
   - **Card Name / Description** — UI'da görünecek metinler
   - **Energy Cost** — oynamak için gereken enerji
   - **Artwork** — illüstrasyon sprite'ı (opsiyonel)
   - **Effects** — `+` ile etki ekle; her etkide **Type** (Attack/Block/Heal/Poison) ve
     **Amount** seç. **Birden fazla etki** eklenebilir (ör. Attack 6 + Poison 2).
4. Desteye katmak için `BattleManager → Starting Deck` listesine bu asset'i ekle. Aynı
   kartı birden çok kez ekleyerek deste içinde kopyasını çoğaltabilirsin.

### Örnek kart fikirleri
| Kart | Cost | Etkiler |
|---|---|---|
| Vuruş | 1 | Attack 6 |
| Ağır Darbe | 2 | Attack 12 |
| Savunma | 1 | Block 5 |
| Zehirli Bıçak | 1 | Attack 3 + Poison 2 |
| Yenilen | 1 | Heal 8 |
| Kalkan Duvarı | 2 | Block 8 + Heal 3 |

## Yeni düşman ekleme
1. `Assets/Data`'da sağ tık → **Create → Card Battler → Enemy**.
2. Doldur:
   - **Enemy Name**, **Max Health**, **Artwork**
   - **Intent Pattern** — `+` ile niyet ekle; her niyette **Type** (Attack/Block) ve
     **Amount**. Düşman bunları **sırayla, döngüsel** uygular.
3. `BattleManager → Enemy Definition` alanına bu asset'i ata.

### Örnek: Goblin
- Max Health: 30
- Intent Pattern: Attack 6 → Attack 8 → Block 5 → (baştan)

## Yeni etki türü eklemek (kod gerekir)
Yeni bir mekanik (ör. "Güçlenme/Strength") istiyorsan:
1. `EffectType` enum'una yeni değer ekle.
2. `BattleManager.ApplyEffect` içindeki `switch`'e o değerin davranışını ekle.
3. Gerekiyorsa `Combatant`'a yeni durum alanı/metot ekle (ör. `Strength`).
4. (Düşman tarafı için) `IntentType` ve `EnemyTurnRoutine`'i de güncelle.
