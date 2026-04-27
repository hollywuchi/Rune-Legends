符文传说
=================================

*本游戏旨在学习交流，素材完全来自网络*

本项目使用Unity2022.3.62开发

## 目录
1. [符文传说](#符文传说)
   1. [目录](#目录)
   2. [开发计划](#开发计划)
   3. [设计思路/笔记](#设计思路笔记)
         1. [关于玩家和敌人的分层状态机（HFSM）](#关于玩家和敌人的分层状态机hfsm)
         2. [对于状态机重构的一些思考](#对于状态机重构的一些思考)

## 开发计划
* [x] **地面状态**
    * [x] 待机状态
    * [x] 行走/跑步状态
    * [x] 冲刺状态
    * [x] 转身状态
* [ ] **跳跃状态**
    * [x] 二段跳
    * [x] 跳跃冲刺
    * [x] 扒墙
    * [ ] 登墙跳
* [ ] **休息状态**
* [ ] **攻击状态**
* [ ] **防御状态**
* [ ] **各种技能**


## 设计思路/笔记
 #### 关于玩家和敌人的分层状态机（HFSM）
  * 新的主角拥有超过*15种基础动作*，*以及超过4种技能动作*，因此，需要一个强大的人物控制器来实现这一操作，防止不断添加的动作与特效让整个系统崩溃
  * 一个标准的面向对象分层状态机（HFSM）框架，核心由三个部分组成：状态机管理者 (StateMachine)、状态基类 (Base State)，以及玩家上下文 (Player/Context)。
  * 分层则是在大状态下分小的状态
  * 主要状态机与分层展示
  > * 地面状态（大状态）
  >   * 待机状态
  >   * 休息状态
  >   * 行走状态
  >   * 跑步状态
  >   * 攻击状态
  >   * 冲刺状态
  > * 空中状态（大状态）
  >   * 跳跃状态
  >   * 空中冲刺状态
  >   * 扒墙状态
  >   * 登墙跳状态?
  > * 攻击状态
  >   * 各种技能开发中……
  > * 防御状态(大状态)
  > * 受伤/死亡状态(大状态) 


  ~~更加健壮的移动状态机：动画使用BlindTree，并使用InputSystem中的输入监听，来达到行走与跑步的丝滑转换，使用了动画状态机之后，已经丝滑到了不可思议的地步~~

 #### 对于状态机重构的一些思考
**真正优秀的状态机通常是：**
  * 状态只调用能力接口
  * 不直接操作变量
  * 不直接读取所有变量

**之前的状态机面临的问题是：**
1. Player变成了一个上帝对象，任何状态都可以访问并修改Player中的参数和组件。
2. 多层状态机中的大状态例如PlayerAirState和下面的小状态例如JumpState存在着互相争抢人物控制权的问题，导致我现在不得不写一些防御性代码来防止我的状态转换
3. 状态转换逻辑不够清晰和优雅，无法承受接下来更多的动作状态。代码耦合性只会越来越高

> *经过重构，状态机现在已经基本解决了前两个问题，第三个问题还要进行测试*

**新增脚本和逻辑解释：**
1. *逻辑脚本：*
* `Player` :前上帝对象，Unity交互接口，整个状态机的核心，所有类的出发点。
* `PlayerState`: 基类，所有类的父类。是一个抽象类。
* `PlayerStateMachine`: 状态转换机保护性代码较多，防止争抢状态。
* `Transiton` :转换请求，不让状态直接访问状态转换机，而是返回一个转换请求，在`PlayerStateMachine`中统一提交和转发，并在`Player`中执行。
* `PlayerServices`:组件封装类，防止冗余的注册导致的耦合。
2. *模块脚本：*
* `PlayerContext`:纯数值类脚本，将`Player`中部分变量，包括计时器，输入传感器，计数器，等等。可以通过这个模块来访问各种变量
* `PlayerMotor2D`:封装Rigidbody2D组件，将`Player`中所有关于RigidBody2D组件的变量全部封装，且将所有调整Velocity的方法一起封装。
* `PlayerAnimatorDriver`:将`Player`中关于Animator组件的方法封装，状态可以访问该模块来setTrigger等方法。
* `PlayerConfig`:将`Player`中数值接口封装为ScriptObject，独立于Player，同时保持了`PlayerContext`的纯净。
* `PlayerFxSpeaker`:特效广播器，通过Unity的事件系统，通知`PoolManager`在指定位置生成指定特效。
* `PlayerStateRegistry`:状态注册机，结合Enum来实现所有状态的灵活添加，并生成一个状态字典，阻止各个状态知道其他状态。
3. *状态转换逻辑解释：*
Player中状态注册逻辑略
* 开始==>`Initialize()`初始化第一个状态(默认为IdleState)
* 此时玩家开始移动,idleState通过PlayerContext获取玩家输入，并在logicUpdate中做判断，防止父状态已经做了转换状态的决定却被子状态覆盖
```csharp
var t = base.LogicUpdate();
if (t.HasTarget) return t;
```
* 判断通过后，状态会返回一个非空的Transition类。并传到`PlayerStateMachine`中进行判断,在当前状态前是否已经有状态待处理了？否则这个状态就是待处理状态，回到`Player`中提交转换
```csharp
public void Tick()
    {
        if (!_isInitialized || currentState == null) return;

        _pendingId = PlayerStateId.None;

        // 1) 当前状态提出“意图”
        var t = currentState.LogicUpdate();

        // 2) 状态内部也可能直接 RequestChangeState（比如动画事件回调）
        // 统一用 pending 优先
        if (_pendingId == PlayerStateId.None && t.HasTarget)
            _pendingId = t.Target;

        if (_pendingId != PlayerStateId.None)
            ChangeStateInternal(_pendingId);
    }
```
* 还有一种情况，有些优先级比较高的状态，因为某些原因脱离了State，此时需要单独的方法来处理这个转换，防止当前状态被其他状态覆盖
```csharp
    public void RequestChangeState(PlayerStateId newStateId)
    {
        if (newStateId == PlayerStateId.None) return;
        if (_pendingId != PlayerStateId.None) return; // 本帧已有人抢到切换权
        _pendingId = newStateId;
    }
```
* 之后进入新的状态。这就是一轮状态转换。