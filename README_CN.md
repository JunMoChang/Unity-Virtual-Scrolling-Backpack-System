# Unity 背包系统 [English](README.md) | [中文](README_CN.md)

一个功能丰富的 Unity 游戏背包管理系统，支持快捷栏、拖放功能和灵活的物品分类。

[TOC]

## 功能特性

### 核心功能

- **多分类背包**：按类型组织物品（武器、消耗品、材料、其他）
- **快捷栏系统**：8个槽位的快速访问栏，用于常用物品
- **拖放功能**：通过拖放界面实现直观的物品管理
- **物品堆叠**：自动堆叠相同物品至最大堆叠数量
- **排序选项**：按数量、稀有度、类型或名称排序（升序/降序）
- **虚拟滚动**：使用对象池实现大型背包的高效渲染

### UI 功能

- 实时物品信息显示（名称、描述、图标）
- 选中物品的视觉反馈
- 空槽位占位符
- 数量指示器
- 分类筛选按钮

## 系统架构

### 主要组件

#### InventoryManager（背包管理器）
背包系统的中央控制器。

**主要职责：**
- 管理不同分类的物品数据容器
- 处理分类切换和 UI 更新
- 协调背包和快捷栏系统
- 显示物品信息面板

**公共方法：**
```csharp
int AddItem(ItemScriptableObject itemSo, int quantity)
void UpdateCurrentDataContainer()
ItemDataContainer GetCurrentDataContainer()
void SetCurrentSelectedSlot(ItemSlotView slotView)
```

#### HotBarManager（快捷栏管理器）
管理拥有 8 个槽位的快速访问快捷栏。

**主要职责：**
- 维护快捷栏物品引用
- 处理快捷栏物品的添加/移除
- 在快捷栏槽位之间交换物品
- 与背包系统同步

**公共方法：**
```csharp
bool AddToHotBar(ItemData itemData, int slotIndex)
bool AddToHotBar(ItemSlotView sourceSlot, ItemSlotView hotBarSlot)
void RemoveFromHotBar(int slotIndex)
HashSet<ItemData> GetHotBarDataSet()
List<ItemData> GetAllHotBarData()
void RefreshDisplay()
```

#### ItemDataContainer（物品数据容器）
存储和管理特定分类的物品。

**主要特性：**
- 默认容量：30 个物品
- 自动堆叠管理
- 背包满员检测

**公共方法：**
```csharp
int AddItem(ItemScriptableObject itemSo, int quantity)
void RemoveItem(ItemData data)
void SetDataCapacity(int maxCapacity)
```

#### ItemSlotView（物品槽位视图）
单个物品槽位的 UI 组件。

**槽位类型：**
- `Inventory`：普通背包槽位
- `HotBar`：快速访问快捷栏槽位

**交互方式：**

- **左键点击**：选中物品 / 使用消耗品
- **右键点击**：丢弃一个物品到游戏世界
- **拖拽**：在槽位之间移动物品

#### ItemDragHandler（物品拖拽处理器）
处理拖放操作。

**功能特性：**
- 拖拽时的视觉反馈
- 自动槽位检测
- 快捷栏-背包交互
- 基于画布的定位

#### SlotsScrollView（槽位滚动视图）

用于高效 UI 渲染的虚拟滚动系统。

**性能特性：**

- 对象池用于槽位复用
- 动态内容大小调整
- 仅渲染可见槽位
- 可配置的网格布局

#### ItemDataSort（物品数据排序）

具有多种条件的排序系统。

**排序选项：**

- 默认（拾取顺序）
- 数量
- 稀有度
- 类型
- 名称

**排序顺序：**
- 升序
- 降序

## 数据结构

### ItemData（物品数据）
运行时物品实例数据。

```csharp
public class ItemData
{
    public ItemScriptableObject itemSo;
    public int storageItemQuantity;
    public bool isFull;
}
```

### ItemScriptableObject（物品配置对象）

使用 ScriptableObject 定义物品，包含以下属性：
- 物品名称、描述、图标
- 物品类型（武器、消耗品、材料、其他）
- 物品稀有度（普通、珍贵、稀有）
- 最大堆叠数量

## 设置说明

### 1. 场景设置
1. 创建带有 `InventoryManager` 组件的 Canvas
2. 分配 UI 引用：
   - 滚动视图容器
   - 物品信息面板（名称、描述、图片）
   - 分类按钮
   - 排序下拉菜单和按钮

### 2. 快捷栏设置

1. 创建包含 8 个槽位对象的快捷栏 UI
2. 添加 `HotBarManager` 组件
3. 将 `ItemSlotView` 组件分配给快捷栏槽位
4. 将每个槽位的 `slotType` 设置为 `HotBar`
5. 为每个槽位设置 `hotBarIndex`（0-7）

### 3. 滚动视图设置

1. 创建带有 `SlotsScrollView` 组件的 ScrollRect
2. 配置网格设置：
   - 列数
   - 单元格大小
   - 间距
   - 内边距
3. 分配槽位预制体

### 4. 拖拽处理器设置
1. 将 `ItemDragHandler` 添加到 Canvas
2. 分配画布引用
3. 创建拖拽物品预制体（需要 Image 组件）

### 5. 物品槽位预制体
创建包含以下内容的预制体：
- `ItemSlotView` 组件
- Image 组件（物品图标）
- TextMeshPro（数量显示）
- 选中面板 GameObject
- 默认槽位图标

### 6. 物品拾取
为世界物品添加 `Item` 组件：
```csharp
public class Item : MonoBehaviour
{
    public int quantity;
    public ItemScriptableObject itemSo;
}
```

## 使用示例

### 通过代码添加物品
```csharp
// 添加 5 个生命药水
ItemScriptableObject healthPotion = // 你的物品 SO
int remaining = InventoryManager.Instance.AddItem(healthPotion, 5);
```

### 添加物品到快捷栏
```csharp
// 将物品添加到快捷栏槽位 0
ItemData itemData = // 你的物品数据
HotBarManager.Instance.AddToHotBar(itemData, 0);
```

### 更新显示
```csharp
// 刷新背包显示
InventoryManager.Instance.UpdateCurrentDataContainer();

// 刷新快捷栏显示
HotBarManager.Instance.RefreshDisplay();
```

## 配置选项

### 背包设置
- **默认容量**：每个分类 30 个物品（可在 `ItemDataContainer` 中修改）
- **快捷栏槽位**：8 个槽位（在 `HotBarManager` 中硬编码）

### 滚动视图设置

- **列数**：可配置的网格列数
- **单元格大小**：单个槽位尺寸
- **间距**：槽位之间的间隙
- **内边距**：容器边距

### 键位绑定
- **E 键**：切换背包菜单（可在 `InventoryManager.Update()` 中修改）

## 技术说明

### 性能优化

1. **对象池**：滚动视图中的槽位被池化和复用
2. **虚拟滚动**：仅渲染可见槽位
3. **HashSet 查找**：使用 HashSet 实现快速的快捷栏物品检测
4. **过滤显示**：快捷栏物品从背包视图中排除

### 物品堆叠逻辑
- 拾取时物品自动堆叠
- 满堆叠标记 `isFull` 标志
- 溢出物品创建新堆叠或返回世界

### 快捷栏-背包同步
- 快捷栏物品从主背包显示中排除
- 物品可以在快捷栏和背包之间拖拽
- 从快捷栏移除会使物品返回背包视图

## 依赖项

### 所需 Unity 包

- TextMeshPro
- Unity UI

### 所需命名空间
```csharp
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;
```

## 扩展系统

### 添加新物品分类
1. 向 `ItemScriptableObject.ItemType` 添加枚举值
2. 在 `InventoryManager.Awake()` 中创建容器
3. 向 UI 添加分类按钮
4. 更新按钮注册逻辑

### 自定义物品动作

在 `ItemSlotView` 中重写 `UseItem()`：
```csharp
private bool UseItem()
{
    // 基于物品类型的自定义逻辑
    switch (currentData.itemSo.itemType)
    {
        case ItemScriptableObject.ItemType.Consumable:
            // 消耗逻辑
            break;
        // 添加更多情况
    }
}
```

### 修改排序条件
向 `ItemSortType` 枚举添加新排序类型，并在 `ItemDataSort.DataListSort()` 中实现。

## 已知限制

1. 物品数量不能手动分割（一次只能丢弃一个）
2. 快捷栏大小固定为 8 个槽位
3. 没有物品比较或工具提示系统
4. 物品世界对象需要 2D 物理设置

## 许可证

MIT
