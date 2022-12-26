using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Player Components
    Animator animator;
    CharacterController controller;
    Camera mainCamera;

    //Player config
    [Header("Player Config")]
    [SerializeField] private float speed;
    [SerializeField] private float turnSpeed = 0.1f;


    [Header("Gravity Settings")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool isGrounded;
    [SerializeField] private Vector3 velocity;
    [SerializeField] private float gravity = -10;
    [SerializeField] private float groundDistance = 0.4f;

    //Input Variables
    float inputX, inputY;
    float turnSmoothVelocity;


    private void Awake()
    {
        //Set Cursor State
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Get Components
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main;

        //Fix Character Controller Collider
        float correctHeight = controller.center.y + controller.skinWidth;
        controller.center = new Vector3(0, correctHeight, 0);
    }

    private void Update()
    {
        HandleInput();
        HandleMovement();
        HandleAnimation();

    }

    void HandleInput()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
    }

    void HandleMovement()
    {
        //Vertical Movement

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2.0f;
        }

        velocity.y += gravity * Time.deltaTime;

        Vector3 direction = new Vector3(inputX, 0, inputY).normalized;

        //Horizontal Movement
        Vector3 moveDir = Vector3.zero;
        if (direction.magnitude >= 0.1f)
        {
            
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, angle, ref turnSmoothVelocity, turnSpeed);
            transform.rotation = Quaternion.Euler(0, smoothAngle, 0);

            moveDir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
        }

        Vector3 move = (moveDir.normalized * speed * Time.deltaTime) + (velocity * Time.deltaTime);
        controller.Move(move);
    }


    void HandleAnimation()
    {
        Vector3 characterVelocity = new Vector3(controller.velocity.x, 0, controller.velocity.z);

        animator.SetFloat("Speed", characterVelocity.magnitude);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "DogStand")
        {
            GameUIManager.Instance.SetCanOpenShop(true);
            GameUIManager.Instance.SetCurrentDogStand(other.GetComponent<DogStand>());
        }
        else if (other.tag == "Meat")
        {
            MeatBehavior meat = other.GetComponent<MeatBehavior>();
            meat.OnPickUp();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "DogStand")
        {
            GameUIManager.Instance.SetCanOpenShop(false);
            GameUIManager.Instance.ClearDogStand();
            GameUIManager.Instance.CloseUpgradeUI();
            GameUIManager.Instance.CloseShopUI();
        }
    }
}
