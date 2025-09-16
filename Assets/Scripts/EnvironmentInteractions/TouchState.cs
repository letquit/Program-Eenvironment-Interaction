using UnityEngine;

/// <summary>
/// 触摸状态类，继承自环境交互状态基类
/// 处理触摸交互的逻辑，包括触摸时间跟踪和IK目标位置更新
/// </summary>
public class TouchState : EnvironmentInteractionState
{
    public float _elapsedTime = 0.0f;
    public float _resetThreshold = 0.5f;
    
    /// <summary>
    /// 触摸状态构造函数
    /// </summary>
    /// <param name="context">环境交互上下文对象</param>
    /// <param name="estate">环境交互状态枚举值</param>
    public TouchState(EnvironmentInteractionContext context,
        EnvironmentInteractionStateMachine.EEnvironmentInteractionState estate) : base(context, estate)
    {
        EnvironmentInteractionContext Context = context;
    }

    /// <summary>
    /// 进入状态时的初始化操作
    /// 重置经过的时间为0
    /// </summary>
    public override void EnterState()
    {
        _elapsedTime = 0.0f;
    }

    /// <summary>
    /// 退出状态时的清理操作
    /// </summary>
    public override void ExitState()
    {
        
    }

    /// <summary>
    /// 状态更新方法
    /// 每帧更新经过的时间
    /// </summary>
    public override void UpdateState()
    {
        _elapsedTime += Time.deltaTime;
    }

    /// <summary>
    /// 获取下一个状态
    /// 根据触摸时间和重置条件判断是否需要切换到重置状态
    /// </summary>
    /// <returns>下一个环境交互状态</returns>
    public override EnvironmentInteractionStateMachine.EEnvironmentInteractionState GetNextState()
    {
        // 检查是否超过重置阈值或需要重置
        if (_elapsedTime > _resetThreshold || CheckShouldReset())
        {
            return EnvironmentInteractionStateMachine.EEnvironmentInteractionState.Reset;
        }
        
        return StateKey;
    }

    /// <summary>
    /// 触发器进入事件处理
    /// 当其他碰撞体进入触发器时开始IK目标位置跟踪
    /// </summary>
    /// <param name="other">进入触发器的碰撞体</param>
    public override void OnTriggerEnter(Collider other)
    {
        StartIkTargetPositionTracking(other);
    }

    /// <summary>
    /// 触发器停留事件处理
    /// 当其他碰撞体在触发器内停留时更新IK目标位置
    /// </summary>
    /// <param name="other">停留在触发器内的碰撞体</param>
    public override void OnTriggerStay(Collider other)
    {
        UpdateIkTargetPosition(other);
    }

    /// <summary>
    /// 触发器退出事件处理
    /// 当其他碰撞体离开触发器时重置IK目标位置跟踪
    /// </summary>
    /// <param name="other">离开触发器的碰撞体</param>
    public override void OnTriggerExit(Collider other)
    {
        ResetIkTargetPositionTracking(other);
    }
}

