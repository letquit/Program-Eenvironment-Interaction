using UnityEngine;
using UnityEngine.Animations.Rigging;

/// <summary>
/// 环境交互上下文类，用于存储和管理角色与环境交互所需的各种约束和物理组件
/// </summary>
public class EnvironmentInteractionContext
{
    public enum EBodySide
    {
        RIGHT,
        LEFT
    }
    
    private TwoBoneIKConstraint _leftIkConstraint;
    private TwoBoneIKConstraint _rightIkConstraint;
    private MultiRotationConstraint _leftMultiRotationConstraint;
    private MultiRotationConstraint _rightMultiRotationConstraint;
    // 如果使用Character Controller 主要获取速度和范围
    // private CharacterController _characterController;
    private Rigidbody _rigidbody;
    private CapsuleCollider _rootCollider;
    private Transform _rootTransform;

    /// <summary>
    /// 初始化环境交互上下文实例
    /// </summary>
    /// <param name="leftIkConstraint">左手IK约束组件，用于控制左手的逆向运动学</param>
    /// <param name="rightIkConstraint">右手IK约束组件，用于控制右手的逆向运动学</param>
    /// <param name="leftMultiRotationConstraint">左手多旋转约束组件，用于控制左手的多关节旋转</param>
    /// <param name="rightMultiRotationConstraint">右手多旋转约束组件，用于控制右手的多关节旋转</param>
    /// <param name="rigidbody">刚体组件，用于处理物理碰撞和运动</param>
    /// <param name="rootCollider">根胶囊碰撞体，用于检测角色与环境的碰撞</param>
    /// <param name="rootTransform">根变换组件，用于获取角色的位置信息</param>
    public EnvironmentInteractionContext(
        TwoBoneIKConstraint leftIkConstraint,
        TwoBoneIKConstraint rightIkConstraint,
        MultiRotationConstraint leftMultiRotationConstraint,
        MultiRotationConstraint rightMultiRotationConstraint,
        Rigidbody rigidbody, 
        CapsuleCollider rootCollider,
        Transform rootTransform)
    {
        _leftIkConstraint = leftIkConstraint;
        _rightIkConstraint = rightIkConstraint;
        _leftMultiRotationConstraint = leftMultiRotationConstraint;
        _rightMultiRotationConstraint = rightMultiRotationConstraint;
        _rigidbody = rigidbody;
        _rootCollider = rootCollider;
        _rootTransform = rootTransform;

        // 初始化角色肩膀高度
        CharacterShoulderHight = leftIkConstraint.data.root.transform.position.y;
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
    
    /// <summary>
    /// 获取根变换组件
    /// </summary>
    public Transform RootTransform => _rootTransform;

    // 添加缺失的属性
    /// <summary>
    /// 获取角色肩膀高度
    /// </summary>
    public float CharacterShoulderHight { get; private set; }

    public Collider CurrentIntersectingCollider { get; set; }
    public TwoBoneIKConstraint CurrentIkConstraint { get; private set; }
    public MultiRotationConstraint CurrentMultiRotationConstraint { get; private set; }
    public Transform CurrentIkTargetTransform { get; private set; }
    public Transform CurrentShoulderTransform { get; private set; }
    public EBodySide CurrentBodySide { get; private set; }
    public Vector3 ClosestPointOnColliderFromShoulder { get; set; } = Vector3.positiveInfinity;

    public void SetCurrentSide(Vector3 positionToCheck)
    {
        Vector3 leftShoulder = _leftIkConstraint.data.root.transform.position;
        Vector3 rightShoulder = _rightIkConstraint.data.root.transform.position;
        
        bool isLeftCloser = Vector3.Distance(positionToCheck, leftShoulder) < Vector3.Distance(positionToCheck, rightShoulder);
        if (isLeftCloser)
        {
            Debug.Log("Left side is closer");
            CurrentBodySide = EBodySide.LEFT;
            CurrentIkConstraint = _leftIkConstraint;
            CurrentMultiRotationConstraint = _leftMultiRotationConstraint;
        }
        else
        {
            Debug.Log("Right side is closer");
            CurrentBodySide = EBodySide.RIGHT;
            CurrentIkConstraint = _rightIkConstraint;
            CurrentMultiRotationConstraint = _rightMultiRotationConstraint;
        }

        CurrentShoulderTransform = CurrentIkConstraint.data.root.transform;
        CurrentIkTargetTransform = CurrentIkConstraint.data.target.transform;
    }
}
