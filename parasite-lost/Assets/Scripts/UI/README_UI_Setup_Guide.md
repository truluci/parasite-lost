# UI System Setup Guide

## Overview
This guide will help you set up the complete UI system for the parasite game, including the lifespan countdown bar and death screen with restart functionality.

## UI Components Created

### 1. LifespanUI.cs
- **Lifespan Bar**: Visual countdown from 5 seconds to 0
- **Color Coding**: Green → Yellow (30%) → Red (10%) with pulsing effects
- **Bonus Display**: Shows lifespan bonus when earned from rhythm battles
- **Real-time Updates**: Updates every frame during gameplay

### 2. DeathScreenUI.cs
- **Death Screen**: Appears when lifespan reaches 0
- **Animated Fade-in**: Smooth transition with staggered element appearance
- **Restart Button**: Reloads the current level
- **Main Menu Button**: Returns to main menu (currently restarts level)
- **Customizable Text**: Can change death title and message

### 3. UIManager.cs
- **Central UI Controller**: Manages all UI elements based on game state
- **State Management**: Shows/hides appropriate UI for each game state
- **Integration**: Connects with GameManager for seamless transitions

## Unity Setup Instructions

### Step 1: Create Canvas Structure

1. **Create Main Canvas**:
   - Right-click in Hierarchy → UI → Canvas
   - Name it "GameUI"
   - Set Canvas Scaler to "Scale With Screen Size"
   - Reference Resolution: 1920x1080

2. **Create UI Panels**:
   ```
   GameUI (Canvas)
   ├── LifespanPanel
   │   ├── LifespanSlider
   │   ├── LifespanText
   │   └── LifespanFill (Image)
   ├── DeathScreenPanel
   │   ├── BackgroundOverlay (Image)
   │   ├── DeathTitle (TextMeshPro)
   │   ├── DeathMessage (TextMeshPro)
   │   ├── RestartButton
   │   └── MainMenuButton
   ├── GameplayUI
   └── PauseUI
   ```

### Step 2: Lifespan Bar Setup

1. **Create LifespanPanel**:
   - Right-click GameUI → UI → Panel
   - Name: "LifespanPanel"
   - Position: Top-left corner
   - Size: 300x50

2. **Add LifespanSlider**:
   - Right-click LifespanPanel → UI → Slider
   - Name: "LifespanSlider"
   - Remove Handle Slide Area and Handle
   - Set Fill Area → Fill color to Green
   - Set Background color to Dark Gray

3. **Add LifespanText**:
   - Right-click LifespanPanel → UI → Text - TextMeshPro
   - Name: "LifespanText"
   - Text: "Lifespan: 5.0s"
   - Position: Below the slider

4. **Add LifespanUI Script**:
   - Add `LifespanUI` component to LifespanPanel
   - Assign references:
     - Lifespan Slider: LifespanSlider
     - Lifespan Text: LifespanText
     - Lifespan Fill Image: Fill Area → Fill

### Step 3: Death Screen Setup

1. **Create DeathScreenPanel**:
   - Right-click GameUI → UI → Panel
   - Name: "DeathScreenPanel"
   - Set to fill entire screen
   - Initially disable (uncheck in Inspector)

2. **Add BackgroundOverlay**:
   - Right-click DeathScreenPanel → UI → Image
   - Name: "BackgroundOverlay"
   - Color: Black with 80% alpha
   - Set to fill entire panel

3. **Add DeathTitle**:
   - Right-click DeathScreenPanel → UI → Text - TextMeshPro
   - Name: "DeathTitle"
   - Text: "YOU DIED"
   - Font Size: 72
   - Color: Red
   - Center alignment
   - Position: Upper third of screen

4. **Add DeathMessage**:
   - Right-click DeathScreenPanel → UI → Text - TextMeshPro
   - Name: "DeathMessage"
   - Text: "Your parasite's lifespan has expired!\nTry to survive longer by possessing fish."
   - Font Size: 24
   - Color: White
   - Center alignment
   - Position: Middle of screen

5. **Add RestartButton**:
   - Right-click DeathScreenPanel → UI → Button - TextMeshPro
   - Name: "RestartButton"
   - Text: "Restart Level"
   - Position: Lower third, left side

6. **Add MainMenuButton**:
   - Right-click DeathScreenPanel → UI → Button - TextMeshPro
   - Name: "MainMenuButton"
   - Text: "Main Menu"
   - Position: Lower third, right side

7. **Add DeathScreenUI Script**:
   - Add `DeathScreenUI` component to DeathScreenPanel
   - Assign all references to the script

### Step 4: Manager Setup

1. **Create UIManager GameObject**:
   - Create Empty GameObject
   - Name: "UIManager"
   - Add `UIManager` component
   - Assign references to all UI panels and components

2. **Update GameManager**:
   - The GameManager will automatically find and use the UIManager
   - No additional setup needed

### Step 5: Button Event Setup

1. **RestartButton Events**:
   - In DeathScreenUI script, the RestartButton automatically calls `RestartLevel()`
   - This is handled by the script, no manual setup needed

2. **MainMenuButton Events**:
   - Currently calls `RestartLevel()` (you can modify this later)
   - No manual setup needed

## Visual Customization

### Lifespan Bar Colors
- **Normal**: Green (when > 30% remaining)
- **Warning**: Yellow (when 10-30% remaining)
- **Danger**: Red (when < 10% remaining)
- **Pulse Effect**: Enabled for warning and danger states

### Death Screen Animation
- **Fade Duration**: 1 second
- **Text Appear Delay**: 0.5 seconds
- **Button Appear Delay**: 1 second
- **Background Alpha**: 80%

## Testing the System

1. **Start the game** - Lifespan bar should appear in top-left
2. **Wait for countdown** - Bar should change colors and pulse as it decreases
3. **Let lifespan reach 0** - Death screen should appear with animation
4. **Click Restart** - Level should reload with full lifespan
5. **Test rhythm battles** - Lifespan should pause during battles
6. **Win rhythm battles** - Should see lifespan bonus notification

## Troubleshooting

### Lifespan Bar Not Showing
- Check if LifespanUI script is attached to LifespanPanel
- Verify all references are assigned in the script
- Ensure GameManager is in the scene

### Death Screen Not Appearing
- Check if DeathScreenPanel is initially disabled
- Verify DeathScreenUI script is attached
- Ensure GameManager calls GameOver() when lifespan reaches 0

### UI Not Updating
- Check if UIManager is in the scene
- Verify GameManager has UIManager reference
- Ensure game state is set to Playing

### Buttons Not Working
- Check if button events are properly assigned
- Verify GameManager is accessible
- Check console for error messages

## Advanced Customization

### Custom Death Messages
```csharp
// In your script
UIManager.Instance.SetCustomDeathMessage("CUSTOM TITLE", "Custom death message here");
```

### Custom Lifespan Colors
- Modify colors in LifespanUI script inspector
- Adjust warning and danger thresholds
- Enable/disable pulse effects

### Additional UI Elements
- Add pause menu UI
- Add main menu UI
- Add settings UI
- All can be managed through UIManager

## Performance Notes
- UI updates every frame during gameplay
- Death screen animations use coroutines for smooth transitions
- Lifespan bonus effects are temporary and self-cleaning
- All UI elements are properly managed to avoid memory leaks
