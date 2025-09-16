using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Assertions;

/// <summary>
/// 环境交互状态机类，用于管理角色与环境交互的各种状态
/// 该类继承自StateManager，专门处理环境交互相关的状态转换和控制
/// </summary>
public class EnvironmentInteractionStateMachine : StateManager<EnvironmentInteractionStateMachine.EEnvironmentInteractionState>
{
    /// <summary>
    /// 环境交互状态枚举，定义了角色与环境交互的不同阶段
    /// </summary>
    public enum EEnvironmentInteractionState
    {
        /// <summary>
        /// 搜索状态 - 角色正在寻找可交互的环境对象
        /// </summary>
        Search,
        /// <summary>
        /// 接近状态 - 角色正在向目标环境对象移动
        /// </summary>
        Approach,
        /// <summary>
        /// 上升状态 - 角色正在调整高度以接触环境对象
        /// </summary>
        Rise,
        /// <summary>
        /// 触摸状态 - 角色正在与环境对象进行实际接触
        /// </summary>
        Touch,
        /// <summary>
        /// 重置状态 - 角色正在恢复到初始状态
        /// </summary>
        Reset,
    }

    private EnvironmentInteractionContext _context;

    [SerializeField] private TwoBoneIKConstraint _leftIkConstraint;
    [SerializeField] private TwoBoneIKConstraint _rightIkConstraint;
    [SerializeField] private MultiRotationConstraint _leftMultiRotationConstraint;
    [SerializeField] private MultiRotationConstraint _rightMultiRotationConstraint;
    // 如果使用Character Controller 主要获取速度和范围
    // [SerializeField] private CharacterController _characterController;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private CapsuleCollider _rootCollider;

    /// <summary>
    /// Unity生命周期方法，在脚本实例被载入时调用
    /// 主要用于初始化组件和验证必要的引用
    /// </summary>
    private void Awake()
    {
        ValidateConstraints();
        
        _context = new EnvironmentInteractionContext(
            _leftIkConstraint, 
            _rightIkConstraint,
            _leftMultiRotationConstraint, 
            _rightMultiRotationConstraint,
            _rigidbody, 
            _rootCollider
        );
        InitializeStates();
    }

    /// <summary>
    /// 验证所有必需的约束和组件引用是否已正确分配
    /// 通过断言检查确保所有序列化字段都不为null，避免运行时错误
    /// </summary>
    private void ValidateConstraints()
    {
        Assert.IsNotNull(_leftIkConstraint, "左IK约束未分配。");
        Assert.IsNotNull(_rightIkConstraint, "右IK约束未分配。");
        Assert.IsNotNull(_leftMultiRotationConstraint, "左多重旋转约束未分配。");
        Assert.IsNotNull(_rightMultiRotationConstraint, "右多重旋转约束未分配。");
        Assert.IsNotNull(_rigidbody, "用于控制角色的刚体未分配。");
        Assert.IsNotNull(_rootCollider, "附加到角色的根碰撞体未分配。");
    }

    /// <summary>
    /// 初始化状态字典，将各个环境交互状态添加到状态管理器中，并设置初始状态
    /// </summary>
    private void InitializeStates()
    {
        // 将状态添加到继承的 StateManager "States"字典并设置初始状态
        States.Add(EEnvironmentInteractionState.Reset, new ResetState(_context, EEnvironmentInteractionState.Reset));
        States.Add(EEnvironmentInteractionState.Search, new SearchState(_context, EEnvironmentInteractionState.Search));
        States.Add(EEnvironmentInteractionState.Approach, new ApproachState(_context, EEnvironmentInteractionState.Approach));
        States.Add(EEnvironmentInteractionState.Rise, new RiseState(_context, EEnvironmentInteractionState.Rise));
        States.Add(EEnvironmentInteractionState.Touch, new TouchState(_context, EEnvironmentInteractionState.Touch));
        CurrentState = States[EEnvironmentInteractionState.Reset];
    }
}

