# Tetris

A classic Tetris game built with Unity. It has a main menu, a level select screen, and the full game with sound effects, music, scoring, and a game over screen.

## Requirements

- **Unity 2022.3.44f1** (LTS). Install it through [Unity Hub](https://unity.com/download). Other 2022.3 versions usually work, but 2022.3.44f1 is the one this project was made with.
- **Git**, to clone the project.

## Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd tetris
   ```
2. **Install Unity 2022.3.44f1** through Unity Hub. If you don't see this version, use *Installs → Install Editor → Archive*.
3. **Add the project** in Unity Hub with *Add → Add project from disk*, then select the cloned `tetris` folder.
4. **Open it.** The first launch takes a few minutes while Unity rebuilds the `Library/` folder. That folder is not in git on purpose.
5. **Open the starting scene** `Assets/Scenes/MainMenu.unity` and press **Play**.

### Build a standalone game

1. Go to *File → Build Settings* and check that `MainMenu`, `Tetris` and `LevelsMenu` are all in the list (`MainMenu` first).
2. Pick your platform (for example *Windows, Mac, Linux*) and click **Build**.

## Controls

| Key | Action |
| --- | --- |
| `A` / `D` | Move left / right |
| `S` | Soft drop (hold) |
| `Space` | Hard drop |
| `Q` / `E` | Rotate counter-clockwise / clockwise |

## Scoring

| Action | Points |
| --- | --- |
| 1 / 2 / 3 / 4 lines at once | 100 / 300 / 500 / 800 |
| Soft drop | 1 per cell |
| Hard drop | 2 per cell |

Your last score is saved with `PlayerPrefs` when the game ends.

## Project structure

```
Assets/
  Scenes/    MainMenu, LevelsMenu, Tetris, LevelUpScreen
  Scripts/
    Board.cs         Playfield, spawning, line clears, game over
    Piece.cs         The falling piece: input, movement, rotation, locking
    Tetromino.cs     Tetromino types and per-piece data
    Data.cs          Piece shapes and wall kick tables (SRS)
    ScoreBoard.cs    Score tracking and score UI
    GameOverScreen.cs, MenuScreen.cs, LevelsMenu.cs, LevelUpScreen.cs   UI screens
  Sprites/, Tiles/   Art
```

## How it works

- **Board** owns the `Tilemap`, which is both the drawing surface and the collision grid. The falling piece is written into the tilemap like any locked block. It is removed before it moves and redrawn afterwards, so it never collides with itself.
- **Piece** reads input every frame but only touches the tilemap on frames where something happens: a key press, a held key repeating, or a gravity step. Idle frames cost almost nothing.
- **Rotation** follows the Super Rotation System (SRS). Cells rotate 90° with `(x, y) → (y, -x)`. The I and O pieces rotate around an offset center. If the rotated position is blocked, the five wall kick offsets from `Data.cs` are tried in order before the rotation is cancelled.
- **Line clears** happen in a single pass. The whole board is read with `GetTilesBlock`, full rows are dropped, the remaining rows are shifted down, and the result is written back with one `SetTilesBlock` call.
- **Locking:** a piece locks when it has gone `lockDelay` seconds without moving. Any successful move or rotation resets that timer.

### Tuning

These values can be changed in the Inspector:

| Component | Field | Default | Meaning |
| --- | --- | --- | --- |
| Piece | `stepDelay` | 1 | Seconds between automatic drops (gravity) |
| Piece | `moveDelay` | 0.1 | Repeat rate when holding a movement key |
| Piece | `lockDelay` | 0.5 | Time a resting piece waits before locking |
| Board | `boardSize` | 10 × 20 | Playfield width and height |
| Board | `spawnPosition` | (-1, 8) | Where new pieces appear |

## Known limitations

- The selected level is saved by the Levels menu, but the game doesn't use it yet, so every level plays the same.
- The `LevelUpScreen` scene isn't in the build settings, so it can't be reached in a built game.
- Pieces are picked fully at random, not with the 7-bag system that modern Tetris uses.

## Troubleshooting

- **"Project has invalid dependencies" or compile errors on first open:** let Unity finish importing, or choose *Reimport All* from the Assets menu.
- **Wrong Unity version warning:** choose *Continue* only if you are using another 2022.3 release. Otherwise install 2022.3.44f1.
- **Missing text or fonts:** go to *Window → TextMeshPro → Import TMP Essential Resources*.
