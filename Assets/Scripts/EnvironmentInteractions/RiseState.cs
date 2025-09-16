using UnityEngine;

/// <summary>
/// 表示环境交互状态机中的“上升”状态，用于控制角色手部IK目标的上升动画和交互逻辑。
/// </summary>
public class RiseState : EnvironmentInteractionState
{
    private float _elapsedTime = 0.0f;
    private float _lerpDuration = 5.0f;
    private float _riseWeight = 1.0f;
    private Quaternion _expectedHandRotation;
    private float _maxDistance = 0.5f;
    protected LayerMask _interactableLayerMask = LayerMask.GetMask("Interactable");
    private float _rotationSpeed = 1000f;
    private float _touchDistanceThreshold = 0.05f;
    private float _touchTimeThreshold = 1f;
    
    /// <summary>
    /// 构造函数，初始化 RiseState 实例。
    /// </summary>
    /// <param name="context">环境交互上下文对象，包含状态共享的数据。</param>
    /// <param name="estate">当前状态枚举值。</param>
    public RiseState(EnvironmentInteractionContext context,
        EnvironmentInteractionStateMachine.EEnvironmentInteractionState estate) : base(context, estate)
    {
        EnvironmentInteractionContext Context = context;
    }

    /// <summary>
    /// 进入状态时调用，重置经过的时间。
    /// </summary>
    public override void EnterState()
    {
        _elapsedTime = 0.0f;
    }

    /// <summary>
    /// 退出状态时调用，可在此清理资源或重置状态。
    /// </summary>
    public override void ExitState()
    {
        
    }

    /// <summary>
    /// 每帧更新状态逻辑，包括插值更新IK偏移、权重和手部旋转。
    /// </summary>
    public override void UpdateState()
    {
        CalculateExpectedHandRotation();

        // 插值更新手部Y轴偏移
        Context.InteractionPointYOffset = Mathf.Lerp(Context.InteractionPointYOffset,
            Context.ClosestPointOnColliderFromShoulder.y, _elapsedTime / _lerpDuration);
        
        // 插值更新IK约束权重
        Context.CurrentIkConstraint.weight = Mathf.Lerp(Context.CurrentIkConstraint.weight,
            _riseWeight, _elapsedTime / _lerpDuration);

        // 插值更新旋转约束权重
        Context.CurrentMultiRotationConstraint.weight = Mathf.Lerp(Context.CurrentMultiRotationConstraint.weight,
            _riseWeight, _elapsedTime / _lerpDuration);

        // 逐步旋转IK目标到预期方向
        Context.CurrentIkTargetTransform.rotation = Quaternion.RotateTowards(Context.CurrentIkTargetTransform.rotation,
            _expectedHandRotation, _rotationSpeed * Time.deltaTime);
        
        _elapsedTime += Time.deltaTime;
    }

    /// <summary>
    /// 计算手部期望的旋转方向，使其面向最近可交互物体的表面法线方向。
    /// </summary>
    private void CalculateExpectedHandRotation()
    {
        Vector3 startPos = Context.CurrentShoulderTransform.position;
        Vector3 endPos = Context.ClosestPointOnColliderFromShoulder;
        Vector3 direction = (endPos - startPos).normalized;

        RaycastHit hit;
        // 向目标方向发射射线，检测可交互物体
        if (Physics.Raycast(startPos, direction, out hit, _maxDistance, _interactableLayerMask))
        {
            Vector3 surfaceNormal = hit.normal;
            Vector3 targetForward = -surfaceNormal;
            _expectedHandRotation = Quaternion.LookRotation(targetForward, Vector3.up);
        }
    }

    /// <summary>
    /// 获取下一个状态，根据距离和时间判断是否进入 Touch 状态或 Reset 状态。
    /// </summary>
    /// <returns>下一个状态的枚举值。</returns>
    public override EnvironmentInteractionStateMachine.EEnvironmentInteractionState GetNextState()
    {
        if (CheckShouldReset())
        {
            return EnvironmentInteractionStateMachine.EEnvironmentInteractionState.Reset;
        }
        
        // 判断是否接近目标点并满足时间条件，进入 Touch 状态
        if (Vector3.Distance(Context.CurrentIkTargetTransform.position,
                Context.ClosestPointOnColliderFromShoulder) < _touchDistanceThreshold
            && _elapsedTime >= _touchTimeThreshold)
        {
            return EnvironmentInteractionStateMachine.EEnvironmentInteractionState.Touch;
        }
        
        return StateKey;
    }

    /// <summary>
    /// 当触发器进入时调用，开始追踪IK目标位置。
    /// </summary>
    /// <param name="other">进入触发器的碰撞体。</param>
    public override void OnTriggerEnter(Collider other)
    {
        StartIkTargetPositionTracking(other);
    }

    /// <summary>
    /// 当触发器停留时调用，持续更新IK目标位置。
    /// </summary>
    /// <param name="other">停留的碰撞体。</param>
    public override void OnTriggerStay(Collider other)
    {
        UpdateIkTargetPosition(other);
    }

    /// <summary>
    /// 当触发器退出时调用，重置IK目标位置追踪。
    /// </summary>
    /// <param name="other">退出的碰撞体。</param>
    public override void OnTriggerExit(Collider other)
    {
        ResetIkTargetPositionTracking(other);
    }
}
