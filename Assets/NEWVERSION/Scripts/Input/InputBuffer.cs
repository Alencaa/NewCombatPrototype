using System;
using UnityEngine;

public class InputBuffer
{
    private GestureData bufferedGesture = null;
    private float bufferTimer = 0f;
    private float bufferDuration = 0.5f;

    public void BufferGesture(GestureData gesture)
    {
        bufferedGesture = gesture;
        bufferTimer = bufferDuration;
    }

    public void Update(float deltaTime)
    {
        if (bufferedGesture != null)
        {
            bufferTimer -= deltaTime;
            if (bufferTimer <= 0f)
                Clear();
        }
    }

    public bool HasGesture => bufferedGesture != null;

    public GestureData ConsumeGesture()
    {
        if (bufferedGesture == null)
            throw new System.InvalidOperationException("No buffered gesture to consume.");

        GestureData result = bufferedGesture;
        Clear();
        return result;
    }

    public GestureData PeekGesture() => bufferedGesture;

    public void Clear()
    {
        bufferedGesture = null;
        bufferTimer = 0f;
    }
}

