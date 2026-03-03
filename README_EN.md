# Inventory System — README

---

[TOC]
---
### Overview

This is a modular **Inventory & HotBar System** built for Unity using the **MVC (Model-View-Controller)** pattern. It supports item storage, categorization, sorting, drag-and-drop between the inventory and hotbar, item usage, and item dropping into the game world.

---

### Architecture

The system is divided into three layers:

**Model** — Pure data, no Unity dependencies.
- `InventoryModel` — Manages item data organized by item type. Handles adding, removing, using, and dropping items.
- `HotBarModel` — Manages an 8-slot hotbar array. Handles adding, removing, and swapping items.
- `ItemDataModel` — Represents a single item stack. Tracks item ScriptableObject reference, quantity, and max stack size.

**View** — Handles all UI rendering and display logic.
- `InventoryView` — Renders inventory panels, item description info, and filters out hotbar items from the inventory display.
- `HotBarView` — Renders the 8 hotbar slots and listens to model change events.
- `ItemSlotView` — A single UI slot. Binds to an `ItemDataModel` and updates its icon, quantity text, and selected state.
- `SlotsScrollView` — A virtualized scroll list for the inventory grid. Only renders visible slots for performance.

**Controller** — Bridges models and views, and handles user input.
- `InventoryController` — Handles keyboard toggle (E), category switching, sorting, item use, item drop, and slot selection.
- `HotBarController` — Handles all hotbar operations including add, remove, swap, and drag-drop resolution.
- `ItemDragController` — Singleton. Manages drag-and-drop UX: creates a floating drag image, tracks mouse position, and resolves the drop target.
- `ItemInteractionController` — Attached to each `ItemSlotView`. Handles left-click (select/use), right-click (drop), and drag events.

**Entry Point**
- `GameManager` — Instantiates both models, injects them into all controllers via `Initialize()` calls.

---

### Key Features

| Feature            | Description                                                  |
| ------------------ | ------------------------------------------------------------ |
| Item categories    | Items are stored per-type (Consumable, Equipment, etc.) with an "All" view. |
| Sorting            | Supports sorting by Default, Quantity, Rarity, Type, and Name, in ascending or descending order. |
| Hotbar             | 8-slot hotbar. Items dragged from inventory appear in the hotbar and are hidden from the inventory list. |
| Drag & Drop        | Drag items between inventory ↔ hotbar slots. Swapping within the hotbar is also supported. |
| Item use           | Left-click a selected item to use it. Consumables decrease in quantity and are removed when empty. |
| Item drop          | Right-click a selected item to drop it into the game world as a physics object. |
| Virtualized scroll | The inventory scroll view only renders visible slot objects, keeping performance stable for large inventories. |
| Event-driven UI    | Models broadcast change events; views listen and refresh only when necessary. |

---

### Data Flow

```
GameManager
  └── creates InventoryModel + HotBarModel
        └── passed into InventoryController, HotBarController
              └── bound to InventoryView, HotBarView
                    └── ItemSlotView (per slot)
                          └── ItemInteractionController (per slot)
                                └── ItemDragController (singleton)
```

---

### Setup

1. Place `GameManager` in your scene and assign `InventoryController`, `HotBarController`, and `ItemDragController` references in the Inspector.
2. Assign `InventoryView` to `InventoryController`, and `HotBarView` to `HotBarController`.
3. Assign `Canvas` and `dragItemPrefab` (a simple Image GameObject) to `ItemDragController`.
4. Assign category buttons, sort dropdown, and sort order button to `InventoryController`.
5. Each `ItemSlotView` prefab should have an `ItemInteractionController` component attached.
6. Create `ItemScriptableObject` assets for each item with `itemType`, `itemName`, `itemSprite`, `itemMaxSuperposition`, `itemRarity`, and `itemDescription` fields populated.

---

### Notes & Known Behaviors

- Items assigned to the hotbar are **hidden from the inventory panel** to avoid duplication in the UI.
- The hotbar holds references to `ItemDataModel` instances — it does **not** copy data.
- Dropping an item (`DropItem`) spawns a world object at the player's position (`+1` on X) with a `Rigidbody2D` and `BoxCollider2D`.