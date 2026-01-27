# SparkMart - GOAP Store Simulation

A supermarket simulation built on Goal-Oriented Action Planning (GOAP) in Unity 6.

## Quick Setup

### 1. Create New Unity Project
- Open Unity Hub
- Create new 3D project with Unity 6
- Name it "SparkMart"

### 2. Import Scripts
- Copy the entire `Scripts` folder into `Assets/Scripts`

### 3. Create Tags
Go to **Edit > Project Settings > Tags and Layers** and add:
- `Cart`
- `CheckoutLane`
- `SelfCheckout`
- `Restroom`
- `BreakRoom`
- `GeneralShelf`
- `ProduceShelf`
- `FrozenShelf`
- `ElectronicsShelf`
- `ClothingShelf`
- `Exit`
- `CustomerSpawn`

### 4. Create ScriptableObjects

**Shelf Data:**
1. Right-click in Project > Create > SparkMart > Shelf Data
2. Name it "GeneralShelfData"
3. Set: Shelf Type = General, Max Stock = 100, Restock Threshold = 20

**Customer Persona:**
1. Right-click in Project > Create > SparkMart > Customer Persona
2. Name it "QuickShopperPersona"
3. Set: Persona Type = QuickShopper, Min List = 1, Max List = 3

### 5. Scene Setup

Create these GameObjects:

1. **SparkWorld** (Empty)
   - Add `SparkWorld` component

2. **MetricsManager** (Empty)
   - Add `MetricsManager` component

3. **TimeScaleController** (Empty)
   - Add `TimeScaleController` component

4. **CustomerSpawner** (Empty at entrance)
   - Add `CustomerSpawner` component
   - Tag as `CustomerSpawn`

5. **Exit** (Empty at exit)
   - Tag as `Exit`

6. **Shelf** (Cube or model)
   - Add `Shelf` component
   - Assign ShelfData
   - Tag as `GeneralShelf`
   - Add child empty named "Destination"

7. **CheckoutLane** (Cube or model)
   - Add `CheckoutLane` component
   - Tag as `CheckoutLane`
   - Add child empty named "Destination"

8. **Cart** (Cube or model)
   - Tag as `Cart`

9. **BreakRoom** (Empty)
   - Tag as `BreakRoom`

### 6. Create Prefabs

**Customer Prefab:**
1. Create Capsule
2. Add: NavMeshAgent, Customer
3. Add actions: GoToShelf, PickUpItem, GoToCheckout, CustomerCheckout, CustomerGoHome, GetCart
4. Save as prefab
5. Assign to CustomerSpawner

**Employee Prefab:**
1. Create Capsule (different color)
2. Add: NavMeshAgent, Employee
3. Add actions: GoToCheckoutLane, OperateCheckout, RestockShelf, TakeBreak
4. Save as prefab

### 7. NavMesh
1. Mark floor as Navigation Static
2. Window > AI > Navigation > Bake

### 8. Wire Up
- Assign Customer prefab to CustomerSpawner
- Add PersonaSpawnConfig with your QuickShopperPersona
- Assign ShelfData to Shelf components

### 9. Play!

## Keyboard Shortcuts
- `Space` - Pause/Resume
- `+` / `-` - Speed up/down
- `1-5` - Preset speeds (0.5x, 1x, 2x, 5x, 10x)

## File Structure
```
Scripts/
├── GOAP/
│   ├── GAction.cs
│   ├── GAgent.cs
│   ├── GInventory.cs
│   ├── GPlanner.cs
│   ├── SparkWorld.cs
│   └── WorldStates.cs
├── Agents/
│   ├── Customer.cs
│   ├── CustomerPersona.cs
│   └── Employee.cs
├── Actions/
│   ├── Customer/
│   │   ├── GetCart.cs
│   │   ├── GoToShelf.cs
│   │   ├── PickUpItem.cs
│   │   ├── GoToCheckout.cs
│   │   ├── CustomerCheckout.cs
│   │   └── CustomerGoHome.cs
│   └── Employee/
│       ├── GoToCheckoutLane.cs
│       ├── OperateCheckout.cs
│       ├── RestockShelf.cs
│       └── TakeBreak.cs
├── Resources/
│   ├── Shelf.cs
│   ├── ShelfData.cs
│   └── CheckoutLane.cs
├── Systems/
│   ├── CustomerSpawner.cs
│   ├── MetricsManager.cs
│   └── TimeScaleController.cs
└── UI/
    └── MetricsDashboardUI.cs
```
