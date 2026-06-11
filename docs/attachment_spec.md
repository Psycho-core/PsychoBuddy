# 🖇️ PsychoBuddy: Attachment Module Specification

## 1. Objective
To provide a safe, user-friendly way to connect PsychoBuddy to one or more World of Warcraft game clients, regardless of whether the executable is named standardly or has been modded/renamed.

The current dashboard workflow is intentionally staged:

`Scan Clients` → `Choose Role/Profile` → `Attach` → `Start / Pause / Stop` → `Detach`

This prevents PsychoBuddy from starting active automation automatically on launch.

---

## 2. Current Dashboard Attachment Workflow

### Step 1: Process Discovery / Auto-Scan
The dashboard scans for running processes.

* **Primary target:** `Wow.exe` / process names beginning with `Wow`.
* **Current implementation:** `AttachmentManager.ScanForClients()`.
* **Logic:**
  1. Use `System.Diagnostics.Process.GetProcesses()`.
  2. Filter candidate processes by the default WoW process name.
  3. Locate the process window handle through `EnumWindows` + `GetWindowThreadProcessId`.
  4. Read the window title through `GetWindowText`.
  5. Extract a display character/client name from the title.
  6. Show unattached clients in the dashboard `Available Clients` list.

### Step 2: Configure Client Role/Profile
Before attaching, the user chooses:

* **Role:** `Tank`, `Healer`, `DPS`, or `Utility`.
* **Profile:** Current UI placeholder options include `Basic Rotation`, `Tank Assist`, `Healer Assist`, `DPS Assist`, and `Manual Follow`.

This assignment is stored on the UI fleet card and on the created `ClientBinding.AssignedProfile`.

### Step 3: Attach Selected Client
When the user clicks `Attach Selected`, the dashboard creates a `ClientBinding` containing:

* `ProcessID (PID)`
* `WindowHandle (HWND)`
* `CharacterName`
* `AssignedProfile`
* `BotStatus.Attached`

The client is moved from `Available Clients` to the attached fleet list.

**Important:** Attached clients are idle by default. Attachment does not automatically start active bot logic.

### Step 4: Start / Pause / Stop
The selected fleet card can be controlled from the dashboard.

* **Start Selected:** Registers the client with the `Orchestrator`, builds a `RotationProfile` from the selected JSON profile through `ProfileService`, and marks status as `Running`.
* **Pause Selected:** Leaves the client attached but marks status as `Paused`.
* **Stop Selected:** Unregisters the client from the `Orchestrator` and marks status as `Stopped`.
* **Start All / Stop All:** Applies start/stop to all real attached clients.

### Step 5: Detach
`Detach Selected` unregisters the client from the `Orchestrator`, removes it from active control, and returns that fleet position to an empty/offline slot.

---

## 3. Status Lifecycle

```text
Disconnected → Attached → Running
                    ↓         ↓
                 Detached   Paused / Stopped
```

| Status | Meaning |
| :--- | :--- |
| `Disconnected` | No real process/window is attached. Used by empty/offline slots. |
| `Attached` | Client is known to the dashboard but active bot logic is not running. |
| `Running` | Client is registered with the orchestrator and eligible for fleet ticks. |
| `Paused` | Client remains attached but should not be actively ticked. |
| `Stopped` | Client remains visible but has been unregistered from active orchestration. |

---

## 4. Technical Implementation Details

### Current Source Components

| Concern | File / Type |
| :--- | :--- |
| Process/window discovery | `src/Core/AttachmentManager.cs` |
| Attached client model | `src/Core/ClientBinding.cs` |
| Fleet UI cards | `src/UI/FleetCardViewModel.cs` |
| Dashboard workflow state | `src/UI/DashboardController.cs` |
| Dashboard layout/actions | `src/UI/MainWindow.xaml` + `.xaml.cs` |
| Runtime registration | `src/Brain/Orchestrator.cs` |

### Win32 API Requirements

The implementation uses these Windows APIs through `src/Core/Win32Api.cs`:

- `EnumWindows`
- `GetWindowThreadProcessId`
- `GetWindowText`
- `PostMessage` / `SendMessage`
- `OpenProcess`
- `ReadProcessMemory`
- `CloseHandle`

### Multi-Instance Model

```csharp
public class ClientBinding
{
    public int Pid { get; set; }
    public IntPtr WindowHandle { get; set; }
    public string? CharacterName { get; set; }
    public string? AssignedProfile { get; set; }
    public BotStatus Status { get; set; }
    public bool IsForeground { get; set; }
}
```

---

## 5. UI Design Requirements

The dashboard must expose:

- **Available Clients:** scanned but unattached clients.
- **Selected-client feedback:** dashboard shows the selected available client and selected fleet card so workflow button targets are clear.
- **Role/Profile selection:** user assigns intent before attachment.
- **Attached Fleet:** always displays four fixed 2x2 slots. Real clients replace empty/offline slots; empty slots remain visible when fewer than four real clients are attached.
- **Lifecycle controls:** Attach, Start, Pause, Stop, Detach, Start All, Stop All.
- **Technical Log:** every scan/attach/start/stop/detach action logs a timestamped message; the log is a read-only selectable/copyable text box.

---

## 6. Pending Improvements

- Persist custom executable path in `settings.json`.
- Add file browser UI for custom WoW executable paths.
- Improve hidden/minimized window filtering with `IsWindowVisible`.
- Add stronger duplicate-client detection if multiple windows share identical titles.
- Replace placeholder profile choices with real profile files loaded from disk.
- Surface attachment errors directly beside the affected client card.
