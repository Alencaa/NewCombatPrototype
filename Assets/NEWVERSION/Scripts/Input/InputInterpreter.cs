using UnityEngine;
using CombatV2.FSM;
using CombatV2.FSM.States;
using CombatV2.InputSystem;

public class InputInterpreter : MonoBehaviour
{
    private GestureInputHandler gestureHandler;
    private PlayerController player;

    void Awake()
    {
        gestureHandler = GetComponent<GestureInputHandler>();
        player = GetComponent<PlayerController>();
    }

    void Update()
    {
        var gesture = gestureHandler.CurrentGesture;
        if (gesture.Count == 1)
        {
            string dir = gesture[0];
            if (dir == "LEFT" || dir == "RIGHT")
            {
                Debug.Log("Detected Parry Swipe!");
                // Trigger FSM transition if needed
            }
            else if (dir == "UP" || dir == "DOWN")
            {
                Debug.Log("Detected Directional Block!");
            }

            gestureHandler.ResetGesture();
        }
        else if (gesture.Count >= 3)
        {
            Debug.Log("Detected Combo Gesture: " + string.Join("→", gesture));
            // TODO: pass gesture to Combo system
            gestureHandler.ResetGesture();
        }
    }
    public bool DidAttack()
    {
        return Input.GetKeyDown(KeyCode.Mouse0); // LMB = Attack
    }
}
