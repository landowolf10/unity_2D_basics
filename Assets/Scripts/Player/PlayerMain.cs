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
    [SerializeField] private float hangTime;
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
        //Dictionary that stores true or false depending if each transform object is on the ground.
        Dictionary<string, bool> onGround = pc.isGrounded(groundCheck1, groundCheck2, groundLayer);

        if (Input.GetButtonDown("Jump") && hangTimeCounter > 0f)
            canJump = true;

        animator.SetBool("isGrounded", (onGround["grounded1"] || onGround["grounded2"]));
        animator.SetFloat("horizontalDirection", Mathf.Abs(pc.playerInput().x));

        if ((pc.playerInput().x > 0 && !facingRight) || (pc.playerInput().x < 0 && facingRight))
            flipCharacter();

        if ((!onGround["grounded1"] || !onGround["grounded2"]))
        {
            //If the player is not touching the ground, then the hang time counter begins to decrease,
            //and while the counter is greater than the hang time (0.2) the player can jump.
            hangTimeCounter -= Time.deltaTime;
            return;
        }

        //Hang time counter will have the hang time value (0.2) only if the player is on the ground,
        //and if the hang time counter is greater than 0 (in this case it is because it now has the 0.2 value)
        //then the player can jump.
        //In other words, the player only has 0.2 seconds to jump after leaving the ground.
        hangTimeCounter = hangTime;
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

        animator.SetBool("isJumping", false);

        if (!onGround["grounded1"] || !onGround["grounded2"])
            animator.SetBool("isJumping", true);
    }

    public void flipCharacter()
    {
        facingRight = !facingRight;
        transform.rotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);
    }
}