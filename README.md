# AI-Powered Cooperative Running Game

A cooperative 3D action prototype where the player and an AI companion each control separate characters. The player gives instructions to the AI companion via a chat interface, and they work together to run through the level.

The natural language processing pipeline starts with a rule-based keyword matching implementation, designed to be swapped out for a local LLM later. All conversion logic sits behind an interface for easy replacement. See [Docs/](./Docs) for the full design.

## Requirements

- Unity 2022.3.21f1

## Setup

1. Open this folder as an existing project in Unity Hub (first launch will take a while to generate packages and the Library folder)
2. Open the `Assets/Added player and AI charactor.unity` scene
3. If the scene is empty, run `Tools > AI Runner > Build Demo Scene` from the menu bar — this auto-generates the player, AI companion, camera, and chat UI

## Controls

- Move: `W` `A` `S` `D`
- Run: hold `Shift` while moving
- Jump: `Space`
- Give instructions to the AI companion: type in the chat box at the bottom-left of the screen and press Enter or click Send

### AI Companion Commands

| Input | Companion action |
|---|---|
| Follow me / Come here / Chase me | Follow the player |
| Stop / Halt | Stop moving |
| Wait / Stand by | Wait in place |
| Jump | Jump |
| Go right / Go left / Go forward / Go back | Move in that direction |

Unrecognised input gets the response: *"Sorry, I don't understand."*

## Directory Structure

```
Assets/
├── Scripts/
│   ├── Player/         # Player movement and input
│   ├── Companion/      # AI companion behaviour and NLP entry point
│   ├── Commands/       # NLP input/output interfaces and data structures
│   ├── UI/             # Chat UI
│   └── Camera/         # Third-person follow camera
├── Editor/
│   └── DemoSceneBuilder.cs  # Auto-builds the test scene
Docs/                        # Design documents
```

## Documentation

- [Docs/architecture.md](./Docs/architecture.md) — Component diagram and data flow
- [Docs/nlp-interface.md](./Docs/nlp-interface.md) — NLP input/output interface spec (including local LLM pipeline design)
- [Docs/conventions.md](./Docs/conventions.md) — Development rules and naming conventions
