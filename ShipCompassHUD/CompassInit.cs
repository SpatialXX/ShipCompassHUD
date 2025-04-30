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



public class CompassInit : ModBehaviour
{
    public bool Initialized = false;
    private GameObject CompassObject;
    private List<Transform> NavBallMarkers = new List<Transform>();
    public List<float> MarkerDistance = new List<float>();
    private GameObject CompassDistance;
    public ReferenceFrameGUI ShipReferenceFrame;
    private GameObject Horizon;
    private GameObject ManeuvreCompass;
    private GameObject Cockpit;
    private Text RotTxt;
    private GameObject MoveScreen;
    private float ShipAutoRotationSpeed;
    //private GameObject MagneticCompass;

    private List<int> WishedRotationX = new List<int>();
    private List<int> WishedRotationY = new List<int>();
    public Vector2 WishedRotation = Vector2.zero;
    public int AtWhichCharacter;
    private bool ReplaceCharacter;
    private float TimeBeforeChangeReplaceCharacter;
    private bool orderMove;
    private int AlignShip;
    private List<SetRotationButton> BtnSAS = new List<SetRotationButton>();
    public void Awake()
    {
        
    }

    public void Start()
    {
    }

    public void Update()
    {
        if (Initialized == false)
        {
            if (GameObject.Find("/Ship_Body/Module_Cockpit") == null)
            {
                return;
            }


            Cockpit = GameObject.Find("/Ship_Body/Module_Cockpit");

            ShipReferenceFrame = GameObject.Find("ShipScreenSpaceUI").transform.gameObject.GetComponent<ReferenceFrameGUI>();


            this.gameObject.transform.parent = Cockpit.transform;
            PlaceCorrectlyLocal(this.gameObject.transform, new Vector3(0, 1.2f, 4.5f), Vector3.zero, new Vector3(0.1f, 0.1f, 0.1f));

            CompassObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            CompassObject.GetComponent<SphereCollider>().enabled = false;
            CompassObject.transform.parent = Cockpit.transform;
            CompassObject.GetComponent<MeshRenderer>().enabled = false;




            //GameObject sphereForward = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //sphereForward.GetComponent<SphereCollider>().enabled = false;
            GameObject sphereForward = Instantiate(ShipCompassHUD.Instance.LoadAsset("Assets/HUDMarkers/Prograde.prefab"), CompassObject.transform);



            //Prograde
            sphereForward.transform.parent = CompassObject.transform;
            PlaceCorrectlyLocal(sphereForward.transform, new Vector3(0, 0, 0.5f), Vector3.zero, new Vector3(0.005f, 0.005f, 0.005f));
            sphereForward.name = "Prograde";

            
            GameObject sphereBackward = Instantiate(ShipCompassHUD.Instance.LoadAsset("Assets/HUDMarkers/Retrograde.prefab"), CompassObject.transform);
            sphereBackward.transform.parent = CompassObject.transform;
            PlaceCorrectlyLocal(sphereBackward.transform, new Vector3(0, 0, -0.5f), Vector3.zero, new Vector3(0.005f, 0.005f, 0.005f));
            sphereBackward.name = "Retrograde";



            //Normal
            GameObject sphereLeft = Instantiate(ShipCompassHUD.Instance.LoadAsset("Assets/HUDMarkers/Normal.prefab"), CompassObject.transform);
            sphereLeft.transform.parent = CompassObject.transform;
            PlaceCorrectlyLocal(sphereLeft.transform, new Vector3(0.5f, 0, 0), new Vector3(0,90,0), new Vector3(0.005f, 0.005f, 0.005f));
            sphereLeft.name = "Normal";


            GameObject AnitNormal = Instantiate(ShipCompassHUD.Instance.LoadAsset("Assets/HUDMarkers/AntiNormal.prefab"), CompassObject.transform);
            AnitNormal.transform.parent = CompassObject.transform;
            PlaceCorrectlyLocal(AnitNormal.transform, new Vector3(-0.5f, 0, 0), new Vector3(0, 90, 0), new Vector3(0.005f, 0.005f, 0.005f));
            AnitNormal.name = "AntiNormal";



            //Radial
            GameObject RadialIn = Instantiate(ShipCompassHUD.Instance.LoadAsset("Assets/HUDMarkers/RadialIn.prefab"), CompassObject.transform);
            RadialIn.transform.parent = CompassObject.transform;
            PlaceCorrectlyLocal(RadialIn.transform, new Vector3(0, -0.5f, 0), new Vector3(90, 0, 0), new Vector3(0.005f, 0.005f, 0.005f));
            RadialIn.name = "RadialIn";

            GameObject RadialOut = Instantiate(ShipCompassHUD.Instance.LoadAsset("Assets/HUDMarkers/RadialOut.prefab"), CompassObject.transform);
            RadialOut.transform.parent = CompassObject.transform;
            PlaceCorrectlyLocal(RadialOut.transform, new Vector3(0, 0.5f, 0), new Vector3(90, 0, 0), new Vector3(0.005f, 0.005f, 0.005f));
            RadialOut.name = "RadialOut";



            ManeuvreCompass = new GameObject("ManeuvreCompass");
            ManeuvreCompass.transform.parent = CompassObject.transform;
            PlaceCorrectlyLocal(ManeuvreCompass.transform, new Vector3(0, 0f, 0), new Vector3(0, 0, 0), new Vector3(1, 1, 1));

            GameObject Maneuvre = Instantiate(ShipCompassHUD.Instance.LoadAsset("Assets/HUDMarkers/Maneuver.prefab"), CompassObject.transform);
            Maneuvre.transform.parent = ManeuvreCompass.transform;
            PlaceCorrectlyLocal(Maneuvre.transform, new Vector3(0, 0, 0.5f), new Vector3(0, 0, 0), new Vector3(0.005f, 0.005f, 0.005f));
            Maneuvre.name = "Maneuvre";

            Horizon = Instantiate(ShipCompassHUD.Instance.LoadAsset("Assets/HUDMarkers/Horizon.prefab"), CompassObject.transform);
            Horizon.transform.parent = CompassObject.transform;
            PlaceCorrectlyLocal(Horizon.transform, new Vector3(0, 0f, 0), new Vector3(0, 0, 0), new Vector3(0.45f, 0.45f, 0.45f));
            Horizon.name = "Horizon";



            PlaceCorrectlyLocal(CompassObject.transform, new Vector3(0,1.2f, 4.5f), Vector3.zero, new Vector3(4.1f, 4.1f, 4.1f));

            ManeuvreCompass.transform.parent = Cockpit.transform;

            CompassDistance = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            CompassDistance.GetComponent<SphereCollider>().enabled = false;
            CompassDistance.transform.parent = Cockpit.transform;
            PlaceCorrectlyLocal(CompassDistance.transform, new Vector3(0, 1.2f, 0f), Vector3.zero, new Vector3(0.1f, 0.1f, 0.1f));
            CompassDistance.GetComponent<MeshRenderer>().enabled = false;


            GameObject SignalScope = GameObject.Find("/Ship_Body/Module_Cockpit/Systems_Cockpit/ShipCockpitUI/SignalScreen/SignalScreenPivot");
            

            GameObject RotationSeter = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            RotationSeter.GetComponent<SphereCollider>().enabled = false;
            RotationSeter.transform.parent = Cockpit.transform;
            RotationSeter.GetComponent<MeshRenderer>().enabled = false;

            GameObject Button = Instantiate(ShipCompassHUD.Instance.LoadAsset("Assets/HUDMarkers/RotationConsoleButton.prefab"), RotationSeter.transform);
            PlaceCorrectlyLocal(Button.transform, new Vector3(0, 0f, 0), new Vector3(0, 0, 0), new Vector3(0.1f, 0.1f, 0.1f));
            Button.name = "RotationKeyboard";

            //CapsuleCollider compR = Button.AddComponent<CapsuleCollider>();
            //compR.radius = 1.5f;
            //compR.height = 6;
            //compR.isTrigger = true;
            //compR.center = new Vector3(0, 6, 0);
            //InteractReceiver comp = Button.AddComponent<InteractReceiver>();

            BtnSAS = new List<SetRotationButton>();

            SetRotationButton RotBut = Button.transform.GetChild(1).gameObject.GetAddComponent<SetRotationButton>();
            RotBut.SetAction("Align to Prograde", 5, this);
            BtnSAS.Add(RotBut);
            
            RotBut = Button.transform.GetChild(2).gameObject.GetAddComponent<SetRotationButton>();
            RotBut.SetAction("Align to Target", 6, this);
            BtnSAS.Add(RotBut);

            RotBut = Button.transform.GetChild(3).gameObject.GetAddComponent<SetRotationButton>();
            RotBut.SetAction("Left", 0, this);
            RotBut = Button.transform.GetChild(4).gameObject.GetAddComponent<SetRotationButton>();
            RotBut.SetAction("Up", 1, this);
            RotBut = Button.transform.GetChild(5).gameObject.GetAddComponent<SetRotationButton>();
            RotBut.SetAction("Down", 2, this);
            RotBut = Button.transform.GetChild(6).gameObject.GetAddComponent<SetRotationButton>();
            RotBut.SetAction("Right", 3, this);

            RotBut = Button.transform.GetChild(7).gameObject.GetAddComponent<SetRotationButton>();
            RotBut.SetAction("Set current rotation as Target", 7, this);

            RotBut = Button.transform.GetChild(8).gameObject.GetAddComponent<SetRotationButton>();
            RotBut.SetAction("Reset Target rotation", 8, this);

            RotBut = Button.transform.GetChild(9).gameObject.GetAddComponent<SetRotationButton>();
            RotBut.SetAction("Hide Markers", 9, this);

            RotTxt = Button.transform.GetChild(10).GetChild(1).GetChild(0).gameObject.GetComponent<Text>();
            Text txtRef = GameObject.Find("/Ship_Body/Module_Cockpit/Systems_Cockpit/ShipCockpitUI/CockpitCanvases/ShipWorldSpaceUI/ConsoleDisplay/Mask/TestText").GetComponent<Text>();
            RotTxt.font = txtRef.font;

            MoveScreen = Button.transform.GetChild(10).gameObject;


            //MagneticCompass = Instantiate(ShipCompassHUD.Instance.LoadAsset("Assets/HUDMarkers/Compass.prefab"), RotationSeter.transform);
            //PlaceCorrectlyLocal(MagneticCompass.transform, new Vector3(0.92f, 0.05f, 0.2f), new Vector3(0, 35, 0), new Vector3(0.1f, 0.1f, 0.1f));
            //MagneticCompass.name = "MagneticCompass";


            PlaceCorrectlyLocal(RotationSeter.transform, new Vector3(0, 0.55f, 6.2f), Vector3.zero, new Vector3(1f, 1f, 1f));
            RotationSeter.GetComponent<MeshRenderer>().enabled = false;
            RotationSeter.name = "SetRotationConsole";


            

            Initialized = true;
            ShipCompassHUD.Instance.LogForStupids("Compass Intialized " + Initialized);


            NavBallMarkers = new List<Transform>();
            NavBallMarkers.Add(sphereForward.transform);
            NavBallMarkers.Add(sphereBackward.transform);
            NavBallMarkers.Add(sphereLeft.transform);
            NavBallMarkers.Add(AnitNormal.transform);
            NavBallMarkers.Add(RadialIn.transform);
            NavBallMarkers.Add(RadialOut.transform);
            NavBallMarkers.Add(Maneuvre.transform);

            MarkerDistance.Add(0);
            MarkerDistance.Add(0);
            MarkerDistance.Add(0);
            MarkerDistance.Add(0);
            MarkerDistance.Add(0);
            MarkerDistance.Add(0);
            MarkerDistance.Add(0);

            WishedRotationX.Add(0);
            WishedRotationX.Add(0);
            WishedRotationX.Add(0);
            WishedRotationX.Add(0);
            WishedRotationX.Add(0);

            WishedRotationY.Add(0);
            WishedRotationY.Add(0);
            WishedRotationY.Add(0);
            WishedRotationY.Add(0);
            WishedRotationY.Add(0);

            ShipCompassHUD.Instance.ConfigureAlarmFirstStep();
        }

        else
        {

            if (ShipReferenceFrame._currentReferenceFrame != null)
            {

                Transform myObject = CompassObject.transform;
                Vector3 targetPos = ShipReferenceFrame._relativeVelocity;
                Vector3 upTarget = ShipReferenceFrame._currentReferenceFrame._attachedOWRigidbody._lastPosition;

                RotateUpObject(myObject, targetPos, upTarget);
                //Horizon.transform.LookAt(upTarget);
                //Horizon.transform.localEulerAngles = new Vector3(Horizon.transform.localEulerAngles.x, Horizon.transform.localEulerAngles.y, 0);
                RotateUpObject(Horizon.transform, upTarget, Cockpit.transform.position);

                //RotateUpObject(MagneticCompass.transform.GetChild(1), upTarget, upTarget);
                //AlignCompassToPlanet(MagneticCompass.transform.GetChild(1), ShipReferenceFrame._currentReferenceFrame._attachedOWRigidbody.transform, Cockpit.transform);
            }


            int i = 0;
            foreach (Transform t in NavBallMarkers)
            {
                MarkerDistance[i] = Vector3.Distance(t.position, CompassDistance.transform.position);
                if (MarkerDistance[i] > 4.8f)
                {
                    NavBallMarkers[i].gameObject.SetActive(true);
                }
                else
                {
                    NavBallMarkers[i].gameObject.SetActive(false);
                }
                i += 1;
            }

            RotTxt.text = rotationText();

            TimeBeforeChangeReplaceCharacter += Time.deltaTime;
            if (TimeBeforeChangeReplaceCharacter > 0.7f)
            {
                TimeBeforeChangeReplaceCharacter = 0;
                ReplaceCharacter = !ReplaceCharacter;
                if(ReplaceCharacter)
                {
                    TimeBeforeChangeReplaceCharacter = 0.2f;
                }
            }

            ManeuvreCompass.transform.eulerAngles = new Vector3(WishedRotation.x, WishedRotation.y, 0);

            if (orderMove)
            {
                if ((CompassObject.activeSelf && MoveScreen.transform.localPosition.y >= 1) || (!CompassObject.activeSelf && MoveScreen.transform.localPosition.y <= 0))
                {
                    orderMove = false;
                }
                else
                {
                    Vector3 ScreenMoveSpeed = new Vector3(0, -0.05f, 0);
                    if (CompassObject.activeSelf)
                    {
                        ScreenMoveSpeed = new Vector3(0, 0.05f, 0);
                    }
                    MoveScreen.transform.localPosition = MoveScreen.transform.localPosition + ScreenMoveSpeed;
                }
            }

            if (AlignShip == 1)
            {
                Transform ShipTransform = Cockpit.transform.parent.transform;
                float newX = RotateTowardSlowly(ShipAutoRotationSpeed, ShipTransform.eulerAngles.x, ShipTransform, CompassObject.transform.eulerAngles.x);
                float newY = RotateTowardSlowly(ShipAutoRotationSpeed, ShipTransform.eulerAngles.y, ShipTransform, CompassObject.transform.eulerAngles.y);

                ShipTransform.eulerAngles = new Vector3(newX, newY, ShipTransform.eulerAngles.z);
            }
            else if (AlignShip == 2)
            {
                Vector2 realSentRot = WishedRotation;
                if (realSentRot.x > 90 && realSentRot.x < 270)
                {
                    if (realSentRot.x <= 180)
                    {
                        realSentRot.x = 90 - (realSentRot.x - 90);
                    }
                    else
                    {
                        realSentRot.x = 360 - (realSentRot.x - 180);
                    }
                    realSentRot.y = 360 - (180 - realSentRot.y);
                    if (realSentRot.y > 360)
                    {
                        realSentRot.y = realSentRot.y - 360;
                    }
                }
                Transform ShipTransform = Cockpit.transform.parent.transform;
                float newX = RotateTowardSlowly(10f, ShipTransform.eulerAngles.x, ShipTransform, realSentRot.x);
                float newY = RotateTowardSlowly(10f, ShipTransform.eulerAngles.y, ShipTransform, realSentRot.y);

                ShipTransform.eulerAngles = new Vector3(newX, newY, ShipTransform.eulerAngles.z);
            }

        }
    }



    public void PlaceCorrectlyLocal(Transform ToChange, Vector3 pos, Vector3 rot, Vector3 scal)
    {
        ToChange.localPosition = pos;
        ToChange.localEulerAngles = rot;
        ToChange.localScale = scal;
    }


    public void RotateUpObject(Transform obj, Vector3 targetPosition, Vector3 upReference)
    {
        // Step 1: Rotate to face the target position
        obj.LookAt(targetPosition);

        // Step 2: Align Up Vector toward the second reference position
        Vector3 forward = obj.forward; // Current forward vector (already looking at target)
        Vector3 upDirection = (upReference - obj.position).normalized; // Direction to align up

        // Create a new rotation that maintains forward but adjusts up direction
        Quaternion newRotation = Quaternion.LookRotation(forward, -upDirection);

        // Apply the final rotation
        obj.rotation = newRotation;
    }

    void AlignCompassToPlanet(Transform compass, Transform planet, Transform Player)
    {
        Vector3 localNorthPole = planet.localPosition + (new Vector3(500 * planet.up.x, 500 * planet.up.y, 500 * planet.up.z));
        Vector3 relativePos = localNorthPole - compass.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos, Player.up);
        compass.rotation = rotation;

        compass.localEulerAngles = new Vector3(0, 0, compass.localEulerAngles.y);
    }

    private string rotationText()
    {
        float x = WishedRotation.x;
        float y = WishedRotation.y;

        string Replace = "_";
        int strgToReplace = AtWhichCharacter;
        if (strgToReplace > 2 && strgToReplace <= 4)
        {
            strgToReplace += 1;
        }

        string xstring = WishedRotationX[0] + "" + WishedRotationX[1] + "" + WishedRotationX[2] + "." + WishedRotationX[3] + "" + WishedRotationX[4];

        if (AtWhichCharacter <= 4 && ReplaceCharacter)
        {
            xstring = xstring.Remove(strgToReplace, 1).Insert(strgToReplace, Replace);
        }
        

        if ((WishedRotation.x <= 360) == false)
        {
            xstring = "<color=red>" + xstring + "</color>";
        }


        strgToReplace = AtWhichCharacter;
        if (strgToReplace > 4)
        {
            strgToReplace = strgToReplace - 5;

            if (strgToReplace > 2)
            {
                strgToReplace += 1;
            }
        }
        else
        {
            strgToReplace = -1;
        }
        
        


        string ystring = WishedRotationY[0] + "" + WishedRotationY[1] + "" + WishedRotationY[2] + "." + WishedRotationY[3] + "" + WishedRotationY[4];


        if (strgToReplace >= 0 && ReplaceCharacter)
        {
            ystring = ystring.Remove(strgToReplace, 1).Insert(strgToReplace, Replace);
        }


        if ((WishedRotation.y <= 360) == false)
        {
            ystring = "<color=red>" + ystring + "</color>";
        }

        string returString = xstring + " X      " + ystring + " Y";

        return returString;
    }

    public void IncreaseWishedRot(int Nmb)
    {
        int AtCharacter = AtWhichCharacter;
        List<int> WishToChange = new List<int>();
        if (AtWhichCharacter > 4)
        {
            AtCharacter = AtCharacter - 5;
            WishToChange = WishedRotationY;
        }

        else
        {
            WishToChange = WishedRotationX;
        }


        IncreaseSpecificInt(WishToChange, AtCharacter, Nmb);


        if (AtWhichCharacter > 4)
        {
            WishedRotationY = WishToChange;
            WishedRotation.y = (WishedRotationY[0] * 100) + (WishedRotationY[1] * 10) + (WishedRotationY[2]) + (WishedRotationY[3] * 0.1f) + (WishedRotationY[0] * 0.01f);
        }
        else
        {
            WishedRotationX = WishToChange;
            WishedRotation.x = (WishedRotationX[0] * 100) + (WishedRotationX[1] * 10) + (WishedRotationX[2]) + (WishedRotationX[3] * 0.1f) + (WishedRotationX[0] * 0.01f);
        }



        TimeBeforeChangeReplaceCharacter = 0;
        ReplaceCharacter = false;
    }

    private List<int> IncreaseSpecificInt(List<int> WishToChange, int AtCharacter, int Change)
    {
        WishToChange[AtCharacter] += Change;

        if (WishToChange[AtCharacter] < 0)
        {
            WishToChange[AtCharacter] = 9;
            if (AtCharacter != 0)
            {
                WishToChange = IncreaseSpecificInt(WishToChange, AtCharacter - 1, -1);
            }
        }

        if (AtCharacter == 0)
        {
            if (WishToChange[AtCharacter] >= 4)
            {
                WishToChange[AtCharacter] = 0;
            }
        }
        else
        {
            if (WishToChange[AtCharacter] > 9)
            {
                WishToChange[AtCharacter] = 0;
                WishToChange = IncreaseSpecificInt(WishToChange, AtCharacter - 1, +1);
            }
        }

        return WishToChange;
    }

    public void ChangeWhichCharacter(int change)
    {
        AtWhichCharacter += change;
        if (AtWhichCharacter == -1)
        {
            AtWhichCharacter = 9;
        }
        if (AtWhichCharacter == 10)
        {
            AtWhichCharacter = 0;
        }
        TimeBeforeChangeReplaceCharacter = 0.2f;
        ReplaceCharacter = true;
    }

    public void SystemOnOff(bool PowerUp)
    {
        orderMove = true;
        CompassObject.SetActive(PowerUp);
        ManeuvreCompass.SetActive(PowerUp);
        //MagneticCompass.SetActive(PowerUp);
    }

    public void ResetTarget()
    {
        if (AtWhichCharacter > 4)
        {
            WishedRotationY[0] = 0;
            WishedRotationY[1] = 0;
            WishedRotationY[2] = 0;
            WishedRotationY[3] = 0;
            WishedRotationY[4] = 0;
            WishedRotation.y = 0;
        }
        else
        {
            WishedRotationX[0] = 0;
            WishedRotationX[1] = 0;
            WishedRotationX[2] = 0;
            WishedRotationX[3] = 0;
            WishedRotationX[4] = 0;
            WishedRotation.x = 0;
        }
    }

    public void AlignTargetForward()
    {
        
        Vector3 forward = Cockpit.transform.forward;

        // Step 2: Construct a rotation that uses the same forward,
        // but forces the up vector to be "world up" (e.g., Vector3.up)
        Quaternion targetRotation = Quaternion.LookRotation(forward, Vector3.up);

        // Step 3: Apply it to the target
        WishedRotation = new Vector2(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y);
        ManeuvreCompass.transform.eulerAngles = new Vector3(WishedRotation.x, WishedRotation.y, 0);

        List<float> fltInNmb = new List<float>();
        fltInNmb.Add(100);
        fltInNmb.Add(10);
        fltInNmb.Add(1);
        fltInNmb.Add(0.1f);
        fltInNmb.Add(0.01f);
        float Comparaison = WishedRotation.x;
        int i = 0;
        int j = 0;
        foreach (float flt in fltInNmb)
        {
            Nested(true, i, Comparaison, flt, out Comparaison, out i);
            WishedRotationX[j] = i;
            i = 0;
            j = j + 1;
        }

        Comparaison = WishedRotation.y;
        i = 0;
        j = 0;
        foreach (float flt in fltInNmb)
        {
            Nested(true, i, Comparaison, flt, out Comparaison, out i);
            WishedRotationY[j] = i;
            i = 0;
            j = j + 1;
        }
    }

    private void Nested(bool MasterReturn,  int Iteration, float Comparaison, float GreaterThan, out float ReturnComparaison, out int ReturnIteration)
    {

        if (Comparaison > GreaterThan)
        {
            Iteration += 1;
            Comparaison = Comparaison - GreaterThan;
            Nested(false, Iteration, Comparaison, GreaterThan, out Comparaison, out Iteration);
        }

        ReturnIteration = Iteration;
        ReturnComparaison = Comparaison;
    }

    public void AlignToTarget(int ToAlign)
    {
        AlignShip = ToAlign;
        if (ToAlign == 1)
        {
            BtnSAS[1].ReturnToStatus(false);
        }
        else if (ToAlign == 2)
        {
            BtnSAS[0].ReturnToStatus(false);
        }

    }

    private float RotateTowardSlowly(float turnspeed, float AxisChange, Transform ShipTransform, float TargetAxis)
    {
        float yawChange = TargetAxis - AxisChange;
        float yawDiffAbs = Mathf.Abs(yawChange);
        float yawDiff = yawChange;



        if (yawDiffAbs > 0)
        {
            if (yawDiffAbs > turnspeed * Time.deltaTime)
            {
                yawChange = turnspeed * Time.deltaTime;

                if(yawDiff < 0)
                {
                    yawChange = - yawChange;
                }
            }
        }

        if (yawDiffAbs > 180)
        {
            yawChange = 0 - yawChange;
        }

        var ideal = AxisChange + yawChange;

        return ideal;
    }


    public void ConfigureInTwoStep(float MarkersSize, string HorizonKind, float RotationSensibility)
    {
        if (Initialized)
        {

            if (HorizonKind == "Separate Lines")
            {
                Horizon.SetActive(true);
                Horizon.transform.GetChild(0).gameObject.SetActive(true);
                Horizon.transform.GetChild(1).gameObject.SetActive(true);
                Horizon.transform.GetChild(2).gameObject.SetActive(false);
            }
            else if (HorizonKind == "Complete Line")
            {
                Horizon.SetActive(true);
                Horizon.transform.GetChild(0).gameObject.SetActive(false);
                Horizon.transform.GetChild(1).gameObject.SetActive(false);
                Horizon.transform.GetChild(2).gameObject.SetActive(true);
            }
            else
            {
                Horizon.SetActive(false);
            }

            ShipAutoRotationSpeed = 10 + (RotationSensibility * 10);

            LazyMarkersResize(MarkersSize,CompassObject.transform.GetChild(0));
            LazyMarkersResize(MarkersSize, CompassObject.transform.GetChild(1));
            LazyMarkersResize(MarkersSize, CompassObject.transform.GetChild(2));
            LazyMarkersResize(MarkersSize, CompassObject.transform.GetChild(3));
            LazyMarkersResize(MarkersSize, CompassObject.transform.GetChild(4));
            LazyMarkersResize(MarkersSize, CompassObject.transform.GetChild(5));
            LazyMarkersResize(MarkersSize, ManeuvreCompass.transform.GetChild(0));

        }
    }

    private void LazyMarkersResize(float newSize, Transform Child)
    {
        Child.localScale = new Vector3((newSize * 0.005f) + 0.0025f, (newSize * 0.005f) + 0.0025f, (newSize * 0.005f) + 0.0025f);
    }


}
