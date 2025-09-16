using UnityEngine;

/// <summary>
/// 环境交互状态抽象基类，继承自基础状态类
/// 用于定义环境交互状态机中各种状态的通用行为和属性
/// </summary>
/// <typeparam name="EnvironmentInteractionStateMachine.EEnvironmentInteractionState">环境交互状态枚举类型</typeparam>
public abstract class EnvironmentInteractionState : BaseState<EnvironmentInteractionStateMachine.EEnvironmentInteractionState>
{
    /// <summary>
    /// 环境交互上下文，包含状态执行所需的数据和引用
    /// </summary>
    protected EnvironmentInteractionContext Context;

    /// <summary>
    /// 环境交互状态构造函数
    /// 初始化状态实例并设置上下文和状态键
    /// </summary>
    /// <param name="context">环境交互上下文对象，提供状态执行所需的环境数据和组件引用</param>
    /// <param name="stateKey">环境交互状态枚举值，用于标识当前状态在状态机中的唯一键值</param>
    public EnvironmentInteractionState(EnvironmentInteractionContext context,
        EnvironmentInteractionStateMachine.EEnvironmentInteractionState stateKey) : base(stateKey)
    {
        Context = context;
    }
}

