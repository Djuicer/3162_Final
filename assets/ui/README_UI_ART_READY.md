# UI Art-Ready Guide

This project is set up so you can replace UI visuals in Godot without rewriting scripts.

## Shared theme
- Theme resource: `res://assets/ui/theme/default_ui_theme.tres`
- Apply/update shared styles for:
  - `Button`
  - `Label`
  - `PanelContainer` / `Panel`

## Panel/frame texture slots (assign later)
Use `NinePatchRect` nodes named `ArtBackground` to assign frame textures later.
- `scenes/UI/StatusOverlay.tscn` → `StatusPanel/ArtBackground`
- `scenes/Screen/screen.tscn` → `ComputerInfoPanel/ArtBackground`
- `scenes/Computer/Email/email.tscn` → `EmailPanel/ArtBackground`
- `scenes/Events/DanceParty/dance_party.tscn` → `UI/DialoguePanel/ArtBackground`
- `scenes/Events/ITBall/it_ball.tscn` → `UI/DialoguePanel/ArtBackground`
- `scenes/Ending/ending.tscn` → `CenterContainer/EndingPanel/ArtBackground`

> Assign UI frame texture here later (NinePatchRect texture + patch margins).

## Decorative nodes and mouse safety
Keep decorative overlays as `MouseFilter = Ignore` so they do not block clicks:
- Any `ArtBackground` node.
- `StatusOverlay/FloatingTextLayer`.
- Non-interactive tint/overlay nodes.

## Buttons: where to reskin
- Preferred: style `Button` in `default_ui_theme.tres`.
- Optional: replace specific buttons with `TextureButton` in editor if desired.
- Important button groups:
  - Computer screen (`EmailButton`, `BrowserButton`, `ReturnButton`)
  - Email panel (`BackButton`, event buttons)
  - Dorm travel menu buttons
  - Dialogue choice buttons
  - Minigame continue buttons
  - Ending `PlayAgainButton`
