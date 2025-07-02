using System.Collections.Generic;
using UnityEngine;
using System;

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
    public GestureConfig config;

    [Header("Events")]
    public Action<GestureData> OnGestureRecognized;

    private Vector2 mouseDownPos;
    private float mouseDownTime;
    private bool isRightClickHeld = false;

    private List<GestureData> gestureHistory = new List<GestureData>();

    private Vector2? gestureStart = null;
    private Vector2? gestureEnd = null;

    // Gán khi gesture bắt đầu
   
    private void Update()
    {
        HandleMouseInput();
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseDownPos = Input.mousePosition;
            mouseDownTime = Time.time;
            StartGesture(mouseDownPos);
        }
        if (Input.GetMouseButtonUp(0))
        {
            InterpretGesture(isRightClick: false);
            EndGesture(Input.mousePosition);
        }

        if (Input.GetMouseButtonDown(1))
        {
            mouseDownPos = Input.mousePosition;
            mouseDownTime = Time.time;
            isRightClickHeld = true;
            StartGesture(mouseDownPos);
        }
        if (Input.GetMouseButtonUp(1))
        {
            float holdDuration = Time.time - mouseDownTime;
            if (holdDuration < config.holdBlockMinDuration)
            {
                InterpretGesture(isRightClick: true);
            }
            EndGesture(Input.mousePosition);
            isRightClickHeld = false;
        }

        if (isRightClickHeld)
        {
            Vector2 offset = (Vector2)Input.mousePosition - mouseDownPos;
            if (offset.magnitude > config.gestureThreshold)
            {
                TriggerBlock(offset.normalized);
            }
        }
    }

    void InterpretGesture(bool isRightClick)
    {
        Vector2 releasePos = Input.mousePosition;
        Vector2 dir = releasePos - mouseDownPos;
        float duration = Time.time - mouseDownTime;

        if (dir.magnitude < config.gestureThreshold) return;

        float speed = dir.magnitude / duration;
        GestureType directionType = DetectDirection(dir.normalized);
        GestureType finalType = GestureType.SlashRight;

        if (isRightClick && speed >= config.parrySpeedThreshold)
        {
            finalType = GestureType.Parry;
        }

        GestureData data = new GestureData(finalType == GestureType.Parry ? GestureType.Parry : directionType, dir.normalized, speed, mouseDownPos, releasePos, Time.time);
        HandleComboMemory(data);
        OnGestureRecognized?.Invoke(data);
    }

    void HandleComboMemory(GestureData current)
    {
        if (gestureHistory.Count > 0)
        {
            Vector2 lastEnd = gestureHistory[gestureHistory.Count - 1].endPoint;
            if (Vector2.Distance(lastEnd, current.startPoint) < config.comboResetDistance)
            {
                // Combo continues
            }
            else
            {
                gestureHistory.Clear(); // Reset combo
            }
        }
        gestureHistory.Add(current);
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

    #region DRAW UI LINES (for debugging)

    void StartGesture(Vector2 pos)
    {
        gestureStart = pos;
        gestureEnd = null;
    }

    // Gán khi gesture kết thúc
    void EndGesture(Vector2 pos)
    {
        gestureEnd = pos;
        // ... detect gesture
        Invoke(nameof(ClearLine), 0.3f); // Auto clear sau 0.3s
    }

    void ClearLine()
    {
        gestureStart = null;
        gestureEnd = null;
    }

    void OnGUI()
    {
        if (gestureStart.HasValue && gestureEnd.HasValue)
        {
            Vector2 start = gestureStart.Value;
            Vector2 end = gestureEnd.Value;

            // Vẽ line trên UI
            DrawLine(start, end, Color.red, 2f);
        }
    }

    void DrawLine(Vector2 start, Vector2 end, Color color, float width)
    {
        Matrix4x4 matrix = GUI.matrix;
        Color oldColor = GUI.color;

        Vector2 d = end - start;
        float angle = Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg;
        float length = d.magnitude;

        GUI.color = color;
        GUIUtility.RotateAroundPivot(angle, start);
        GUI.DrawTexture(new Rect(start.x, start.y, length, width), Texture2D.whiteTexture);
        GUIUtility.RotateAroundPivot(-angle, start);

        GUI.color = oldColor;
        GUI.matrix = matrix;
    }

    #endregion
}

