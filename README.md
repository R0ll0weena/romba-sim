# Romba Sim

A robot vacuum cleaner simulation game with a twist, built in Unity.

This is a hobby project and part of my portfolio. I use it to practice and show off gameplay programming, VFX, shaders, and asset pipeline work in Unity.

> **The twist:** _TODO: describe the twist here._

## Gameplay

You drive a robot vacuum around a house and clean up dirt before time runs out.

- **Cleaning:** Dirt is scattered randomly across the room's floors at the start of a round (`ScatterObjects`). Driving over dirt collects it (`Collectable`).
- **Win and lose:** Collect all the dirt to win. If the countdown (`GameOverTimer`) reaches zero first, you lose. A game-over screen lets you restart (`GameOverUI`, `GameOverManager`).
- **Movement:** Physics-based top-down WASD movement using the Unity Input System (`PlayerMovement`).
- **Damage and destruction:** The house reacts when you bump into things:
  - Hard impacts damage the robot in stages, and it eventually catches fire with smoke, sound, and a charred material (`DamageDetector`, `Burn`).
  - Fragile objects such as mugs shatter into pieces (`Shatter`).
  - A boombox knocked to the floor starts playing a broken radio track (`FallDetector`, `Timer`).
- **2D mode:** A top-down 2D version with a cat that reacts when you hit it (`CatHit`).

## Tech stack

- **Engine:** Unity 6 (`6000.0.28f1`) with the Universal Render Pipeline (URP)
- **Packages:** Input System, Cinemachine, Splines, Shader Graph, Timeline, Post Processing, Recorder, TextMeshPro
- **Tools:** Blender for preparing room and furniture models

## Project structure

| Path | Contents |
| --- | --- |
| `Assets/_Unity Essentials/Scenes/` | Game scenes: main menu, kids room, kitchen, living room, and the top-down 2D scene |
| `Assets/_Unity Essentials/Prefabs/`, `Rooms/` | Room, character, and object prefabs |
| `Assets/*.cs` | Core gameplay scripts: movement, dirt, timer, and game over |
| `Assets/Scripts/` | Damage, burn, shatter, and fall-detection behaviours |
| `Assets/GogoGaga/` | Experiments with A* grid pathfinding, splines, and ropes/cables |
| `Assets/Shader Learning/` | Hologram shaders, both hand-written HLSL and Shader Graph |
| `Assets/Portfolio/` | Portfolio pieces, such as a dual-UV Shader Graph and an optimized URP mask (MOS) shader |
| `Assets/EditorScripts/` | Custom editor tools, such as **Custom > Scale Collider** |
| `Assets/Blender Scripts/` | Python scripts for Blender, such as combining meshes from a house interior asset pack |

## Getting started

1. Clone the repository.
2. Open the project folder in **Unity Hub** with Unity `6000.0.28f1` (or a compatible Unity 6 version).
3. Open `Assets/_Unity Essentials/Scenes/0_MainMenu_Scene.unity` and press **Play**.

The scenes in the build are:

1. `0_MainMenu_Scene`
2. `5_TopDown_2D_Scene`
3. `4_LivingRoom_Programming_Scene`

### Controls

| Action | Input |
| --- | --- |
| Move | `W` `A` `S` `D` |

## Blender scripts

Some assets are prepared in Blender before import. To run the mesh-combining script headless:

```sh
blender --background --python "Assets/Blender Scripts/combine_blend_meshes.py"
```

The input and output paths are set at the top of the script.

## Credits

The project started from Unity Learn's **Unity Essentials** pathway and has grown from there. It also uses these third-party assets:

- Bezier Solution (`Assets/Plugins/BezierSolution`)
- Optimized Ropes and Cables (`Assets/GogoGaga/OptimizedRopesAndCables`)
- LED Light Blocks (`Assets/Tomerinio`)
- Free kitchen and food asset packs (`Assets/Download tests`)
- Ultimate House Interior Pack (used with the Blender scripts)
- Music: "Retro Arcade Game Music" (`Assets/Sounds`)

All rights to third-party assets belong to their respective authors.
