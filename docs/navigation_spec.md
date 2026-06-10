# NAVIGATION & PATHFINDING SPECIFICATION (A* & NavMesh)

## 1. OBJECTIVE
To implement a robust, 3D navigation system that allows the PsychoBuddy "fleet" to autonomously traverse the world, follow a leader (the user), and navigate dungeons without colliding with static geometry or getting stuck.

---

## 2. DATA ACQUISITION: THE NAVMESH
A bot cannot navigate using raw 3D models (too complex). It requires a **Navigation Mesh (NavMesh)**—a simplified set of connected polygons representing the "walkable" surface of the world.

### 2.1 Source of Data
For versions 7.3.5 and 8.3.7, the navigation data is stored in the game's data files (CASC/MPQ).
- **V-Maps:** Provide the verticality and ground-level data.
- **M-Maps (MoveMaps):** Provide the "walkable" vs "non-walkable" binary data.
- **TrinityCore Reference:** We leverage the `Movement` and `NavMesh` implementation from the TrinityCore source (8.3.7/35662) to mirror how the server handles NPC pathing.

### 2.2 Extraction Process
1. **CASC Extraction:** Use a CASC viewer to extract the map files for the target zone.
2. **Conversion:** Convert raw binary map data into a workable format (JSON or a custom binary format) that the C# bot can load into memory.
3. **Polygonization:** Represent the map as a graph where each **Polygon** is a **Node**, and shared edges between polygons are **Arcs**.

---

## 3. THE BRAIN: A* PATHFINDING ALGORITHM
To move from Point A to Point B, PsychoBuddy uses the A* (A-Star) algorithm.

### 3.1 The Mathematical Model
The bot calculates the path by minimizing the function: `f(n) = g(n) + h(n)`
- **`g(n)` (Cost):** The actual distance traveled from the start to the current polygon.
- **`h(n)` (Heuristic):** The estimated distance (Euclidean) from the current polygon to the target.
- **`f(n)`:** The total estimated cost. The bot always expands the node with the lowest `f(n)`.

### 3.2 Smoothing: The Funnel Algorithm
A* returns a sequence of polygons, which results in a "jagged" path (center-to-center). To make movement look human and efficient, we implement the **Funnel Algorithm (String Pulling)**.
- **Logic:** Imagine a string stretched from the start to the end through a series of "portals" (the edges of the polygons). The algorithm "pulls" the string tight, removing unnecessary turns and creating a straight line wherever possible.

---

## 4. FLEET COORDINATION: LEADER-FOLLOWER LOGIC
For multi-boxing, the bots must not just "go to the leader," but maintain a tactical formation.

### 4.1 Synchronization Architecture
- **Leader:** The user's main character.
- **Followers:** The 4 bot clients.
- **Tethering:** Each bot maintains a "Tether Distance" (e.g., 5-10 yards). If the distance exceeds this, the bot triggers a pathfind to the leader's current coordinates.

### 4.2 Role-Based Positioning (Tactical Offset)
To prevent bots from overlapping (which looks robotic and causes collisions), we implement **Position Offsets**:
- **Tank Bot:** Stays 2 yards in front of the leader (if the leader is the healer/dps) or follows closely.
- **Healer Bot:** Stays 10 yards behind the leader, maintaining a clear line of sight (LoS).
- **DPS Bots:** Flank the leader on the left and right.

### 4.3 Local Avoidance
To prevent bots from bumping into each other:
- **Repulsion Logic:** If two bots are within 2 yards of each other, they apply a slight "repulsive force" to their movement vector to push them apart.

---

## 5. IMPLEMENTATION STEPS FOR DEVELOPER
1. **Mesh Loader:** Build a system to load the extracted NavMesh data into a graph structure.
2. **A* Implementation:** Write the `Pathfinder.GetPath(Vec3 start, Vec3 end)` method.
3. **Funnel Integration:** Apply the smoothing pass to the A* result.
4. **Movement Controller:** Convert the path (a list of Vec3 points) into actual `MoveForward` / `Turn` inputs using `PostMessage`.
5. **Formation Logic:** Implement the offset coordinates for the 4-bot fleet.
