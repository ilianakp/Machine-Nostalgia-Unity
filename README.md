# Machine Nostalgia video game

This Unity project enables interactive visualization of cities using point cloud data. It provides functionality for switching between different views (first-person and third-person), limiting player movement within defined boundaries, and toggling between game and information screens.
![machine](https://github.com/ilianakp/Machine-Nostalgia-Unity/assets/37414826/be841b7f-6e31-416e-a57a-810e6e134b7e)
![interaction_logic](https://github.com/ilianakp/Machine-Nostalgia-Unity/assets/37414826/d49551bd-56a2-41da-b25f-b2a5219b3915)

## Scripts Overview

### 1. **InstantiateLibrary.cs**

- **Purpose**: Handles instantiation and positioning of city elements (buildings, roads, vegetation) based on point cloud data.
- **Functionality**:
  - Loads prefabs from `library_london` and `library_kyoto` directories.
  - Instantiates prefabs at specified positions and orientations.
  - Moves instantiated objects (`gos_ldn`, `gos_kyo`) to correct scene locations.
  - Updates bars and percentages based on player interaction.

### 2. **MouseLook.cs**

- **Purpose**: Controls first-person camera view and mouse movement.
- **Functionality**:
  - Locks cursor to center of screen.
  - Captures mouse movement for horizontal and vertical view rotation.

### 3. **PlayerMovement.cs**

- **Purpose**: Manages basic player movement controls.
- **Functionality**:
  - Moves player character using arrow keys (right, left, up, down).

### 4. **PlayerView.cs**

- **Purpose**: Manages camera switching and limits player/camera movement.
- **Functionality**:
  - Toggles between `FirstViewCamera` and `ThirdViewCamera` using 'C' key.
  - Restricts player and camera within `boxborder` boundaries.
  - Adjusts camera position to stay within boundaries (`varia` margin).

### 5. **SwitchCanvas.cs**

- **Purpose**: Handles toggling between game and information screens.
- **Functionality**:
  - Toggles visibility of `GameCanvas` and `InfoCanvas` using 'I' key.
  - Ensures `InfoCanvas` is hidden and `GameCanvas` is shown by default.

## Using Other Cities and Point Clouds

To use this project with other cities and point clouds:

1. **Assets/Resources Directory**:
   - Place city prefabs (buildings, roads, etc.) under `Assets/Resources`.
   - Organize into subdirectories (`library_london`, `library_kyoto`, etc.).

2. **Index Files**:
   - Create corresponding index files (`match_build.txt`, `match_road.txt`) under `Assets/Index`.
   - Files should map prefabs to counterparts in other cities/point clouds.
   - Each line in index files should match data loading and instantiation logic in `InstantiateLibrary.cs`.

3. **Script Adjustments**:
   - Modify `InstantiateLibrary.cs` to load prefabs and read index files specific to new city/point cloud data.
   - Ensure `movegos` function positions and orients objects based on new city specifications.

## Team Members
Iliana Papadopoulou, Xudong Liu, Tengfei Zhang

## Digital exhibition
https://bpro2021.bartlettarchucl.com/architectural-computation/xudong-liu-iliana-papadopoulou-tengfei-zhang
