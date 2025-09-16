using UnityEngine;

/// <summary>
/// 相机跟随系统，使相机跟随目标但不跟随其旋转
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("目标设置")]
    [SerializeField] private Transform target; // 要跟随的目标
    [SerializeField] private Vector3 offset = new Vector3(0, 5, -5); // 相机相对于目标的偏移
    
    [Header("跟随设置")]
    [SerializeField] private float followSpeed = 5f; // 跟随速度
    [SerializeField] private bool smoothFollow = true; // 是否平滑跟随
    
    private Vector3 targetPosition;

    /// <summary>
    /// 初始化相机跟随系统
    /// 在游戏开始时查找目标对象并设置初始相机位置
    /// </summary>
    private void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("Camera follow target not assigned. Looking for Player object...");
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
        }
        
        if (target == null)
        {
            Debug.LogError("Camera follow target is not assigned and no Player found!");
            return;
        }
        
        // 初始化相机位置
        targetPosition = target.position + offset;
        transform.position = targetPosition;
    }

    /// <summary>
    /// 每帧更新相机位置，确保在目标移动后执行以获得最新位置
    /// 根据设置决定是否使用平滑跟随，并使相机始终看向目标
    /// </summary>
    private void LateUpdate()
    {
        if (target != null)
        {
            // 计算目标位置（基于目标位置和偏移量，但不考虑目标旋转）
            targetPosition = target.position + offset;
            
            if (smoothFollow)
            {
                // 平滑跟随
                transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
            }
            else
            {
                // 直接跟随
                transform.position = targetPosition;
            }
            
            // 让相机看向目标
            transform.LookAt(target);
        }
    }
}

