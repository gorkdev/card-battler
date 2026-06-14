# Editör Kurulumu

Sahneyi sıfırdan kurma adımları ve referans bağlama listesi. Sahne: `Assets/Scenes/SampleScene`.

## Sahne hiyerarşisi (hedef)
```
SampleScene
├─ Main Camera            (Camera Fitter)
├─ Global Light 2D
├─ Background
├─ Player                 (Combatant View)
│  ├─ Art                 (Sprite Renderer + Animator: idle)
│  └─ Health Bar
│     └─ Bar BG
│        └─ Health Fill   (x ölçeği 0..1 doldurur)
│           └─ Fill
├─ Enemy (Goblin)         (Combatant View)
│  ├─ Art
│  ├─ Health Bar ...
│  └─ Intent              (Intent View)
├─ Hand                   (Hand View)         -> kart prefab'ını dizer
├─ BattleManager          (Battle Manager)
├─ EventSystem                                -> sürükle-bırak için ŞART
└─ HUD Canvas             (Screen Space, Canvas Scaler)
   ├─ Energy Text         (Energy View)
   ├─ End Turn Button     (End Turn Button)
   └─ Result Panel        (Battle Result View)
```

## Kritik genel ayarlar
- **Run In Background:** Project Settings → Player → Resolution and Presentation →
  **Run In Background** açık (oyun odağı kaybedince durmasın).
- **Sürükle-bırak için:**
  - Sahnede bir **EventSystem** olmalı (yeni Input System modülüyle).
  - **Main Camera**'da **Physics 2D Raycaster** bileşeni olmalı.
  - Kart prefab'ının kökünde bir **Collider2D** (kart alanını kaplayan BoxCollider2D) olmalı.

## Can barı hilesi (soldan dolması için)
`Health Fill` (boş parent) barın **sol kenarına** konur; `Fill` sprite'ı onun çocuğu
olup **sol kenarı parent'ın pivotuna oturacak** şekilde sağa ötelenir. Böylece
`Health Fill`'in X ölçeği 0→1 olunca bar **soldan** dolar.
Doğrulama: `Health Fill` Scale X = 0.5 → bar sadece soldan yarıya inmeli.

## Boyutlandırma (responsive)
- `Camera Fitter → Target Width` ekranın kaç dünya birimi göstereceğini belirler (≈12).
- Bir objenin ekran boyutu = dünya boyutu ÷ Target Width.
- Karakterleri ~2.5 birim yüksekliğe ölçekle (Art Scale veya import PPU ile). Player ve
  Goblin'de **aynı yöntemi** kullan.

## BattleManager referans bağlama listesi
`BattleManager` seçiliyken Inspector'da şunları doldur:
- [ ] **Starting Deck** → kart asset'leri (Attack, Block, Heal, Poison...) — tekrar
      ekleyerek deste büyütülebilir
- [ ] **Enemy Definition** → `Goblin.asset`
- [ ] **Player View** → Player objesi (Combatant View)
- [ ] **Enemy View** → Enemy objesi (Combatant View)
- [ ] **Hand View** → Hand objesi (Hand View)
- [ ] **Intent View** → Goblin'in Intent objesi (Intent View)
- [ ] **Energy View** → HUD'daki Energy Text (Energy View)
- [ ] **End Turn Button** → HUD'daki buton (End Turn Button)
- [ ] **Result View** → Result Panel (Battle Result View) — opsiyonel

> Eksik referans varsa oyun başında Console'a `[BattleManager] Eksik referans...` hatası
> düşer; hangisini unuttuğunu oradan görürsün.

## CombatantView bağlama (her karakter için)
- [ ] **Artwork Renderer** → Art (Sprite Renderer)
- [ ] **Health Fill** → Health Fill (boş parent)
- [ ] **Health Text** → can yazısı (opsiyonel)
- [ ] **Block Root / Block Text**, **Poison Root / Poison Text** (opsiyonel göstergeler)

## CardView bağlama (prefab içinde)
- [ ] **Name Text** → Card Name (TMP)
- [ ] **Description Text** → Description (TMP)
- [ ] **Cost Text** → enerji maliyeti yazısı (TMP)
- [ ] **Illustration** → Illustration (Sprite Renderer)
