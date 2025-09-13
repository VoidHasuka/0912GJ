using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D))]
public class ReceiverMove : MonoBehaviour
{
    public float moveSpeed = 5f;           // �ƶ��ٶȣ���λ/�룩
    public Camera cam;                     // ����ת������������

    private Rigidbody2D rb;
    private Vector3 targetPos;            // nullable����Ϊ null ��ʾĿǰ����

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
            // �ж��Ƿ����� UI Ԫ����
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {

                // ����� UI����ť������ UI Ԫ���ϣ������ƶ�
                return;
            }

            Vector3 mouseScreen = Input.mousePosition;
            // z ���ܣ���Ϊ�� 2D ���������ScreenToWorldPoint �� z �ᱻ���ԣ�����������ľ��룩
            Vector3 worldPos = cam.ScreenToWorldPoint(mouseScreen);
            // ���������ĳ�� z �㣬���� z=0���Ͱ�Ŀ��� z ��Ϊ��һ��
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
        //    // �Ѿ������ǳ��ӽ�Ŀ�ֹ꣬ͣ�ƶ�
        //    targetPos = null;
        //    rb.velocity = Vector2.zero;
        //    return;
        //}

        //Vector2 moveStep = dir.normalized * moveSpeed * Time.fixedDeltaTime;

        //// ��ֹ overshoot���߹�ͷ��
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
