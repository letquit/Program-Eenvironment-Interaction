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

    private Vector3 GetClosestPointOnCollider(Collider intersectingCollider, Vector3 positionToCheck)
    {
        return intersectingCollider.ClosestPoint(positionToCheck);
    }

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
    
    protected void UpdateIkTargetPosition(Collider intersectingCollider)
    {
        if (intersectingCollider == Context.CurrentIntersectingCollider)
        {
            SetIkTargetPosition();
        }
    }
    
    protected void ResetIkTargetPositionTracking(Collider intersectingCollider)
    {
        if (intersectingCollider == Context.CurrentIntersectingCollider)
        {
            Context.CurrentIntersectingCollider = null;
            Context.ClosestPointOnColliderFromShoulder = Vector3.positiveInfinity;
        }
    }

    private void SetIkTargetPosition()
    {
        Context.ClosestPointOnColliderFromShoulder = GetClosestPointOnCollider(Context.CurrentIntersectingCollider,
            new Vector3(Context.CurrentShoulderTransform.position.x, Context.CharacterShoulderHight, Context.CurrentShoulderTransform.position.z));
        
        Vector3 rayDirection = Context.CurrentShoulderTransform.position - Context.ClosestPointOnColliderFromShoulder;
        Vector3 normalizedRayDirection = rayDirection.normalized;
        float offsetDistance = 0.05f;
        Vector3 offset = normalizedRayDirection * offsetDistance;
        
        Vector3 offsetPosition = Context.ClosestPointOnColliderFromShoulder + offset;
        Context.CurrentIkTargetTransform.position = offsetPosition;
    }
}

