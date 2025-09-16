using UnityEngine;

/// <summary>
/// 表示环境交互状态机中的“重置”状态。
/// 在此状态下，系统将逐渐恢复到初始状态，包括位置、旋转和约束权重的插值。
/// </summary>
public class ResetState : EnvironmentInteractionState
{
    private float _elapsedTime = 0.0f;
    private float _resetDuration = 2.0f;
    private float _lerpDuration = 10.0f;
    private float _rotationSpeed = 500.0f;
    
    /// <summary>
    /// 构造函数，初始化 ResetState 实例。
    /// </summary>
    /// <param name="context">环境交互上下文对象，包含当前状态所需的数据。</param>
    /// <param name="estate">表示当前状态的枚举值。</param>
    public ResetState(EnvironmentInteractionContext context,
        EnvironmentInteractionStateMachine.EEnvironmentInteractionState estate) : base(context, estate)
    {
        EnvironmentInteractionContext Context = context;
    }

    /// <summary>
    /// 进入该状态时调用。初始化计时器，并重置上下文中的部分状态数据。
    /// </summary>
    public override void EnterState()
    {
        Debug.Log("ENTERING RESET STATE");
        _elapsedTime = 0.0f;
        Context.ClosestPointOnColliderFromShoulder = Vector3.positiveInfinity;
        Context.CurrentIntersectingCollider = null;
    }

    /// <summary>
    /// 离开该状态时调用。目前为空实现。
    /// </summary>
    public override void ExitState()
    {
        
    }

    /// <summary>
    /// 每帧更新状态逻辑。包括：
    /// - 更新经过的时间；
    /// - 对 IK 约束权重、旋转约束权重、目标位置和旋转进行插值；
    /// </summary>
    public override void UpdateState()
    {
        _elapsedTime += Time.deltaTime;

        // 插值调整交互点 Y 轴偏移量
        Context.InteractionPointYOffset = Mathf.Lerp(Context.InteractionPointYOffset,
            Context.ColliderCenterY, _elapsedTime / _lerpDuration);

        // 插值减少 IK 约束权重
        Context.CurrentIkConstraint.weight = Mathf.Lerp(Context.CurrentIkConstraint.weight,
            0, _elapsedTime / _lerpDuration);

        // 插值减少旋转约束权重
        Context.CurrentMultiRotationConstraint.weight = Mathf.Lerp(Context.CurrentMultiRotationConstraint.weight,
            0, _elapsedTime / _lerpDuration);

        // 插值恢复 IK 目标局部位置
        Context.CurrentIkTargetTransform.localPosition = Vector3.Lerp(Context.CurrentIkTargetTransform.localPosition,
            Context.CurrentOriginalTargetPosition, _elapsedTime / _lerpDuration);

        // 逐步旋转 IK 目标朝向原始方向
        Context.CurrentIkTargetTransform.rotation = Quaternion.RotateTowards(Context.CurrentIkTargetTransform.rotation,
            Context.OriginalTargetRotation, _rotationSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 判断是否应切换到下一个状态。
    /// 如果经过时间大于等于重置持续时间且刚体正在移动，则切换到 Search 状态。
    /// 否则保持当前状态。
    /// </summary>
    /// <returns>返回下一个状态的枚举值。</returns>
    public override EnvironmentInteractionStateMachine.EEnvironmentInteractionState GetNextState()
    {
        bool isMoving = Context.Rb.linearVelocity != Vector3.zero;
        if (_elapsedTime >= _resetDuration && isMoving)
        {
            return EnvironmentInteractionStateMachine.EEnvironmentInteractionState.Search;
        }
        return StateKey;
    }

    /// <summary>
    /// 当触发器进入时调用。当前未实现具体逻辑。
    /// </summary>
    /// <param name="other">进入触发器的 Collider 对象。</param>
    public override void OnTriggerEnter(Collider other)
    {
    }

    /// <summary>
    /// 当触发器停留时调用。当前未实现具体逻辑。
    /// </summary>
    /// <param name="other">停留在触发器内的 Collider 对象。</param>
    public override void OnTriggerStay(Collider other)
    {
    }

    /// <summary>
    /// 当触发器退出时调用。当前未实现具体逻辑。
    /// </summary>
    /// <param name="other">离开触发器的 Collider 对象。</param>
    public override void OnTriggerExit(Collider other)
    {
    }
}
