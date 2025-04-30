using System.IO;
using System.Reflection;
using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using UnityEngine;

using ShipCompassHUD;
using System.Collections.Generic;
using Tessellation;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.Image;
using TMPro;
using Unity.Collections;
using ShipCompassHUD.Components;

namespace ShipCompassHUD.button;

public class SetRotationButton : ModBehaviour
{

    public InteractReceiver _interactReceiver;
    public ScreenPrompt _fuelDepletedPrompt;
    public bool _focused;

    [SerializeField]

    private float _timeOnPressed;
    private bool _pressed;
    private int _actionOnPressed;
    private CompassInit CompassInit;
    private bool _NegativeBehaviour;

    void Awake()
    {

        gameObject.layer = LayerMask.NameToLayer("Interactible");
        var col = gameObject.GetAddComponent<SphereCollider>();
        col.radius = 0.4f;
        col.isTrigger = true;
        gameObject.GetAddComponent<OWCollider>();
        _interactReceiver = gameObject.GetAddComponent<InteractReceiver>();
        _interactReceiver._usableInShip = true;
        _interactReceiver._interactRange = 2.5f;
        _interactReceiver.EnableInteraction();
        _interactReceiver.ChangePrompt("Toggle Light Switch");
        _interactReceiver.OnPressInteract += OnPressInteract;
    }


    void FixedUpdate()
    {

    }

   

    private void Update()
    {

        if (_focused)
        {
            if (_pressed)
            {
                _timeOnPressed += Time.deltaTime;
                if (_timeOnPressed > 0.15f)
                {
                    _timeOnPressed = 0;
                    _pressed = false;
                    
                }
            }
        }
        
        if (this.gameObject.transform.localPosition.y < 0.4f && _NegativeBehaviour == false)
        {
            this.gameObject.transform.localPosition = this.gameObject.transform.localPosition + new Vector3(0, 0.01f, 0);
        }

    }


    private void pressedAction()
    {
        if (_actionOnPressed == 0) //Left
        {
            CompassInit.ChangeWhichCharacter(-1);

        }
        else if (_actionOnPressed == 1) //Up
        {
            CompassInit.IncreaseWishedRot(1);
        }
        else if (_actionOnPressed == 2) //Down
        {
            CompassInit.IncreaseWishedRot(-1);
        }
        else if (_actionOnPressed == 3) //Right
        {
            CompassInit.ChangeWhichCharacter(1);
        }
        
        else if (_actionOnPressed == 5)
        {
            _NegativeBehaviour = !_NegativeBehaviour;

            if (_NegativeBehaviour)
            {
                CompassInit.AlignToTarget(1);
            }
            else
            {
                CompassInit.AlignToTarget(0);
            }
        }
        else if (_actionOnPressed == 6)
        {
            _NegativeBehaviour = !_NegativeBehaviour;

            if (_NegativeBehaviour)
            {
                CompassInit.AlignToTarget(2);
            }
            else
            {
                CompassInit.AlignToTarget(0);
            }
        }
        
        else if (_actionOnPressed == 7)
        {
            CompassInit.AlignTargetForward();
        }
            
        else if (_actionOnPressed == 8)
        {
            CompassInit.ResetTarget();
        }
        else if ( _actionOnPressed == 9)
        {
            CompassInit.SystemOnOff(_NegativeBehaviour);
            
            _NegativeBehaviour = !_NegativeBehaviour;

            if (_NegativeBehaviour)
            {
                _interactReceiver.ChangePrompt("Show Markes");
            }
            else
            {
                _interactReceiver.ChangePrompt("Hide Markers");
            }
        }

    }

    public void ReturnToStatus(bool NewStatus)
    {
        _NegativeBehaviour = NewStatus;
    }

    private void OnPressInteract()
    {

        _pressed = true;
        _timeOnPressed = 0;
        _interactReceiver._hasInteracted = false;
        pressedAction();

        this.gameObject.transform.localPosition = new Vector3(this.gameObject.transform.localPosition.x, 0.2f, this.gameObject.transform.localPosition.z);
    }

    private void OnGainFocus()
    {
        _focused = true;
        
        Locator.GetPromptManager().AddScreenPrompt(_fuelDepletedPrompt, PromptPosition.Center, false);
        
    }

    private void OnLoseFocus()
    {
        _focused = false;

        _pressed = false;

        Locator.GetPromptManager().RemoveScreenPrompt(_fuelDepletedPrompt, PromptPosition.Center);
    }

    

    public void SetAction(string prompt, int actionInt, CompassInit refe)
    {
        _interactReceiver.ChangePrompt(prompt);
        _actionOnPressed = actionInt;
        CompassInit = refe;
    }
}
