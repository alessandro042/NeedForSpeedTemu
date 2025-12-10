using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public static class InputHelper
{
    public static bool GetKey(KeyCode code)
    {
#if ENABLE_INPUT_SYSTEM
        
        var kb = Keyboard.current;
        if (kb != null)
        {
            if (code == KeyCode.W) return kb.wKey.isPressed;
            if (code == KeyCode.S) return kb.sKey.isPressed;
            if (code == KeyCode.A) return kb.aKey.isPressed;
            if (code == KeyCode.D) return kb.dKey.isPressed;
            if (code == KeyCode.Space) return kb.spaceKey.isPressed;
        }
#endif
        return Input.GetKey(code);
    }

    public static float GetHorizontal()
    {
        float h = 0f;
#if ENABLE_INPUT_SYSTEM
        var kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) h -= 1f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) h += 1f;
        }

        var gp = Gamepad.current;
        if (gp != null)
        {
            h += gp.leftStick.ReadValue().x;
        }
#endif
        if (Mathf.Approximately(h, 0f))
        {
            h = Input.GetAxisRaw("Horizontal");
        }
        return Mathf.Clamp(h, -1f, 1f);
    }
}
