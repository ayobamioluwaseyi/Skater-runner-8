using System.Collections;
using System.Collections.Generic;
/////using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;

    // Animation
    private Animator animator;

    private const float LANE_DISTANCE = 2.5f;
    private const float TURN_SPEED = 0.05f;

    private bool isRunning = false;

    // Movement
    private float jumpForce = 7.0f;
    private float gravity = 12.0f;
    private float verticalVelocity;

    // speed modifier
    private float defaultSpeed = 2.0f;
    private float speed;
    private float speedIncreaseLastTick;
    private float speedIncreaseTime = 2.5f;
    private float speedIncreaseAmount = 0.1f;

    private int desiredLane = 1; // 0 -left 1 -middle 2-right



    // Start is called before the first frame update
    void Start()
    {
        speed = defaultSpeed;
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isRunning)
        {
            return;
        }

        if (Time.time - speedIncreaseLastTick > speedIncreaseTime)
        {
            speedIncreaseLastTick = Time.time;
            speed += speedIncreaseAmount;
            // change the modifier text
            GameManager.Instance.UpdateModifier(speed - defaultSpeed);
        }

        // get user input on which lane he should be
        if (MobileInput.Instance.SwipeLeft)
        {
            // Move Left
            MoveLane(false);
        }
        if (MobileInput.Instance.SwipeRight)
        {
            MoveLane(true);
        }

        // calculate where player should be
        Vector3 targetPosition = transform.position.z * Vector3.forward;
        if (desiredLane == 0) // left
        {
            targetPosition += Vector3.left * LANE_DISTANCE;
        }
        else if (desiredLane == 2) // right
        {
            targetPosition += Vector3.right * LANE_DISTANCE;
        }

        // move vector calculation
        Vector3 moveVector = Vector3.zero;

        moveVector.x = (targetPosition - transform.position).normalized.x * speed; // where player is supposed to be - where he is at the moment

        bool isGrounded = IsGrounded();
        animator.SetBool("Grounded", isGrounded);

        // calculate Y
        if (isGrounded) //on the ground
        {
            verticalVelocity = -0.1f; // snap to the floor at all times

            if (MobileInput.Instance.SwipeUp)
            {
                //Jump
                animator.SetTrigger("Jump");
                verticalVelocity = jumpForce;
            }
            else if (MobileInput.Instance.SwipeDown)
            {
                //Slide
                StartSliding();
                Invoke("StopSliding", 1.5f);
            }
        }
        else
        {
            // make the num go down
            verticalVelocity -= (gravity * Time.deltaTime);

            // fast falling mechanic
            if (MobileInput.Instance.SwipeDown)
            {
                verticalVelocity = -jumpForce;
            }
        }


        moveVector.y = verticalVelocity;
        moveVector.z = speed;

        // Move the player
        controller.Move(moveVector * Time.deltaTime);

        // Rotate player to where he is going
        Vector3 direction = controller.velocity;
        if (direction != Vector3.zero)
        {
            direction.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, direction, TURN_SPEED);
        }

    }

    private void MoveLane(bool goingRight)
    {
        //// left
        //if(!goingRight)
        //{
        //    desiredLane--;
        //    if(desiredLane == -1)
        //    {
        //        desiredLane = 0;
        //    }
        //} else
        //{
        //    desiredLane++;
        //    if(desiredLane == 3)
        //    {
        //        desiredLane = 2;
        //    }
        //}
        //The above line of code can be written as 
        desiredLane += (goingRight) ? 1 : -1;
        desiredLane = Mathf.Clamp(desiredLane, 0, 2);
    }

    private bool IsGrounded()
    {
        // a ray is a 2 parameter object that takes in an original position and direction
        Ray groundRay = new Ray(new Vector3(
            controller.bounds.center.x,
            (controller.bounds.center.y - controller.bounds.extents.y) + 0.2f, controller.bounds.center.z), Vector3.down);
        Debug.DrawLine(groundRay.origin, groundRay.direction, Color.cyan, 1.0f);

        return (Physics.Raycast(groundRay, 0.2f + 0.1f));

    }

    public void StartRunning()
    {
        isRunning = true;
        // at game start, the player is idle. This makes the player run when screen is touched
        animator.SetTrigger("Start Running");
    }

    private void StartSliding()
    {
        animator.SetBool("Sliding", true);
        controller.height /= 2;
        controller.center = new Vector3(controller.center.x, controller.center.y / 2, controller.center.z);
    }

    private void StopSliding()
    {
        animator.SetBool("Sliding", false);
        controller.height *= 2;
        controller.center = new Vector3(controller.center.x, controller.center.y * 2, controller.center.z);
    }
    private void Crash()
    {
        animator.SetTrigger("Death");
        isRunning = false;
        GameManager.Instance.OnDeath();
    }

    // this works for only character controller
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        switch (hit.gameObject.tag)
        {
            case "Obstacle":
                Crash();
                break;
        }
    }
}
