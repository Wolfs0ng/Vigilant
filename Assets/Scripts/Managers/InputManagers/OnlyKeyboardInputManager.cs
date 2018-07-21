using System;
using System.Collections.Generic;
using UnityEngine;
using Vigilant.Enums;
using Vigilant.Exceptions;
using Vigilant.Interface.Managers;

namespace Vigilant.Managers.InputManagers
{
    /// <summary>
    /// This input controller use keyboard only.
    /// </summary>
    public sealed class OnlyKeyboardInputManager : MonoBehaviour, IInputManager
    {
        #region Fields

        public event Action<Vector2> MoveDirectionChanged = delegate { };
        public event Action<Vector2> WeaponTowerDirectionChanged = delegate { };
        public event Action<ButtonCode, ButtonState> Button = delegate { };
        
        [Header("Keys for abilities and attack buttons")]
        [SerializeField] private KeyCode primaryFire = KeyCode.Q;
        [SerializeField] private KeyCode secondaryFire = KeyCode.E;
        [SerializeField] private KeyCode special = KeyCode.RightShift;

        [Header("Keys for weapon tower direction change")] 
        [SerializeField] private KeyCode towerLeftDirection = KeyCode.LeftArrow;
        [SerializeField] private KeyCode towerRightDirection = KeyCode.RightArrow;
        [SerializeField] private KeyCode towerUpDirection = KeyCode.UpArrow;
        [SerializeField] private KeyCode towerDownDirection = KeyCode.DownArrow;

        [Header("Keys for move direction change")]
        [SerializeField] private KeyCode moveLeftDirection = KeyCode.A;
        [SerializeField] private KeyCode moveRightDirection = KeyCode.D;
        [SerializeField] private KeyCode moveForwardDirection = KeyCode.W;
        [SerializeField] private KeyCode moveBackwardDirection = KeyCode.S;

        [Header("Keys for gear shift")]
        [SerializeField] private KeyCode shiftGearUp = KeyCode.P;
        [SerializeField] private KeyCode shiftGearDown = KeyCode.L;
        
        [Header("Keys for other game things")]
        [SerializeField] private KeyCode handBrakeButton = KeyCode.Space;
        [SerializeField] private KeyCode changeCameraButton = KeyCode.C;

        /// <summary>
        /// Temp variable for directions change.
        /// </summary>
        private Vector3 changeDirection = Vector2.zero;

        private int SetGearNumber = 0;
        
        #endregion

        #region Properties
        
        

        #endregion

        #region Unity Events

        private void Update()
        {
            //Changing weapon tower direction
            changeDirection = Vector3.zero;

            if (Input.GetKey(towerLeftDirection))
            {
                changeDirection += -transform.right;
            }

            if (Input.GetKey(towerRightDirection))
            {
                changeDirection += transform.right;
            }

            if (Input.GetKey(towerUpDirection))
            {
                changeDirection += transform.up;
            }

            if (Input.GetKey(towerDownDirection))
            {
                changeDirection += -transform.up;
            }

            WeaponTowerDirectionChanged(changeDirection);

            //Fire button clicks

            if (Input.GetKeyDown(primaryFire))
            {
                Button(ButtonCode.PrimaryFire, ButtonState.Down);
            }

            if (Input.GetKey(primaryFire))
            {
                Button(ButtonCode.PrimaryFire, ButtonState.Hold);
            }

            if (Input.GetKeyUp(primaryFire))
            {
                Button(ButtonCode.PrimaryFire, ButtonState.Up);
            }

            if (Input.GetKeyDown(secondaryFire))
            {
                Button(ButtonCode.SecondaryFire, ButtonState.Down);
            }

            if (Input.GetKey(secondaryFire))
            {
                Button(ButtonCode.SecondaryFire, ButtonState.Hold);
            }

            if (Input.GetKeyUp(secondaryFire))
            {
                Button(ButtonCode.SecondaryFire, ButtonState.Up);
            }

            //Special button click

            if (Input.GetKeyDown(special))
            {
                Button(ButtonCode.Special, ButtonState.Down);
            }
            else if (Input.GetKeyUp(special))
            {
                Button(ButtonCode.Special, ButtonState.Up);
            }

            //Move direction change
            changeDirection = Vector3.zero;

            if (Input.GetKey(moveLeftDirection))
            {
                changeDirection -= transform.right;
            }

            if (Input.GetKey(moveRightDirection))
            {
                changeDirection += transform.right;
            }

            if (Input.GetKey(moveForwardDirection))
            {
                changeDirection += transform.up;
            }

            if (Input.GetKey(moveBackwardDirection))
            {
                changeDirection -= transform.up;
            }
            
            MoveDirectionChanged(changeDirection);



            /* Other buttons click */

            //Change camera click
            if (Input.GetKeyDown(changeCameraButton))
            {
                Button(ButtonCode.CameraChange, ButtonState.Down);
            }

            if (Input.GetKey(changeCameraButton))
            {
                Button(ButtonCode.CameraChange, ButtonState.Hold);
            }

            if (Input.GetKeyUp(changeCameraButton))
            {
                Button(ButtonCode.CameraChange, ButtonState.Up);
            }

            //Handbrake click
            if (Input.GetKeyDown(handBrakeButton))
            {
                Button(ButtonCode.Handbrake, ButtonState.Down);
            }

            if (Input.GetKey(handBrakeButton))
            {
                Button(ButtonCode.Handbrake, ButtonState.Hold);
            }

            if (Input.GetKeyUp(handBrakeButton))
            {
                Button(ButtonCode.Handbrake, ButtonState.Up);
            }

            //Gear changes
            if (Input.GetKeyDown(shiftGearUp))
            {
                Button(ButtonCode.GearShiftUp, ButtonState.Down);
            }

            if (Input.GetKey(shiftGearUp))
            {
                Button(ButtonCode.GearShiftUp, ButtonState.Hold);
            }

            if (Input.GetKeyUp(shiftGearUp))
            {
                Button(ButtonCode.GearShiftUp, ButtonState.Up);
            }
            
            if (Input.GetKeyDown(shiftGearDown))
            {
                Button(ButtonCode.GearShiftDown, ButtonState.Down);
            }

            if (Input.GetKey(shiftGearDown))
            {
                Button(ButtonCode.GearShiftDown, ButtonState.Hold);
            }

            if (Input.GetKeyUp(shiftGearDown))
            {
                Button(ButtonCode.GearShiftDown, ButtonState.Up);
            }

        }

        #endregion

        #region Methods
        
        #endregion

    }
}