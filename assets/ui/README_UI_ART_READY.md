# UI Art-Ready Guide

This project is set up so you can replace UI visuals in Godot without rewriting scripts.

## New global profile UI
- User Profile overlay scene: `res://scenes/UI/UserProfileOverlay/user_profile_overlay.tscn`
- Profile hint scene: `res://scenes/UI/ProfileHint/profile_hint.tscn`
- Overlay spawner: `res://scripts/ui/StatusOverlay.cs`

## Where to assign future art
- Profile panel frame/background:
  - `user_profile_overlay.tscn` → `ProfilePanel/ProfileFrame` (`NinePatchRect`, decorative)
- Small Tab hint frame/background:
  - `profile_hint.tscn` → `HintFrame/ArtBackground` (`NinePatchRect`, decorative)
- Existing event/dialogue frames remain unchanged.

## Decorative nodes and mouse safety
Decorative nodes are non-interactive and should remain `MouseFilter = Ignore`:
- `ProfilePanel/ProfileFrame`
- `ProfileHint/HintFrame/ArtBackground`
- `StatusOverlay/FloatingTextLayer`

When the User Profile is hidden, it does not block clicks.

## Editing text or icons later
- Profile text labels are in `user_profile_overlay.tscn` under:
  - `HeaderLabel`
  - `ProfileInfoContainer/ProfileInfoLabel`
  - `AttributesContainer/AttributesLabel`
  - `RouteStatusContainer/RouteStatusLabel`
  - `HintLabel`
- Hint text is in `profile_hint.tscn` → `HintFrame/Label`.
- If you want attribute icons, add `TextureRect` nodes inside `AttributesContainer` rows.

## Shared theme
- Theme resource: `res://assets/ui/theme/default_ui_theme.tres`
- Apply/update shared styles for:
  - `Button`
  - `Label`
  - `PanelContainer` / `Panel`
