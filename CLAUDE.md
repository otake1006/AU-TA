# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

AU-TA is a Unity-based card battle game built with C#. The project follows a manager-based architecture with event-driven communication between systems. The game features turn-based combat with card mechanics, buff/debuff systems, and AI opponents.

## Unity Version & Development Environment

- Unity Version: 6000.1.15f1 (Unity 6)
- Language: C# 
- Platform: Windows (win32)
- Solution File: `AU-TA.sln`
- Main Assembly: `Assembly-CSharp.csproj`

## Key Development Commands

Since this is a Unity project, development primarily happens within the Unity Editor. However, for code analysis and debugging:

```bash
# Open Unity project (if Unity is in PATH)
unity -projectPath "D:\Users\Lpond\Desktop\AU-TA"

# Build project (requires Unity command line tools)
unity -batchmode -quit -projectPath "D:\Users\Lpond\Desktop\AU-TA" -buildTarget StandaloneWindows64
```

## Architecture Overview

### Core Managers (Singleton Pattern)
- **GameManager**: Central singleton managing game states, scene transitions, and high-level coordination
- **BattleManager**: Controls battle flow, state transitions, and combat sequences
- **RoundManager**: Manages individual rounds within matches
- **TurnManager**: Handles turn-based gameplay logic
- **CardManager**: Controls card mechanics and hand management

### Main Systems
1. **Character System**: `Character.cs` with stats, health, mana management
2. **Card System**: Base cards, skill cards, conditional cards with effects
3. **Buff/Debuff System**: Turn-based status effects (attack boost, defense boost, poison)
4. **AI System**: Strategic AI for enemy decision making
5. **UI System**: Game UI, health bars, mana bars, card displays
6. **Audio System**: Background music, sound effects, voice management

### Event System
The project uses a centralized event system through `GameEvents.cs`:
- `OnMatchEnd`: Match completion events
- `OnCharacterDeath`: Character defeat events  
- `OnBattleStateChanged`: Battle state transitions
- `OnCardUsed`: Card usage notifications
- Debug events for logging

### Key File Locations
```
Assets/Scripts/NewProgram/
├── Core/           # GameManager, GameConfig, GameEvents, Constants
├── Battle/         # BattleManager, RoundManager, TurnManager
├── Cards/          # Card system and mechanics  
├── Characters/     # Character data and stats
├── AI/             # Enemy AI strategy
├── UI/             # User interface components
├── Audio/          # Audio management
├── Buffs/          # Status effect system
├── Input/          # Input handling
└── Utils/          # Utility classes and extensions
```

## Configuration System

The game uses `GameConfig` ScriptableObject for centralized configuration:
- Match settings (wins needed, max turns)
- Card settings (hand size, draw rates)
- Character settings (mana regeneration)
- Debug settings (debug mode, verbose logging)
- UI/Audio settings (volumes, animation speeds)
- Balance settings (damage multipliers, critical hits)

## Game States & Flow

### GameState Enum
- `MainMenu`: Main menu navigation
- `Battle`: Active combat
- `Paused`: Game paused
- `GameOver`: Match completed
- `Settings`: Configuration screens
- `Loading`: Asset loading

### BattleState Flow
1. `Initializing`: Setting up battle
2. `PlayerTurn`: Player's turn to act
3. `EnemyTurn`: AI's turn to act  
4. `GameOver`: Battle concluded

## Debug Features

When `GameConfig.enableDebugMode` is true:
- Debug logging for state transitions
- Force round end functionality
- Skip turn capabilities  
- Current state inspection methods

## Common Development Patterns

1. **Manager Initialization**: Managers auto-create missing components and initialize in proper order
2. **Event-Driven Communication**: Use `GameEvents` for loose coupling between systems
3. **Coroutine Safety**: Battle sequences use coroutine cancellation for safe state transitions
4. **State Management**: Clear state transitions with validation and logging
5. **Japanese Comments**: Original code contains Japanese comments for game logic

## Important Notes

- The project uses Unity's new Input System
- Characters and game data are stored as ScriptableObjects
- Battle system prevents duplicate initialization and handles cleanup properly
- UI components are modular and manager-controlled
- Audio system supports BGM, SFX, and voice with separate volume controls