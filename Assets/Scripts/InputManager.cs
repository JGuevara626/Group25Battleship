using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    // Start is called before the first frame update
    private TouchControls touchControls;
    private void Awake()
    {
        touchControls = new TouchControls();
    }
    private void OnEnable()
    {
        touchControls.Enable();
    }
    private void OnDisable()
    {
        touchControls.Disable();
    }
    void Start()
    {
        touchControls.Touch.TouchPress.started += ctx => StartTouch(ctx);
        touchControls.Touch.TouchPress.started += ctx => EndTouch(ctx);
    }
    private void StartTouch(InputAction.CallbackContext context)
    {
        Debug.Log("Touch Started " + touchControls.Touch.TouchPosition.ReadValue<Vector2>());
    }
    private void EndTouch(InputAction.CallbackContext context)
    {

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
