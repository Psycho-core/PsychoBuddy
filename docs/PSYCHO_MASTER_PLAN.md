# 🧠 PSYCHOBUDDY: Master Automation Blueprint

## 0. CURRENT PROJECT BASELINE
- **Canonical project root:** repository root (`PsychoBuddy.csproj` + `src/`).
- **Framework:** .NET 8 / `net8.0-windows`.
- **UI framework:** WPF.
- **Application entry point:** `src/App.xaml` / `src/App.xaml.cs`, which opens `src/UI/MainWindow.xaml`.
- **Current shell:** `assets/UI_Option_1_5.png` in a fixed `1700x925` Canvas/Viewbox overlay.
- **End-goal UI reference:** `assets/UI_Option_1.png`.
- **Theme resources:** `src/UI/Themes/PsychoTheme.xaml`.
- **Dashboard startup:** four empty/offline fleet slots are shown when no clients are attached.
- **Dashboard attachment workflow:** Scan Clients → Choose Role/Profile → Attach → Start/Pause/Stop → Detach.
- **Confirmed build status:** `PsychoBuddy.exe` builds and launches; dashboard-level workflow is functional.
- **Development status:** active prototype; not a complete/fully working bot platform yet.
- **Application icon:** `assets/PsychoBuddy.ico`, generated from `assets/Psycho.JPG`, is configured as the executable and MainWindow icon.
- **Temporary build copy removed:** `PsychoBuddy-Built/` was only a test-build output folder and is no longer part of the source tree.
- **License location:** `LICENSE.MYCODE.txt` in the repository root is the authoritative license referenced by source headers.
- **Next functional milestone:** real Settings panel + `settings.json` persistence. See `docs/CURRENT_STATUS_AND_NEXT_STEPS.md`.

---

## 1. VISION
PsychoBuddy is a professional-grade multi-boxing and automation platform for World of Warcraft. Its primary purpose is to allow a user to manage a "fleet" of characters—typically allowing the user to play one main account manually while PsychoBuddy autonomously controls and synchronizes multiple other clients (e.g., 4+ bot accounts) in the background. 

It combines the **extensibility of a platform** (HonorBuddy), the **precision of memory-reading** (WRobot), and the **stealth of pixel-analysis** (Modern Bots), designed specifically for the complexity of versions **7.3.5 (Legion)** and **8.3.7 (BFA)**.

---

## 2. HIGH-LEVEL ARCHITECTURE
The system is split into three distinct layers to ensure that the "Brain" never needs to know how the "Senses" or "Muscles" actually work.

### Layer A: The Senses (Data Acquisition)
*This layer provides the game state to the Brain. It is modular, allowing the user to switch between "Power" and "Stealth" modes.*

*   **Smart Attachment System:**
    *   **Auto-Scan:** Scans for `Wow.exe` and identifies instances via Window Titles (Character Names).
    *   **Fallback Pathing:** Allows users to manually specify a custom executable path (e.g., `wow8.3.7.exe`) if the client is renamed or modded.
    *   **Instance Mapping:** Maps specific Window Handles (HWND) and PIDs to independent Bot Profiles, enabling multi-boxing.
*   **Mode 1: Power (Memory Reading)**
    *   **Tech:** P/Invoke `ReadProcessMemory` via a C++ DLL.
    *   **Targets:** `UnitFields`, `PlayerFields`, and `ObjectFields` (refer to `modern_versions_research.txt`).
    *   **Feature:** **Pattern Scanning**. Instead of hardcoded offsets, the bot scans for opcode patterns to find addresses automatically after game patches.
*   **Mode 2: Stealth (Pixel Sensing)**
    *   **Tech:** DXGI / Windows Graphics Capture (WGC).
    *   **Mechanism:** A companion Lua addon encodes game state (HP, MP, Coordinates) into a small 10x10 pixel grid of colors on the screen.
    *   **Decoding:** The C# core reads these pixels and converts colors back into data.

### Layer B: The Brain (Decision Engine)
*This is where the "Psycho" logic lives. It uses a hierarchical priority system.*

*   **The Core API:** A library of functions (`Bot.Cast()`, `Bot.MoveTo()`, `Bot.GetTarget()`) that Profiles use to interact with the game.
*   **The Behavior System (HB Style):**
    *   Profiles are lists of **Behaviors**.
    *   Example: `IF (Health < 30%) THEN Execute(HealBehavior)`.
*   **The Priority Rotation Engine (WRobot Style):**
    *   Specifically for 7.3.5/8.3.7.
    *   **Logic:** A priority queue of spells. The bot evaluates requirements (Procs, Cooldowns, Resource levels) and casts the highest-priority valid spell.
*   **The Navigation System:**
    *   A* Pathfinding over triangle meshes extracted from MPQ files.
    *   Waypoint-based recording for questing.

### Layer C: The Muscles (Input Simulation)
*How the bot actually "presses" the keys. Since PsychoBuddy is designed for multi-boxing, the priority is **100% Software-Based Background Control**.*

*   **Background Input Engine:** 
    *   Use `PostMessage` and `SendMessage` (Win32 API) to send keystrokes and mouse clicks directly to the target window's message queue. 
    *   This allows the bot to control a fleet of clients in the background while the user interacts with their main client in the foreground.
*   **Software Humanization:** 
    *   Implements randomized timing (Gaussian distribution) for key-down/key-up durations and intervals between actions.
    *   Uses Bezier curves for simulated mouse movement to avoid linear patterns.
*   **RunMacroText Bypass:** 
    *   Integration for servers that filter standard `PostMessage` inputs.


---

## 3. VERSION-SPECIFIC IMPLEMENTATION (7.3.5 & 8.3.7)
To handle the complexity of Legion and BFA, the bot implements:
*   **Spec-Awareness:** The Brain reads the `CurrentSpecID` to load the correct rotation profile.
*   **Proc Tracking:** The Senses track "AuraState" to detect procs, triggering high-priority "Burst" spells in the rotation.
*   **Artifact Integration:** Special handling for Artifact-specific abilities and XP tracking.

---

## 4. DEVELOPMENT ROADMAP

### Phase 1: The Foundation (The "Core")
- [ ] Establish the C# project structure.
- [ ] Implement the `Senses` interface (Generic data provider).
- [ ] Build the basic `Muscles` (Software input).

### Phase 2: The Senses (Power & Stealth)
- [ ] Build the Memory Reader with Pattern Scanning.
- [ ] Create the Lua "Pixel-Painter" addon.
- [ ] Implement the DXGI screen capture decoder.

### Phase 3: The Brain (Logic & Rotations)
- [ ] Implement the Finite State Machine (FSM) for general states (IDLE, COMBAT, LOOT).
- [ ] Build the Priority Rotation Engine (Requirement $\rightarrow$ Action).
- [ ] Create the Profile loader (JSON or C# Scripts).

*   **Phase 4: The Muscles (Software Humanization)**
    - [ ] Implement `PostMessage` wrapper for background inputs.
    - [ ] Develop the Gaussian Randomization engine for input delays.
    - [ ] Implement Bezier curve mouse movement logic.
    - [ ] Integrate `RunMacroText` bypasses.


### Phase 5: Integration & Testing
- [ ] Test on 7.3.5 and 8.3.7 private servers.
- [ ] Stress test the "Behavior" system with a leveling profile.
- [ ] Finalize the GUI dashboard for profile management.

---

## 5. RISK MITIGATION MATRIX
| Risk | Mitigation Strategy |
| :--- | :--- |
| **Warden Scan** | Use Pixel-Sensing + Software Humanization (Input Jitter). |
| **Pattern Detection** | Randomize timing, paths, and rotation priority. |
| **Game Patch** | Use Pattern Scanning instead of static offsets. |
| **Player Report** | Implement a "Whisper-Responder" or "Auto-Stop" on interaction. |
