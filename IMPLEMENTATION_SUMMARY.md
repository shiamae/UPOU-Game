# UPOU-Game Trash Can Hint System & Teleportation - Implementation Summary

## What Was Added

### 1. **Trash Can Hint System**

When players hover over or look at a trash can, they see helpful hints WITHOUT spoiling the answer:

**Example Display:**

```
┌─────────────────────────────────┐
│     Recyclable Bin              │
├─────────────────────────────────┤
│ For materials that belong in    │
│ the recycling stream.           │
│                                 │
│ 💡 HINT: This bin accepts       │
│ materials that can be processed │
│ and reused. Look for items made │
│ of paper, plastic, or metal.    │
│                                 │
│ Examples: Cardboard, glass      │
│ bottles, aluminum cans,         │
│ plastic bottles                 │
└─────────────────────────────────┘
```

### 2. **Teleportation Shortcuts**

Players can now teleport between trash cans:

- **Press T**: Jump to nearest trash can
- **Press 1-5**: Jump to trash can #1-5

### 3. **Help Menu**

- **Press H**: Open keyboard shortcuts help menu
- **Press ESC**: Close help menu

## Files Created

### New Scripts

1. **TrashCanTeleporter.cs**
   - Handles teleportation logic
   - Finds nearest trash can
   - Manages keyboard input for T and number keys
   - Attach to Player GameObject

2. **ShortcutHelpUI.cs** (Optional Enhancement)
   - Displays available keyboard shortcuts
   - Toggles with H key
   - Provides player guidance
   - Attach to a UI Canvas

## Files Modified

### 1. **TrashCan.cs**

Added two new methods:

- `GetHint()` - Returns helpful hints without giving away answers
- `GetCategoryExamples()` - Shows example items that belong in each bin

Example hints:

- **Recyclable**: "Look for items made of paper, plastic, or metal"
- **Biodegradable**: "Think about materials from nature that decompose naturally"
- **Hazardous**: "Look for items that require special handling and safety precautions"

### 2. **GameUIManager.cs**

Enhanced to display hints:

- Added `trashBinHintText` field for hint display
- Added `trashBinExamplesText` field for examples display
- Updated `ShowTrashBinInfo()` to show hints and examples

### 3. **PlayerPickup.cs**

Added helper methods:

- `GetHeldObject()` - Returns the object player is carrying
- `GetNearestTrashCan()` - Finds the nearest trash can in the scene

## Quick Setup Guide

### Step 1: Add TrashCanTeleporter to Player

1. Select your Player GameObject
2. Add Component > TrashCanTeleporter
3. Done! (It will auto-find all trash cans)

### Step 2: (Optional) Add ShortcutHelpUI to Canvas

1. Create an empty GameObject on your Canvas
2. Add Component > ShortcutHelpUI
3. Create a Panel for the help display
4. Assign the panel in the Inspector

### Step 3: Update Trash Bin Info Panel (UI)

In your Canvas, update the trashBinInfoPanel to display:

- Title (existing)
- Description (existing)
- Hint Text (NEW - add if missing)
- Examples Text (NEW - add if missing)

Connect these in GameUIManager Inspector

## How Players Use It

### Scenario: Unsure Which Bin to Use

1. **Look at the trash can** - Info panel appears with hints
2. **Read the hint** - Gets guidance without spoiling
3. **Check examples** - Sees what normally goes there
4. **Make informed choice** - Decides if current item belongs

### Scenario: Need to Reach Another Trash Can

1. **Press T** - Instantly teleports to nearest trash can
2. **Alternative**: **Press 1-5** - Jumps to specific trash can

### Scenario: Need Help

1. **Press H** - Opens shortcuts help menu
2. **Reviews controls** - Learns available actions
3. **Press H or ESC** - Closes menu

## Key Features

✅ **Non-Spoiling Hints**

- Hints describe what categories accept (e.g., "materials that can be recycled")
- NOT: "Put plastic bottles here"
- Players must think and learn!

✅ **Category Examples**

- Shows common items in each category
- Helps players understand waste types
- Educational and helpful

✅ **Quick Teleportation**

- Save time navigating the scene
- Learn where each trash can is located
- Press T = instant travel to nearest

✅ **Keyboard Shortcuts**

- Easy to remember (T for Teleport, 1-5 for specific)
- Help menu available with H key
- No UI clutter - info appears on hover

## Customization Options

### Change Hints

Edit in TrashCan.cs `GetHint()` method:

```csharp
case WasteCategory.Recyclable:
    return "Your custom hint here...";
```

### Change Teleport Settings

In TrashCanTeleporter.cs:

- `teleportHeight` - How high above ground player spawns
- `teleportDistance` - Distance in front of trash can (for future use)

### Change Help Menu Hotkey

In ShortcutHelpUI.cs:

```csharp
if (Keyboard.current.hKey.wasPressedThisFrame) // Change to different key
```

## Testing Checklist

- [ ] TrashCanTeleporter attached to Player
- [ ] Press T successfully teleports to nearest trash can
- [ ] Press 1-5 successfully teleports to numbered trash cans
- [ ] Looking at trash can shows info panel with hints
- [ ] Hints don't give away the answers
- [ ] Examples are helpful
- [ ] ShortcutHelpUI displays help (if installed)
- [ ] H key toggles help menu
- [ ] All text elements properly assigned in Inspector

## Troubleshooting

**Q: Teleportation not working**
A: Make sure TrashCanTeleporter is attached to Player (same GameObject with PlayerMovement)

**Q: Hints don't show**
A: Check that GameUIManager has hint text elements assigned in Inspector

**Q: Can't see which trash can is which**
A: Press H to view the help menu, or use number keys 1-5 to visit them

**Q: Want more teleportation keys**
A: Edit TrashCanTeleporter.cs Update() method to add more key inputs

## Educational Value

This system teaches players about waste classification through:

1. **Reading hints** - Learn category descriptions
2. **Seeing examples** - Understand what belongs where
3. **Trial and error** - If wrong, they see feedback
4. **Repeated exposure** - Multiple waste items teach patterns
5. **Quick navigation** - Focus on learning, not wandering

Players develop understanding WITHOUT direct answers given away!
