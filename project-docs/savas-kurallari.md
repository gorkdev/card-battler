# Savaş Kuralları

Tek savaş: oyuncu (sol) vs. düşman/Goblin (sağ). Tüm akışı `BattleManager` yürütür.

## Sayısal varsayılanlar
| Ayar | Varsayılan | Nerede |
|---|---|---|
| Oyuncu canı | 50 | `BattleManager.playerMaxHealth` |
| Tur başına enerji | 3 | `BattleManager.energyPerTurn` |
| Tur başına çekilen kart | 5 | `BattleManager.cardsPerTurn` |
| Düşman canı | 30 | `Goblin.asset → maxHealth` |
| Düşman eylem gecikmesi | 0.6 sn | `BattleManager.enemyActionDelay` |

> Bu değerler asset/Inspector'dan değiştirilebilir; kodda sabit değil.

## Tur döngüsü

### Savaş başı (`StartBattle`)
- Oyuncu ve düşman `Combatant` nesneleri yaratılır (canlar dolu).
- Deste `startingDeck`'ten kurulup karıştırılır.
- Düşman ilk niyetini gösterir.

### Oyuncu turu başı (`StartPlayerTurn`)
1. Oyuncunun **bloğu sıfırlanır** (`ResetBlock`).
2. Oyuncunun **zehri tıklar** (`TickPoison`) — varsa hasar alır.
3. Enerji **yenilenir** (3'e set edilir).
4. Desteden **5 kart çekilir**, el yelpazesi dizilir.

### Kart oynama (`PlayCard`)
- Yalnızca oyuncu turundaysa ve **enerji yetiyorsa** oynanabilir (`CanPlay`).
- Enerji, kartın maliyeti kadar düşer.
- Kartın `effects` listesindeki her etki uygulanır:
  - `Attack`  → düşmana hasar (`Enemy.TakeDamage`)
  - `Poison`  → düşmana zehir (`Enemy.ApplyPoison`)
  - `Block`   → oyuncuya blok (`Player.GainBlock`)
  - `Heal`    → oyuncuya can (`Player.Heal`)
- Kart atma yığınına gider, elden kaldırılır.

### Tur bitişi (`EndTurn` → `EnemyTurnRoutine`)
1. Kalan el atılır (`DiscardHand`).
2. Kısa bekleme (his için).
3. Düşmanın **bloğu sıfırlanır**, **zehri tıklar**.
4. Düşman mevcut niyetini uygular:
   - `Attack` → oyuncuya hasar (oyuncunun bloğunu aşan kısım cana işler)
   - `Block`  → düşman kendine blok kazanır
5. Düşman **bir sonraki niyete** geçer (desende döngüsel).
6. Sıra tekrar oyuncuya döner.

## Durum efektleri

### Blok (geçici zırh)
- Gelen hasarı emer; önce blok düşer, kalan cana işler.
- **Sahibinin tur başında sıfırlanır** (kalıcı değil).

### Zehir (DoT)
- **Sahibinin tur başında** yığın kadar hasar verir; bu hasar **bloğu yok sayar**.
- Her tıklamadan sonra yığın **1 azalır** (ör. 4 → 3 → 2 → 1 → 0).

## Kazanma / kaybetme
- **Düşman canı 0** → oyuncu kazanır → `BattleResultView` "Kazandın!"
- **Oyuncu canı 0** → oyuncu kaybeder → "Kaybettin!"
- Bu kontrol `Combatant.Died` olayıyla otomatik tetiklenir (zehirle ölüm dahil).
