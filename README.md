# 🧠 PsychoBuddy

**PsychoBuddy** is a professional-grade, hybrid automation platform for World of Warcraft. It provides a highly flexible and powerful environment for automating gameplay, specifically designed for high-complexity expansions.

Unlike basic scripts, PsychoBuddy is a full-featured platform that combines the precision of memory-reading with the invisibility of pixel-analysis, allowing users to choose the perfect balance between power and security.

---

## 🛠️ What PsychoBuddy Does

### 👁️ Adaptive Senses (Data Acquisition)
PsychoBuddy allows you to choose how the bot "sees" the game world:
*   **Power Mode (Memory):** Uses high-speed memory reading and **Pattern Scanning** to track player and NPC states with frame-perfect accuracy.
*   **Stealth Mode (Pixels):** A "Zero-Touch" system that uses a companion Lua addon to encode game data into pixels, which are then read by the bot via DXGI/WGC. This eliminates memory tampering and DLL injection.

### 🧠 The Decision Brain (Adaptive Logic)
*   **Priority Rotation Engine:** Engineered for **Legion (7.3.5)** and **BFA (8.3.7)**. It uses a requirement-based priority queue to handle complex procs, resource management, and artifact abilities.
*   **Behavioral Framework:** A modular "If-Then" system that allows for complex logic (e.g., "If health < 30% $\rightarrow$ Use Healthstone $\rightarrow$ Cast Shield").
*   **Advanced Navigation:** A* Pathfinding using triangle meshes for seamless movement across the game world.

### 💪 The Muscles (Input Simulation)
*   **Background Input Engine:** High-performance simulated keypresses using the Win32 API, allowing PsychoBuddy to control multiple clients in the background without requiring window focus.
*   **Humanization:** Randomized timing and curved movement to mimic real human behavior and bypass input pattern detection.


---

## 🎮 Supported Versions
PsychoBuddy is currently optimized for:
*   **Legion (7.3.5)**
*   **Battle for Azeroth (8.3.7)**

---

## 🧩 Current Development Stack
*   **Framework:** .NET 8 / `net8.0-windows`
*   **UI:** WPF
*   **Primary project file:** `PsychoBuddy.csproj` in the repository root
*   **Canonical source folder:** `src/`
*   **Application entry point:** `src/App.xaml` + `src/App.xaml.cs`
*   **Dashboard window:** `src/UI/MainWindow.xaml`
*   **UI theme resources:** `src/UI/Themes/PsychoTheme.xaml`
*   **Current dashboard workflow:** Scan clients → choose role/profile → attach → start/pause/stop → detach
*   **Current dashboard usability:** fixed 2x2 Fleet Monitor slots with empty/offline startup slots, visible detach/profile controls, selectable/copyable technical log, placeholder-button log feedback, custom minimize/close controls, and selected-client status labels
*   **Application icon:** `assets/PsychoBuddy.ico`, generated from `assets/Psycho.JPG`
*   **Primary UI visual target:** `assets/UI_Option_1.png`; `assets/UI_Option_1_1.png` is now used as a low-opacity layout underlay while the skin pass progresses; duplicate live labels have been removed so the background art provides most titles
*   **Note:** `PsychoBuddy-Built/` was a temporary test-build folder and is not part of the canonical source tree.

---

## ⚠️ Rules & Disclaimers

### 🚫 Prohibited Use
**The use of PsychoBuddy on official Blizzard Entertainment servers is STRICTLY PROHIBITED.** 

This software is intended exclusively for use on:
*   **Approved Private Servers**
*   **Personal / Local Servers**

### ⚖️ Liability & Risk
*   **Risk of Bans:** Botting is a violation of the Terms of Service of almost every game server. While PsychoBuddy implements advanced anti-detection, **no bot is 100% undetectable**.
*   **No Liability:** The author of PsychoBuddy is **NOT liable** for any account bans, suspensions, data loss, or hardware issues resulting from the use of this software. Use this tool entirely at your own risk.
*   **Educational Purpose:** This project is shared for development and educational purposes regarding game automation and memory architecture.

---

## 📜 License
All original source code and logic are the proprietary intellectual property of **[Psycho-core]**. 

**The canonical license file is `LICENSE.MYCODE.txt` in the repository root.** Commercial use, unauthorized distribution, or charging for access to the source code is strictly prohibited.
