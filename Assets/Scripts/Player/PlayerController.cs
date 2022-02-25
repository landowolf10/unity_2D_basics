using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController
{
    public Vector2 playerInput()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    public void movePlayer(Rigidbody2D rb, float movementAcceleration, float maxMoveSpeed)
    {
        Vector2 direction = playerInput();

        rb.AddForce(new Vector2(direction.x, 0f) * movementAcceleration);

        if (Mathf.Abs(rb.velocity.x) > maxMoveSpeed)
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxMoveSpeed, rb.velocity.y);
    }

    public void jump(Rigidbody2D rb, float jumpForce)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0); //Reset vertical velocity
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    public Dictionary<string, bool> isGrounded(Transform groundCheck1, Transform groundCheck2,
        LayerMask groundLayer)
    {
        bool grounded1 = Physics2D.OverlapCircle(groundCheck1.position, 0.2f, groundLayer);
        bool grounded2 = Physics2D.OverlapCircle(groundCheck2.position, 0.2f, groundLayer);

        Dictionary<string, bool> onGround = new Dictionary<string, bool>();
        onGround.Add("grounded1", grounded1);
        onGround.Add("grounded2", grounded2);

        return onGround;
    }

    public void modifyPhysics(Rigidbody2D rb, float linearDrag, float gravity, float fallMultiplier,
        Transform groundCheck1, Transform groundCheck2, LayerMask groundLayer)
    {
        Vector2 direction = playerInput();
        Dictionary<string, bool> onGround = isGrounded(groundCheck1, groundCheck2, groundLayer);

        bool changingDirections = (direction.x > 0f && rb.velocity.x < 0f) || (direction.x < 0f && rb.velocity.x > 0f);

        if(!onGround["grounded1"] || !onGround["grounded2"])
        {
            rb.gravityScale = gravity;
            rb.drag = linearDrag * 0.15f;

            if (rb.velocity.y < 0)
                rb.gravityScale = gravity * fallMultiplier;
            else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
                rb.gravityScale = gravity * (fallMultiplier / 2);

            return;
        }

        if (Mathf.Abs(direction.x) < 0.4f || changingDirections)
            rb.drag = linearDrag;
        else
            rb.drag = 0f;

        rb.gravityScale = 0;
    }
}