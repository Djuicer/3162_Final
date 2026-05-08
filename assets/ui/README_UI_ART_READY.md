# UI Art-Ready Guide (User Profile)

The Tab User Profile is intentionally set up so visual style is edited in Godot resources, not C# code.

## User Profile scene and theme paths
- User Profile scene: `res://scenes/UI/UserProfileOverlay/user_profile_overlay.tscn`
- User Profile theme: `res://assets/ui/theme/user_profile_theme.tres`

## What the script should do (and not do)
- Script (`res://scripts/ui/UserProfileOverlay.cs`) should only:
  - toggle show/hide on Tab/Esc
  - update attribute text values
- Script should **not** be used to set font colors, font sizes, panel colors, outlines, or shadows.

## Where to change font color and font size
Open `res://assets/ui/theme/user_profile_theme.tres` in the Godot Inspector.

Recommended entries to edit:
- `Label/colors/font_color`
- `Label/font_sizes/font_size`
- `ProfileHeader/colors/font_color`
- `ProfileHeader/font_sizes/font_size`
- `ProfileAttribute/colors/font_color`
- `ProfileAttribute/font_sizes/font_size`
- Optional readability:
  - `Label/colors/font_outline_color`
  - `Label/constants/outline_size`

## Where to assign future panel/background art
Open `res://scenes/UI/UserProfileOverlay/user_profile_overlay.tscn` and select:
- `ProfilePanel/ProfileFrame` (`NinePatchRect`)

Assign art by changing:
- `texture` (your PNG frame/background)
- nine-patch margins (`patch_margin_*`) to fit your frame borders
- `modulate` if you want tint/transparency adjustments

The text content is inside `ProfilePanel/MarginContainer`, which keeps spacing from edges.

## Where to add attribute icons
In `user_profile_overlay.tscn`, each attribute row has a placeholder `TextureRect`:
- `EnergyRow/EnergyIcon`
- `FocusRow/FocusIcon`
- `KnowledgeRow/KnowledgeIcon`
- `ConfidenceRow/ConfidenceIcon`
- `CareerReadinessRow/CareerReadinessIcon`
- `NetworkingRow/NetworkingIcon`
- `PortfolioRow/PortfolioIcon`

Assign textures directly to those nodes in the editor.

## Important warning
If text becomes unreadable, fix it in the scene/theme resources above.

**Avoid changing C# just to adjust colors, size, or panel art.**
