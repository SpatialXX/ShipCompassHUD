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
    public bool CatastrophicInit = false;
    private GameObject CompassObject;
    private bool CompassObjectGameRule;
    private List<Transform> NavBallMarkers = new List<Transform>();
    public List<float> MarkerDistance = new List<float>();
    private GameObject CompassDistance;
    public ReferenceFrameGUI ShipReferenceFrame;
    private GameObject Horizon;
    private bool HorizonGameRule;
    private GameObject ManeuvreCompass;
    private GameObject Cockpit;
    private Text RotTxt;
    private GameObject MoveScreen;
    private float ShipAutoRotationSpeed;


    private GameObject MagneticCompass;
    private bool MagneticCompassGameRule;
    private Text MagneticCompassTxt;
    private Transform VirtualNorth;
    private Transform VirtualNeedle;
    private Transform ShipNeedle;

    private List<int> WishedRotationX = new List<int>();
    private List<int> WishedRotationY = new List<int>();
    public Vector2 WishedRotation = Vector2.zero;
    public int AtWhichCharacter;
    private bool ReplaceCharacter;
    private float TimeBeforeChangeReplaceCharacter;
    private bool orderMove;
    private int AlignShip;
    private List<SetRotationButton> BtnSAS = new List<SetRotationButton>();
    private GameObject Button;

    public Text HSpeedTxt;
    private GameObject FakeShip;
    private List<float> VelocityMemory = new List<float>();
    private float longerTimer;
    private GameObject ShipBody;
    private int AccUnits;
    private List<bool> HUDEnabled = new List<bool>();

    private GameObject HUDAltimeter;
    private Transform HUDAltimeterShip;
    private Transform HUDAltimeterGround;
    private Transform HUDVanillaAltimeterShip;
    private Transform HUDVanillaAltimeterGround;
    private bool AltimeterGameRule;

    private ShipComponent[] ShipDamaged;
    private float TimeBetweenRdm;
    private List<Vector3> RdmVect = new List<Vector3>();
    private bool DisableOnDamageGameRule;


    public void Awake()
    {
        
    }

    public void Start()
    {
    }

    public void Update()
    {
        if (Initialized == false && CatastrophicInit == false)
        {
            if (GameObject.Find("/Ship_Body/Module_Cockpit") == null)
            {
                return;
            }


            Cockpit = GameObject.Find("/Ship_Body/Module_Cockpit");
            CatastrophicInit = true;

            //MOAR COCKPITS

            ShipBody = GameObject.Find("/Ship_Body");
            Transform ShipB = ShipBody.transform;
            
            //GameObject CockpitNew = Instantiate(Cockpit, ShipB);
            //PlaceCorrectlyLocal(CockpitNew.transform, new Vector3(0, 8, 0), Vector3.zero, new Vector3(1, 1, 1));
            //CockpitNew = Instantiate(Cockpit, ShipB);
            //PlaceCorrectlyLocal(CockpitNew.transform, new Vector3(0, 4, 0), Vector3.zero, new Vector3(1, 1, 1));
            GameObject CabinB = GameObject.Find("/Ship_Body/Module_Cabin");
            GameObject SupplyB = GameObject.Find("/Ship_Body/Module_Supplies");
            GameObject EngineB = GameObject.Find("/Ship_Body/Module_Engine");
            GameObject LandGB = GameObject.Find("/Ship_Body/Module_LandingGear");
            GameObject CameraB = GameObject.Find("/Ship_Body/Module_Cockpit/Systems_Cockpit/LandingCamera");


            //CockpitNew = Instantiate(SupplyB, ShipB);
            //PlaceCorrectlyLocal(CockpitNew.transform, new Vector3(0, 8, 0), Vector3.zero, new Vector3(1, 1, 1));

            //FELDSPAR SHIP
            //EngineB.SetActive(false);
            //PlaceCorrectlyLocal(CabinB.transform, new Vector3(0, 0, 0), new Vector3(0, 90, 0), new Vector3(1, 1, 1));
            //PlaceCorrectlyLocal(SupplyB.transform, new Vector3(0, 0, 0), new Vector3(0, 90, 0), new Vector3(1, 1, 1));

            //CameraB.transform.parent = GameObject.Find("/Ship_Body/Module_LandingGear/LandingGear_Front/Systems_LandingGear_Front/LandingCameraComponent").transform;
            //PlaceCorrectlyLocal(CameraB.transform, new Vector3(-0.75f, 3, 0.75f), new Vector3(90, 0, 0), new Vector3(1, 1, 1));
            //PlaceCorrectlyLocal(LandGB.transform, new Vector3(0, 2, -4.5f), new Vector3(90, 0, 0), new Vector3(1, 1, 1));
            //PlaceCorrectlyLocal(Cockpit.transform, new Vector3(0, 0, -0.35f), new Vector3(0, 0, 0), new Vector3(1, 1, 1));

            //FELDSPAR SHIP

            ShipReferenceFrame = GameObject.Find("ShipScreenSpaceUI").transform.GetChild(0).gameObject.GetComponent<ReferenceFrameGUI>();


            this.gameObject.transform.parent = Cockpit.transform;
            PlaceCorrectlyLocal(this.gameObject.transform, new Vector3(0, 1.2f, 4.5f), Vector3.zero, new Vector3(0.1f, 0.1f, 0.1f));

            
            CompassObject = new GameObject();
            CompassObject.name = "3D_HUD_Trajectory";
            CompassObject.transform.parent = Cockpit.transform;


            //GameObject BetterCallSmoke = GameObject.Find("VolcanicMoon_Body/Sector_VM/Effects_VM/VolcanoPivot/MeteorLauncher");
            //GameObject smokinBabe = BetterCallSmoke.GetComponent<MeteorLauncher>()._meteorPool[0].gameObject.transform.GetChild(4).gameObject;
            //GameObject newSmoke = Instantiate(smokinBabe, Cockpit.transform);
            //PlaceCorrectlyLocal(newSmoke.transform, new Vector3(0, 0f, 0), new Vector3(0, 180, 0), new Vector3(1, 1, 1));
            //newSmoke.name = "ShipSmoke";
            //newSmoke.GetComponent<ParticleSystem>().Play();


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

            CompassDistance = new GameObject();
            CompassDistance.transform.parent = Cockpit.transform;
            PlaceCorrectlyLocal(CompassDistance.transform, new Vector3(0, 1.2f, 0f), Vector3.zero, new Vector3(0.1f, 0.1f, 0.1f));


            GameObject SignalScope = GameObject.Find("/Ship_Body/Module_Cockpit/Systems_Cockpit/ShipCockpitUI/SignalScreen/SignalScreenPivot");
            

            GameObject RotationSeter = new GameObject();
            RotationSeter.transform.parent = Cockpit.transform;

            Button = Instantiate(ShipCompassHUD.Instance.LoadAsset("Assets/HUDMarkers/RotationConsoleButton.prefab"), RotationSeter.transform);
            PlaceCorrectlyLocal(Button.transform, new Vector3(0, 0f, 0), new Vector3(0, 0, 0), new Vector3(0.1f, 0.1f, 0.1f));
            Button.name = "RotationKeyboard";


            //newSmoke = Instantiate(ShipCompassHUD.Instance.LoadAsset("Assets/HUDMarkers/SmokeEffect.prefab"), Cockpit.transform);
            //PlaceCorrectlyLocal(newSmoke.transform, new Vector3(0, 0f, 0), new Vector3(0, 180, 0), new Vector3(1, 1, 1));
            //newSmoke.name = "ShimSmoke";

            

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




            MagneticCompass = Instantiate(ShipCompassHUD.Instance.LoadAsset("Assets/HUDMarkers/Compass.prefab"), RotationSeter.transform);
            PlaceCorrectlyLocal(MagneticCompass.transform, new Vector3(0.92f, 0.05f, 0.2f), new Vector3(0, 35, 0), new Vector3(0.1f, 0.1f, 0.1f));
            MagneticCompass.name = "MagneticCompass";
            ShipNeedle = MagneticCompass.transform.GetChild(1);

            GameObject VirtualCompass = new GameObject();
            VirtualCompass.name = "VirtualNorth";
            VirtualNorth = VirtualCompass.transform;
            VirtualCompass = new GameObject();
            VirtualCompass.name = "VirtualNeedle";
            VirtualNeedle = VirtualCompass.transform;
            VirtualNeedle.parent = VirtualNorth;
            VirtualNeedle.localPosition = Vector3.zero;

            Transform CompassTxt = MagneticCompass.transform.GetChild(0).GetChild(0);
            CompassTxt.GetChild(0).GetComponent<Text>().font = txtRef.font;
            MagneticCompassTxt = CompassTxt.GetChild(0).GetChild(0).GetComponent<Text>();
            MagneticCompassTxt.font = txtRef.font;
            CompassTxt.GetChild(1).GetComponent<Text>().font = txtRef.font;
            CompassTxt.GetChild(2).GetComponent<Text>().font = txtRef.font;
            CompassTxt.GetChild(3).GetComponent<Text>().font = txtRef.font;
            




            PlaceCorrectlyLocal(RotationSeter.transform, new Vector3(0, 0.55f, 6.2f), Vector3.zero, new Vector3(1f, 1f, 1f));
            RotationSeter.name = "SetRotationConsole";




            GameObject HorizontalSpeed = Instantiate(ShipCompassHUD.Instance.LoadAsset("Assets/HUDMarkers/HorizontalSpeed.prefab"), RotationSeter.transform);
            PlaceCorrectlyLocal(HorizontalSpeed.transform, new Vector3(0.5f, 0.8f, 0), new Vector3(0, 0, 0), new Vector3(0.08f, 0.08f, 0.08f));
            HSpeedTxt = HorizontalSpeed.transform.GetChild(0).gameObject.GetComponent<Text>();
            HSpeedTxt.font = txtRef.font;

            FakeShip = new GameObject();
            FakeShip.name = "FakeShipCalculs";

            VelocityMemory.Add(0);
            VelocityMemory.Add(0);



            GameObject AltimeterMesh = GameObject.Find("/Ship_Body/Module_Cockpit/Geo_Cockpit/Cockpit_Tech/Cockpit_Tech_Interior/AltimeterShipPivot/AltimeterShip");
            GameObject AltimeterGroundMesh = GameObject.Find("/Ship_Body/Module_Cockpit/Geo_Cockpit/Cockpit_Tech/Cockpit_Tech_Interior/AltimeterFluidPivot/AltimeterFluid_Geo");

            HUDAltimeter = Instantiate(ShipCompassHUD.Instance.LoadAsset("Assets/HUDMarkers/Altimeter.prefab"), RotationSeter.transform);
            PlaceCorrectlyLocal(HUDAltimeter.transform, new Vector3(0.95f, 0.2f, 0), new Vector3(0, 90, 270), new Vector3(2f, 2f, 2f));

            HUDAltimeterShip = HUDAltimeter.transform.GetChild(0).GetChild(0);
            HUDAltimeterGround = HUDAltimeter.transform.GetChild(1).GetChild(0);
            HUDVanillaAltimeterShip = AltimeterMesh.transform;
            HUDVanillaAltimeterGround = AltimeterGroundMesh.transform;


            HUDAltimeterShip.GetComponent<MeshFilter>().mesh = AltimeterMesh.GetComponent<MeshFilter>().mesh;
            HUDAltimeterGround.GetComponent<MeshFilter>().mesh = AltimeterGroundMesh.GetComponent<MeshFilter>().mesh;
            HUDAltimeter.transform.GetChild(2).GetComponent<MeshFilter>().mesh = AltimeterGroundMesh.GetComponent<MeshFilter>().mesh;



            ShipDamaged = GameObject.Find("/Ship_Body/Module_Cockpit/Systems_Cockpit/ShipCockpitUI/DamageScreen/HUD_ShipDamageDisplay").GetComponent<ShipDamageDisplayV2>()._shipComponents;




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

            RdmVect.Add(Vector3.zero);
            RdmVect.Add(Vector3.zero);
            RdmVect.Add(Vector3.zero);

            ShipCompassHUD.Instance.ConfigureAlarmFirstStep();

            CatastrophicInit = false;
        }

        else if (Initialized == true && CatastrophicInit == false)
        {

            if (ShipDamaged[3]._damaged && DisableOnDamageGameRule)
            {
                CompassObject.SetActive(false);
                MagneticCompass.SetActive(false);
                HSpeedTxt.gameObject.SetActive(false);
                HUDAltimeter.SetActive(false);
                return;
            }
            else
            {
                CompassObject.SetActive(CompassObjectGameRule);
                MagneticCompass.SetActive(MagneticCompassGameRule);
                HSpeedTxt.gameObject.SetActive(true);
                HUDAltimeter.SetActive(AltimeterGameRule);
            }

            Transform myObject = CompassObject.transform;
            Vector3 relativeV = ShipBody.GetComponent<ShipBody>()._currentVelocity;
            Vector3 upTarget = Vector3.zero;

            if (ShipDamaged[0]._damaged && DisableOnDamageGameRule)
            {
                TimeBetweenRdm += Time.deltaTime;
                if (TimeBetweenRdm > 1)
                {
                    TimeBetweenRdm = 0;
                    RdmVect[0] = RdmVect[1];
                    RdmVect[1] = new Vector3(Random.Range(-50.0f, 50.0f), Random.Range(-50.0f, 50.0f), Random.Range(-50.0f, 50.0f));
                }
                RdmVect[2] = Vector3.Lerp(RdmVect[0], RdmVect[1], TimeBetweenRdm);
                relativeV = RdmVect[2];
                upTarget = RdmVect[2];
            }

            if (ShipReferenceFrame._currentReferenceFrame != null)
            {

                if (!ShipDamaged[0]._damaged || !DisableOnDamageGameRule)
                {
                    //Vector3 relativeV = ShipReferenceFrame._relativeVelocity;
                    relativeV = ShipBody.GetComponent<ShipBody>()._currentVelocity - ShipReferenceFrame._currentReferenceFrame._attachedOWRigidbody._currentVelocity;

                    upTarget = ShipReferenceFrame._currentReferenceFrame._attachedOWRigidbody._lastPosition;
                }

                
                Horizon.SetActive(HorizonGameRule);
                RotateUpObject(myObject, relativeV, upTarget);
                RotateUpObject(Horizon.transform, upTarget, Cockpit.transform.position);

                NPoleCompass(Cockpit.transform, ShipReferenceFrame._currentReferenceFrame._attachedOWRigidbody.transform);

                FakeShip.transform.position = Cockpit.transform.position;
                FakeShip.transform.rotation = Cockpit.transform.rotation;
                Vector3 Velocity = ParallelVelocityToBody(FakeShip.transform, upTarget, relativeV);

                string FinalText = "";

                if (HUDEnabled[0])
                {
                    FinalText = FinalText + ((int)Velocity.z + "m/s alt\n");
                }
                if (HUDEnabled[1])
                {
                    FinalText = FinalText + ((int)Mathf.Abs(Velocity.x) + "m/s spd\n");
                }
                if (HUDEnabled[2])
                {
                    FinalText = FinalText + ((int)relativeV.magnitude + "m/s tru\n");
                }

                if (HUDEnabled[3])
                {
                    float acc = ((VelocityMemory[0] - VelocityMemory[1]) / 0.1f);
                    string accString = "m/s G";

                    if (AccUnits == 0)
                    {
                        //int First = (int)Mathf.Floor(acc);
                        accString = (acc / 12).ToString("0.00") + "x  G";
                    }
                    else if (AccUnits == 1)
                    {
                        accString = (int)acc + "m/s  G";
                    }
                    else if (AccUnits == 2)
                    {
                        accString = (acc / 9.8f).ToString("0.00") + "x  G";
                    }
                    else
                    {
                        accString = "";
                    }

                    FinalText = FinalText + accString + "\n";
                }
                if (HUDEnabled[4])
                {
                    if (ShipReferenceFrame._currentReferenceFrame._attachedOWRigidbody._attachedGravityVolume != null)
                    {
                        var Attractor = ShipReferenceFrame._currentReferenceFrame._attachedOWRigidbody._attachedGravityVolume;
                        float OrbitalSpeed = 0;
                        if (Attractor._falloffType == GravityVolume.FalloffType.inverseSquared)
                        {
                            float R = Vector3.Distance(Cockpit.transform.position, upTarget);
                            OrbitalSpeed = Mathf.Sqrt((Attractor._gravitationalMass * 0.001f) / R);
                        }
                        else
                        {
                            OrbitalSpeed = Mathf.Sqrt(Attractor._gravitationalMass * 0.001f);
                        }

                        FinalText = FinalText + (int)OrbitalSpeed + "m/s orb";
                    }
                    
                    else
                    {
                        FinalText = FinalText + "ERR:G_NULL orb";
                    }
                    
                }

                HSpeedTxt.text = FinalText;

            }
            else
            {
                HSpeedTxt.text = ("NO DATA\nSELECT TARGET");
                RotateUpObject(myObject, relativeV, upTarget);
                Horizon.SetActive(false);
            }

            if (ShipDamaged[0]._damaged && DisableOnDamageGameRule)
            {
                HSpeedTxt.text = HSpeedTxt.text + "\nERR:SENSORS_DMG";
            }

            if (AltimeterGameRule)
            {
                if (HUDVanillaAltimeterGround.localScale.x < 0.001f && HUDVanillaAltimeterShip.localPosition.x <= -0.389f)
                {
                    HUDAltimeter.SetActive(false);
                }
                else
                {
                    HUDAltimeter.SetActive(true);
                    HUDAltimeterShip.localPosition = HUDVanillaAltimeterShip.localPosition;
                    HUDAltimeterGround.localScale = HUDVanillaAltimeterGround.localScale;
                }
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

        else
        {
            ShipCompassHUD.Instance.LogForStupids("FLIGHT PATH VECTOR INITIALIZATION FAILED\nDEACTIVATE THIS MOD IF YOU HAVE ISSUES");
            this.gameObject.SetActive(false);
        }
    }

    public void FixedUpdate()
    {
        if (Initialized == true)
        {
            if (HUDEnabled[3])
            {
                longerTimer += Time.fixedDeltaTime;
                if (longerTimer >= 0.1f)
                {
                    longerTimer = 0;
                    VelocityMemory[1] = VelocityMemory[0];
                    VelocityMemory[0] = (ShipBody.GetComponent<ShipBody>()._currentVelocity - ShipReferenceFrame._currentReferenceFrame._attachedOWRigidbody._currentVelocity).magnitude;
                }
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

    public void NPoleCompass(Transform p1, Transform Planet)
    {

        VirtualNorth.position = p1.position;
        VirtualNorth.rotation = p1.rotation;

        ParallelToBody(Planet, VirtualNorth, -Planet.up);

        VirtualNeedle.rotation = p1.rotation;
        ParallelToBody(Planet, VirtualNeedle, VirtualNeedle.forward);

        float Angle = Vector3.Angle(-VirtualNeedle.right, VirtualNorth.right);
        string StringAngle = "" + (int)Angle;
        Vector3 cross = Vector3.Cross(-VirtualNeedle.right, VirtualNorth.right);
        if (cross.y < 0)
        {
            Angle = -Angle;
            StringAngle = "" + (360 + (int)Angle);
        }

        MagneticCompassTxt.text = StringAngle + "°";
        ShipNeedle.localEulerAngles = new Vector3(0, 0, Angle);
    }

    private void ParallelToBody(Transform body, Transform p1, Vector3 v)
    {
        Vector3 forward = (body.position - p1.position).normalized;
        Vector3 right = Vector3.Cross(v, forward).normalized;
        p1.rotation = Quaternion.LookRotation(forward, right);
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


    public void ConfigureInTwoStep(bool ShowMark, float MarkersSize, string HorizonKind, float RotationSensibility, bool DisableOnDamage, bool ShowComp, bool ShowAltimeter, bool ShowAlt, bool ShowSpd, bool  ShowOrb, bool ShowAcc, bool ShowOrbSpd, string AccUnit, bool ShowConsole)
    {
        if (Initialized)
        {

            if (HorizonKind == "Separate Lines")
            {
                HorizonGameRule = true;
                Horizon.transform.GetChild(0).gameObject.SetActive(true);
                Horizon.transform.GetChild(1).gameObject.SetActive(true);
                Horizon.transform.GetChild(2).gameObject.SetActive(false);
            }
            else if (HorizonKind == "Complete Line")
            {
                HorizonGameRule = true;
                Horizon.transform.GetChild(0).gameObject.SetActive(false);
                Horizon.transform.GetChild(1).gameObject.SetActive(false);
                Horizon.transform.GetChild(2).gameObject.SetActive(true);
            }
            else
            {
                HorizonGameRule = false;
            }
            Horizon.SetActive(HorizonGameRule);

            if (AccUnit == "Timber Hearth Gravity")
            {
                AccUnits = 0;
            }
            else if (AccUnit == "m/s")
            {
                AccUnits = 1;
            }
            else if (AccUnit == "Earth Gravity")
            {
                AccUnits = 2;
            }
            else
            {
                AccUnits = -1;
            }

            ShipAutoRotationSpeed = 10 + (RotationSensibility * 10);

            LazyMarkersResize(MarkersSize,CompassObject.transform.GetChild(0));
            LazyMarkersResize(MarkersSize, CompassObject.transform.GetChild(1));
            LazyMarkersResize(MarkersSize, CompassObject.transform.GetChild(2));
            LazyMarkersResize(MarkersSize, CompassObject.transform.GetChild(3));
            LazyMarkersResize(MarkersSize, CompassObject.transform.GetChild(4));
            LazyMarkersResize(MarkersSize, CompassObject.transform.GetChild(5));
            LazyMarkersResize(MarkersSize, ManeuvreCompass.transform.GetChild(0));


            AltimeterGameRule = ShowAltimeter;
            HUDAltimeter.SetActive(AltimeterGameRule);

            MagneticCompassGameRule = ShowComp;
            MagneticCompass.SetActive(ShowComp);


            HUDEnabled = new List<bool>();
            
            HUDEnabled.Add(ShowAlt);
            HUDEnabled.Add(ShowSpd);
            HUDEnabled.Add(ShowOrb);
            HUDEnabled.Add(ShowAcc);
            HUDEnabled.Add(ShowOrbSpd);


            SystemOnOff(ShowMark);
            CompassObjectGameRule = ShowMark;
            Button.SetActive(ShowConsole);
            ManeuvreCompass.SetActive(ShowConsole);

            DisableOnDamageGameRule = DisableOnDamage;

        }
    }

    private void LazyMarkersResize(float newSize, Transform Child)
    {
        Child.localScale = new Vector3((newSize * 0.005f) + 0.0025f, (newSize * 0.005f) + 0.0025f, (newSize * 0.005f) + 0.0025f);
    }

    private Vector3 ParallelVelocityToBody(Transform ShipTrans, Vector3 AttractorPos, Vector3 Velocity)
    {
        Vector3 forward = (AttractorPos - ShipTrans.position).normalized;
        Vector3 velocityDir = Velocity.normalized;
        Vector3 right = Vector3.Cross(velocityDir, forward).normalized;
        ShipTrans.transform.rotation = Quaternion.LookRotation(forward, right);

        float Parallel = Vector3.Dot(Velocity, ShipTrans.right);

        float Closing = Vector3.Dot(Velocity, ShipTrans.forward);

        return new Vector3(Parallel, 0, Closing);
    }


}

