# Placeholder Art Replacement Guide

This project now separates **visual nodes** from **interaction nodes** for key NPCs and interactable objects.

## How to replace placeholder art
1. Open a scene in Godot (for example `computer_lab.tscn`, `innovation_hub.tscn`, `dance_party.tscn`, or `it_ball.tscn`).
2. Select the object or NPC root (for example `ComputerLabPC`, `InnovationHubDeveloperStudentNPC`, `DancePartyClassmateNPC`).
3. Assign your PNG to the `Visual` child (replace the `ColorRect` with `Sprite2D` if desired).
4. Keep `InteractionArea/CollisionShape2D` for interaction range.
5. Keep `PromptPosition` (if present) to control where prompts appear near the object/NPC.

> Do not delete `InteractionArea` or its `CollisionShape2D`, or interaction prompts and `E` actions will break.
