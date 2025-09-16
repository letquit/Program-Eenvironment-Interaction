using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 玩家状态机类，用于管理玩家的移动和动画状态
/// 使用Unity新输入系统
/// </summary>
public class PlayerStateMachine : MonoBehaviour
{
    [Header("移动设置")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    
    [Header("组件引用")]
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private Animator playerAnimator;
    
    private string walkBoolParameter = "walk";
    
    private PlayerInput playerInput;
    private Vector2 moveInput;
    private Vector3 moveDirection;
    private bool isMoving = false;

    /// <summary>
    /// 在Awake中初始化组件引用检查
    /// </summary>
    private void Awake()
    {
        // 如果没有在Inspector中分配，则尝试自动获取组件
        if (playerRigidbody == null)
            playerRigidbody = GetComponent<Rigidbody>();
            
        if (playerAnimator == null)
            playerAnimator = GetComponent<Animator>();
            
        // 确保Rigidbody的设置正确
        if (playerRigidbody != null)
        {
            // 冻结旋转，防止物理碰撞导致的意外旋转
            playerRigidbody.freezeRotation = true;
        }
    }

    /// <summary>
    /// 在Start中进行验证
    /// </summary>
    private void Start()
    {
        // 确保必要的组件存在
        if (playerRigidbody == null)
        {
            Debug.LogError("Player Rigidbody is not assigned!", this);
        }
        
        if (playerAnimator == null)
        {
            Debug.LogError("Player Animator is not assigned!", this);
        }
    }

    /// <summary>
    /// 每帧更新输入处理
    /// </summary>
    private void Update()
    {
        HandleInput();
        UpdateAnimation();
    }

    /// <summary>
    /// 物理更新中处理移动
    /// </summary>
    private void FixedUpdate()
    {
        HandleMovement();
    }

    /// <summary>
    /// 处理玩家输入 - 使用新输入系统
    /// </summary>
    private void HandleInput()
    {
        Keyboard keyboard = Keyboard.current;
        
        if (keyboard != null)
        {
            float horizontal = 0f;
            float vertical = 0f;
            
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                horizontal = -1f;
            else if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                horizontal = 1f;
                
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
                vertical = 1f;
            else if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
                vertical = -1f;
                
            moveInput = new Vector2(horizontal, vertical);
            moveDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
            
            isMoving = moveDirection.magnitude > 0.1f;
        }
    }

    /// <summary>
    /// 处理玩家移动逻辑
    /// </summary>
    private void HandleMovement()
    {
        if (isMoving && playerRigidbody != null)
        {
            // 计算移动向量 - 使用 Rigidbody.velocity 确保物理碰撞检测
            Vector3 movement = moveDirection * moveSpeed;
            
            // 设置刚体速度，确保物理引擎能正确处理碰撞
            playerRigidbody.linearVelocity = new Vector3(movement.x, playerRigidbody.linearVelocity.y, movement.z);
            
            // 旋转玩家朝向移动方向
            RotateTowardsDirection(moveDirection);
        }
        else if (playerRigidbody != null)
        {
            // 当没有输入时，停止水平移动但保持垂直速度（如重力）
            playerRigidbody.linearVelocity = new Vector3(0, playerRigidbody.linearVelocity.y, 0);
        }
    }

    /// <summary>
    /// 旋转玩家朝向指定方向
    /// </summary>
    /// <param name="direction">要朝向的方向</param>
    private void RotateTowardsDirection(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            playerRigidbody.MoveRotation(Quaternion.Slerp(playerRigidbody.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }
    }

    /// <summary>
    /// 更新动画状态
    /// </summary>
    private void UpdateAnimation()
    {
        // 设置动画参数控制行走状态
        if (playerAnimator != null)
            playerAnimator.SetBool(walkBoolParameter, isMoving);
    }
}
