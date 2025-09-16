using UnityEngine;
using UnityEngine.Animations.Rigging;

/// <summary>
/// 环境交互上下文类，用于存储和管理角色与环境交互所需的各种约束和物理组件
/// </summary>
public class EnvironmentInteractionContext
{
    /// <summary>
    /// 身体侧别枚举，表示左手或右手
    /// </summary>
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
    private Vector3 _leftOriginalTargetPosition;
    private Vector3 _rightOriginalTargetPosition;

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
        _leftOriginalTargetPosition = _leftIkConstraint.data.target.transform.localPosition;
        _rightOriginalTargetPosition = _rightIkConstraint.data.target.transform.localPosition;
        OriginalTargetRotation = _leftIkConstraint.data.target.rotation;

        // 初始化角色肩膀高度
        CharacterShoulderHight = leftIkConstraint.data.root.transform.position.y;
        SetCurrentSide(Vector3.positiveInfinity);
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

    /// <summary>
    /// 获取角色肩膀高度
    /// </summary>
    public float CharacterShoulderHight { get; private set; }

    /// <summary>
    /// 当前相交的碰撞体
    /// </summary>
    public Collider CurrentIntersectingCollider { get; set; }

    /// <summary>
    /// 当前使用的IK约束组件（根据最近侧别动态设置）
    /// </summary>
    public TwoBoneIKConstraint CurrentIkConstraint { get; private set; }

    /// <summary>
    /// 当前使用的多旋转约束组件（根据最近侧别动态设置）
    /// </summary>
    public MultiRotationConstraint CurrentMultiRotationConstraint { get; private set; }

    /// <summary>
    /// 当前IK目标变换组件
    /// </summary>
    public Transform CurrentIkTargetTransform { get; private set; }

    /// <summary>
    /// 当前肩膀变换组件
    /// </summary>
    public Transform CurrentShoulderTransform { get; private set; }

    /// <summary>
    /// 当前身体侧别（左或右）
    /// </summary>
    public EBodySide CurrentBodySide { get; private set; }

    /// <summary>
    /// 从肩膀到碰撞体上的最近点位置
    /// </summary>
    public Vector3 ClosestPointOnColliderFromShoulder { get; set; } = Vector3.positiveInfinity; 

    /// <summary>
    /// 交互点Y轴偏移量
    /// </summary>
    public float InteractionPointYOffset { get; set; } = 0.0f;

    /// <summary>
    /// 碰撞体中心Y坐标
    /// </summary>
    public float ColliderCenterY { get; set; }

    /// <summary>
    /// 当前IK目标的原始局部位置
    /// </summary>
    public Vector3 CurrentOriginalTargetPosition { get; private set; }

    /// <summary>
    /// IK目标的初始旋转
    /// </summary>
    public Quaternion OriginalTargetRotation { get; private set; }

    /// <summary>
    /// 最近距离值，用于判断是否更新交互点
    /// </summary>
    public float LowestDistance { get; set; } = Mathf.Infinity;
    
    /// <summary>
    /// 根据指定位置确定当前应使用哪一侧的身体进行交互，并更新相关引用
    /// </summary>
    /// <param name="positionToCheck">需要比较距离的目标位置</param>
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
            CurrentOriginalTargetPosition = _leftOriginalTargetPosition;
        }
        else
        {
            Debug.Log("Right side is closer");
            CurrentBodySide = EBodySide.RIGHT;
            CurrentIkConstraint = _rightIkConstraint;
            CurrentMultiRotationConstraint = _rightMultiRotationConstraint;
            CurrentOriginalTargetPosition = _rightOriginalTargetPosition;
        }

        CurrentShoulderTransform = CurrentIkConstraint.data.root.transform;
        CurrentIkTargetTransform = CurrentIkConstraint.data.target.transform;
    }
}
