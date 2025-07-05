using DG.Tweening;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum GestureType
{
    None,
    SlashUp,
    SlashDown,
    SlashLeft,
    SlashRight,
    SlashUpLeft,
    SlashUpRight,
    SlashDownLeft,
    SlashDownRight,
    Block,
    Parry
}

public class GestureData
{
    public GestureType type;
    public Vector2 direction;
    public float strength;
    public Vector2 startPoint;
    public Vector2 endPoint;
    public float time;

    public GestureData(GestureType type, Vector2 dir, float str, Vector2 start, Vector2 end, float time)
    {
        this.type = type;
        direction = dir;
        strength = str;
        startPoint = start;
        endPoint = end;
        this.time = time;
    }
}

public class GestureInputHandler : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private GestureConfig config;
    [SerializeField] private PlayerCombatConfig playerCombatConfig;

    [Header("Events")]
    public Action<GestureData> OnGestureRecognized;
    public Action<List<GestureData>> OnComboRecognized;
    public Action<Vector2> OnBlockDirectionChanged;
    public Action OnBlockStart;
    public Action OnBlockEnd;
    private Vector2 lastBlockDirection = Vector2.zero;

    private Vector2 mouseDownPos;
    private float mouseDownTime;

    private List<GestureData> gestureBuffer = new();
    private float lastSwipeTime;

    private LineRenderer lineRenderer;
    public Material lineMaterial;
    public float lineWidth = 0.05f;

    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = 0;
        lineRenderer.useWorldSpace = true;
    }

    void Update()
    {
        HandleInputGesture();

        // Nếu buffer bị treo vì swipe quá chậm → reset
        if (gestureBuffer.Count > 0 && Time.time - lastSwipeTime > playerCombatConfig.maxComboInterval)
        {
            gestureBuffer.Clear();
        }
    }

    void HandleInputGesture()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseDownTime = Time.time;
            mouseDownPos = Input.mousePosition;


        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector2 mouseUpPos = Input.mousePosition;
            float duration = Time.time - mouseDownTime;
            Vector2 dir = mouseUpPos - mouseDownPos;


            if (dir.magnitude < config.gestureThreshold) return;

            float speed = dir.magnitude / duration;
            Vector2 normalized = dir.normalized;
            GestureType gestureType = DetectDirection(normalized);

            GestureData gesture = new GestureData(gestureType, normalized, speed, mouseDownPos, mouseUpPos, Time.time);

            HandleComboWithImmediateAttack(gesture);
        }

        if (Input.GetMouseButtonDown(1))
        {
            mouseDownPos = Input.mousePosition;
            mouseDownTime = Time.time;
            lastBlockDirection = Vector2.zero;
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 playerPos = transform.position;
            Vector2 rawDir = (mouseWorld - (Vector3)playerPos);
            if (rawDir.magnitude > 0.1f) // không dùng gestureThreshold ở đây
            {
                Vector2 normalized = rawDir.normalized;
                if (lastBlockDirection == Vector2.zero ||
                                   Vector2.Angle(lastBlockDirection, normalized) > 5f)
                {
                    OnBlockDirectionChanged?.Invoke(normalized);
                    lastBlockDirection = normalized;
                }
            }
            OnBlockStart?.Invoke();
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 playerPos = transform.position;

            Vector2 rawDir = (mouseWorld - (Vector3)playerPos);
            if (rawDir.magnitude > 0.1f) // không dùng gestureThreshold ở đây
            {
                Vector2 normalized = rawDir.normalized;

                if (lastBlockDirection == Vector2.zero ||
                    Vector2.Angle(lastBlockDirection, normalized) > 5f)
                {
                    OnBlockDirectionChanged?.Invoke(normalized);
                    lastBlockDirection = normalized;
                }
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            Vector2 mouseUpPos = Input.mousePosition;
            float holdDuration = Time.time - mouseDownTime;


            if (holdDuration < config.holdBlockMinDuration)
            {
                InterpretGesture(true, mouseDownPos, mouseUpPos, mouseDownTime); // Parry
            }
            else
            {
                OnBlockEnd?.Invoke(); // End block state
            }
        }
    }

    void HandleComboWithImmediateAttack(GestureData gesture)
    {
        float now = Time.time;

        // Reset nếu swipe quá chậm so với trước đó
        if (gestureBuffer.Count > 0 && now - lastSwipeTime > playerCombatConfig.maxComboInterval)
        {
            gestureBuffer.Clear();
        }

        gestureBuffer.Add(gesture);
        DrawSwipeLine(gesture.startPoint, gesture.endPoint, gestureBuffer.Count);

        lastSwipeTime = now;

        // TODO insert SFX combo recording

        // 🔥 Gửi attack đơn LẬP TỨC
        OnGestureRecognized?.Invoke(gesture);

        if (gestureBuffer.Count == playerCombatConfig.maxComboSteps)
        {
            OnComboRecognized?.Invoke(new List<GestureData>(gestureBuffer));
            gestureBuffer.Clear();
        }
    }

    void InterpretGesture(bool isRightClick, Vector2 start, Vector2 end, float startTime)
    {
        Vector2 dir = end - start;
        float duration = Time.time - startTime;

        if (dir.magnitude < config.gestureThreshold) return;

        float speed = dir.magnitude / duration;
        GestureType directionType = DetectDirection(dir.normalized);
        GestureType finalType = isRightClick && speed >= config.parrySpeedThreshold ? GestureType.Parry : directionType;

        GestureData data = new GestureData(finalType, dir.normalized, speed, start, end, Time.time);
        OnGestureRecognized?.Invoke(data);
    }

    void TriggerBlock(Vector2 direction)
    {

        GestureData blockData = new GestureData(GestureType.Block, direction, 0, mouseDownPos, Input.mousePosition, Time.time);
        OnGestureRecognized?.Invoke(blockData);
    }

    GestureType DetectDirection(Vector2 dir)
    {
        float angle = Vector2.SignedAngle(Vector2.right, dir);

        if (angle >= -22.5f && angle <= 22.5f) return GestureType.SlashRight;
        if (angle > 22.5f && angle <= 67.5f) return GestureType.SlashUpRight;
        if (angle > 67.5f && angle <= 112.5f) return GestureType.SlashUp;
        if (angle > 112.5f && angle <= 157.5f) return GestureType.SlashUpLeft;
        if (angle > 157.5f || angle <= -157.5f) return GestureType.SlashLeft;
        if (angle < -112.5f && angle >= -157.5f) return GestureType.SlashDownLeft;
        if (angle < -67.5f && angle >= -112.5f) return GestureType.SlashDown;
        if (angle < -22.5f && angle >= -67.5f) return GestureType.SlashDownRight;

        return GestureType.None;
    }
   
    void ClearLine()
    {
        if (lineRenderer != null)
            lineRenderer.positionCount = 0;
    }

    void DrawSwipeLine(Vector2 start, Vector2 end, int index)
    {
        GameObject lineObj = new GameObject($"SwipeLine_{index}");
        lineObj.transform.parent = this.transform;

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.material = lineMaterial;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.positionCount = 2;
        lr.useWorldSpace = true;

        // Set màu theo chỉ số swipe
        Color[] debugColors = { Color.red, Color.green, Color.blue, Color.yellow, Color.cyan, Color.magenta, Color.white };
        Color color = debugColors[index % debugColors.Length];
        lr.startColor = color;
        lr.endColor = color;

        Vector3 worldStart = Camera.main.ScreenToWorldPoint(new Vector3(start.x, start.y, 10));
        Vector3 worldEnd = Camera.main.ScreenToWorldPoint(new Vector3(end.x, end.y, 10));

        lr.SetPosition(0, worldStart);
        lr.SetPosition(1, worldEnd);

        Destroy(lineObj, 0.4f); // auto destroy after 2 sec
    }

}


