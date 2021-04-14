using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class controller : MonoBehaviour
{
    // a reference to the action
    public SteamVR_Action_Boolean leftGripDown;
    public SteamVR_Action_Boolean rightGripDown;
    // a reference to the hand
    public SteamVR_Input_Sources leftHandController;
    public SteamVR_Input_Sources rightHandController;

    public Transform leftHandPosition;
    public Transform rightHandPosition;
    public Transform playerPosition;
    public Transform cameraPosition;

    private Vector3 beforePosition;
    private Vector3 midpoint;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // rotate
        if (leftGripDown.stateDown && rightGripDown.stateDown)
        {
            midpoint = (leftHandPosition.position + rightHandPosition.position) / 2;
        }
        if (leftGripDown.state && rightGripDown.state)
        {
            Vector3 newMidpoint = (leftHandPosition.position + rightHandPosition.position) / 2;
            double angle = Math.Atan2(midpoint.x, newMidpoint.x);
            // decide which direction to turn
            float oldDistFromLeft = Vector3.Distance(leftHandPosition.position, midpoint);
            float newDistFromLeft = Vector3.Distance(leftHandPosition.position, newMidpoint);
            float oldDistFromRight = Vector3.Distance(rightHandPosition.position, midpoint);
            float newDistFromRight = Vector3.Distance(rightHandPosition.position, newMidpoint);
            // turning right
            if (newDistFromLeft < oldDistFromLeft && newDistFromRight > oldDistFromRight)
            {
                playerPosition.RotateAround(midpoint, Vector3.up, (float)angle * 0.5f);
            }
            // turning left
            else
            {
                playerPosition.RotateAround(midpoint, Vector3.up, -(float)angle * 0.5f);
            }
            midpoint = newMidpoint;
        }

        // grab air
        if (rightGripDown.stateDown && !leftGripDown.stateDown)
        {
            beforePosition = rightHandPosition.position;
        }
        if (rightGripDown.state && !leftGripDown.state)
        {
            Vector3 afterPosition = rightHandPosition.position;
            // compare distance of before and after positions with body position
            afterPosition.y = playerPosition.position.y;
            beforePosition.y = playerPosition.position.y;
            float afterDist = Vector3.Distance(afterPosition, playerPosition.position);
            float beforeDist = Vector3.Distance(beforePosition, playerPosition.position);
            float travDist = Vector3.Distance(rightHandPosition.position, beforePosition);
            float playerY = playerPosition.position.y;

            if (beforeDist > afterDist)
            {
                //playerPosition.Translate(cameraPosition.forward * travDist * 0.1f);
                playerPosition.Translate(cameraPosition.forward * travDist * 0.1f);
            }
            else
            {
                //playerPosition.Translate(cameraPosition.forward * -travDist * 0.1f);
                playerPosition.Translate(cameraPosition.forward * -travDist * 0.1f);
            }
            // anchor player's y position
            playerPosition.position = new Vector3(playerPosition.position.x, playerY, playerPosition.position.z);
            beforePosition = rightHandPosition.position;
        }
    }
}
