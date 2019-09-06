using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour {

    private CharacterController controller;
    private Vector3 velocity;
    public float Gravity = -9.81f;
    public float MoveSpeed = 10.0f;
    public float RotateSpeed = 80.0f;
    public float JumpHeight = 2.5f;
    public Vector3 Drag;

    public bool isMoving = false;
    private bool isRotating = false;
    public bool UseMouseKeyboard = true;

    Vector3 Velocity;
    private Vector3 rotateDirection;
    public float JumpDelay = 1f;
    private float jumpTimer;
    private bool isJumping = false;

    public bool IsEnabled = true;

    void Start()
    {
        int joyCount = Input.GetJoystickNames().Length;
        Debug.Log(joyCount + " controller(s) detected.");

        if (joyCount > 0)
            UseMouseKeyboard = false;

        controller = GetComponent<CharacterController>();
    }

    public bool IsMoving
    {
        get { return isMoving; }
    }

    void Update()
    {
        if (!IsEnabled || GetComponent<RPGActor>().State == ActorState.Dead || GameManager.Instance.IsPausedForUI)
        {
            return;
        }

        if (controller.isGrounded && velocity.y < 0)
            velocity.y = 0f;

        //Movement
        Vector3 keyMove = Vector3.zero;

        if (GameManager.Instance.KeyLayout == KeyboardLayout.Azerty)
            HandleInputAzerty(ref keyMove);
        else
            HandleInputQuerty(ref keyMove);

        if(Input.GetMouseButton(0))
        {
            Vector3 mousePos = GetWorldPoint();
            Vector3 dir = mousePos - this.transform.position;
            dir.y = 0;
            keyMove = dir.normalized;
        }

        controller.Move(keyMove * Time.deltaTime * MoveSpeed);

        if (keyMove != Vector3.zero)
        {
            transform.forward = keyMove; //Set rotation towards move vector
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        //Gravity 
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            velocity.y += Mathf.Sqrt(JumpHeight * -6f * Gravity);
            isJumping = true;
        }

        if(isJumping)
        {
            jumpTimer += Time.deltaTime;
            if(jumpTimer > JumpDelay)
            {
                jumpTimer = 0;
                isJumping = false;
            }
        }

        velocity.y += Gravity * Time.deltaTime;

        velocity.x /= 1 + Drag.x * Time.deltaTime;
        velocity.y /= 1 + Drag.y * Time.deltaTime;
        velocity.z /= 1 + Drag.z * Time.deltaTime;

        //Apply gravity
        controller.Move(velocity * Time.deltaTime);

        //LeftStickInput();
        //RightStickInput();
    }

    public void HandleInputAzerty(ref Vector3 keyMove)
    {
        if (Input.GetKey(KeyCode.Q))
            keyMove.x = -1;
        if (Input.GetKey(KeyCode.D))
            keyMove.x = 1;
        if (Input.GetKey(KeyCode.S))
            keyMove.z = -1;
        if (Input.GetKey(KeyCode.Z))
            keyMove.z = 1;
    }

    public void HandleInputQuerty(ref Vector3 keyMove)
    {
        if (Input.GetKey(KeyCode.A))
            keyMove.x = -1;
        if (Input.GetKey(KeyCode.D))
            keyMove.x = 1;
        if (Input.GetKey(KeyCode.S))
            keyMove.z = -1;
        if (Input.GetKey(KeyCode.W))
            keyMove.z = 1;
    }

    Vector3 GetWorldPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 5000f))
            return hit.point;

        return this.transform.position;
    }

    void RightStickInput()
    {
        float x = Input.GetAxis("HorizontalRight");
        float y = Input.GetAxis("VerticalRight");

        if (Mathf.Abs(x) + Mathf.Abs(y) > 0.1f)
        {
            Vector3 rot = new Vector3(x, 0, y);
            RotateTowards(rot);
            isRotating = true;
        }
        else
        {
            isRotating = false;
        }
    }

    void LeftStickInput()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        if (x == 0 && y == 0)
        {
            isMoving = false;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            return;
        }

        isMoving = true;
        Vector3 rot = new Vector3(x, 0, y);

        RotateTowards(rot);
        Velocity = new Vector3(x * MoveSpeed * Time.deltaTime * 100, 0, y * MoveSpeed * Time.deltaTime * 100);

        GetComponent<Rigidbody>().velocity = Velocity;
    }

    void RotateTowards(Vector3 direction)
    {
        if (rotateDirection == direction)
            return;

        rotateDirection = Vector3.RotateTowards(transform.forward, direction, RotateSpeed * Time.deltaTime, 0.0f);
    }
}
