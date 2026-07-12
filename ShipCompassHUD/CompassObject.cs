using System.IO;
using System.Reflection;
using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using UnityEngine;

using ShipCompassHUD;
using ShipCompassHUD.button;
using System.Collections.Generic;
using Tessellation;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.Image;
using TMPro;
using System.Drawing;
using UnityEngine.UI;
using GhostEnums;

namespace ShipCompassHUD.Components;



public class CompassObjectBehaviour : ModBehaviour
{
    public OWRigidbody RB;
    public ReferenceFrameGUI RF;
    private Vector3 velocity = Vector3.zero;
    private Vector3 TrgBody;
    private GameObject ReferenceAtTarget;
    private GameObject ReferenceAtTargetHorizon;
    
    private Transform TCam;

    public void Start()
    {
        ReferenceAtTarget = new GameObject();
        ReferenceAtTargetHorizon = new GameObject();
        TCam = GameObject.Find("/Player_Body/PlayerCamera").transform;
        ReferenceAtTarget.transform.parent = TCam;
        ReferenceAtTarget.transform.localPosition = Vector3.zero;
        ReferenceAtTargetHorizon.transform.parent = ReferenceAtTarget.transform ;
        ReferenceAtTargetHorizon.transform.localPosition = Vector3.zero;
    }
    public void Update()
    {
        SetUp();
        TrajectoryBehaviour();
    }
    public void SetUp()
    {
        if (RF._currentReferenceFrame != null)
        {
            TrgBody = RF._currentReferenceFrame._attachedOWRigidbody._lastPosition;
            velocity = RB._currentVelocity - RF._currentReferenceFrame._attachedOWRigidbody._currentVelocity;
        }
        else
        {
            velocity = RB._currentVelocity;
        }
    }
    public void TrajectoryBehaviour ()
    {
        //ReferenceAtTarget.transform.position = RB._lastPosition;
        RotateUpObject(ReferenceAtTarget.transform, velocity, TrgBody);
        this.transform.rotation = ReferenceAtTarget.transform.localRotation; // LookAt(this.transform.position + (ReferenceAtTarget.transform.forward + TCam.forward));
    }

    public void ManeuvreBehaviour ()
    {

    }

    public void HorizonBehaviour ()
    {
        RotateUpObject(ReferenceAtTargetHorizon.transform, TrgBody, ReferenceAtTargetHorizon.transform.position);
        this.transform.GetChild(6).rotation = ReferenceAtTargetHorizon.transform.localRotation; // LookAt(this.transform.position + (ReferenceAtTarget.transform.forward + TCam.forward));
    }

    public void BestTrajectoryBehaviour()
    {

    }


    public void RotateUpObject(Transform ToRotate, Vector3 targetPosition, Vector3 upReference)
    {
        ToRotate.LookAt(targetPosition);

        Vector3 forward = ToRotate.forward;
        Vector3 up = (upReference - ToRotate.position).normalized;

        Quaternion newRotation = Quaternion.LookRotation(forward, -up);

        ToRotate.rotation = newRotation;
    }
}