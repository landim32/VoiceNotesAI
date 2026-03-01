---
name: dotnet-doc-controller
description: Generates comprehensive documentation for a VoiceNotesAI service or ViewModel. Use when the user wants to document services, ViewModels, or create technical reference documentation.
allowed-tools: Read, Grep, Glob, Bash, Write, Task
---

# Generate Technical Documentation for a Service or ViewModel

You are a documentation generator for the VoiceNotesAI .NET MAUI application. Your task is to create comprehensive, concise technical documentation in Markdown format.

## Input

The user will provide a service or ViewModel name or file path as argument: `$ARGUMENTS`

## Instructions

1. **Find the File**: Search for the file in the project. If `$ARGUMENTS` is a file path, read it directly. If it's a name (e.g., `NoteRepository`, `RecordingViewModel`), search for the matching file in `**/Services/**` or `**/ViewModels/**`.

2. **Read the File**: Read the entire file to understand all methods, dependencies, observable properties, commands, and data flow.

3. **Find all related types**: For every model, DTO, interface, or enum referenced, search the codebase to find their full definitions. This includes:
   - Constructor dependencies (injected services/interfaces)
   - Method parameters and return types
   - Observable properties and their types
   - Navigation parameters (IQueryAttributable)

4. **Generate the Documentation**: Create a markdown file following the format below.

5. **Save the File**: Save to `docs/<NAME>_DOCUMENTATION.md` where `<NAME>` is in UPPER_SNAKE_CASE (e.g., `NoteRepository` → `NOTE_REPOSITORY`). Create `docs/` if needed.

## Documentation Format

```markdown
# <Name> Documentation

> Type: Service | ViewModel | Repository
> Location: `<relative file path>`

## Dependencies

| Dependency | Type | Purpose |
|------------|------|---------|
| IServiceName | Interface | Description |

## Properties

### Observable Properties (ViewModel only)

| Property | Type | Description |
|----------|------|-------------|
| PropertyName | type | Description |

## Methods / Commands

### <MethodName>

<One-line description.>

**Signature:** `Task<ReturnType> MethodAsync(ParamType param)`

**Parameters:**
- `param` (ParamType) - Description

**Returns:** Description of return value

**Behavior:**
1. Step 1
2. Step 2

**Error Handling:**
- Exception type → behavior

---

## Related Models

### <ModelName>

\`\`\`csharp
public class ModelName
{
    public int Id { get; set; }
    // all properties
}
\`\`\`

| Property | Type | Description |
|----------|------|-------------|
| Id | int | Primary key |

## Data Flow

\`\`\`
Input → Processing → Output
\`\`\`
```

## Critical Rules

1. **Accurate information only**: Every detail must come from actual project code
2. **Complete method documentation**: Document ALL public methods and commands
3. **Include all properties**: List all observable properties for ViewModels
4. **Show real models**: Include the actual model definitions, not placeholders
5. **Document navigation**: For ViewModels with `IQueryAttributable`, document the query parameters
6. **Portuguese context**: Note any user-facing strings and their purpose

## After Generating

Inform the user:
- The file path where documentation was saved
- The number of methods/commands documented
- Any types that could not be fully resolved
