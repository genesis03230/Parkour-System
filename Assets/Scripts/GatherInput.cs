using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ParkourSystem
{
    public class GatherInput : MonoBehaviour
    {
        private PlayerInput playerInput; 

        private InputAction lookAction;
        private InputAction moveAction;
        private InputAction jumpAction;
        private InputAction dropAction;
        private InputAction runAction;
        private InputAction longAction;

        [SerializeField] private float smoothTime = 4f;

        public Vector2 lookInput;
        public Vector2 smoothedDirection;
        private Vector2 _direction;
        public Vector2 Direction { get => _direction; }

        public bool tryToJump;
        public bool tryToDrop;
        public bool tryToRun;
        public bool tryToLong;

        public bool usingGamepad;

        private void Awake() 
        {
            playerInput = GetComponent<PlayerInput>(); 
            lookAction = playerInput.actions["Look"];
            moveAction = playerInput.actions["Move"];
            jumpAction = playerInput.actions["Jump"];
            dropAction = playerInput.actions["Drop"];
            runAction = playerInput.actions["FastRun"];
            longAction = playerInput.actions["Long"];
        }

        private void OnEnable()
        {
            lookAction.performed += ReadLookInput;
            lookAction.canceled += OnLookCanceled;
            jumpAction.performed += Jump;
            jumpAction.canceled += OnJumpCanceled;
            dropAction.performed += Drop;
            dropAction.canceled += OnDropCanceled;
            runAction.performed += Run;
            runAction.canceled += OnRunCanceled;
            longAction.performed += Long;
            longAction.canceled += OnLongCanceled;
        }

        private void JumpAction_canceled(InputAction.CallbackContext obj)
        {
            throw new System.NotImplementedException();
        }

        private void Update()
        {
            _direction = moveAction.ReadValue<Vector2>();
            smoothedDirection = new Vector2(
                Mathf.MoveTowards(smoothedDirection.x, _direction.x, smoothTime * Time.deltaTime),
                Mathf.MoveTowards(smoothedDirection.y, _direction.y, smoothTime * Time.deltaTime)
                );
        }

        private void ReadLookInput(InputAction.CallbackContext context) 
        {
            lookInput = context.ReadValue<Vector2>();
            usingGamepad = context.control.device is Gamepad;
        }

        private void OnLookCanceled(InputAction.CallbackContext context)
        {
                lookInput = Vector2.zero;
        }

        private void Jump(InputAction.CallbackContext context)
        {
            tryToJump = true;
        }

        private void OnJumpCanceled(InputAction.CallbackContext context)
        {
            tryToJump= false;
        }

        private void Drop(InputAction.CallbackContext context)
        {
            tryToDrop = true;
        }

        private void OnDropCanceled(InputAction.CallbackContext context)
        {
            tryToDrop= false;
        }

        private void Run(InputAction.CallbackContext context)
        {
            tryToRun = true;
        }

        private void OnRunCanceled(InputAction.CallbackContext context)
        {
            tryToRun = false;
        }

        private void Long(InputAction.CallbackContext context)
        {
            tryToLong = true;
        }

        private void OnLongCanceled(InputAction.CallbackContext context)
        {
            tryToLong = false;
        }

        private void OnDisable()
        {
            lookAction.performed -= ReadLookInput;
            lookAction.canceled -= OnLookCanceled;
            jumpAction.performed -= Jump;
            jumpAction.canceled -= OnJumpCanceled;
            dropAction.performed -= Drop;
            dropAction.canceled -= OnDropCanceled;
            runAction.performed -= Run;
            runAction.canceled -= OnRunCanceled;
            longAction.performed -= Long;
            longAction.canceled -= OnLongCanceled;
        }
    }
}
