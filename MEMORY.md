# Unity Project Memory

## Project
- Unity 6 URP project: `D:\Play4\game4`
- Active scene: `Assets/Scenes/Game.unity`
- Unity MCP relay is configured in `.vscode/mcp.json` with the Windows relay and project path targeting this project.
- Unity MCP connection was tested successfully through console access and editor command execution.

## Scene Setup
- `Player` exists in the `Game` scene.
- `Player` has `CharacterController` and `PlayerMovementController`.
- `Player/stick 1` has an Animator with the bool parameter `running`; movement sets this parameter for Idle/Run animation transitions.
- `NPC1` exists in the `Game` scene and has a `CharacterController` and `FishingNetAssist`.
- `NPC1/Canvas` contains the RTL text object `Text - RTLTMP`.
- `NPC1/Canvas` is hidden at startup.
- No balance Slider was found in the scene during setup; assign/create one and connect it to `FishingNetAssist.balanceSlider`.

## Scripts
### `Assets/Script/PlayerMovementController.cs`
- WASD and arrow-key movement.
- Uses `CharacterController` and gravity.
- Rotates the Player toward movement.
- Sets child Animator bool `running`.

### `Assets/Script/FishingNetAssist.cs`
- Automatically finds `Player`, NPC Canvas, and RTL NPC label when references are missing.
- Quest activates after 10 seconds and displays Arabic prompt: `اضغط E للمساعدة`.
- Press `E` within 3 units to start the mini-game.
- Balance slider starts at `0.5` and decreases by `0.3 * Time.deltaTime`.
- Press `R` adds `0.1` to the slider, then the value is clamped to `[0, 1]`.
- `timeInZone` increases while the slider is strictly between `0.4` and `0.6`.
- Displays stability text in the format `Stability: 3.5s / 10.0s`.
- Automatically creates an `RTLTextMeshPro` timer object if `timerDisplayText` is null.
- Current timer font size is `50`, centered, and white.
- Timer is hidden at startup and after win/loss, and shown during the mini-game.
- Win calls `AddFoodStock(20)` and logs completion.
- Current script fails after `totalTaskTime > 14f`; change to `30f` if the original 30-second requirement is still desired.

## Important Current Caveats
- `PositionTimerDisplay()` currently has its position assignment and camera-facing rotation commented out, so the dynamically created timer may not be positioned above the slider.
- The `timeInZone` reset when the slider leaves the stability zone is currently commented out. Restore `else timeInZone = 0f;` if the strict reset behavior is required.
- `sliderHeightAbovePlayer` is currently `2f`, but the timer positioning requirement was `1.5` units above the slider.
- The dynamically created timer is parented to the slider Canvas when available, otherwise the first Canvas, otherwise `NPC1`.

## Validation History
- Unity script validation and editor command compilation passed for the controller and fishing script at earlier stages.
- Unity console had no errors during the last integration checks.
