# STEALTH MODE: LUA ADDON SPECIFICATION (The Pixel-Painter)

## 1. OBJECTIVE
To create a "Zero-Touch" data acquisition system. Instead of reading the game's memory (which can be detected by some anti-cheats), the bot reads a small area of the screen. A companion Lua addon "paints" the current game state into this area using colors.

---

## 2. TECHNICAL ARCHITECTURE

### 2.1 The Pixel Grid (The "Canvas")
The addon creates a small, non-interactive frame (texture) on the screen.
- **Size:** 10x10 pixels.
- **Position:** Fixed (e.g., Top-Left corner `0,0` or a user-configurable offset).
- **Transparency:** The frame is set to `alpha = 0.01` (nearly invisible to the human eye, but fully readable by the DXGI screen capture).
- **Update Rate:** `OnUpdate` script runs every frame to ensure real-time data.

### 2.2 Encoding Logic (State $\rightarrow$ Color)
The 100 available pixels are divided into "Data Zones." Each pixel's color represents a specific value or state.

#### Zone A: Vitality (Pixels 0-9)
- **Purpose:** Health and Mana percentages.
- **Logic:** 
    - **Pixel 0-4:** Health. (Red intensity = % of Health).
    - **Pixel 5-9:** Mana/Energy. (Blue intensity = % of Resource).

#### Zone B: Target Status (Pixels 10-19)
- **Purpose:** Is the target alive? Is it in range?
- **Logic:**
    - **Pixel 10:** Target Existence (Green = Yes, Black = No).
    - **Pixel 11:** Target Alive (White = Yes, Red = No).
    - **Pixel 12:** In Combat Range (Yellow = Yes, Black = No).
    - **Pixel 13:** Target Casts (Blue = Casting, Black = Idle).

#### Zone C: Combat Procs & Buffs (Pixels 20-59)
- **Purpose:** Triggering high-priority spells.
- **Logic:**
    - Each pixel is mapped to a specific `SpellID` or `BuffID`.
    - **Color Mapping:**
        - **Bright White:** Proc Active.
        - **Dark Grey:** Proc on Cooldown.
        - **Black:** Inactive.

#### Zone D: Navigation & Coordinates (Pixels 60-99)
- **Purpose:** Communicating X, Y, Z and Facing.
- **Logic:** Since a single pixel cannot represent a coordinate, we use **Color-Coded Quantization**.
    - X-coord is split into 4 pixels. Each pixel's Hue represents a range of the map.
    - Facing is represented by the Hue of a single pixel (0-360 degrees $\rightarrow$ Hue 0-360).

---

## 3. LUA IMPLEMENTATION DETAILS

### 3.1 Gathering Data
The addon uses standard, approved WoW API calls:
- `UnitHealth("player")` / `UnitHealthMax("player")`
- `UnitMana("player")` / `UnitManaMax("player")`
- `UnitExists("target")`
- `UnitCanAttack("player", "target")`
- `UnitAura("player", spellID, "HELPFUL")`

### 3.2 Rendering the Pixels
Because WoW's UI doesn't allow drawing individual pixels easily, the addon uses a **small texture sheet** or a **custom font string** with a 1x1 character size.
- **Method:** Use a `Texture` frame and update its `SetTexCoord` or use a `Canvas` approach to modify the color of specific sub-regions of the texture.

---

## 4. SECURITY & UNDETECTABILITY

### 4.1 API Compliance
- **No Forbidden Calls:** The addon does not use `GetCursorPosition` or other functions that trigger "secure" warnings.
- **No Memory Access:** The addon is a standard `.toc` / `.lua` file. It does not inject DLLs or modify the game binary.
- **Passive Nature:** The addon only *outputs* data to the screen; it does not *input* anything into the game.

### 4.2 Visual Cloaking
- The 10x10 grid is placed in a location where it does not interfere with the user's UI.
- By setting the alpha to near-zero, it remains invisible to the user while remaining detectable by the C# bot's `GetPixel` or `BitBlt` calls.

---

## 5. INTEGRATION WITH THE BOT (C# SIDE)
1. **Capture:** The bot uses DXGI to capture the 10x10 region.
2. **Sample:** The bot reads the RGB values of the pixels.
3. **Translate:**
    - `If Pixel(0).R > 200 Then Health = 100%`
    - `If Pixel(10).G > 200 Then TargetExists = True`
4. **Act:** The Brain processes this data and triggers the corresponding rotation.
