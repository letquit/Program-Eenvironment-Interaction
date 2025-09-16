using UnityEngine;

/// <summary>
/// 搜索状态类，用于处理环境交互中的搜索逻辑
/// 继承自EnvironmentInteractionState，负责检测目标接近条件并切换到接近状态
/// </summary>
public class SearchState : EnvironmentInteractionState
{
    /// <summary>
    /// 接近目标的距离阈值，当角色与目标点距离小于此值时将切换到接近状态
    /// </summary>
    public float _approachDistanceThreshold = 2.0f;
    
    /// <summary>
    /// SearchState构造函数
    /// </summary>
    /// <param name="context">环境交互上下文，包含状态运行所需的数据和引用</param>
    /// <param name="estate">环境交互状态枚举值，标识当前状态类型</param>
    public SearchState(EnvironmentInteractionContext context,
        EnvironmentInteractionStateMachine.EEnvironmentInteractionState estate) : base(context, estate)
    {
        EnvironmentInteractionContext Context = context;
    }

    /// <summary>
    /// 进入搜索状态时调用的方法
    /// 记录进入状态的日志信息
    /// </summary>
    public override void EnterState()
    {
        Debug.Log("ENTERING SEARCH STATE");
    }

    /// <summary>
    /// 退出搜索状态时调用的方法
    /// 目前为空实现，可供后续扩展
    /// </summary>
    public override void ExitState()
    {
    }

    /// <summary>
    /// 每帧更新搜索状态的逻辑
    /// 目前为空实现，可供后续扩展
    /// </summary>
    public override void UpdateState()
    {
    }

    /// <summary>
    /// 获取下一个应该切换到的状态
    /// 根据距离检测和重置条件来决定状态转换
    /// </summary>
    /// <returns>下一个环境交互状态的枚举值</returns>
    public override EnvironmentInteractionStateMachine.EEnvironmentInteractionState GetNextState()
    {
        // 检查是否需要重置状态
        if (CheckShouldReset())
        {
            return EnvironmentInteractionStateMachine.EEnvironmentInteractionState.Reset;
        }
        
        // 检查是否接近目标点
        bool isCloseToTarget = Vector3.Distance(Context.ClosestPointOnColliderFromShoulder, 
            Context.RootTransform.position) < _approachDistanceThreshold;
        
        // 检查最近点是否有效
        bool isClosestPointOnColliderValid = Context.ClosestPointOnColliderFromShoulder != Vector3.positiveInfinity;

        // 如果最近点有效且角色接近目标，则切换到接近状态
        if (isClosestPointOnColliderValid && isCloseToTarget)
        {
            return EnvironmentInteractionStateMachine.EEnvironmentInteractionState.Approach;
        }
        
        // 否则保持当前状态
        return StateKey;
    }

    /// <summary>
    /// 当触发器进入时调用的方法
    /// 开始追踪IK目标位置
    /// </summary>
    /// <param name="other">进入触发器的碰撞体</param>
    public override void OnTriggerEnter(Collider other)
    {
        StartIkTargetPositionTracking(other);
    }

    /// <summary>
    /// 当触发器停留时调用的方法
    /// 更新IK目标位置
    /// </summary>
    /// <param name="other">停留在触发器中的碰撞体</param>
    public override void OnTriggerStay(Collider other)
    {
        UpdateIkTargetPosition(other);
    }

    /// <summary>
    /// 当触发器退出时调用的方法
    /// 重置IK目标位置追踪
    /// </summary>
    /// <param name="other">退出触发器的碰撞体</param>
    public override void OnTriggerExit(Collider other)
    {
        ResetIkTargetPositionTracking(other);
    }
}

