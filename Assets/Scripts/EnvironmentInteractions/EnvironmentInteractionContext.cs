using UnityEngine;
using UnityEngine.Animations.Rigging;

/// <summary>
/// 环境交互上下文类，用于存储和管理角色与环境交互所需的各种约束和物理组件
/// </summary>
public class EnvironmentInteractionContext
{
    private TwoBoneIKConstraint _leftIkConstraint;
    private TwoBoneIKConstraint _rightIkConstraint;
    private MultiRotationConstraint _leftMultiRotationConstraint;
    private MultiRotationConstraint _rightMultiRotationConstraint;
    // 如果使用Character Controller 主要获取速度和范围
    // private CharacterController _characterController;
    private Rigidbody _rigidbody;
    private CapsuleCollider _rootCollider;

    /// <summary>
    /// 初始化环境交互上下文实例
    /// </summary>
    /// <param name="leftIkConstraint">左手IK约束组件，用于控制左手的逆向运动学</param>
    /// <param name="rightIkConstraint">右手IK约束组件，用于控制右手的逆向运动学</param>
    /// <param name="leftMultiRotationConstraint">左手多旋转约束组件，用于控制左手的多关节旋转</param>
    /// <param name="rightMultiRotationConstraint">右手多旋转约束组件，用于控制右手的多关节旋转</param>
    /// <param name="rigidbody">刚体组件，用于处理物理碰撞和运动</param>
    /// <param name="rootCollider">根胶囊碰撞体，用于检测角色与环境的碰撞</param>
    public EnvironmentInteractionContext(
        TwoBoneIKConstraint leftIkConstraint,
        TwoBoneIKConstraint rightIkConstraint,
        MultiRotationConstraint leftMultiRotationConstraint,
        MultiRotationConstraint rightMultiRotationConstraint,
        Rigidbody rigidbody, 
        CapsuleCollider rootCollider)
    {
        _leftIkConstraint = leftIkConstraint;
        _rightIkConstraint = rightIkConstraint;
        _leftMultiRotationConstraint = leftMultiRotationConstraint;
        _rightMultiRotationConstraint = rightMultiRotationConstraint;
        _rigidbody = rigidbody;
        _rootCollider = rootCollider;
    }
    
    /// <summary>
    /// 获取左手IK约束组件
    /// </summary>
    public TwoBoneIKConstraint LeftIkConstraint => _leftIkConstraint;
    
    /// <summary>
    /// 获取右手IK约束组件
    /// </summary>
    public TwoBoneIKConstraint RightIkConstraint => _rightIkConstraint;
    
    /// <summary>
    /// 获取左手多旋转约束组件
    /// </summary>
    public MultiRotationConstraint LeftMultiRotationConstraint => _leftMultiRotationConstraint;
    
    /// <summary>
    /// 获取右手多旋转约束组件
    /// </summary>
    public MultiRotationConstraint RightMultiRotationConstraint => _rightMultiRotationConstraint;
    
    /// <summary>
    /// 获取刚体组件
    /// </summary>
    public Rigidbody Rb => _rigidbody;
    
    /// <summary>
    /// 获取根胶囊碰撞体
    /// </summary>
    public CapsuleCollider RootCollider => _rootCollider;
}

