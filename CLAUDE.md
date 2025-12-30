# Looter Shooter - Claude Code Configuration

## Project Overview

- **Unity Version:** 6000.3.2f1 (Unity 6)
- **Genre:** First-person looter shooter
- **Status:** Early prototype

## Project Structure

```
Assets/Scripts/
├── Common/       # Shared interfaces (IDamageable, IInteractable, ILocomotion, PlayerReference)
├── Player/       # Movement, camera, weapon, interaction, inventory
├── Enemy/        # Enemy AI, health, locomotion, BehaviorTree/
├── Item/         # Pickups and item data
├── Weapons/      # Weapon system and projectiles
└── UI/           # Inventory display, weapon UI, crosshair
```

## Critical Design Rules

**1. NEVER use OnGUI() for UI**
- Use Canvas + TextMeshPro for all UI elements

**2. NEVER use FindGameObjectWithTag() or GameObject.Find()**
- Use `PlayerReference.Instance` for player access
- Use SerializeField references or events for other objects

**3. NEVER call GetComponent in Update loops**
- Cache component references in Awake/Start

**4. Shared interfaces go in root namespace**
- `IDamageable`, `IInteractable` belong in `LooterShooter` namespace
- Place them in `Assets/Scripts/Common/`

**5. Use event-driven UI updates**
- UI subscribes to events, never polls state
- Always unsubscribe in OnDestroy()

## Architecture Patterns

- **Namespaces:** `LooterShooter` (root), `LooterShooter.Player`, `LooterShooter.Enemy`, `LooterShooter.Item`, `LooterShooter.UI`, `LooterShooter.Weapons`
- **Singletons:** `Inventory.Instance`, `PlayerReference.Instance`
- **Events:** `Inventory.OnInventoryChanged`, `Weapon.OnFire`, `Weapon.OnReload`, `Weapon.OnAmmoChanged`

## Locomotion System

Enemy movement is handled by locomotion components that implement `ILocomotion`. Behavior tree actions delegate movement to these components.

### Interface

```csharp
public interface ILocomotion
{
    bool HasArrived { get; }
    bool IsMoving { get; }
    float Speed { get; set; }
    float StoppingDistance { get; set; }
    void SetDestination(Vector3 destination);  // Move to static point
    void SetTarget(Transform target);          // Track moving target
    void Stop();
}
```

### Locomotion Types

| Component | Use Case | Movement |
|-----------|----------|----------|
| `GroundLocomotion` | Ground enemies | XZ plane only, instant rotation |
| `FlyingLocomotion` | Flying enemies | Full 3D, smooth rotation |

### Enemy Setup

```
Enemy GameObject
├── Enemy (component)
├── GroundLocomotion OR FlyingLocomotion (component)
└── BehaviorGraphAgent (component)
```

Movement speed and stopping distance are configured on the locomotion component, not in the behavior tree.

## Unity Behavior Package

Scripts in `Assets/Scripts/Enemy/BehaviorTree/`.

### Single Responsibility Principle

Each action node performs exactly ONE atomic action. The behavior tree graph handles composition.

**Do:** `MoveToPositionAction` - only moves | `FaceTargetAction` - only rotates | `WaitAction` - only waits

**Don't:** `PatrolAction` that picks points, moves, rotates, AND waits

### Creating Nodes

**Condition:**
```csharp
[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "My Condition", story: "[Agent] checks something", category: "Conditions", id: "unique-guid")]
public partial class MyCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    public override bool IsTrue() { return true; }
}
```

**Action:**
```csharp
[Serializable, GeneratePropertyBag]
[NodeDescription(name: "My Action", story: "[Agent] does something", category: "Action/Enemy", id: "unique-guid")]
public partial class MyAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    protected override Status OnStart() { return Status.Running; }
    protected override Status OnUpdate() { return Status.Success; }
}
```

### Available Actions

| Action | Purpose | Blackboard Vars |
|--------|---------|-----------------|
| `MoveToPositionAction` | Moves agent to static point (uses ILocomotion) | TargetPosition |
| `ChasePlayerAction` | Chases player, tracks live position (uses ILocomotion) | ChaseRange |
| `FaceTargetAction` | Rotates to face target | TargetPosition |
| `PickRandomPatrolPointAction` | Picks random point | PatrolRadius, TargetPosition (out), Is3DPatrol |
| `WaitAction` | Waits for duration | Duration |
| `DealDamageAction` | Deals damage to player | Damage |
| `SetTargetToPlayerAction` | Sets target to player position (one-time) | TargetPosition (out) |
| `CheckPlayerInRange` | Condition: player in range | Range |

### Behavior Tree Structure

```
Start (Repeat)
└── Try In Order (Selector)
    ├── Attack Branch (CheckPlayerInRange → AttackRange)
    │   └── Sequence: SetTargetToPlayer → FaceTarget → DealDamage
    ├── Chase Branch (CheckPlayerInRange → ChaseRange)
    │   └── ChasePlayer (tracks player live, stops if out of range)
    └── Patrol Branch (fallback)
        └── Sequence: PickRandomPatrolPoint → MoveToPosition → Wait
```

Attack must come BEFORE chase (higher priority = checked first).

**Note:** Use `Abort If` nodes to interrupt lower-priority branches when higher-priority conditions become true. Do NOT use `Restart If` (causes spinning).

## File Handling

- Scene files (.unity) and prefabs (.prefab) are binary - avoid editing directly
- Meta files are auto-generated - include in version control
