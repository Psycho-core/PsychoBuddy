# PsychoBuddy Current Status & Next Steps

_Last updated: 2026-06-10_

## 1. Confirmed Current Build Status

`PsychoBuddy.exe` builds and launches under:

```text
.NET 8 / net8.0-windows
WPF
```

The current UI shell uses:

```text
assets/UI_Option_1_5.png
```

as a fixed `1700x925` Canvas/Viewbox visual shell. Artwork-provided buttons are clicked through invisible WPF hitboxes.

## 2. Confirmed Working Features

Verified by user test builds/logs:

- WPF startup through `src/App.xaml`.
- Custom app/window icon from `assets/PsychoBuddy.ico`.
- Canvas/Viewbox command-center shell loads.
- Invisible hitboxes work for artwork buttons:
  - Menu
  - Profiles
  - Navigation
  - Senses
  - Settings
  - Minimize
  - Close
- Window dragging works.
- Technical log works and is selectable/copyable.
- Four fixed Fleet Monitor slots show on startup.
- Empty/offline slots show when no clients are attached.
- Client scan finds running WoW client windows.
- Attach fills first available fleet slot.
- Detach returns that slot to empty/offline.
- Start selected works.
- Pause selected works.
- Stop selected works.
- Start All works.
- Stop All works.
- Tick reports real running-client count or no running clients.
- Power/Stealth UI toggle works.
- Senses settings panel opens/saves/cancels and persists Power/Stealth acquisition configuration.
- Navigation settings panel opens/saves/cancels and persists route/follow/formation scaffold settings.
- Target game version setting supports Vanilla 1.12 through BFA 8.3.7.
- Mock client/offline simulation can test fleet slots, start/stop/tick, and profile rules without a live WoW server.
- Settings panel opens/saves/cancels and persists settings to `settings.json`.
- Profiles panel opens/closes, reloads `profiles/**/*.json`, applies selected profiles to the dashboard dropdown, and builds executable placeholder `RotationProfile` rules from profile JSON. Generated Legion/BFA class/spec profile scaffolds exist under `profiles/Legion_7_3_5/` and `profiles/BFA_8_3_7/`.
- Placeholder panels log clear messages for Profiles, Navigation, Senses, and Menu.

## 3. Known Limitations

The project is still in active development and is not a complete/fully working bot platform yet.

Current limitations:

- Navigation button is still a placeholder.
- Settings UI exists, but more validation/polish may be needed as features expand.
- No real profile file loading yet.
- Power Mode memory reading is still scaffold/placeholder-level.
- Stealth Mode capture/addon pipeline is not implemented yet.
- Navigation/pathfinding execution is documented but not implemented as live movement code. A dashboard Navigation scaffold now exists.
- Final UI cosmetics still need alignment/polish toward `assets/UI_Option_1.png`.

## 4. What We Were Working On Before The UI Cosmetic Rabbit Hole

Before the deep UI skin/alignment work, the next planned functional milestone was:

```text
Build the real Settings panel with settings.json persistence.
```

That milestone has now been started/implemented at the dashboard level:

1. `settings.json` persistence is handled by `SettingsService`.
2. `AppSettings` stores runtime settings.
3. The Settings button opens a real Settings panel.
4. The panel persists:
   - custom WoW executable path
   - preferred/default Role
   - preferred/default Profile
   - preferred Senses mode
   - auto-scan on startup setting
   - log verbosity / debug mode
   - last window size/position
5. The custom executable path is wired into `AttachmentManager` for scan fallback.

The Profiles page + JSON profile loading milestone has now been extended with placeholder rotation-rule loading/execution at dashboard/orchestrator level.

Implemented:

```text
profiles/*.json
src/Brain/ProfileDefinition.cs
src/Brain/ProfileService.cs
assets/Profiles_Panel.png
```

The Senses settings page + real Power/Stealth mode configuration milestone has now been started/implemented at dashboard configuration level.

The Navigation page scaffold milestone has now been started/implemented at dashboard configuration level.

The current no-server development support now includes target game version selection and mock clients. The next planned functional feature is now:

```text
Expanded profile execution / real class rotation rules
```

## 5. Recommended Next Functional Roadmap

### Step 1 — Settings Panel / Persistence

Status: **implemented at dashboard level**. Continue testing/polishing as new settings are added.

Implemented files:

```text
src/Core/AppSettings.cs
src/Core/SettingsService.cs
assets/Settings-Panel-final.png
settings.json (runtime generated)
```

### Step 2 — Real Profile Loading

Status: **implemented with recursive JSON profile loading and placeholder executable rules**. Generated Legion/BFA class/spec rotation scaffolds exist, but exact spell IDs/keybinds, aura/proc logic, and target conditions are still pending.

Implemented files:

```text
profiles/*.json
src/Brain/ProfileDefinition.cs
src/Brain/ProfileService.cs
assets/Profiles_Panel.png
```

### Step 3 — Senses Settings Page

Status: **implemented at dashboard configuration level**. Real backend capture/memory resolution is still pending.

Implemented:

- Power/Stealth mode choice
- scan interval
- pixel grid position/size
- capture/reliability toggles
- diagnostic/test log action

### Step 4 — Navigation Page Scaffold

Status: **implemented at dashboard configuration/scaffold level**. Real pathfinding, route recording, and movement execution are still pending.

Implemented:

- Navigation mode
- route name
- follow distance
- formation spacing
- waypoint radius
- auto-follow/avoid-overlap/show-path toggles
- placeholder waypoint list
- add/remove/clear placeholder waypoints
- test configuration log action

### Step 5 — Profile Execution / Rotation Rules

Implement loading and execution of real profile/rotation rules instead of metadata-only profile selection.

### Step 6 — UI Cosmetic Pass 2

Continue alignment/polish toward:

```text
assets/UI_Option_1.png
```

The current shell is usable, but final polish is not complete.
