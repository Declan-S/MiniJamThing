using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class thescrunkly : MonoBehaviour
{
    static bool cameraRotatesWithPlayer = false;

    private Vector3 velocity;
    private Vector3 trigAppliedVelocity;

    private Rigidbody2D c_rigidbody;
    private CapsuleCollider2D c_capsuleCollider;
    [SerializeField]
    private BoxCollider2D c_verticalBonk;
    private BoxCollider2D c_leftBonk;
    private BoxCollider2D c_rightBonk;

    [SerializeField]
    private LayerMask collisionLayerMask;

    private bool grounded;

    // Start is called before the first frame update
    void Start()
    {
        // Setup components
        c_rigidbody = GetComponent<Rigidbody2D>();
        c_capsuleCollider = GetComponent<CapsuleCollider2D>();
        c_verticalBonk = transform.Find("verticalBonk").GetComponent<BoxCollider2D>();
        c_leftBonk = transform.Find("leftBonk").GetComponent<BoxCollider2D>();
        c_rightBonk = transform.Find("rightBonk").GetComponent<BoxCollider2D>();

        collisionLayerMask = LayerMask.GetMask("Level");

        grounded = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Check whether player is bonking on walls and stuff
        if (c_leftBonk.IsTouchingLayers(collisionLayerMask) && velocity.x < 0)
        {
            velocity.x = 0;
        }
        if (c_rightBonk.IsTouchingLayers(collisionLayerMask) && velocity.x > 0)
        {
            velocity.x = 0;
        }

        // Check if player is on solid ground
        if (c_verticalBonk.IsTouchingLayers(collisionLayerMask))
        {
            grounded = true;
        }
        else
        {
           grounded = false;
        }

        // Make calls to unity input axes system to get player input
        velocity += new Vector3(Input.GetAxisRaw("Horizontal") * 0.5f, 0, 0);

        if(grounded)
        {
            // Set y velocity to 0 because that just makes sense
            velocity.y = 0;

            if (Input.GetAxisRaw("Vertical") > 0.5f)
            {
                velocity.y = 10;
            }
        }
        else
        {
            // Acceleration due to gravity
            if(velocity.y > -10f)
            {
                velocity.y -= 0.5f;
            }
        }

        // limits and drag
        if(velocity.x != 0)
        {
            float direction = velocity.x / Mathf.Abs(velocity.x);
            if (Mathf.Abs(velocity.x) > 10)
            {
                velocity.x = 10 * direction;
            }

            velocity.x -= (Mathf.Abs(velocity.x) / 20) * direction;
        }

        // Calculate movement based on angle and move player
        // -y to +x is 90
        float currentZRotation = Mathf.Deg2Rad * transform.rotation.eulerAngles.z;
        trigAppliedVelocity = new Vector3(velocity.x * Mathf.Cos(currentZRotation) - velocity.y * Mathf.Sin(currentZRotation), velocity.y * Mathf.Cos(currentZRotation) - velocity.x * Mathf.Sin(currentZRotation), 0);

        c_rigidbody.MovePosition(transform.position + trigAppliedVelocity * Time.fixedDeltaTime);

    }
}
