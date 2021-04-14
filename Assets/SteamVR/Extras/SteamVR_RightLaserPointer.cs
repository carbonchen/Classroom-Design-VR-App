//======= Copyright (c) Valve Corporation, All rights reserved. ===============
using UnityEngine;
using System.Collections;

namespace Valve.VR.Extras
{
    public class SteamVR_RightLaserPointer : MonoBehaviour
    {
        public SteamVR_Behaviour_Pose pose;

        public SteamVR_Action_Boolean rightTrigger;
        public SteamVR_Action_Boolean spawnChair;
        public SteamVR_Action_Boolean spawnDesk;

        public bool active = true;
        public Color color;
        public float thickness = 0.002f;
        public Color clickColor = Color.green;
        public GameObject holder;
        public GameObject pointer;
        bool isActive = false;
        public bool addRigidBody = false;
        public Transform reference;
        public event PointerEventHandler PointerIn;
        public event PointerEventHandler PointerOut;
        public event PointerEventHandler PointerClick;

        public Transform handController;
        public GameObject chair;
        public GameObject desk;
        public float width = 0.0f;

        Transform previousContact = null;

        private GameObject lastSelected;
        private GameObject chairObj;
        private GameObject deskObj;
        private void Start()
        {
            if (pose == null)
                pose = this.GetComponent<SteamVR_Behaviour_Pose>();
            if (pose == null)
                Debug.LogError("No SteamVR_Behaviour_Pose component found on this object", this);

            if (rightTrigger == null)
                Debug.LogError("No ui interaction action has been set on this component.", this);


            holder = new GameObject();
            holder.transform.parent = this.transform;
            holder.transform.localPosition = Vector3.zero;
            holder.transform.localRotation = Quaternion.identity;

            pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pointer.transform.parent = holder.transform;
            pointer.transform.localScale = new Vector3(thickness, thickness, 100f);
            pointer.transform.localPosition = new Vector3(0f, 0f, 50f);
            pointer.transform.localRotation = Quaternion.identity;
            BoxCollider collider = pointer.GetComponent<BoxCollider>();
            if (addRigidBody)
            {
                if (collider)
                {
                    collider.isTrigger = true;
                }
                Rigidbody rigidBody = pointer.AddComponent<Rigidbody>();
                rigidBody.isKinematic = true;
            }
            else
            {
                if (collider)
                {
                    Object.Destroy(collider);
                }
            }
            Material newMaterial = new Material(Shader.Find("Unlit/Color"));
            newMaterial.SetColor("_Color", color);
            pointer.GetComponent<MeshRenderer>().material = newMaterial;
        }

        public virtual void OnPointerIn(PointerEventArgs e)
        {
            if (PointerIn != null)
            {
                PointerIn(this, e);
            }

            // don't interact with the ground, wall, or hand colliders
            if (e.target.name != "Ground" && e.target.name != "Mesh1" && e.target.name != "vr_glove_left_model_slim" && e.target.name != "vr_glove_right_model_slim")
            {
                e.target.gameObject.GetComponent<Outline>().OutlineWidth = 10.0f;
            }
        }

        public virtual void OnPointerClick(PointerEventArgs e)
        {
            if (PointerClick != null)
            {
                PointerClick(this, e);
            }
            // don't interact with the ground, wall, or hand colliders
            if (e.target.name != "Ground" && e.target.name != "Mesh1" && e.target.name != "vr_glove_left_model_slim" && e.target.name != "vr_glove_right_model_slim")
            {
                lastSelected = e.target.gameObject;
                e.target.transform.parent = handController;
                e.target.GetComponent<Rigidbody>().isKinematic = true;
            }
        }

        public virtual void OnPointerOut(PointerEventArgs e)
        {
            if (PointerOut != null)
            {
                PointerOut(this, e);
            }

            // don't interact with the ground, wall, or hand colliders
            if (e.target.name != "Ground" && e.target.name != "Mesh1" && e.target.name != "vr_glove_left_model_slim" && e.target.name != "vr_glove_right_model_slim")
            {
                e.target.gameObject.GetComponent<Outline>().OutlineWidth = 0.0f;
            }
        }


        private void Update()
        {
            if (!isActive)
            {
                isActive = true;
                this.transform.GetChild(0).gameObject.SetActive(true);
            }

            float dist = 100f;

            Ray raycast = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            bool bHit = Physics.Raycast(raycast, out hit);

            // spawning logic
            if (spawnChair.stateDown)
            {
                chairObj = Instantiate(chair, raycast.GetPoint(3.0f), Quaternion.identity);
                chairObj.transform.Rotate(new Vector3(-90, 180, 0));
                chairObj.transform.parent = handController;
                chairObj.GetComponent<Rigidbody>().isKinematic = true;
            }
            if (spawnChair.stateUp && chairObj != null)
            {
                chairObj.transform.parent = null;
                chairObj.GetComponent<Rigidbody>().isKinematic = false;
            }
            if (spawnDesk.stateDown)
            {
                deskObj = Instantiate(desk, raycast.GetPoint(3.0f), Quaternion.identity);
                deskObj.transform.Rotate(new Vector3(-90, 180, 0));
                deskObj.transform.parent = handController;
                deskObj.GetComponent<Rigidbody>().isKinematic = true;
            }
            if (spawnDesk.stateUp && deskObj != null)
            {
                deskObj.transform.parent = null;
                deskObj.GetComponent<Rigidbody>().isKinematic = false;
            }

            if (previousContact && previousContact != hit.transform)
            {
                PointerEventArgs args = new PointerEventArgs();
                args.fromInputSource = pose.inputSource;
                args.distance = 0f;
                args.flags = 0;
                args.target = previousContact;
                OnPointerOut(args);
                previousContact = null;
            }
            if (bHit && previousContact != hit.transform)
            {
                PointerEventArgs argsIn = new PointerEventArgs();
                argsIn.fromInputSource = pose.inputSource;
                argsIn.distance = hit.distance;
                argsIn.flags = 0;
                argsIn.target = hit.transform;
                OnPointerIn(argsIn);
                previousContact = hit.transform;
            }
            if (!bHit)
            {
                previousContact = null;
            }
            if (bHit && hit.distance < 100f)
            {
                dist = hit.distance;
            }

            if (bHit && rightTrigger.GetState(pose.inputSource))
            {
                PointerEventArgs argsClick = new PointerEventArgs();
                argsClick.fromInputSource = pose.inputSource;
                argsClick.distance = hit.distance;
                argsClick.flags = 0;
                argsClick.target = hit.transform;
                OnPointerClick(argsClick);
            }

            if (rightTrigger != null && rightTrigger.GetState(pose.inputSource))
            {
                pointer.transform.localScale = new Vector3(thickness * 5f, thickness * 5f, dist);
                pointer.GetComponent<MeshRenderer>().material.color = clickColor;
            }
            else
            {
                pointer.transform.localScale = new Vector3(thickness, thickness, dist);
                pointer.GetComponent<MeshRenderer>().material.color = color;
            }
            pointer.transform.localPosition = new Vector3(0f, 0f, dist / 2f);

            // released trigger during selection
            if (rightTrigger.stateUp && lastSelected != null)
            {
                lastSelected.transform.parent = null;
                lastSelected.GetComponent<Rigidbody>().isKinematic = false;
            }

        }
    }

    public struct PointerEventArgs
    {
        public SteamVR_Input_Sources fromInputSource;
        public uint flags;
        public float distance;
        public Transform target;
    }

    public delegate void PointerEventHandler(object sender, PointerEventArgs e);
}