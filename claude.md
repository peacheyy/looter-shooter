# Unity Project - Claude Code Configuration

## Project Overview
This is a 3D Unity game development project.

- **Unity Version:** 6000.3.2f1 (Unity 6)
- **Project Type:** 3D
- **Team Size:** Solo developer
- **Status:** Early prototype - core mechanics implemented
- **Genre:** First-person looter shooter

## Project Structure
```
Assets/
├── Scripts/
│   ├── Common/       # Shared interfaces (IDamageable, IInteractable, PlayerReference)
│   ├── Player/       # Movement, camera, weapon, interaction, inventory
│   ├── Enemy/        # Enemy AI and health
│   ├── Item/         # Pickups and item data
│   ├── Weapons/      # Weapon system and projectiles
│   └── UI/           # Inventory display, weapon UI, crosshair
├── Scenes/           # Scene1.unity (main scene)
├── Settings/         # URP rendering configuration
├── Input/            # InputSystem_Actions (new input system)
└── [Empty: Prefabs, Resources, Animations, Audio, Materials, Textures]
```

## Implemented Features
- First-person movement (WASD, sprint, jump, mouse look)
- Raycast-based shooting with fire rate limiting
- Enemy AI (follows player, takes damage, drops loot)
- Item system (random loot, bobbing/glowing pickups, E to interact)
- Inventory system (Tab to toggle, tracks items by type)

## Architecture Patterns
- **Namespaces:** LooterShooter (root for shared), LooterShooter.Player, LooterShooter.Enemy, LooterShooter.Item, LooterShooter.UI, LooterShooter.Weapons
- **Singleton:** Inventory.Instance for global access, PlayerReference.Instance for player access
- **Interfaces:** IDamageable (combat), IInteractable (pickups) - both in root LooterShooter namespace
- **Player discovery:** Use PlayerReference.Instance instead of tag-based lookups
- **Event system:** Inventory.OnInventoryChanged, Weapon events (OnFire, OnReload, OnAmmoChanged) for UI updates
- **Future consideration:** Migrate Singleton to ScriptableObject-based architecture if scope expands beyond single-player (co-op, unit testing, multiple inventories)

## Development Guidelines

### Code Style
- Use C# naming conventions (PascalCase for public members, camelCase for private)
- Keep MonoBehaviour scripts focused and single-responsibility
- Use SerializeField for inspector-visible private fields
- Add XML documentation comments for public APIs

### Unity Best Practices
- Avoid using GameObject.Find() in Update/FixedUpdate loops
- Cache component references in Awake() or Start()
- Use object pooling for frequently instantiated objects
- Implement proper cleanup in OnDestroy()
- Use layers and tags appropriately for organization

### Critical Design Rules (MUST FOLLOW)

**1. NEVER use OnGUI() for UI rendering**
- OnGUI is deprecated, slow, and creates garbage every frame
- ALWAYS use Canvas + TextMeshPro for all UI elements
- Reference: WeaponUI.cs for the correct pattern

**2. NEVER use FindGameObjectWithTag() or GameObject.Find()**
- These are fragile (magic strings) and slow
- Use PlayerReference.Instance for player access
- For other objects, use SerializeField references or events
- If you need global access, create a singleton like PlayerReference

**3. Shared interfaces go in root namespace**
- Interfaces used across multiple domains (IDamageable, IInteractable) belong in `LooterShooter` namespace
- Place them in `Assets/Scripts/Common/`
- Domain-specific interfaces stay in their domain namespace

**4. Use event-driven UI updates**
- UI components subscribe to events, not poll state
- Use C# events (Action, Action<T>) for loose coupling
- Always unsubscribe in OnDestroy()

**5. Cache frequently accessed references**
- Cache Camera.main in Awake/Start
- Cache component references (GetComponent calls)
- Never call GetComponent in Update loops

### Common Patterns
- Use ScriptableObjects for shared data and configuration
- Implement event-driven systems with UnityEvents or custom events
- Follow the Component pattern - avoid monolithic scripts
- Use coroutines for time-based operations

### Architecture Notes
- No strict architecture enforced yet - keep it simple and iterate
- Focus on clean, readable code over complex patterns initially
- Refactor as patterns emerge naturally during development

## Unity Behavior Package

This project uses **Unity Behavior** for AI behavior trees. Scripts are in `Assets/Scripts/Enemy/BehaviorTree/`.

### Creating Conditions vs Actions

**Conditions** (for branching logic):
```csharp
using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "My Condition", story: "[Agent] checks something", category: "Conditions", id: "unique-guid-here")]
public partial class MyCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    public override bool IsTrue() { return true; }
    public override void OnStart() { }
    public override void OnEnd() { }
}
```

**Actions** (for executing behavior):
```csharp
using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "My Action", story: "[Agent] does something", category: "Action/Enemy", id: "unique-guid-here")]
public partial class MyAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    protected override Status OnStart() { return Status.Running; }
    protected override Status OnUpdate() { return Status.Success; }
    protected override void OnEnd() { }
}
```

**Key differences:**
- Conditions use `[Condition(...)]` attribute, Actions use `[NodeDescription(...)]`
- Conditions return `bool` via `IsTrue()`, Actions return `Status` enum
- Both need `using LooterShooter;` to access `PlayerReference` and `IDamageable`

### Enemy Behavior Tree Structure

Standard enemy AI pattern using Selector (Try In Order):
```
Start (Repeat)
└── Try In Order (Selector) - runs first successful branch
    ├── [1] Attack Branch (highest priority, smallest range)
    │   └── Conditional Guard (CheckPlayerInRange → AttackRange)
    │       └── Attack Player Action
    │
    ├── [2] Chase Branch (medium priority)
    │   └── Conditional Guard (CheckPlayerInRange → ChaseRange)
    │       └── Chase Player Action
    │
    └── [3] Idle/Patrol Branch (fallback when player out of range)
        └── Wait or Patrol Action
```

**Important:** Attack check must come BEFORE chase (higher priority = checked first).

## File Handling
- Scene files (.unity) and prefabs (.prefab) are binary - avoid editing directly
- Meta files are auto-generated - include in version control
- Use .gitignore for Library/, Temp/, and Logs/ folders

## Testing
- Write unit tests in Assets/Tests/
- Use Unity Test Framework for editor and play mode tests
- Test critical gameplay mechanics and edge cases

## Notes for Claude Code
- This project uses Unity 6 (6000.x) - reference updated API documentation
- When creating new scripts, include proper namespaces
- Suggest performance optimizations when relevant
- Be mindful of Unity's component lifecycle (Awake, Start, Update, etc.)
- Unity 6 has improved ECS (Entities) support if needed for performance-critical systems
- Focus on clarity and maintainability for solo development
