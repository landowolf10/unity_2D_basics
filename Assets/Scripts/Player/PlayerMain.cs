using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMain : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb;

    [Header("Layer masks")]
    [SerializeField] private LayerMask groundLayer;

    [Header("Movement variables")]
    private PlayerController pc;
    [SerializeField] private float movementAcceleration = 70;
    [SerializeField] private float maxMoveSpeed = 12;
    [SerializeField] private float linearDrag = 7;
    private bool facingRight = true;


    [Header("Jump variables")]
    [SerializeField] private float jumpForce = 20f;
    [SerializeField] private float gravity = 1f;
    [SerializeField] private float fallMultiplier = 5f;
    [SerializeField] private bool canJump;
    [SerializeField] private float hangTime = 0.1f;
    private float hangTimeCounter;

    [Header("Ground collision variables")]
    [SerializeField] private Transform groundCheck1;
    [SerializeField] private Transform groundCheck2;
    [SerializeField] private Dictionary<string, bool> _onGround;

    [Header("Animations")]
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        pc = new PlayerController();
    }

    // Update is called once per frame
    void Update()
    {
        Dictionary<string, bool> onGround = pc.isGrounded(groundCheck1, groundCheck2, groundLayer);

        if (Input.GetButtonDown("Jump") && hangTimeCounter > 0)
            canJump = true;

        animator.SetBool("isGrounded", (onGround["grounded1"] || onGround["grounded2"]));
        animator.SetFloat("horizontalDirection", Mathf.Abs(pc.playerInput().x));

        if ((pc.playerInput().x > 0 && !facingRight) || (pc.playerInput().x < 0 && facingRight))
            flipCharacter();

        if ((onGround["grounded1"] || onGround["grounded2"]))
        {
            hangTimeCounter = hangTime;
            //Debug.Log("Hang time: " + hangTimeCounter);
        }
        
        else
            hangTimeCounter -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        pc.movePlayer(rb, movementAcceleration, maxMoveSpeed);
        pc.modifyPhysics(rb, linearDrag, gravity, fallMultiplier, groundCheck1, groundCheck2, groundLayer);

        if (canJump)
        {
            pc.jump(rb, jumpForce);
            canJump = false;
            hangTimeCounter = 0f;
        }
    }

    private void LateUpdate()
    {
        Dictionary<string, bool> onGround = pc.isGrounded(groundCheck1, groundCheck2, groundLayer);

        if (onGround["grounded1"] || onGround["grounded2"])
            animator.SetBool("isJumping", false);
        else
            animator.SetBool("isJumping", true);
    }

    public void flipCharacter()
    {
        facingRight = !facingRight;
        transform.rotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);
    }
}