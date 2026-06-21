# 符文传说 (Rune Legends)

> 一款基于Unity开发的2D动作游戏项目，专注于游戏系统设计与架构优化

*本游戏旨在学习交流，素材完全来自网络*

![Language](https://img.shields.io/badge/Language-C%23-239120?style=flat-square)
![Engine](https://img.shields.io/badge/Engine-Unity%202022.3.62-000000?style=flat-square)
![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)

---

## 📋 目录

1. [项目概述](#-项目概述)
2. [快速开始](#-快速开始)
3. [核心特性](#-核心特性)
4. [系统设计](#系统设计)
   - [分层状态机（HFSM）](#分层状态机hfsm)
   - [攻击系统](#攻击系统)
   - [架势系统](#架势系统)
5. [开发计划](#-开发计划)
6. [设计思路/笔记](#-设计思路笔记)

---

## 🎮 项目概述

**符文传说**是一个展示现代游戏架构设计的项目案例，通过实现一个完整的2D动作游戏框架，验证优秀的设计模式在游戏开发中的应用。

### 技术栈
- **游戏引擎**: Unity 2022.3.62
- **主要语言**: C# (83%), ShaderLab (14.5%), HLSL (2.5%)
- **开发模式**: 组件化 + 系统化架构
- **设计模式**: 状态机模式、策略模式、对象池模式等

### 项目目标
- ✅ 建立**健壮的HFSM状态机框架**，支持复杂的人物控制
- ✅ 实现**高度可配置的攻击系统**，支持连招和伤害倍率
- ✅ 设计**架势/破防系统**，增加游戏的策略性
- 🔄 建立**UI与存档系统**
- 🔄 完善**敌人AI和行为树系统**

---
## 🚀 快速开始

### 环境要求
- Unity 2022.3.62 或更高版本
- .NET Framework 4.x

### 克隆项目
```bash
git clone https://github.com/hollywuchi/Rune-Legends.git
```

### 打开项目
1. 打开 Unity Hub
2. 选择 "Open project at path"
3. 选择项目文件夹

### 运行游戏
1. 打开 Scenes 文件夹
2. 双击 `Initialition.unity` 打开场景
<!-- 3. TODO:需要重新定位 -->
4. 按 Play 按钮运行

### 基本控制

| 按键 | 功能 |
|------|------|
| `A` / `D` | 左移 / 右移 |
| `Space` | 跳跃 |
| `Space + Space` | 二段跳 |
| `Shift` | 冲刺 |
| `鼠标左键` | 攻击 |
| `鼠标右键` | 格挡 |
| `S` | 休息 |
| `长按 E` | 治疗符文 |
| `长按 R` | 战意符文 |
| `长按 S` | 灵魂铭刻 |
| `长按 F` | 流光闪击 |



### 项目中应用的设计模式
- **状态模式** - 分层状态机
- **策略模式** - 不同的移动和攻击策略
- **观察者模式** - 事件系统
- **对象池模式** - 特效和敌人管理

### 代码规范
- 遵循 C# 命名规范(PascalCase for classes, camelCase for methods) 
- 添加必要的注释和文档
- 确保代码通过编译且没有警告
- 为新功能添加测试

---
## ✨ 核心特性

### 🎯 高级人物控制系统
- **15+种基础动作**：待机、行走、跑步、冲刺、跳跃、二段跳、扒墙等
- **4+种技能动作**：治疗符文、流光闪击等
- **分层状态机**：清晰的状态层次，易于扩展

### ⚔️ 完整的攻击系统
- **三段连招**：精确的攻击判定和位移
- **攻击判定框**：精确的碰撞检测
- **位移补偿**：攻击时的精确移动控制
- **伤害系统**：灵活的伤害倍率配置

### 🛡️ 架势/破防系统
- **架势值累积**：通过受击累积架势值
- **自动恢复机制**：破防后自动恢复
- **策略性防御**：增加对战的深度

### 🎨 视觉反馈系统
- **特效系统**：通过事件广播的特效管理
- **动画驱动**：基于帧事件的动画动画状态机

---

## 系统设计

### 分层状态机（HFSM）

#### 核心架构

采用**分层有限状态机(Hierarchical Finite State Machine)** 设计，通过将复杂的行为分解为多个层级的状态，实现高效的人物控制。

**核心组件：**
1. **Player** - 管理器，所有类的出发点
2. **PlayerContext** - 上下文，纯通信使用
3. **PlayerStateMachine** - 状态机管理，保证状态转换的唯一性
4. **PlayerState** - 状态基类，所有状态的父类
5. **Transition** - 状态转换请求，解耦状态与状态机
6. **PlayerServices** - 组件封装，提供服务注入

#### 状态层次结构

```
根状态机 (PlayerState)
├── 地面状态 (GroundState) - 大状态
│   ├── 待机状态 (IdleState)
│   ├── 行走状态 (WalkState)
│   ├── 跑步状态 (RunState)
│   ├── 冲刺状态 (DashState)
│   ├── 休息状态 (RestState)
│   └── 攻击状态 (AttackState)
├── 空中状态 (AirState) - 大状态
│   ├── 跳跃状态 (JumpState)
│   ├── 二段跳状态 (DoubleJumpState)
│   ├── 空中攻击状态（AirAttackState）
│   ├── 空中冲刺状态 (AirDashState)
│   ├── 扒墙状态 (WallClimbState)
│   └── 登墙跳状态 (WallJumpState)
├── 攻击状态 (AttackState) - 大状态
│   ├── 第一段攻击（AttackCombo1State）
│   ├── 第二段攻击（AttackCombo2State）
│   └── 第三段攻击（AttackCombo3State）
├── 防御状态 (BlockState) - 大状态
├── 弹反状态 (ParryState) - 大状态
├── 破防状态 (PostureBrokenState) - 大状态
├── 受伤状态 (HurtState)
└── 死亡状态 (DeadState)
```

#### 状态转换流程

```
玩家输入 → 当前状态逻辑更新
  ↓
状态评估转换条件 → 返回转换请求 (Transition)
  ↓
状态机检查转换请求
  ↓
是否存在待处理转换？
  ├─ 是 → 等待当前帧完成
  └─ 否 → 执行状态切换
  ↓
新状态初始化 → 下一帧循环
```

#### 关键设计思想

- **单一职责**：每个状态只负责自己的逻辑
- **松耦合**：状态通过上下文而非直接调用进行通信
- **转换保护**：状态机确保任何时刻只有一个有效的转换
- **防御性编程**：防止父子状态争抢控制权

---

### 攻击系统

#### 系统概述
完整的三段连招系统，支持连招窗口、伤害倍率、位移补偿等功能。
*ps：伤害倍率未实装，未进行数值设计*

#### 核心组件

**状态类：**
- `PlayerAttackState` - 攻击基类，管理通用逻辑
- `PlayerAttackCombo1State` - 第一段攻击
- `PlayerAttackCombo2State` - 第二段攻击
- `PlayerAttackCombo3State` - 第三段终结技

**组件类：**
- `Attack` - 攻击组件：敌人与玩家共用
- `Character` - 角色组件：可互动实体通用

**数据与配置：**
- `PlayerContext` - 攻击数据：`AttackComboIndex`, `CanCombo`, `IsAttacking`
- `Attack` - 攻击参数：
  - `attackDamage` - 基础伤害值
  - `knockbackForce` - 击退距离
- `Player` - 动画帧事件：
  - `Animation_Move` - 动画帧事件实现更精准各段位移
  - `attackComboWindow` - 动画帧事件控制是否可以进行下一段攻击

#### 连招流程图

```
玩家按下攻击键 (鼠标左键)
  ↓
PlayerGroundState 检测输入
  ↓
转换到 AttackCombo1State
  ↓
  播放第一段攻击动画
  应用位移补偿
  启用攻击判定框
  ↓
动画事件：打开连招窗口 (ComboWindowOpen)
  ↓
在窗口内按下攻击键？
├─ 是 → 转换到 AttackCombo2State
│         播放第二段攻击动画
│         应用位移补偿
│         重复上述流程
├─ 否 → 关闭连招窗口
└─ 结束 → 返回 IdleState
  ↓
第三段结束后 → 返回 IdleState
```

#### 关键动画事件配置

在 `PlayerAnimatorDriver`和`Player` 中配置以下事件：

| 位置 | 事件名称 | 触发时机 | 作用 |
|------|---|---------|------|
|`Player`| `Animation_Callback` | 攻击动画完成 | 触发状态转换回Idle |
|`Player`|`Animation_ComboWindowOpen`|连招窗口打开|允许输入下一段攻击|
|`Player`|`Animation_ComboWindowClose`|连招窗口关闭|禁止连招|
|`PlayerAnimatorDriver`| `SetAttackCombo` | 进入连招状态时 | 允许输入下一段攻击 |
|`PlayerAnimatorDriver`| `TriggerAttack` | 进入攻击状态时 | 触发下一Combo |
|`PlayerAnimatorDriver`| `SetIsAttacking` | 进入攻击状态时 | 当前处于攻击状态 |

---

### 架势系统

#### 系统概述

**架势系统**模拟游戏中的"防御破坏"机制。当玩家或敌人受到攻击时，架势值逐步累积。当架势值达到上限时，目标进入 **"破防"状态**，期间无法防御，可以被连续攻击。

#### 核心参数

```csharp
public class PostureSystem : MonoBehaviour
{
    public float maxPosture = 100f;                 // 最大架势值
    public float postureRecoveryRate = 15f;         // 破防后每秒恢复的架势值
    public float postureRecoveryDelay = 1.5f;       // 破防后恢复延迟时间
    
    public float currentPosture;                    // 当前架势值
    public bool isBroken;                           // 是否处于破防状态
    public float recoveryTimer;                     // 恢复计时器
}
```

#### 工作流程

**架势值累积阶段：**
```
玩家受到攻击 → AddPosture(amount)
  ↓
currentPosture += amount
  ↓
currentPosture >= maxPosture?
├─ 是 → BreakPosture() - 进入破防状态
│        ① isBroken = true
│        ② 触发 OnPostureBroken 事件
│        ③ 敌人可以进行连续攻击
└─ 否 → 继续正常防御
```

**完美格挡判定**
```
玩家按下（或长按）格挡按键 → 进入格挡状态
  ↓
Player脚本中动画帧事件Animation_ParryWindowOn判定是否在完美格挡关键帧内
├─ 是 → 使用Attack通知：
|         玩家减少架势值
|         敌人增加架势值
└─ 否 → 继续正常防御
```

**破防恢复阶段：**
```
进入破防状态 (isBroken = true)
  ↓
每帧更新 (Update)
  ↓
recoveryTimer -= Time.deltaTime
  ↓
recoveryTimer <= 0?
├─ 是 → 开始恢复架势值
│        currentPosture -= postureRecoveryRate * Time.deltaTime
└─ 否 → 等待延迟结束
  ↓
currentPosture <= 0?
├─ 是 → ResetPosture() - 恢复正常状态
│        ① isBroken = false
│        ② currentPosture = 0
│        ③ 触发 OnPostureRecovered 事件
└─ 否 → 继续恢复
```

#### 关键特性

| 功能 | 描述 |
|------|------|
| **架势累积** | 通过 `AddPosture()` 增加架势值，上限为 `maxPosture` |
| **架势减少** | 通过 `ReducePosture()` 减少架势值（弹反成功） |
| **破防判定** | 架势值达到100%时自动触发 |
| **自动恢复** | 破防后延迟指定时间自动恢复 |
| **事件系统** | 通过 `OnPostureBroken` 和 `OnPostureRecovered` 事件通知其他系统 |

---

## 📋 开发计划

### 已完成 ✅
- [x] **多层人物状态机** 
  - [x] 地面状态（待机、行走、跑步、冲刺、转身）
  - [x] 跳跃状态（二段跳、跳跃冲刺、扒墙、登墙跳）
  - [x] 攻击状态（基础三段连招、连招窗口、伤害倍率、位移补偿）
- [x] **攻击系统**
  - [x] 三段连招系统
  - [x] 攻击判定框
  - [x] 伤害倍率系统
- [x] **架势系统**
  - [x] 架势值累积
  - [x] 自动恢复机制
- [x] **技能系统**
  - [x] 神圣治疗
  - [x] 霹雳一闪
- [x] **防御状态**
  - [x] 格挡判定
  - [x] 格挡伤害减少
  - [x] 弹反系统
- [x] **更多技能**
  - [x] 神圣干预
  - [x] 神圣屏障
  - [x] 光明之冠
- [x] **UI与菜单**
  - [x] 主菜单
  - [x] 游戏UI（生命值、架势值、技能冷却）
  - [x] 暂停菜单
  - [x] 设置菜单

### 计划中 📅
- [ ] **敌人系统**
  - [ ] 敌人AI行为树
  - [ ] 敌人状态机
  - [ ] 敌人配置系统
- [ ] **存档系统**
  - [ ] 游戏存档
  - [ ] 玩家数据持久化
- [ ] **音效系统**
  - [ ] 背景音乐
  - [ ] 音效管理
- [ ] **关卡设计**
  - [ ] 关卡1：教程
  - [ ] 关卡2：基础战斗
  - [ ] Boss关卡

---

## 📖 设计思路/笔记

### 关于分层状态机（HFSM）

#### 为什么需要分层状态机？

新角色拥有 **15+种基础动作** 和 **4+种技能动作**，如果用传统平面状态机实现，会出现：
- 状态爆炸：状态数量成指数增长
- 转换复杂：从任何状态都可能转向任何其他状态
- 代码耦合：难以维护和扩展

分层状态机通过**将状态分组**解决这个问题：
```
根状态
├── 地面状态
│   ├── 待机
│   ├── 行走
│   └── 攻击
├── 空中状态
│   ├── 跳跃
│   ├── 扒墙
└── ...
```

#### 核心设计原则

**优秀的状态机应该满足：**
1. 状态只调用能力接口（而非直接修改变量）
2. 状态不直接操作其他模块的变量
3. 状态不直接读取所有变量（通过接口获取）

**之前重构前的问题：**
- Player变成了"上帝对象"，任何状态都可以访问和修改其变量
- 大状态和子状态争抢控制权，需要防御性代码
- 状态转换逻辑不清晰，耦合度高

**重构后的解决方案：**
1. **PlayerServices** - 组件封装，防止冗余注册
2. **PlayerContext** - 纯数据模块，存储计时器、计数器、输入等
3. **PlayerMotor2D** - Rigidbody2D封装，统一速度控制
4. **PlayerAnimatorDriver** - Animator封装，统一动画控制
5. **PlayerConfig** - ScriptableObject配置，独立于运行时
6. **PlayerFxSpeaker** - 特效广播器，通过事件解耦特效管理

#### 状态转换的执行流程

```csharp
// 1. 初始化（仅一次）
player.Initialize();

// 2. 每帧循环
void Update() {
    stateMachine.Tick();
}

// 3. 状态机Tick流程
public void Tick() {
    // 步骤1：当前状态提出"意图"（返回转换请求）
    var transition = currentState.LogicUpdate();
    
    // 步骤2：检查是否有优先级更高的状态转换请求
    if (_pendingId == PlayerStateId.None && transition.HasTarget) {
        _pendingId = transition.Target;
    }
    
    // 步骤3：如果有待处理的转换，立即执行
    if (_pendingId != PlayerStateId.None) {
        ChangeStateInternal(_pendingId);
    }
}

// 4. 状态内部也可以直接请求转换（高优先级）
public void RequestChangeState(PlayerStateId newStateId) {
    if (newStateId == PlayerStateId.None) return;
    if (_pendingId != PlayerStateId.None) return;  // 本帧已有转换
    _pendingId = newStateId;
}
```

这种设计确保：
- ✅ 每帧最多转换一次
- ✅ 优先级高的请求能被正确处理
- ✅ 防止状态争抢和冲突

---

### 攻击系统设计详解

#### 连招窗口机制

连招窗口是指在一段攻击动画中，玩家可以输入下一段攻击的时间段。

```csharp
// 在Animator中配置的时间轴（以第一段为例）
Timeline: |--[动画播放]--|
Events:   |--[准备]--[窗口打开]--[窗口关闭]--[结束]--|
Time:     0s      0.3s      0.5s       0.7s        1.0s

// 玩家可以在0.5s-0.7s之间按下攻击键
// 超时则连招断裂，返回Idle
```

#### 位移补偿系统

攻击时需要对玩家进行位移（冲刺距离），确保攻击的视觉感受。

```csharp
public float[] comboXMoveDistance = {
    0.5f,    // 第一段：向前移动0.5个单位
    0.7f,    // 第二段：向前移动0.7个单位
    1.0f,    // 第三段：向前移动1.0个单位
};

// 在状态中执行
motor.SetVelocity(new Vector2(moveDistance * direction, 0f));
```

---

## 👨‍💻 作者

**hollywuchi** - *初始工作*

---

## 🙏 致谢

- 游戏美术资源来自网络
- 灵感来自多款优秀的2D动作游戏
