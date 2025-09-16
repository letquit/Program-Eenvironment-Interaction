using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状态管理器类，用于管理游戏对象的各种状态
/// 主要优势是可以利用这些抽象类和泛型来创建状态机
/// 每个状态机可以定义一个枚举列表，然后可以将其作为状态管理器泛型的参数类型来使用
/// 该类继承自MonoBehaviour，可以作为Unity组件使用
/// 泛型参数EState必须是枚举类型，表示状态的类型
/// </summary>
/// <typeparam name="EState">状态枚举类型，必须继承自Enum</typeparam>
public class StateManager<EState> : MonoBehaviour where EState : Enum
{
    private Dictionary<EState, BaseState<EState>> States = new Dictionary<EState, BaseState<EState>>();
    private BaseState<EState> CurrentState;

    private bool IsTransitioningState = false;

    /// <summary>
    /// Unity生命周期函数，在对象启用时调用
    /// 初始化当前状态，调用当前状态的进入状态方法
    /// </summary>
    private void Start()
    {
        CurrentState.EnterState();
    }

    /// <summary>
    /// Unity生命周期函数，每帧调用一次
    /// 处理状态更新和状态转换逻辑
    /// </summary>
    private void Update()
    {
        EState nextStateKey = CurrentState.GetNextState();

        // 如果没有状态转换且下一个状态等于当前状态，则更新当前状态
        if (!IsTransitioningState && nextStateKey.Equals(CurrentState.StateKey))
        {
            CurrentState.UpdateState();
        }
        // 如果需要状态转换，则执行状态转换
        else if (!IsTransitioningState)
        {
            TransitionState(CurrentState.StateKey);
        }
    }

    /// <summary>
    /// 执行状态转换操作
    /// 依次调用当前状态的退出方法和新状态的进入方法
    /// </summary>
    /// <param name="stateKey">要转换到的状态键值</param>
    private void TransitionState(EState stateKey)
    {
        IsTransitioningState = true;
        CurrentState.ExitState();
        CurrentState = States[stateKey];
        CurrentState.EnterState();
        IsTransitioningState = false;
    }

    /// <summary>
    /// Unity物理触发器进入事件回调函数
    /// 当碰撞体进入触发器时调用
    /// 将触发事件传递给当前状态处理
    /// </summary>
    /// <param name="other">进入触发器的碰撞体</param>
    private void OnTriggerEnter(Collider other)
    {
        CurrentState.OnTriggerEnter(other);
    }

    /// <summary>
    /// Unity物理触发器停留事件回调函数
    /// 当碰撞体在触发器内停留时持续调用
    /// 将触发事件传递给当前状态处理
    /// </summary>
    /// <param name="other">在触发器内停留的碰撞体</param>
    private void OnTriggerStay(Collider other)
    {
        CurrentState.OnTriggerStay(other);
    }

    /// <summary>
    /// Unity物理触发器退出事件回调函数
    /// 当碰撞体退出触发器时调用
    /// 将触发事件传递给当前状态处理
    /// </summary>
    /// <param name="other">退出触发器的碰撞体</param>
    private void OnTriggerExit(Collider other)
    {
        CurrentState.OnTriggerExit(other);
    }
}

