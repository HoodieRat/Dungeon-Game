# 3D Console Raycasting Game

A retro-style 3D game using raycasting, rendered directly in the console. The game features basic movement, interactive objects, doors, keys, and procedurally generated rooms and hallways. Inspired by the early days of 3D games, it combines a simplified rendering engine with modern C# concepts.

## Features
- **Raycasting Engine**: Provides a simple 3D effect using text-based graphics.
- **Procedural Map Generation**: Randomly generates rooms and hallways for a new experience each time.
- **Interactive Objects**: Doors that can be opened with keys, keys that can be collected, and walls that can be interacted with.
- **Player Movement**: Move through the maze-like environment using basic controls.
- **Minimap**: A small minimap to track your position and explore the environment.
- **Console Graphics**: Uses text characters to simulate walls, floors, ceilings, and objects.

## Controls
| Key   | Action                                |
|-------|---------------------------------------|
| W     | Move forward                          |
| S     | Move backward                         |
| Q     | Strafe left                           |
| E     | Strafe right                          |
| A     | Rotate left                           |
| D     | Rotate right                          |
| Space | Interact with doors or keys           |
| Esc   | Exit the game                         |

## How to Play
- **Goal**: Explore the randomly generated maze of rooms and hallways, find keys, and unlock doors to progress.
- **Keys**: Collect keys scattered throughout the map to unlock doors.
- **Doors**: Locked doors can be opened with a key if you have one in your inventory. Simply face a locked door and press `Space` to unlock it.
- **Minimap**: The minimap on the right side of the console shows your current position (`P`) and helps you navigate through the rooms and corridors.

## Setup and Installation
1. **Clone the Repository**:
    ```bash
    git clone https://github.com/yourusername/3d-console-raycasting-game.git
    cd 3d-console-raycasting-game
    ```

2. **Build the Project**:
   - Open the project in your preferred C# IDE (like Visual Studio).
   - Restore any dependencies and build the solution.

3. **Run the Game**:
   - Start the game by running the executable or starting the project from your IDE.

## Project Structure

- **`Renderer.cs`**: Handles the rendering of the 3D environment, including raycasting logic, drawing walls, floors, ceilings, and interactive objects like keys and doors.
- **`Player.cs`**: Contains the player's movement, interaction logic, and directional handling.
- **`Map.cs`**: Manages the procedural generation of rooms, hallways, and the placement of interactive objects like keys and doors.
- **`Program.cs`**: Entry point of the game, initializes the game loop and manages input.
- **`CellType.cs`**: Enum representing the different types of cells that can exist on the map, such as empty space, walls, doors, and keys.
- **`Direction.cs`**: Enum representing the possible directions the player can face (e.g., North, South, East, West).
- **`Game.cs`**: Manages the game loop, player actions, and interactions with the map.
- **`Rectangle.cs`**: A utility class used to define rectangular regions, such as rooms, for the procedural map generation.
- **`README.md`**: This file, providing an overview of the game and setup instructions.

## Contributing
Contributions are welcome! If you'd like to improve the game, fix bugs, or add new features, follow these steps:

1. **Fork the Repository**.
2. **Create a New Branch** for your feature or bug fix.
3. **Make Your Changes** and commit them.
4. **Push to Your Branch**.
5. **Create a Pull Request** with a description of your changes.

## Known Issues
- **"Finning" Effect**: Side walls in hallways can appear layered or inconsistent in appearance. This is a visual artifact due to how raycasting is processed in a text-based console.
- **Out of Bounds Errors**: Rapid interactions or movements might cause index errors if the player attempts to move beyond the map boundaries.

## Future Plans
- **Enhance Rendering**: Improve wall and floor shading for a smoother visual experience.
- **Additional Interactions**: Add new objects or enemies to increase the depth of gameplay.
- **Customizable Controls**: Allow players to redefine controls for a more personalized experience.
- **Story Mode**: Implement a simple storyline or quest system to give more purpose to the exploration.

## License
This project is licensed under the MIT License. Feel free to use, modify, and distribute this game.
