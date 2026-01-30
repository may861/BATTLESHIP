# AI Battleships – README

## Game Overview
AI Battleships is a single-player Battleships game featuring a probability-based, explainable AI opponent. The game allows players not only to play against the AI, but also to understand and visualize the AI’s reasoning process through a Hint and Heatmap system.

---

## How to Run the Game
1. Download the game build or open the project in Unity.
2. Run the executable (or press Play in the Unity Editor).
3. The game starts automatically in single-player mode.

No additional configuration is required.

---

## Gameplay Instructions
- The game is turn-based.
- On your turn, click on any unvisited cell on the enemy grid to fire a shot.
- The result of the shot will be displayed as **Hit** or **Miss**.
- Turns alternate between the player and the AI.
- The game ends when all ships of one side are sunk.

---

## Hint System – How to Use
The Hint feature is designed to help players understand optimal decision-making using probability analysis.

### How to Activate the Hint
- Click the **Hint** button during your turn.

### What the Hint Does
- Calculates the probability distribution of remaining enemy ship locations.
- Displays a heatmap over the enemy board.
- Highlights recommended target areas based on probability.

### Heatmap Color Legend
- **Green** – A possible ship location with a reasonable probability.
- **Red** – A cell that has already been hit.
- **Blue** – A confirmed MISS cell.
- **White** – A cell where a ship cannot be located based on the current game state.

The Hint does not play the move automatically; it only provides guidance. The final decision remains with the player.

---

## AI Behavior Explanation
The AI uses the same probability engine as the Hint system. It evaluates all valid ship placements based on the current game state and selects the cell with the highest calculated probability.

This ensures:
- Deterministic behavior
- Transparent decision-making
- Identical logic between hints and AI moves

---

## Repository Structure (if applicable)
- `/Scripts` – Game logic and AI implementation
- `/UI` – User interface components
- `/Assets` – Visual assets
- `/Scenes` – Unity scenes

---

## Known Limitations
- Single-player mode only
- Fixed board size (10x10)
- No online multiplayer support
