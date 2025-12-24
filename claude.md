# Unity Project - Claude Code Configuration

## Project Overview
This is a 3D Unity game development project.

- **Unity Version:** 6000.3.2f1 (Unity 6)
- **Project Type:** 3D
- **Team Size:** Solo developer
- **Status:** Brand new project

## Project Structure
```
Assets/
├── Scripts/          # C# scripts for game logic
├── Scenes/           # Unity scene files
├── Prefabs/          # Reusable game objects
├── Materials/        # Materials and shaders
├── Textures/         # Texture assets
├── Audio/            # Sound effects and music
├── Animations/       # Animation clips and controllers
└── Resources/        # Runtime-loaded assets
```

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

### Common Patterns
- Use ScriptableObjects for shared data and configuration
- Implement event-driven systems with UnityEvents or custom events
- Follow the Component pattern - avoid monolithic scripts
- Use coroutines for time-based operations

### Architecture Notes
- No strict architecture enforced yet - keep it simple and iterate
- Focus on clean, readable code over complex patterns initially
- Refactor as patterns emerge naturally during development

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
