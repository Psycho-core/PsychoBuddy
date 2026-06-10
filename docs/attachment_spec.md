# 🖇️ PsychoBuddy: Attachment Module Specification

## 1. Objective
To provide a seamless, user-friendly way to connect PsychoBuddy to one or more World of Warcraft game clients, regardless of whether the executable is named standardly or has been modded/renamed.

## 2. The Attachment Workflow

### Step 1: Process Discovery (The Auto-Scan)
The bot will periodically scan for running processes.
*   **Primary Target:** `Wow.exe`
*   **Logic:** 
    1.  Use `System.Diagnostics.Process.GetProcesses()` to retrieve all running tasks.
    2.  Filter for processes matching the target name.
    3.  For each match, use `User32.dll`'s `GetWindowText` to retrieve the window title.
    4.  **UI Presentation:** Display a list to the user: *"World of Warcraft - [Character Name]"*.

### Step 2: Fallback Identification (The Custom Path)
If the user is using a modded client (e.g., `wow8.3.7.exe`), the auto-scan may fail.
*   **User Configuration:** A "Browse for Executable" button in the settings.
*   **Storage:** The path is saved to `settings.json`.
*   **Logic:** 
    1.  The bot checks the saved custom path.
    2.  If a process is running from that specific path, it is added to the "Available Clients" list regardless of the `.exe` name.

### Step 3: Binding & Multi-boxing
Once a client is selected from the list, PsychoBuddy creates a **Binding**.
*   **Binding Data:** 
    *   `ProcessID (PID)`: For memory reading and process management.
    *   `WindowHandle (HWND)`: For sending targeted inputs (`PostMessage`) and capturing specific window pixels (WGC).
    *   `ProfileID`: Which `.json` profile is controlling this specific window.

## 3. Technical Implementation Details

## 3. Technical Implementation Details

### API Requirements (Windows User32.dll)
To implement this, the following Win32 API calls are required:
- `EnumWindows`: To iterate through all open windows.
- `GetWindowThreadProcessId`: To link a window handle to a PID.
- `GetWindowText`: To read the "World of Warcraft - [Character]" string.
- `IsWindowVisible`: To ensure we don't try to attach to hidden background processes.
- `PostMessage` / `SendMessage`: **CRITICAL** for multi-boxing. This allows sending inputs to background windows without bringing them to the foreground.

### Multi-Instance Handling (The Fleet Manager)
The system will support a `List<ClientBinding>` structure designed for multi-boxing:
```csharp
public class ClientBinding {
    public int Pid { get; set; }
    public IntPtr WindowHandle { get; set; }
    public string CharacterName { get; set; }
    public string AssignedProfile { get; set; }
    public BotStatus Status { get; set; } // Attached, Running, Stopped
    public bool IsForeground { get; set; } // Tracks if this is the user's main client
}
```

## 4. User Interface (UI) Design
- **The Selector:** A dropdown menu showing "Available Clients."
- **The Fleet Dashboard:** A grid/list view of all attached clients.
    - Column 1: Character Name.
    - Column 2: Assigned Profile (Dropdown).
    - Column 3: Status (Idle/Farming/Combat).
    - Column 4: Control (Start/Stop/Pause).
- **The Settings:** A "Custom Executable Path" field with a file browser.

