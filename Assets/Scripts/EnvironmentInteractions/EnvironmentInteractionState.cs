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
    private float _movingAwayOffset = 0.005f;
    private bool _shouldReset;

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

    /// <summary>
    /// 检查是否需要重置交互状态
    /// 根据角色移动状态、角度、跳跃等条件判断是否应重置最近距离记录
    /// </summary>
    /// <returns>如果需要重置返回true，否则返回false</returns>
    protected bool CheckShouldReset()
    {
        if (_shouldReset)
        {
            Context.LowestDistance = Mathf.Infinity;
            _shouldReset = false;
            return true;
        }
        
        // 若使用其他移动方式，此处替换角色速度验证逻辑
        bool isPlayerStopped = Context.Rb.linearVelocity == Vector3.zero;
        bool isMovingAway = CheckIsMovingAway();
        bool isBadAngle = CheckIsBadAngle();
        bool isPlayerJumping = Mathf.Round(Context.Rb.linearVelocity.y) >= 1;

        if (isPlayerStopped || isMovingAway || isBadAngle || isPlayerJumping)
        {
            Context.LowestDistance = Mathf.Infinity;
            return true;
        }
        
        return false;
    }

    /// <summary>
    /// 检查当前交互角度是否不适宜继续交互
    /// 判断角色面向方向与目标点方向之间的夹角是否过大
    /// </summary>
    /// <returns>若角度不合适返回true，否则返回false</returns>
    protected bool CheckIsBadAngle()
    {
        if (Context.CurrentIntersectingCollider == null)
        {
            return false;
        }

        Vector3 targetDirection =
            Context.ClosestPointOnColliderFromShoulder - Context.CurrentShoulderTransform.position;
        Vector3 shoulderDirection = Context.CurrentBodySide == EnvironmentInteractionContext.EBodySide.RIGHT
            ? Context.RootTransform.right : -Context.RootTransform.right;
        
        float dotProduct = Vector3.Dot(shoulderDirection, targetDirection.normalized);
        bool isBadAngle = dotProduct < 0;
        
        return isBadAngle;
    }

    /// <summary>
    /// 检查角色是否正在远离交互目标点
    /// 通过比较当前距离与记录的最小距离判断移动趋势
    /// </summary>
    /// <returns>如果是远离目标返回true，否则返回false</returns>
    protected bool CheckIsMovingAway()
    {
        float currentDistanceToTarget = Vector3.Distance(Context.RootTransform.position, Context.ClosestPointOnColliderFromShoulder);

        bool isSearchingForNewInteraction = Context.CurrentIntersectingCollider == null;
        if (isSearchingForNewInteraction)
        {
            Debug.Log("Searching for new interaction");
            return false;
        }
        
        bool isGettingCloserToTarget = currentDistanceToTarget <= Context.LowestDistance;
        if (isGettingCloserToTarget)
        {
            Debug.Log("Getting closer to target: " + currentDistanceToTarget + "---" + Context.LowestDistance);
            Context.LowestDistance = currentDistanceToTarget;
            return false;
        }
        
        bool isMovingAwayFromTarget = currentDistanceToTarget > Context.LowestDistance + _movingAwayOffset;
        if (isMovingAwayFromTarget)
        {
            Debug.Log("Moving away from target");
            Context.LowestDistance = Mathf.Infinity;
            return true;
        }
        
        return false;
    }

    /// <summary>
    /// 获取指定碰撞体上距离给定点最近的表面点
    /// </summary>
    /// <param name="intersectingCollider">要检测的碰撞体</param>
    /// <param name="positionToCheck">用于计算最近点的参考位置</param>
    /// <returns>碰撞体表面上距离参考点最近的空间坐标</returns>
    private Vector3 GetClosestPointOnCollider(Collider intersectingCollider, Vector3 positionToCheck)
    {
        return intersectingCollider.ClosestPoint(positionToCheck);
    }

    /// <summary>
    /// 开始跟踪交互目标点的位置
    /// 当碰撞体属于可交互层且当前未跟踪任何目标时进行初始化
    /// </summary>
    /// <param name="intersectingCollider">触发交互的碰撞体</param>
    protected void StartIkTargetPositionTracking(Collider intersectingCollider)
    {
        // 使用可交互层俩过滤目标对象且确保程序动画启动后不会切换到其它碰撞体并重置该值
        if (intersectingCollider.gameObject.layer == LayerMask.NameToLayer("Interactable") &&
            Context.CurrentIntersectingCollider == null)
        {
            Context.CurrentIntersectingCollider = intersectingCollider;
            Vector3 closestPointFromRoot = GetClosestPointOnCollider(intersectingCollider, Context.RootTransform.position);
            Context.SetCurrentSide(closestPointFromRoot);
            
            SetIkTargetPosition();
        }
    }
    
    /// <summary>
    /// 更新交互目标点的位置
    /// 只有当传入的碰撞体与当前跟踪的碰撞体一致时才更新位置
    /// </summary>
    /// <param name="intersectingCollider">当前交互的碰撞体</param>
    protected void UpdateIkTargetPosition(Collider intersectingCollider)
    {
        if (intersectingCollider == Context.CurrentIntersectingCollider)
        {
            SetIkTargetPosition();
        }
    }
    
    /// <summary>
    /// 重置交互目标点的跟踪状态
    /// 清除当前跟踪的碰撞体信息并将相关变量恢复默认值
    /// </summary>
    /// <param name="intersectingCollider">即将退出交互的碰撞体</param>
    protected void ResetIkTargetPositionTracking(Collider intersectingCollider)
    {
        if (intersectingCollider == Context.CurrentIntersectingCollider)
        {
            Context.CurrentIntersectingCollider = null;
            Context.ClosestPointOnColliderFromShoulder = Vector3.positiveInfinity;
            _shouldReset = true;
        }
    }

    /// <summary>
    /// 设置IK目标点的位置
    /// 计算角色肩膀到碰撞体最近点的偏移位置作为IK目标点
    /// </summary>
    private void SetIkTargetPosition()
    {
        Context.ClosestPointOnColliderFromShoulder = GetClosestPointOnCollider(Context.CurrentIntersectingCollider,
            new Vector3(Context.CurrentShoulderTransform.position.x, Context.CharacterShoulderHight, Context.CurrentShoulderTransform.position.z));
        
        Vector3 rayDirection = Context.CurrentShoulderTransform.position - Context.ClosestPointOnColliderFromShoulder;
        Vector3 normalizedRayDirection = rayDirection.normalized;
        float offsetDistance = 0.05f;
        Vector3 offset = normalizedRayDirection * offsetDistance;
        
        Vector3 offsetPosition = Context.ClosestPointOnColliderFromShoulder + offset;
        Context.CurrentIkTargetTransform.position = new Vector3(offsetPosition.x, Context.InteractionPointYOffset, offsetPosition.z);
    }
}
