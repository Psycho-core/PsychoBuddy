# PsychoBuddy Comprehensive Research & Development Roadmap

This document serves as the master checklist for every single technical and content-related requirement needed to transform PsychoBuddy from a design concept into a professional-grade, functioning automation platform.

## Status Key / Reality Check
- `[x]` in this roadmap means the research/specification document exists or the concept has a first-pass scaffold.
- It does **not** necessarily mean the feature is production-complete.
- Current codebase status: .NET 8 WPF foundation/prototype with core scaffolding for Attachment, Senses, Muscles, Brain, and UI.
- `PsychoBuddy.exe` builds and launches; current dashboard-level scan/attach/start/pause/stop/detach workflow is functional.
- The project is still in active development and is not a complete/fully working bot platform yet.
- Removed legacy temporary build copy: `PsychoBuddy-Built/`. The repository root is now the canonical project location.
- See `docs/CURRENT_STATUS_AND_NEXT_STEPS.md` for the current verified status and next functional milestone.

## 1. The Senses: Data Acquisition (The "Eyes")
*How the bot perceives the game state.*

### 1.1 Power Mode (Memory Reading)
- [x] ReadProcessMemory / P/Invoke Architecture
- [x] Pattern Scanning for Offset Discovery
- [x] **Comprehensive Offset Table (7.3.5 & 8.3.7):**
    - [x] `Player` / `Unit` base addresses
    - [x] `UnitFields` (HP, MP, Level, Target, Position, Facing)
    - [x] `PlayerFields` (Currency, Experience, Bag slots)
    - [x] `Spell` / `Ability` Cooldown timers
    - [x] `Buff` / `Debuff` lists (ID, Duration, Stacks)
    - [x] `Action Bar` mapping (which spell is in which slot)
    - [x] `World` state (Zone ID, Local Map coordinates)


### 1.2 Stealth Mode (Pixel Sensing)
- [x] DXGI / Windows Graphics Capture (WGC) Architecture
- [x] **Lua Addon Development:**
    - [x] Design 10x10 pixel grid encoding system
    - [x] Map game state (Health %, Target Status, Proc Active) to specific pixel colors
    - [x] Ensure addon remains undetected (no unauthorized API calls)
- [x] **Decoding Logic:**
    - [x] Color-to-State translation table
    - [x] Sampling rate optimization (FPS vs. CPU usage)
    - [x] Noise filtering and flicker suppression

---

## 2. The Muscles: Input Simulation (The "Hands")
*How the bot interacts with the game.*

### 2.1 Software-Level Input
- [x] Win32 API `PostMessage` / `SendMessage` (Background input)
- [ ] **Window Management:**
    - [ ] HWND identification per character name
    - [ ] Handling window resizing/minimization without losing input
    - [ ] Coordinate mapping for non-focused windows

### 2.2 Input Humanization
- [x] Software-side random delay implementation
- [x] Bezier curve mouse movement logic
- [x] Gaussian distribution for action intervals



---

## 3. The Brain: Decision Engine (The "Mind")
*The logic that converts data into action.*

### 3.1 Combat Logic
- [x] Class Priority Rotations (All Classes, Both Versions)
- [x] **Racial Ability Integration:**
    - [x] Map all racials (Orc, Troll, etc.) to priority lists
- [ ] **Talent-Dynamic Logic:**
    - [ ] Ability to switch rotations based on detected active talents
- [ ] **Condition-Based Triggers:**
    - [ ] `IF Health < X% THEN Use [Healing Ability]`
    - [ ] `IF Target_Dead THEN Search_Next_Target`
    - [ ] `IF Proc_X == Active THEN Cast [Burst Ability]`

### 3.2 Navigation & World Interaction
- [x] **A* Pathfinding System:**
    - [x] MPQ/CASC Mesh Extraction theory (NavMesh/V-Maps)
    - [x] Implementation of A* over polygon graphs
    - [x] Funnel Algorithm (String Pulling) for path smoothing
- [x] **Interaction Logic:**
    - [x] NPC interaction triggers (Quest turn-ins, Vendors)
    - [x] Object interaction (Mining nodes, Herbs, Chests)
    - [x] Coordinate-based "Hotspots" for farming/leveling
- [x] **Role-Based Coordination:**
    - [x] Leader-Follower distance synchronization
    - [x] Tactical offsets (Tank/Healer/DPS positioning)
    - [x] Local collision avoidance between fleet members
    - [x] Shared Fleet State (SFS) Orchestration
    - [x] Inter-role dependency logic (Healer $\rightarrow$ Tank)
    - [x] Coordinated burst synchronization
    - [x] Emergency triage/role-swap protocols

---

## 4. Game-Specific Content & Data
*The "What" to do in the world.*

- [ ] **Leveling Databases:**
    - [ ] Optimal paths for 1-110 (Legion) and 1-120 (BFA)
    - [ ] Coordinate lists for all primary quest NPCs
- [ ] **Dungeon/Raid Logic:**
    - [ ] Positioning markers (Tank spot, Healer spot)
    - [ ] Boss-specific mechanic triggers (e.g., "Run away from center")
- [ ] **Profession Automation:**
    - [ ] Gathering routes (Mining/Herbalism)
    - [ ] Crafting loops (Alchemy/Jewelcrafting)
    - [ ] Fishing bobber detection (Pixel-based)
- [ ] **Economy Integration:**
    - [ ] Auction House price monitoring (Memory or API)
    - [ ] Auto-posting/Auto-buying logic

---

## 5. The Platform: User Experience (The "Body")
*The software surrounding the bot.*

- [x] **Build Target:** .NET 8 / `net8.0-windows`
- [x] **WPF Startup Shell:** `src/App.xaml` launches `MainWindow` and displays four empty/offline fleet slots when no clients are found.
- [x] **Dashboard Attachment Workflow:** Scan available clients, choose role/profile, attach, start, pause, stop, and detach from the UI.
- [x] **Dashboard Usability Pass:** Fixed 2x2 Fleet Monitor slots, empty/offline startup slots, visible detach/profile controls, copyable technical log, placeholder-button feedback, custom minimize/close wiring, selected-client status labels, clearer tick logging, and reduced fleet-card clipping.
- [x] **Application Icon:** `assets/PsychoBuddy.ico` generated from `assets/Psycho.JPG` and configured in `PsychoBuddy.csproj` plus `MainWindow.xaml`.
- [x] **UI Skin Pass 1:** Added `src/UI/Themes/PsychoTheme.xaml` and moved the dashboard toward `assets/UI_Option_1.png` with obsidian/lava/parchment styling, a fixed `1700x925` Canvas/Viewbox shell based on `assets/UI_Option_1_5.png`, invisible hitboxes over artwork-provided buttons, compact red/green fleet status indicators, duplicate live labels removed, and Client Attachment kept visible for debugging.
- [x] **Settings Panel / Persistence:** Added `assets/Settings-Panel-final.png`, `AppSettings`, `SettingsService`, runtime `settings.json` save/load, custom WoW executable path persistence, default role/profile, preferred Senses mode, auto-scan, debug/log settings, and window placement persistence.
- [x] **Profiles Panel / JSON Loading:** Added `assets/Profiles_Panel.png`, `profiles/*.json`, `ProfileDefinition`, `ProfileService`, JSON profile loading, profile reload, and selected profile application to the dashboard dropdown.
- [x] **Profile Execution / Rotation-Rule Loading:** Added JSON rule fields for priority, ability, key code, cooldown, condition, and threshold; `ProfileService` now recursively loads `profiles/**/*.json` and builds `RotationProfile` objects from profile JSON; start actions now register profile-built rotation rules with the orchestrator; generated Legion/BFA class/spec scaffold profiles from the existing `data/` sheets.
- [x] **Senses Settings Panel:** Added `assets/Senses_Panel.png`, Power/Stealth mode configuration, scan interval, Stealth pixel grid offset/size, window capture toggle, flicker suppression, blackout/data-loss check, and test configuration log action.
- [x] **Navigation Settings Scaffold:** Added `assets/Navigation_Panel.png`, navigation mode, route name, follow distance, formation spacing, waypoint radius, auto-follow/avoid-overlap/show-path toggles, placeholder waypoint list, and test configuration log action.
- [x] **GameVersion Support:** Added target game version selection for Vanilla 1.12, TBC 2.4.3, WotLK 3.3.5a, Cataclysm 4.3.4, MoP 5.4.8, Legion 7.3.5, and BFA 8.3.7.
- [x] **Mock Client / Offline Simulation:** Added mock client fleet slots, mock start/stop/tick simulation, and selected profile rule test logging for development without a live WoW server.

- [x] **GUI Blueprint & UX Design:**
    - [x] Visual Concept Review (Command Center / Navigator)
    - [x] Layout Architecture (Fleet Monitor, Senses Hub, Technical Log)
    - [x] UX Interaction Flows
    - [x] Technical Component Mapping


---

## 6. Legal & Safety
- [x] User Disclaimer / Liability Waiver
- [x] Restricted Server List (Private/Local only)
- [ ] **Detection Vector Research:**
    - [ ] Study of current server-side detection methods (Heartbeats, Input patterns)
    - [ ] Implementation of mitigation strategies


---

## 7. Future AI / Ollama Integration
- [ ] **Optional Ollama Support:** Local LLM integration for chat/whisper reply suggestions, log summarization, profile explanations, and debug assistance. This should remain disabled by default and should not directly control combat, movement, memory reads, or input decisions.
