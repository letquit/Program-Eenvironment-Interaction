using UnityEngine;

/// <summary>
/// 表示角色接近环境交互目标的状态。
/// 在此状态下，角色会逐渐调整IK目标的位置和旋转，使其朝向地面，并根据距离判断是否进入下一个状态。
/// </summary>
public class ApproachState : EnvironmentInteractionState
{
    private float _elapsedTime = 0.0f;
    private float _lerpDuration = 5.0f;
    private float _approachDuration = 2.0f;
    private float _approachWeight = 0.5f;
    private float _approachRotationWeight = 0.75f;
    private float _rotationSpeed = 500f;
    private float _riseDistanceThreshold = 0.5f;

    /// <summary>
    /// 初始化 ApproachState 实例。
    /// </summary>
    /// <param name="context">环境交互上下文对象，包含当前状态所需的数据。</param>
    /// <param name="estate">表示当前状态的枚举值。</param>
    public ApproachState(EnvironmentInteractionContext context,
        EnvironmentInteractionStateMachine.EEnvironmentInteractionState estate) : base(context, estate)
    {
        EnvironmentInteractionContext Context = context;
    }

    /// <summary>
    /// 进入该状态时调用。初始化计时器并输出调试日志。
    /// </summary>
    public override void EnterState()
    {
        Debug.Log("ENTERING APPROACH STATE");
        Debug.Log("Original Target Rotation: " + Context.OriginalTargetRotation);
        _elapsedTime = 0.0f;
    }

    /// <summary>
    /// 退出该状态时调用。当前未实现具体逻辑。
    /// </summary>
    public override void ExitState()
    {
        
    }

    /// <summary>
    /// 每帧更新状态逻辑。包括：
    /// - 调整 IK 目标的旋转方向为朝向地面；
    /// - 插值更新 IK 约束权重；
    /// </summary>
    public override void UpdateState()
    {
        // 创建一个Z轴向下指向地面的四元数
        Quaternion expectedGroundRotation = Quaternion.LookRotation(-Vector3.up, Context.RootTransform.forward);
        _elapsedTime += Time.deltaTime;

        Context.CurrentIkTargetTransform.rotation = Quaternion.RotateTowards(Context.CurrentIkTargetTransform.rotation,
            expectedGroundRotation, _rotationSpeed * Time.deltaTime);
        
        Context.CurrentMultiRotationConstraint.weight = Mathf.Lerp(Context.CurrentMultiRotationConstraint.weight, _approachRotationWeight,
            _elapsedTime / _lerpDuration);
        
        Context.CurrentIkConstraint.weight = Mathf.Lerp(Context.CurrentIkConstraint.weight, _approachWeight,
            _elapsedTime / _lerpDuration);
    }

    /// <summary>
    /// 判断并返回下一个状态。
    /// 如果已超过持续时间或需要重置，则返回 Reset 状态；
    /// 如果目标点在手臂可及范围内，则返回 Rise 状态；
    /// 否则保持当前状态。
    /// </summary>
    /// <returns>下一个状态的枚举值。</returns>
    public override EnvironmentInteractionStateMachine.EEnvironmentInteractionState GetNextState()
    {
        bool isOverStateLifeDuration = _elapsedTime >= _approachDuration;
        if (isOverStateLifeDuration || CheckShouldReset())
        {
            return EnvironmentInteractionStateMachine.EEnvironmentInteractionState.Reset;
        }
        
        bool isWithinArmsReach = Vector3.Distance(Context.ClosestPointOnColliderFromShoulder,
            Context.CurrentShoulderTransform.position) < _riseDistanceThreshold;
        bool isClosestPointOnColliderReal = Context.ClosestPointOnColliderFromShoulder != Vector3.positiveInfinity;

        if (isClosestPointOnColliderReal && isWithinArmsReach)
        {
            return EnvironmentInteractionStateMachine.EEnvironmentInteractionState.Rise;
        }
        
        return StateKey;
    }

    /// <summary>
    /// 当触发器检测到碰撞体进入时调用。开始跟踪 IK 目标位置。
    /// </summary>
    /// <param name="other">进入触发器的碰撞体。</param>
    public override void OnTriggerEnter(Collider other)
    {
        StartIkTargetPositionTracking(other);
    }

    /// <summary>
    /// 当触发器持续检测到碰撞体停留时调用。更新 IK 目标位置。
    /// </summary>
    /// <param name="other">停留于触发器中的碰撞体。</param>
    public override void OnTriggerStay(Collider other)
    {
        UpdateIkTargetPosition(other);
    }

    /// <summary>
    /// 当触发器检测到碰撞体离开时调用。重置 IK 目标位置跟踪。
    /// </summary>
    /// <param name="other">离开触发器的碰撞体。</param>
    public override void OnTriggerExit(Collider other)
    {
        ResetIkTargetPositionTracking(other);
    }
}
