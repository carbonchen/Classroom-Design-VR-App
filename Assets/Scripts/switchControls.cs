using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.Extras;

public class switchControls : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject leftLaser;
    public GameObject leftVirtual;
    public GameObject rightLaser;
    public GameObject rightVirtual;
    public SteamVR_Action_Boolean switchButton;

    public GameObject leftControllerModel;
    public GameObject rightControllerModel;

    public bool usingLasers = true;

    // Update is called once per frame
    void Update()
    {
        if (switchButton.stateDown)
        {
            if (usingLasers)
            {
                // switch scripts
                leftLaser.GetComponent<SteamVR_LeftLaserPointer>().enabled = false;
                rightLaser.GetComponent<SteamVR_RightLaserPointer>().enabled = false;
                leftVirtual.GetComponent<leftVirtualHand>().enabled = true;
                rightVirtual.GetComponent<rightVirtualHand>().enabled = true;
                
                // stop rendering controller model and raycast line
                leftControllerModel.SetActive(false);
                rightControllerModel.SetActive(false);
                GameObject.Find("/[CameraRig]/Controller (left)/New Game Object").SetActive(false);
                GameObject.Find("/[CameraRig]/Controller (right)/New Game Object").SetActive(false);

                usingLasers = false;
            }
            else
            {
                // switch scripts
                leftLaser.GetComponent<SteamVR_LeftLaserPointer>().enabled = true;
                rightLaser.GetComponent<SteamVR_RightLaserPointer>().enabled = true;
                leftVirtual.GetComponent<leftVirtualHand>().enabled = false;
                rightVirtual.GetComponent<rightVirtualHand>().enabled = false;

                // start rendering controller model and raycast line
                leftControllerModel.SetActive(true);
                rightControllerModel.SetActive(true);
                GameObject.Find("/[CameraRig]/Controller (left)/New Game Object").SetActive(true);
                GameObject.Find("/[CameraRig]/Controller (right)/New Game Object").SetActive(true);

                usingLasers = true;
            }
        }
    }
}
