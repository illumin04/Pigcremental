using UnityEngine;

public class FreePig : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpDistance = 100f;
    public float jumpDuration = 0.3f;
    public float jumpHeight = 45f;

    [Header("Hop Burst")]
    public int minHops = 1;
    public int maxHops = 3;

    [Header("Idle Settings")]
    public float minIdleTime = 0.7f;
    public float maxIdleTime = 2.0f;

    [Header("Air Squash")]
    public float horizontalStretch = 0.30f;
    public float verticalSquash = 0.20f;

    private RectTransform rectTransform;
    private RectTransform parentRect;

    private Vector2 jumpStart;
    private Vector2 jumpTarget;

    private float jumpTimer;
    private float idleTimer;

    private int hopsRemaining;

    private bool isJumping;

    private Vector3 originalScale;

    // 1 = 朝右
    // -1 = 朝左
    private float facing = 1f;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        parentRect = transform.parent.GetComponent<RectTransform>();

        originalScale = transform.localScale;

        StartIdle();
    }

    void Update()
    {
        if (isJumping)
        {
            UpdateJump();
        }
        else
        {
            UpdateIdle();
        }
    }

    void UpdateIdle()
    {
        idleTimer -= Time.deltaTime;

        // 停下来时恢复正常比例，
        // 但保留猪最后一次面朝的方向。
        ApplyNormalScale();

        if (idleTimer <= 0f)
        {
            // Random.Range(int, int) 的最大值不包含，
            // 所以 +1 才能真的随机到 maxHops。
            hopsRemaining = Random.Range(
                minHops,
                maxHops + 1
            );

            StartJump();
        }
    }

    void StartIdle()
    {
        isJumping = false;

        ApplyNormalScale();

        idleTimer = Random.Range(
            minIdleTime,
            maxIdleTime
        );
    }

    void StartJump()
    {
        isJumping = true;
        jumpTimer = 0f;

        jumpStart = rectTransform.anchoredPosition;

        // 随机选择这一跳的移动方向。
        Vector2 direction =
            Random.insideUnitCircle.normalized;

        float halfWidth =
            parentRect.rect.width / 2f;

        float halfHeight =
            parentRect.rect.height / 2f;

        float pigHalfWidth =
            rectTransform.rect.width / 2f;

        float pigHalfHeight =
            rectTransform.rect.height / 2f;

        Vector2 potentialTarget =
            jumpStart + direction * jumpDistance;

        // 如果这一跳会撞左右边界，
        // 就把水平方向反射回来。
        if (
            potentialTarget.x <
            -halfWidth + pigHalfWidth
            ||
            potentialTarget.x >
            halfWidth - pigHalfWidth
        )
        {
            direction.x *= -1f;
        }

        // 如果这一跳会撞上下边界，
        // 就把垂直方向反射回来。
        if (
            potentialTarget.y <
            -halfHeight + pigHalfHeight
            ||
            potentialTarget.y >
            halfHeight - pigHalfHeight
        )
        {
            direction.y *= -1f;
        }

        // 根据真正最终使用的方向决定朝向。
        if (direction.x > 0.01f)
        {
            facing = -1f;
        }
        else if (direction.x < -0.01f)
        {
            facing = 1f;
        }

        jumpTarget =
            jumpStart + direction * jumpDistance;

        // 防止极端情况下稍微越界。
        jumpTarget =
            ClampToBounds(jumpTarget);
    }

    void UpdateJump()
    {
        jumpTimer += Time.deltaTime;

        float t =
            jumpTimer / jumpDuration;

        if (t >= 1f)
        {
            rectTransform.anchoredPosition =
                jumpTarget;

            ApplyNormalScale();

            hopsRemaining--;

            // 这一轮还有剩余跳数，
            // 就立刻进行下一跳。
            if (hopsRemaining > 0)
            {
                StartJump();
            }
            else
            {
                StartIdle();
            }

            return;
        }

        // 水平方向从起点平滑移动到终点。
        Vector2 movePosition =
            Vector2.Lerp(
                jumpStart,
                jumpTarget,
                t
            );

        // 0 -> 1 -> 0
        // 形成一个拱形跳跃轨迹。
        float jumpCurve =
            Mathf.Sin(t * Mathf.PI);

        float verticalOffset =
            jumpCurve * jumpHeight;

        rectTransform.anchoredPosition =
            movePosition
            + Vector2.up * verticalOffset;

        ApplyAirSquash(jumpCurve);
    }

    void ApplyAirSquash(float jumpCurve)
    {
        // 空中越接近最高点，
        // 横向越宽。
        float scaleX =
            Mathf.Abs(originalScale.x)
            * (1f + jumpCurve * horizontalStretch);

        // 空中越接近最高点，
        // 纵向越扁。
        float scaleY =
            originalScale.y
            * (1f - jumpCurve * verticalSquash);

        transform.localScale =
            new Vector3(
                scaleX * facing,
                scaleY,
                originalScale.z
            );
    }

    void ApplyNormalScale()
    {
        transform.localScale =
            new Vector3(
                Mathf.Abs(originalScale.x) * facing,
                originalScale.y,
                originalScale.z
            );
    }

    Vector2 ClampToBounds(Vector2 pos)
    {
        float halfWidth =
            parentRect.rect.width / 2f;

        float halfHeight =
            parentRect.rect.height / 2f;

        float pigHalfWidth =
            rectTransform.rect.width / 2f;

        float pigHalfHeight =
            rectTransform.rect.height / 2f;

        pos.x = Mathf.Clamp(
            pos.x,
            -halfWidth + pigHalfWidth,
            halfWidth - pigHalfWidth
        );

        pos.y = Mathf.Clamp(
            pos.y,
            -halfHeight + pigHalfHeight,
            halfHeight - pigHalfHeight
        );

        return pos;
    }
}