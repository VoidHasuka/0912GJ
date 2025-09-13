using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D))]
public class ReceiverMove : MonoBehaviour
{
    public float moveSpeed = 5f;           // 移动速度（单位/秒）
    public Camera cam;                     // 用来转换鼠标点击的相机

    private Rigidbody2D rb;
    private Vector3 targetPos;            // nullable：若为 null 表示目前不动

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (cam == null)
            cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 判断是否点击在 UI 元素上
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {

                // 点击在 UI（按钮或其他 UI 元素上），不移动
                return;
            }

            Vector3 mouseScreen = Input.mousePosition;
            // z 不管，因为是 2D 正交相机，ScreenToWorldPoint 的 z 会被忽略（或者用相机的距离）
            Vector3 worldPos = cam.ScreenToWorldPoint(mouseScreen);
            // 如果对象在某个 z 层，比如 z=0，就把目标的 z 设为这一层
            worldPos.z = transform.position.z;

            targetPos = new Vector3(worldPos.x, worldPos.y, worldPos.z = transform.position.z);
            rb.gameObject.transform.DOMove(targetPos, 0.7f).SetEase(Ease.OutCubic);
        }
    }

    void FixedUpdate()
    {
        //if (!targetPos.HasValue)
        //    return;

        //Vector2 currentPos = rb.position;
        //Vector2 destPos = targetPos.Value;
        //Vector2 dir = destPos - currentPos;
        //float dist = dir.magnitude;

        //if (dist < 0.01f)
        //{
        //    // 已经到达或非常接近目标，停止移动
        //    targetPos = null;
        //    rb.velocity = Vector2.zero;
        //    return;
        //}

        //Vector2 moveStep = dir.normalized * moveSpeed * Time.fixedDeltaTime;

        //// 防止 overshoot（走过头）
        //if (moveStep.sqrMagnitude >= dir.sqrMagnitude)
        //{
        //    rb.MovePosition(destPos);
        //}
        //else
        //{
        //    rb.MovePosition(currentPos + moveStep);
        //}
    }
}
