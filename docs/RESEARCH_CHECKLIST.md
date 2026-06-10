# PsychoBuddy Comprehensive Research & Development Roadmap

This document serves as the master checklist for every single technical and content-related requirement needed to transform PsychoBuddy from a design concept into a professional-grade, functioning automation platform.

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
    - [ ] Map all racials (Orc, Troll, etc.) to priority lists
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

- [ ] **GUI Development:**
    - [ ] Account Manager (Login/Password/Server storage)
    - [ ] Character Selector & Profile Binder
    - [ ] Real-time Fleet Monitor (HP/MP/Status of all clients)
    - [ ] Console/Log for debugging bot decisions
- [ ] **Configuration System:**
    - [ ] JSON/XML profiles for rotations and paths
    - [ ] Global settings (Input method, Stealth vs. Power mode)
- [ ] **Anti-Detection (The "Humanizer"):**
    - [ ] Input jitter (Randomized delays)
    - [ ] Movement imperfections (Avoid perfectly straight lines)
    - [ ] Rotation variation (Slight timing shifts to avoid "metronome" patterns)

---

## 6. Legal & Safety
- [x] User Disclaimer / Liability Waiver
- [x] Restricted Server List (Private/Local only)
- [ ] **Detection Vector Research:**
    - [ ] Study of current server-side detection methods (Heartbeats, Input patterns)
    - [ ] Implementation of mitigation strategies
