# System Architecture - Trash Can Hint & Teleportation System

## Overview Diagram

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                         UPOU-GAME HINT SYSTEM                               │
│                                                                              │
│  ┌──────────────────────────────────────────────────────────────────────┐   │
│  │ INPUT LAYER - Player Input Handling                                 │   │
│  ├──────────────────────────────────────────────────────────────────────┤   │
│  │                                                                       │   │
│  │  TrashCanTeleporter.cs (NEW)                ShortcutHelpUI.cs (NEW)  │   │
│  │  ├─ Listens for T key                       ├─ Listens for H key    │   │
│  │  ├─ Listens for 1-5 keys                    ├─ Listens for ESC key  │   │
│  │  └─ Calls teleport methods                  └─ Toggles help panel   │   │
│  │                                                                       │   │
│  └──────────────────────────────────────────────────────────────────────┘   │
│                                                                              │
│  ┌──────────────────────────────────────────────────────────────────────┐   │
│  │ LOGIC LAYER - Data & Processing                                     │   │
│  ├──────────────────────────────────────────────────────────────────────┤   │
│  │                                                                       │   │
│  │  TrashCan.cs (MODIFIED)                  PlayerPickup.cs (MODIFIED) │   │
│  │  ├─ GetHint()                           ├─ GetNearestTrashCan()    │   │
│  │  ├─ GetCategoryExamples()               └─ GetHeldObject()         │   │
│  │  └─ GetDescription()                                                │   │
│  │                                                                       │   │
│  │  TrashCanTeleporter.cs (NEW)                                        │   │
│  │  ├─ FindNearestTrashCan()                                           │   │
│  │  ├─ TeleportToPosition()                                            │   │
│  │  └─ TeleportToTrashCan(index)                                       │   │
│  │                                                                       │   │
│  └──────────────────────────────────────────────────────────────────────┘   │
│                                                                              │
│  ┌──────────────────────────────────────────────────────────────────────┐   │
│  │ UI LAYER - Display Management                                       │   │
│  ├──────────────────────────────────────────────────────────────────────┤   │
│  │                                                                       │   │
│  │  GameUIManager.cs (MODIFIED)                                        │   │
│  │  ├─ ShowTrashBinInfo()  ← Shows hints + examples                   │   │
│  │  ├─ ShowHint()                                                      │   │
│  │  ├─ ShowResult()                                                    │   │
│  │  └─ ShowEducation()                                                 │   │
│  │                                                                       │   │
│  │  ShortcutHelpUI.cs (NEW)                                            │   │
│  │  ├─ ShowHelp()                                                      │   │
│  │  ├─ HideHelp()                                                      │   │
│  │  └─ ToggleHelp()                                                    │   │
│  │                                                                       │   │
│  └──────────────────────────────────────────────────────────────────────┘   │
│                                                                              │
│  ┌──────────────────────────────────────────────────────────────────────┐   │
│  │ PRESENTATION LAYER - Canvas & UI Elements                           │   │
│  ├──────────────────────────────────────────────────────────────────────┤   │
│  │                                                                       │   │
│  │  Canvas                                                              │   │
│  │  ├─ trashBinInfoPanel                                               │   │
│  │  │  ├─ Title (Recyclable Bin, etc.)                                │   │
│  │  │  ├─ Description                                                 │   │
│  │  │  ├─ HintText (NEW) 💡                                          │   │
│  │  │  └─ ExamplesText (NEW)                                         │   │
│  │  │                                                                  │   │
│  │  └─ HelpPanel (NEW)                                                │   │
│  │     └─ HelpText                                                    │   │
│  │                                                                       │   │
│  └──────────────────────────────────────────────────────────────────────┘   │
│                                                                              │
│  ┌──────────────────────────────────────────────────────────────────────┐   │
│  │ SCENE OBJECTS                                                        │   │
│  ├──────────────────────────────────────────────────────────────────────┤   │
│  │                                                                       │   │
│  │  Player                              TrashCan (1..N)                │   │
│  │  ├─ PlayerMovement (existing)        ├─ TrashCan component        │   │
│  │  ├─ PlayerPickup (existing)          ├─ Position in scene         │   │
│  │  ├─ TrashCanTeleporter (NEW) ◄────┐  └─ Category (Recyclable, etc)│   │
│  │  └─ CharacterController (existing)  │                              │   │
│  │                                     └──────────────────────────┐    │   │
│  │                                                                 │    │   │
│  │  GameUIManager (existing)          ShortcutHelpUIManager (NEW) │    │   │
│  │  └─ Updated to show hints ◄─────────────────────────────────┘    │   │
│  │                                                                       │   │
│  └──────────────────────────────────────────────────────────────────────┘   │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
```

## Data Flow Diagrams

### Flow 1: Player Presses T (Teleportation)

```
Player presses T
    ↓
TrashCanTeleporter.Update() detects T
    ↓
TeleportToNearestTrashCan()
    ↓
FindNearestTrashCan()
    ├─ Scans all TrashCan components
    ├─ Calculates distance to each
    └─ Returns closest one
    ↓
TeleportToPosition(position)
    ├─ Disable CharacterController
    ├─ Set transform.position
    ├─ Enable CharacterController
    └─ Log success
    ↓
ShowTeleportNotification()
    ├─ Debug log
    └─ Show trash bin info in UI
    ↓
Player is now at trash can!
```

### Flow 2: Player Looks at Trash Can (Hover)

```
PlayerPickup.Update() runs
    ↓
HandleTrashBinHover()
    ├─ Raycast from camera center
    └─ Check if hit something
    ↓
TrashCan found?
    ├─ YES: Get TrashCan component
    │   ↓
    │   GameUIManager.ShowTrashBinInfo(trashCan)
    │   ├─ trashBinInfoPanel.SetActive(true)
    │   ├─ Set Title: trashCan.GetDisplayName()
    │   ├─ Set Description: trashCan.GetDescription()
    │   ├─ Set Hint: trashCan.GetHint() ◄─ NEW
    │   └─ Set Examples: trashCan.GetCategoryExamples() ◄─ NEW
    │   ↓
    │   UI Panel displays with all info
    │
    └─ NO: Hide trash bin info
```

### Flow 3: Player Presses H (Help Menu)

```
Player presses H
    ↓
ShortcutHelpUI.Update() detects H
    ↓
ToggleHelp()
    ├─ Is help visible?
    ├─ YES: HideHelp()
    │   └─ helpPanel.SetActive(false)
    └─ NO: ShowHelp()
        ├─ helpPanel.SetActive(true)
        └─ helpText.text = GetHelpText()
            └─ Displays all shortcuts
```

## Component Interaction Map

```
TrashCanTeleporter
├─ Uses: PlayerMovement.CharacterController
├─ Uses: TrashCan[] (FindObjectsByType)
├─ Calls: GameUIManager.ShowTrashBinInfo()
└─ Reads: Keyboard input (T, 1-5)

PlayerPickup  
├─ Uses: TrashCan (on raycast hit)
├─ Calls: GameUIManager.ShowTrashBinInfo()
├─ Calls: GameUIManager.ShowHint()
├─ Provides: GetNearestTrashCan()
└─ Reads: Mouse.current.leftButton

GameUIManager
├─ Updates: UI Text elements
├─ Calls: TrashCan.GetHint()
├─ Calls: TrashCan.GetCategoryExamples()
├─ Calls: TrashCan.GetDescription()
├─ Calls: TrashCan.GetDisplayName()
└─ Manages: All UI panels

ShortcutHelpUI
├─ Manages: Help panel visibility
├─ Reads: Keyboard input (H, ESC)
└─ Updates: Help text content

TrashCan
├─ Provides: GetHint()
├─ Provides: GetCategoryExamples()
├─ Provides: GetDisplayName()
├─ Provides: GetDescription()
└─ Owns: acceptedCategory
```

## Method Call Hierarchy

```
Update() - Main loop
├─ TrashCanTeleporter.Update()
│  ├─ Keyboard.current.tKey.wasPressedThisFrame
│  │  └─ TeleportToNearestTrashCan()
│  │     ├─ FindNearestTrashCan()
│  │     ├─ TeleportToPosition()
│  │     └─ ShowTeleportNotification()
│  │        └─ GameUIManager.ShowTrashBinInfo()
│  │           ├─ TrashCan.GetDisplayName()
│  │           ├─ TrashCan.GetDescription()
│  │           ├─ TrashCan.GetHint()
│  │           └─ TrashCan.GetCategoryExamples()
│  │
│  └─ Keyboard.current.digit(1-5)Key.wasPressedThisFrame
│     └─ TeleportToTrashCan(index)
│
├─ PlayerPickup.Update()
│  ├─ HandleTrashBinHover()
│  │  ├─ Physics.Raycast()
│  │  └─ GameUIManager.ShowTrashBinInfo()
│  │     └─ [Same as above]
│  │
│  └─ Mouse.current.leftButton.wasPressedThisFrame
│     ├─ TrashCan.TryDispose()
│     └─ GameUIManager.ShowResult()
│
└─ ShortcutHelpUI.Update()
   ├─ Keyboard.current.hKey.wasPressedThisFrame
   │  └─ ToggleHelp()
   │     ├─ ShowHelp()
   │     └─ HideHelp()
   │
   └─ Keyboard.current.escapeKey.wasPressedThisFrame
      └─ HideHelp()
```

## State Diagram - Help Menu

```
                ┌─────────────────────┐
                │  Help Menu States   │
                └─────────────────────┘
                           │
                ┌──────────┴──────────┐
                │                     │
            ┌───▼────┐            ┌──▼────┐
            │ HIDDEN │            │VISIBLE│
            └───┬────┘            └──┬────┘
                │                    │
            H key│                H key
                │                    │
    ESC key─────┘                    │
                │                    │
                └────────┬───────────┘
                    ESC key
```

## State Diagram - Teleportation

```
         ┌──────────────────────────┐
         │   Teleportation Ready    │
         └──────────────┬───────────┘
                        │
                        │ T pressed
                        │
              ┌─────────▼────────────┐
              │ Find Nearest Trash   │
              └─────────┬────────────┘
                        │
                        │ Found
                        │
              ┌─────────▼────────────┐
              │  Teleport Player     │
              └─────────┬────────────┘
                        │
                        │ Position Set
                        │
              ┌─────────▼────────────┐
              │ Show Bin Information │
              └─────────┬────────────┘
                        │
                        │ Info Displayed
                        │
         ┌──────────────▼───────────┐
         │  Teleportation Complete  │
         └──────────────────────────┘
```

## File Dependencies

```
TrashCanTeleporter.cs
├─ Depends on: GameUIManager.cs
├─ Depends on: TrashCan.cs
└─ No special dependencies

ShortcutHelpUI.cs
├─ Depends on: UnityEngine.UI
└─ No game-specific dependencies

TrashCan.cs (MODIFIED)
├─ Already existed
└─ Added GetHint() and GetCategoryExamples()

GameUIManager.cs (MODIFIED)
├─ Already existed
├─ Added trashBinHintText field
├─ Added trashBinExamplesText field
└─ Updated ShowTrashBinInfo()

PlayerPickup.cs (MODIFIED)
├─ Already existed
├─ Added GetHeldObject()
└─ Added GetNearestTrashCan()
```

## Performance Considerations

```
TrashCanTeleporter
├─ FindObjectsByType<TrashCan>() - Called once in Awake ✓
├─ Distance calculations - O(n) where n = trash cans
│  └─ Should be fast (< 10 trash cans typical)
└─ Physics.Raycast - Only in player input handling ✓

PlayerPickup  
├─ Physics.Raycast - Every frame for hover
│  └─ Optimized: Only checks center of screen ✓
└─ GetNearestTrashCan() - Called rarely (optional feature) ✓

GameUIManager
├─ Text updates - Only when hover/teleport changes ✓
└─ No performance concerns

ShortcutHelpUI
├─ Keyboard input check - Every frame but fast ✓
└─ No performance concerns
```

---

**Note**: Green ✓ = Optimized, No issues expected
