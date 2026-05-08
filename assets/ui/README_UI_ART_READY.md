# UI Art-Ready Guide

## Art-ready scenes/components updated
- `res://scenes/UI/StatusOverlay.tscn`
- `res://scenes/UI/FloatingStatPopup.tscn`
- `res://scripts/ui/StatusOverlay.cs`
- `res://scripts/ui/FloatingStatPopup.cs`

## Where to assign future panel/background art
- **Status panel frame**: `StatusOverlay/StatusPanel/ArtBackground` (`NinePatchRect`)
- **Reaction panel frame**: `StatusOverlay/ReactionPanel` (`PanelContainer` style/theme)
- **Floating popup icon slot**: `FloatingStatPopup/IconTextureRect`

## Where to assign future button styles/textures
Use shared theme (`res://assets/ui/theme/default_ui_theme.tres`) for:
- `Button`
- `Label`
- `PanelContainer`

If needed later, replace specific `Button` nodes with `TextureButton` in-scene without changing gameplay scripts.

## Icon placeholders
- `FloatingStatPopup/IconTextureRect` (hidden by default)

## Decorative nodes that should keep MouseFilter Ignore
- `StatusOverlay/StatusPanel/ArtBackground`
- `StatusOverlay/FloatingTextLayer`
- `FloatingStatPopup` root + its non-interactive children

These are intentionally non-clickable so gameplay/UI buttons below still receive input.

## Floating stat popup replacement workflow
1. Open `res://scenes/UI/FloatingStatPopup.tscn`.
2. Assign icon texture to `IconTextureRect` (optional).
3. Style `ValueLabel` via theme overrides or shared theme.
4. Keep node names unchanged so `StatusOverlay` can spawn/configure popups.

## Notes
- Behavior is preserved: stat popups still stack, move up-right, fade out, and avoid input blocking.
- This pass focuses on art-readiness/editor-friendliness only (no gameplay/reward/ending logic changes).
