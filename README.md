# 背包系统 [中文](README.md) | [English](README_EN.md)

---

[TOC]
---


### 概述

本系统是一个基于 Unity 的模块化**背包与快捷栏系统**，采用 **MVC（模型-视图-控制器）** 架构设计。支持物品存储、分类浏览、排序、拖拽操作（背包与快捷栏之间）、物品使用及丢弃至游戏世界等功能。

---

### 架构说明

系统分为三层：

**Model（数据层）** — 纯数据逻辑，不依赖 Unity。

- `InventoryModel` — 管理按物品类型分类存储的物品数据，处理添加、移除、使用和丢弃逻辑。
- `HotBarModel` — 管理 8 个快捷栏槽位的数据，支持添加、移除和交换操作。
- `ItemDataModel` — 表示单个物品堆叠，记录 ScriptableObject 引用、当前数量和最大叠加数量。

**View（视图层）** — 处理所有 UI 渲染与显示逻辑。

- `InventoryView` — 渲染背包面板与物品描述信息，并从背包列表中过滤掉已在快捷栏中的物品。
- `HotBarView` — 渲染 8 个快捷栏槽位，监听 Model 数据变更事件。
- `ItemSlotView` — 单个 UI 格子，绑定 `ItemDataModel`，同步更新图标、数量文本和选中状态。
- `SlotsScrollView` — 背包网格的虚拟化滚动列表，仅渲染可见格子以保证性能。

**Controller（控制层）** — 连接 Model 和 View，处理用户输入。
- `InventoryController` — 处理键盘开关（E 键）、分类切换、排序、物品使用、丢弃和格子选中逻辑。
- `HotBarController` — 处理快捷栏的所有操作，包括添加、移除、交换和拖拽落点解析。
- `ItemDragController` — 单例。管理拖拽 UX：创建浮动拖拽图像、追踪鼠标位置、解析放置目标。
- `ItemInteractionController` — 挂载于每个 `ItemSlotView`，处理左键（选中/使用）、右键（丢弃）和拖拽事件。

**入口**

- `GameManager` — 实例化两个 Model，通过 `Initialize()` 方法将其注入所有控制器。

---

### 主要功能

| 功能 | 说明 |
|---|---|
| 物品分类 | 物品按类型分类存储（消耗品、装备等），支持"全部"视图。 |
| 排序 | 支持按默认顺序、数量、稀有度、类型、名称排序，可切换升序/降序。 |
| 快捷栏 | 8 个快捷栏格子。拖入快捷栏的物品将从背包列表中隐藏。 |
| 拖拽操作 | 支持在背包与快捷栏之间拖拽物品，也支持快捷栏内格子互换。 |
| 物品使用 | 在已选中的格子上左键单击即可使用物品，消耗品数量减少，耗尽后自动移除。 |
| 物品丢弃 | 在已选中的格子上右键单击可将物品丢入游戏世界，生成带物理组件的实体对象。 |
| 虚拟化滚动 | 背包滚动视图仅渲染可见格子对象，在大容量背包下依然保持流畅性能。 |
| 事件驱动 UI | Model 广播数据变更事件，View 按需监听并刷新，避免不必要的全量更新。 |

---

### 数据流向

```
GameManager
  └── 创建 InventoryModel + HotBarModel
        └── 注入 InventoryController、HotBarController
              └── 绑定 InventoryView、HotBarView
                    └── ItemSlotView（每个格子）
                          └── ItemInteractionController（每个格子）
                                └── ItemDragController（单例）
```

---

### 配置步骤

1. 在场景中放置 `GameManager`，在 Inspector 中指定 `InventoryController`、`HotBarController`、`ItemDragController` 的引用。
2. 将 `InventoryView` 指定给 `InventoryController`，将 `HotBarView` 指定给 `HotBarController`。
3. 将 `Canvas` 和 `dragItemPrefab`（一个简单的 Image GameObject）指定给 `ItemDragController`。
4. 在 `InventoryController` 中指定分类按钮组、排序下拉框和排序方向按钮。
5. 每个 `ItemSlotView` 预制体上应挂载 `ItemInteractionController` 组件。
6. 为每种物品创建 `ItemScriptableObject` 资产，填写 `itemType`、`itemName`、`itemSprite`、`itemMaxSuperposition`、`itemRarity`、`itemDescription` 字段。

---

### 注意事项与已知行为

- 已加入快捷栏的物品将**从背包面板中隐藏**，避免 UI 重复显示。
- 快捷栏保存的是 `ItemDataModel` 实例的**引用**，而非数据副本。
- 丢弃物品（`DropItem`）会在玩家位置（X 轴 +1）生成一个携带 `Rigidbody2D` 和 `BoxCollider2D` 的世界对象。

