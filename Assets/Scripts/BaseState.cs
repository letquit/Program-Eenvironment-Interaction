using System;
using UnityEngine;

/// <summary>
/// 状态机系统中的基础状态类，为有限状态机提供抽象基类实现
/// 该类定义了状态的基本行为接口，包括状态的进入、退出、更新以及物理触发事件处理
/// 泛型参数EState用于限定状态枚举类型，确保类型安全
/// </summary>
/// <typeparam name="EState">状态枚举类型，必须是枚举类型</typeparam>
public abstract class BaseState<EState> where EState : Enum
{
    /// <summary>
    /// 获取当前状态的唯一标识键值
    /// </summary>
    public EState StateKey { get; private set; }
    
    /// <summary>
    /// 当进入该状态时调用的抽象方法
    /// 子类需要实现此方法来定义进入状态时的具体行为
    /// </summary>
    public abstract void EnterState();
    
    /// <summary>
    /// 当退出该状态时调用的抽象方法
    /// 子类需要实现此方法来定义退出状态时的具体行为
    /// </summary>
    public abstract void ExitState();
    
    /// <summary>
    /// 状态更新方法，在每帧调用
    /// 子类需要实现此方法来定义状态运行期间的持续行为
    /// </summary>
    public abstract void UpdateState();
    
    /// <summary>
    /// 获取下一个应该转换到的状态
    /// 子类需要实现此方法来定义状态转换逻辑
    /// </summary>
    /// <returns>返回下一个状态的枚举值，如果保持当前状态则返回当前状态</returns>
    public abstract EState GetNextState();
    
    /// <summary>
    /// 当触发器碰撞体进入时调用的抽象方法
    /// 子类需要实现此方法来处理触发器进入事件
    /// </summary>
    /// <param name="other">进入触发器的碰撞体组件</param>
    public abstract void OnTriggerEnter(Collider other);
    
    /// <summary>
    /// 当触发器碰撞体停留时调用的抽象方法
    /// 子类需要实现此方法来处理触发器停留事件
    /// </summary>
    /// <param name="other">停留在触发器内的碰撞体组件</param>
    public abstract void OnTriggerStay(Collider other);
    
    /// <summary>
    /// 当触发器碰撞体退出时调用的抽象方法
    /// 子类需要实现此方法来处理触发器退出事件
    /// </summary>
    /// <param name="other">退出触发器的碰撞体组件</param>
    public abstract void OnTriggerExit(Collider other);
    
    /// <summary>
    /// 基础状态类的构造函数
    /// 初始化状态实例并设置状态键值
    /// </summary>
    /// <param name="key">状态的唯一标识枚举值</param>
    public BaseState(EState key)
    {
        StateKey = key;
    }
}

