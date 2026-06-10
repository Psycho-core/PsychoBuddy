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
- Placeholder panels log clear messages.

## 3. Known Limitations

The project is still in active development and is not a complete/fully working bot platform yet.

Current limitations:

- Profiles button is still a placeholder.
- Navigation button is still a placeholder.
- Senses settings page is still a placeholder.
- Settings page is still a placeholder.
- Profile choices are still hardcoded.
- No `settings.json` persistence yet.
- No real profile file loading yet.
- Power Mode memory reading is still scaffold/placeholder-level.
- Stealth Mode capture/addon pipeline is not implemented yet.
- Navigation/pathfinding is documented but not implemented as live code.
- Final UI cosmetics still need alignment/polish toward `assets/UI_Option_1.png`.

## 4. What We Were Working On Before The UI Cosmetic Rabbit Hole

Before the deep UI skin/alignment work, the next planned functional milestone was:

```text
Build the real Settings panel with settings.json persistence.
```

The intended Settings work was:

1. Create a `settings.json` file.
2. Add a settings service/model to load and save settings.
3. Replace the Settings placeholder click with a real Settings panel.
4. Persist:
   - custom WoW executable path
   - preferred/default Role
   - preferred/default Profile
   - preferred Senses mode
   - auto-scan on startup setting
   - log verbosity / debug mode
   - last window size/position if desired
5. Wire the custom executable path into `AttachmentManager`.

After Settings, the next planned feature was:

```text
Profiles page + JSON profile loading
```

That would replace the current hardcoded profile dropdown with profile files loaded from disk.

## 5. Recommended Next Functional Roadmap

### Step 1 — Settings Panel / Persistence

Implement:

```text
settings.json
src/Core/AppSettings.cs
src/Core/SettingsService.cs
Settings panel UI
```

### Step 2 — Real Profile Loading

Implement:

```text
profiles/*.json
Profile loader service
Dropdown populated from profile files
```

### Step 3 — Senses Settings Page

Implement a UI for:

- Power/Stealth mode choice
- scan interval
- pixel grid position
- diagnostic readouts

### Step 4 — UI Cosmetic Pass 2

Continue alignment/polish toward:

```text
assets/UI_Option_1.png
```

The current shell is usable, but final polish is not complete.
