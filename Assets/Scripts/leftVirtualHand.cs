﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class leftVirtualHand : MonoBehaviour
{

    public SteamVR_Action_Boolean leftTrigger;
    public Transform handController;
    void Start()
    {

    }
    void OnTriggerEnter(Collider other)
    {
        // only interact with desk and chair
        if (other.gameObject.name != "Ground" && other.gameObject.name != "Mesh1" && other.gameObject.name != "vr_glove_left_model_slim" && other.gameObject.name != "vr_glove_right_model_slim")
        {
            other.gameObject.GetComponent<Outline>().OutlineWidth = 10.0f;
        }
    }
    void OnTriggerExit(Collider other)
    {
        // only interact with desk and chair
        if (other.gameObject.name != "Ground" && other.gameObject.name != "Mesh1" && other.gameObject.name != "vr_glove_left_model_slim" && other.gameObject.name != "vr_glove_right_model_slim")
        {
            other.gameObject.GetComponent<Outline>().OutlineWidth = 0.0f;
        }
    }
    void OnTriggerStay(Collider other)
    {
        // only interact with desk and chair
        if (other.gameObject.name != "Ground" && other.gameObject.name != "Mesh1" && other.gameObject.name != "vr_glove_left_model_slim" && other.gameObject.name != "vr_glove_right_model_slim")
        {
            if (leftTrigger.state)
            {
                other.gameObject.transform.parent = handController;
                other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            }
            if (leftTrigger.stateUp)
            {
                other.gameObject.transform.parent = null;
                other.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            }
        }

    }
}
