# Trash Can Hint System & Teleportation Setup Guide

## Overview

This guide explains how to set up the new trash can hint UI system and teleportation shortcuts in your UPOU-Game project.

## Features Implemented

### 1. **Trash Can Hint & Info Display**

When the player looks at a trash can, they see:

- **Bin Name**: The type of trash can (e.g., "Recyclable Bin")
- **Description**: What the bin is for
- **Hint**: A helpful hint about what the bin accepts (without revealing the answer)
- **Examples**: Common items that belong in this bin

### 2. **Teleportation Shortcuts**

- **Press T**: Teleport to the nearest trash can
- **Press 1-5**: Teleport to trash cans #1-5 (in scene order)

## Setup Instructions

### Step 1: Add TrashCanTeleporter Component to Player

1. Open your main scene in the Unity Editor
2. Select the **Player** GameObject (or the object that has the `PlayerMovement` script)
3. In the Inspector, click **Add Component**
4. Search for and add **TrashCanTeleporter**
5. Adjust settings if needed:
   - **Teleport Height**: How high above the ground to spawn (default: 0.5)
   - **Teleport Distance**: Distance in front of trash can (currently not used, for future enhancement)

### Step 2: Create the Trash Bin Info Panel in Canvas

> **NOTE**: The `trashBinInfoPanel` does NOT exist in your game yet. You need to CREATE it!

**👉 DETAILED GUIDE**: Follow [CREATE_TRASHBIN_PANEL.md](CREATE_TRASHBIN_PANEL.md) for step-by-step instructions (takes 5-10 minutes)

**Quick Overview** - You will create:

```
Canvas
└─ trashBinInfoPanel (You create this panel)
   ├─ Title (Shows "Recyclable Bin", etc.)
   ├─ Description (Shows what the bin is for)
   ├─ HintText (NEW) - Shows helpful hints
   └─ ExamplesText (NEW) - Shows example items
```

**What the panel looks like:**

```
┌─────────────────────────────────┐
│  Recyclable Bin                 │  ← Title
│                                 │
│  For materials that belong in   │
│  the recycling stream.          │  ← Description
│                                 │
│  💡 HINT: This bin accepts      │
│  materials that can be          │
│  processed and reused. Look     │  ← HintText
│  for items made of paper,       │
│  plastic, or metal.             │
│                                 │
│  Examples: Cardboard, glass     │
│  bottles, aluminum cans,        │  ← ExamplesText
│  plastic bottles                │
└─────────────────────────────────┘
```

### Step 3: Connect UI Elements to GameUIManager

### Step 3: Connect UI Elements to GameUIManager

Once you've created the panel (from Step 2):

1. Select the **GameUIManager** GameObject in the scene
2. In the Inspector, find the **Trash Bin Info** section
3. Drag and drop the text elements you created:
   - **Trash Bin Info Panel**: Drag `trashBinInfoPanel`
   - **Trash Bin Title Text**: Drag the `Title` text element
   - **Trash Bin Description Text**: Drag the `Description` text element
   - **Trash Bin Hint Text**: Drag the `HintText` element (NEW)
   - **Trash Bin Examples Text**: Drag the `ExamplesText` element (NEW)

**Visual Guide** - In GameUIManager Inspector:

```
Trash Bin Info
├─ Trash Bin Info Panel: [trashBinInfoPanel]
├─ Trash Bin Title Text: [Title]
├─ Trash Bin Description Text: [Description]
├─ Trash Bin Hint Text: [HintText]
└─ Trash Bin Examples Text: [ExamplesText]
```

1. Click Play to test - look at a trash can and the panel should appear!

### Step 4: (Optional) Customize Hints in TrashCan Script

The hints are defined in `TrashCan.cs` methods:

- `GetHint()` - Returns hints without giving away answers
- `GetCategoryExamples()` - Returns examples of what belongs

You can customize these messages by editing the TrashCan.cs file:

```csharp
public string GetHint()
{
    switch (acceptedCategory)
    {
        case WasteCategory.Recyclable:
            return "Your custom hint here...";
        // ... more categories
    }
}
```

## How to Use

### For Players

1. **View Trash Can Hints**: Look at (hover over) any trash can to see its information panel with hints
2. **Quick Teleport to Nearest Bin**: Press **T** while holding an item to instantly teleport to the nearest trash can
3. **Teleport to Specific Bins**: Press **1-5** to jump to trash can #1-5 (in order they appear in the scene)

### For Developers

**Show Hints Programmatically:**

```csharp
// Get the nearest trash can
TrashCan nearest = GetComponent<PlayerPickup>().GetNearestTrashCan();

// Show its info
if (nearest != null)
{
    GameUIManager.Instance.ShowTrashBinInfo(nearest);
}
```

**Get Hint Text:**

```csharp
TrashCan trashCan = GetComponent<TrashCan>();
string hint = trashCan.GetHint();
string examples = trashCan.GetCategoryExamples();
```

## Scripts Modified/Created

1. **TrashCanTeleporter.cs** (NEW)
   - Handles teleportation to nearest or specific trash cans
   - Attached to Player GameObject
   - Listens for T, 1-5 keyboard inputs

2. **TrashCan.cs** (MODIFIED)
   - Added `GetHint()` method
   - Added `GetCategoryExamples()` method
   - These provide hints without revealing answers

3. **GameUIManager.cs** (MODIFIED)
   - Added `trashBinHintText` field
   - Added `trashBinExamplesText` field
   - Updated `ShowTrashBinInfo()` to display hints and examples

4. **PlayerPickup.cs** (MODIFIED)
   - Added `GetHeldObject()` method
   - Added `GetNearestTrashCan()` method
   - These support the teleportation system

## Troubleshooting

**Problem**: Teleportation doesn't work

- **Solution**: Make sure TrashCanTeleporter is attached to the Player and that there are trash cans in the scene

**Problem**: Hints don't show up

- **Solution**: Check that GameUIManager has the hint text elements assigned in the Inspector

**Problem**: Trash cans not found

- **Solution**: Ensure your trash cans have the `TrashCan` component attached and are active in the scene

## Future Enhancements

Potential features to add:

- [ ] Sound effect when teleporting
- [ ] Visual effect (flash, fade) when teleporting
- [ ] UI overlay showing which trash can number is which (press U to show legend)
- [ ] Teleport animation
- [ ] Different colors for different waste categories
- [ ] Proximity alert when near a trash can
