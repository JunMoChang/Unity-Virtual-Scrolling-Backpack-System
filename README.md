# Unity Inventory System  [English](README.md) | [中文](README_CN.md)

A feature-rich inventory management system for Unity games with hotbar support, drag-and-drop functionality, and flexible item categorization.

[TOC]



## Features

### Core Functionality
- **Multi-category Inventory**: Organize items by type (Weapon, Consumable, Material, Other)
- **Hotbar System**: Quick access bar with 8 slots for frequently used items
- **Drag & Drop**: Intuitive item management through drag-and-drop interface
- **Item Stacking**: Automatic stacking of identical items up to max stack size
- **Sorting Options**: Sort by quantity, rarity, type, or name (ascending/descending)
- **Virtual Scrolling**: Efficient rendering for large inventories using object pooling

### UI Features
- Real-time item information display (name, description, sprite)
- Visual feedback for selected items
- Empty slot placeholders
- Quantity indicators
- Category filtering buttons

## System Architecture

### Main Components

#### InventoryManager
Central controller for the inventory system.

**Key Responsibilities:**
- Manages item data containers for different categories
- Handles category switching and UI updates
- Coordinates between inventory and hotbar systems
- Displays item information panel

**Public Methods:**
```csharp
int AddItem(ItemScriptableObject itemSo, int quantity)
void UpdateCurrentDataContainer()
ItemDataContainer GetCurrentDataContainer()
void SetCurrentSelectedSlot(ItemSlotView slotView)
```

#### HotBarManager
Manages the quick-access hotbar with 8 slots.

**Key Responsibilities:**
- Maintains hotbar item references
- Handles item addition/removal from hotbar
- Swaps items between hotbar slots
- Syncs with inventory system

**Public Methods:**
```csharp
bool AddToHotBar(ItemData itemData, int slotIndex)
bool AddToHotBar(ItemSlotView sourceSlot, ItemSlotView hotBarSlot)
void RemoveFromHotBar(int slotIndex)
HashSet<ItemData> GetHotBarDataSet()
List<ItemData> GetAllHotBarData()
void RefreshDisplay()
```

#### ItemDataContainer
Stores and manages items for a specific category.

**Key Features:**
- Default capacity: 30 items
- Automatic stack management
- Full inventory detection

**Public Methods:**
```csharp
int AddItem(ItemScriptableObject itemSo, int quantity)
void RemoveItem(ItemData data)
void SetDataCapacity(int maxCapacity)
```

#### ItemSlotView
Individual item slot UI component.

**Slot Types:**
- `Inventory`: Regular inventory slots
- `HotBar`: Quick access hotbar slots

**Interactions:**
- **Left Click**: Select item / Use consumable
- **Right Click**: Drop one item into game world
- **Drag**: Move items between slots

#### ItemDragHandler
Handles drag-and-drop operations.

**Features:**
- Visual feedback during drag
- Automatic slot detection
- Hotbar-inventory interaction
- Canvas-based positioning

#### SlotsScrollView
Virtual scrolling system for efficient UI rendering.

**Performance Features:**
- Object pooling for slot reuse
- Dynamic content sizing
- Only renders visible slots
- Configurable grid layout

#### ItemDataSort
Sorting system with multiple criteria.

**Sort Options:**
- Default (pickup order)
- Quantity
- Rarity
- Type
- Name

**Sort Orders:**
- Ascending
- Descending

## Data Structures

### ItemData
Runtime item instance data.

```csharp
public class ItemData
{
    public ItemScriptableObject itemSo;
    public int storageItemQuantity;
    public bool isFull;
}
```

### ItemScriptableObject (Referenced)

Define items using ScriptableObjects with properties:
- Item name, description, sprite
- Item type (Weapon, Consumable, Material, Other)
- Item rarity (Common, Valuable, Rare)
- Max stack size

## Setup Instructions

### 1. Scene Setup
1. Create a Canvas with `InventoryManager` component
2. Assign UI references:
   - Scroll view container
   - Item info panel (name, description, image)
   - Category buttons
   - Sort dropdown and button

### 2. Hotbar Setup
1. Create hotbar UI with 8 slot objects
2. Add `HotBarManager` component
3. Assign `ItemSlotView` components to hotbar slots
4. Set each slot's `slotType` to `HotBar`
5. Set `hotBarIndex` (0-7) for each slot

### 3. Scroll View Setup
1. Create ScrollRect with `SlotsScrollView` component
2. Configure grid settings:
   - Column count
   - Cell size
   - Spacing
   - Padding
3. Assign slot prefab

### 4. Drag Handler Setup
1. Add `ItemDragHandler` to Canvas
2. Assign canvas reference
3. Create drag item prefab (Image component required)

### 5. Item Slot Prefab
Create prefab with:
- `ItemSlotView` component
- Image component (item sprite)
- TextMeshPro (quantity display)
- Selected panel GameObject
- Default slot sprite

### 6. Item Pickup
Add `Item` component to world items:
```csharp
public class Item : MonoBehaviour
{
    public int quantity;
    public ItemScriptableObject itemSo;
}
```

## Usage Examples

### Adding Items Programmatically
```csharp
// Add 5 health potions
ItemScriptableObject healthPotion = // your item SO
int remaining = InventoryManager.Instance.AddItem(healthPotion, 5);
```

### Adding Items to Hotbar
```csharp
// Add item to hotbar slot 0
ItemData itemData = // your item data
HotBarManager.Instance.AddToHotBar(itemData, 0);
```

### Updating Display
```csharp
// Refresh inventory display
InventoryManager.Instance.UpdateCurrentDataContainer();

// Refresh hotbar display
HotBarManager.Instance.RefreshDisplay();
```

## Configuration

### Inventory Settings
- **Default Capacity**: 30 items per category (modifiable in `ItemDataContainer`)
- **Hotbar Slots**: 8 slots (hardcoded in `HotBarManager`)

### Scroll View Settings
- **Column Count**: Configurable grid columns
- **Cell Size**: Individual slot dimensions
- **Spacing**: Gap between slots
- **Padding**: Container margins

### Keybindings
- **E Key**: Toggle inventory menu (modifiable in `InventoryManager.Update()`)

## Technical Notes

### Performance Optimizations
1. **Object Pooling**: Slots are pooled and reused in scroll view
2. **Virtual Scrolling**: Only visible slots are rendered
3. **HashSet Lookup**: Fast hotbar item detection using HashSet
4. **Filtered Display**: Hotbar items excluded from inventory view

### Item Stacking Logic
- Items stack automatically when picked up
- Full stacks are marked with `isFull` flag
- Overflow items create new stacks or return to world

### Hotbar-Inventory Sync
- Hotbar items are excluded from main inventory display
- Items can be dragged between hotbar and inventory
- Removing from hotbar returns items to inventory view

## Dependencies

### Required Unity Packages
- TextMeshPro
- Unity UI

### Required Namespaces
```csharp
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;
```

## Extending the System

### Adding New Item Categories
1. Add enum value to `ItemScriptableObject.ItemType`
2. Create container in `InventoryManager.Awake()`
3. Add category button to UI
4. Update button registration logic

### Custom Item Actions
Override `UseItem()` in `ItemSlotView`:
```csharp
private bool UseItem()
{
    // Custom logic based on item type
    switch (currentData.itemSo.itemType)
    {
        case ItemScriptableObject.ItemType.Consumable:
            // Consume logic
            break;
        // Add more cases
    }
}
```

### Modifying Sort Criteria
Add new sort type to `ItemSortType` enum and implement in `ItemDataSort.DataListSort()`.

## Known Limitations

1. Item quantities cannot be split manually (only drop one at a time)
2. Hotbar size is fixed at 8 slots
3. No item comparison or tooltip system
4. Item world objects require 2D physics setup

## License

MIT
