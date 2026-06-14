# Card Battler — Project Documentation

My character on the left, an enemy (Goblin) on the right. I play my cards (skills) via
drag-and-drop to defeat the enemy. A **turn-based deckbuilder** card game.

This folder exists so that when I reopen the project later, I can quickly recall
"what does what."

> Note: the README is in English; the other docs are written in Turkish.

## Document index

| File | Contents |
|---|---|
| [mimari.md](mimari.md) | The code's 3-layer structure (data / model / view) and why |
| [scriptler.md](scriptler.md) | What each script does and its important fields |
| [savas-kurallari.md](savas-kurallari.md) | Turn loop, energy, block, poison, win/lose rules |
| [editor-kurulumu.md](editor-kurulumu.md) | Steps to build the scene + reference-wiring checklist |
| [icerik-ekleme.md](icerik-ekleme.md) | How to add new cards / enemies (no coding) |

## Tech stack

- **Unity:** 6000.4.4f1 (Unity 6)
- **Render:** Universal Render Pipeline (URP) 2D
- **Input:** Input System (new). Drag-and-drop requires an **EventSystem** in the scene
  and a **Physics 2D Raycaster** on the camera.
- **Text:** TextMeshPro (world-space 3D text on cards and indicators)

## Folder structure

```
Assets/
  Scripts/
    Data/     -> Card and enemy definitions (ScriptableObject)
    Model/    -> Pure C# game logic (engine-independent)
    Combat/   -> BattleManager (the brain of the battle)
    View/     -> Components that render the screen / handle input
  Data/       -> Generated card & enemy assets (Goblin, Attack, Block, Heal, Poison)
  Art/        -> Pixel art (Hero Knight, etc.) and animations
  Sprites/    -> pixelCardAssest.png (card frames, icons)
  Scenes/     -> SampleScene (the main battle scene)
project-docs/ -> These documents
```

## Architecture in one breath

Three layers, deliberately separated so battle rules stay independent of the engine:

- **Data** (ScriptableObject): cards & enemies live as assets, created via right-click →
  Create. No code needed to add content.
- **Model** (pure C#): `Combatant` (health/block/poison) and `BattleDeck` (deck). Knows
  nothing about Unity → unit-testable.
- **Logic** (`BattleManager`): the conductor — runs the turn loop, energy, win/lose.
- **View** (MonoBehaviours): subscribe to model events (`Changed`, `Damaged`, `Died`)
  and draw the screen / take input.

See [mimari.md](mimari.md) for the full picture and a sample data flow.

## Current status (roadmap)

1. ✅ Scripts written (data / model / logic / view)
2. ✅ Data assets created (Goblin + 4 cards)
3. ✅ Player set up (health bar + CombatantView + Hero Knight idle animation)
4. ⏳ Goblin scene setup
5. ⏳ Card prefab + drag-and-drop plumbing (EventSystem, Physics 2D Raycaster)
6. ⏳ HUD (energy, End Turn, Win/Lose) — responsive Canvas
7. ⏳ Wire up BattleManager + first gameplay test
8. ⏳ Polish (animation transitions, audio, visual effects)
