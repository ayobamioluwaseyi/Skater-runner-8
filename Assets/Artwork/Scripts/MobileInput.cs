using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileInput : MonoBehaviour
{
    private const float DEADZONE = 100.0f;
    public static MobileInput Instance { get; set; }

    private bool tap, swipeLeft, swipeRight, swipeUp, swipeDown;
    private Vector2 swipeDelta, startTouch;
    public bool Tap { get { return tap; } }
    public Vector2 SwipeDelta { get { return swipeDelta; }}
    //public Vector2 StartTouch { get { return startTouch; }}
    public bool SwipeLeft { get { return swipeLeft; }}
    public bool SwipeRight { get { return swipeRight; }}
    public bool SwipeUp { get { return swipeUp; }}
    public bool SwipeDown { get { return swipeDown; }}


    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        // start by resetting all the booleans to false
        tap = swipeLeft = swipeRight = swipeUp = swipeDown = false;

        //check for PC inputs
        #region Standalone Inputs
        if( Input.GetMouseButtonDown(0)){
            tap = true;
            startTouch = Input.mousePosition;
        } else if (Input.GetMouseButtonUp(0))
        {
            startTouch = swipeDelta = Vector2.zero;
        }
        #endregion

        //check for Mobile inputs
        #region Mobile Inputs
        if (Input.touches.Length != 0) // if there is a touch on the screen
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                tap = true;
                startTouch = Input.mousePosition;
            }

            else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled) // if the touch stops
            {
                startTouch = swipeDelta = Vector2.zero;
            }
        }
        #endregion

        // Calculate distance
        swipeDelta = Vector2.zero;
        if(startTouch != Vector2.zero)
        {
            // check the mobile
            if(Input.touches.Length != 0)
            {
                swipeDelta = Input.touches[0].position - startTouch; // returns the touch between where the finger currently is and where the touch began
            }else if (Input.GetMouseButton(0))
            {
                swipeDelta = (Vector2)Input.mousePosition - startTouch;
            }
        }

        //check to know if user is beyond deadzone
        if(swipeDelta.magnitude > DEADZONE)
        {
            // confirmed Swipe
            float x = swipeDelta.x;
            float y = swipeDelta.y;

            if(Mathf.Abs(x) > Mathf.Abs(y)) // Mathf.Abs changes negative values to positive
            {
                // left or right on a single frame
                if (x < 0)
                {
                    swipeLeft = true;
                }
                else
                {
                    swipeRight = true;
                }
            } else
            {
                // Up or Down on a single frame
                if (y < 0)
                {
                    swipeDown = true;
                }
                else
                {
                    swipeUp = true;
                }
            }
            // to avoid 2 confirmed swipes 2 frames in a row
            startTouch = swipeDelta = Vector2.zero; // this disables swipe until the user releases his fingers and swipes again
        }
    }
}
