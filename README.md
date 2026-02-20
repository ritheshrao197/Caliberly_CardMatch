# CarDish-Match The Dishes,Master the Memory - Memory Card Match (Unity 2021 LTS)

Lightweight, event-driven memory card matching game built with Unity **2021.3.45f1**.

## Overview

This project focuses on clean gameplay code, scalable UI handling, and persistent player progress.
Core game flow is coordinated through an internal event bus and a thin composition root.

## Features

- Card matching gameplay with flip/match/mismatch flow
- Flexible board sizes via level definitions (for example: 2x2, 2x3, 5x6)
- Dynamic board generation and runtime teardown
- Score + combo tracking (`ScoreTracker`)
- Timer and move-limit based rules (`LevelRules`, `TimerService`)
- UI panel system with typed panels (`PanelType`) and fade transitions (`UIPanel`)
- Home, HUD, Pause, Result, and Level Select screens
- JSON-based persistence for:
  - highest unlocked level
  - SFX on/off
  - music on/off
- Audio events for gameplay and UI interactions (`AudioManager`)

## Architecture (Current)

### Composition Root
- `Assets/Scripts/GameManager.cs`
  - Validates scene references
  - Composes runtime collaborators
  - Enables/disables flow coordinator

### Runtime Layer
- `Assets/Scripts/Application/GameFlowCoordinator.cs`
  - Owns event subscriptions for game flow
  - Handles start/restart/home/pause/resume/win/loss transitions
- `Assets/Scripts/Application/LevelSessionController.cs`
  - Owns level start/restart/stop session logic
  - Triggers board build/clear and timer lifecycle
- `Assets/Scripts/Application/BoardRuntime.cs`
  - Owns board setup/teardown through `BoardController`

### Services
- `Assets/Scripts/Services/MatchResolver.cs` - selection + pair resolution
- `Assets/Scripts/Services/ScoreTracker.cs` - score/combo updates and score events
- `Assets/Scripts/Services/ProgressService.cs` - progress boundary implementation
- `Assets/Scripts/Services/SettingsStorage.cs` - JSON storage backend
- `Assets/Scripts/Services/TimerService.cs` - elapsed time control

### UI
- `Assets/Scripts/UI/UIPanel.cs` - base panel + fade animation behavior
- `Assets/Scripts/UI/UIManager.cs` - state and popup coordination
- `Assets/Scripts/UI/*.cs` - concrete views/widgets

## Project Structure

```text
Assets/
  Configs/                 ScriptableObject configs (game/level/card data)
  Scenes/
    GamePLay.unity         Main gameplay scene
  Scripts/
    Application/           Runtime orchestration classes
    Controllers/           Board/card/audio/level rule controllers
    Data/                  Config/data models
    Events/                Event bus + event contracts
    Services/              Stateful/stateless gameplay services
    UI/                    Panels/views/ui utilities
    Utilities/             Common helpers (pooling, input lock, etc.)
```

## Requirements

- Unity **2021.3.45f1** (LTS)
- TextMeshPro package (included in standard Unity setup)

## Getting Started

1. Clone the repository.
2. Open the project in Unity **2021.3.45f1**.
3. Open scene: `Assets/Scenes/GamePLay.unity`.
4. Press Play.

## Build

1. `File -> Build Settings`
2. Add `Assets/Scenes/GamePLay.unity` to build scenes.
3. Choose target platform (Desktop recommended first).
4. Build / Build and Run.

## Persistence

Runtime settings and progress are stored in:

- `Application.persistentDataPath/settings.json`

Stored keys:
- `sfxEnabled`
- `musicEnabled`
- `highestLevelIndex`

## Controls

- Click/Tap card: flip/select card
- Home:
  - Start
  - Level Select
  - Reset Progress
  - Toggle SFX / Music
  - Exit
- HUD:
  - Pause
- Pause popup:
  - Resume / Restart / Home

## Notes

- `MatchService` currently exists as a compatibility wrapper around `MatchResolver`.
