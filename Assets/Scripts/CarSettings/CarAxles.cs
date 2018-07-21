using System.Collections.Generic;
using UnityEngine;
using Vigilant.Enums;

namespace Vigilant.CarSettings
{
    /// <summary>
    /// Contains all car axles and configure them.
    /// </summary>
    public sealed class CarAxles : MonoBehaviour
    {
        #region Fields

        [SerializeField] private CarAxle frontAxle;
        [SerializeField] private CarAxle rearAxle;
        [SerializeField] private List<CarAxle> otherAxles;

        private List<Wheel> allWheels;

        #endregion

        #region Properties

        public List<Wheel> AllWheels
        {
            get { return allWheels; }
            set { allWheels = value; }
        }

        public CarAxle FrontAxle
        {
            get { return frontAxle; }
            set { frontAxle = value; }
        }

        public CarAxle RearAxle
        {
            get { return rearAxle; }
            set { rearAxle = value; }
        }

        public List<CarAxle> OtherAxles
        {
            get { return otherAxles; }
            set { otherAxles = value; }
        }

        #endregion

        #region Unity Events

        private void Start()
        {
            CheckWheels();
        }

        private void Awake()
        {
            SetWheels();
        }

        #endregion

        #region Methods

        private void SetWheels()
        {
            //В ожидании моего Wheel класса
            /*if (frontAxle.leftWheel)
            {
                frontAxle.leftWheel.wheelPos = WheelPosition.FrontLeft;
            }

            if (frontAxle.rightWheel)
            {
                frontAxle.rightWheel.wheelPos = WheelPosition.FrontRight;
            }

            if (rearAxle.leftWheel)
            {
                rearAxle.leftWheel.wheelPos = WheelPosition.RearLeft;
            }

            if (rearAxle.rightWheel)
            {
                rearAxle.rightWheel.wheelPos = WheelPosition.RearRight;
            }*/
            
            FrontAxle.Wheels = new List<Wheel>();
            RearAxle.Wheels = new List<Wheel>();
            
            FrontAxle.Camber = Mathf.Clamp(FrontAxle.Camber, -10, 10);
            RearAxle.Camber = Mathf.Clamp(FrontAxle.Camber, -10, 10);
            
            AllWheels = new List<Wheel>();
            AllWheels.AddRange(FrontAxle.Wheels);
            AllWheels.AddRange(RearAxle.Wheels);
            
            foreach (CarAxle otherAxle in OtherAxles)
            {
                otherAxle.Wheels = new List<Wheel>();
                otherAxle.Camber = Mathf.Clamp(FrontAxle.Camber, -10, 10);
                AllWheels.AddRange(otherAxle.Wheels);
            }
        }

        private void CheckWheels()
        {
            if (FrontAxle.LeftWheel == null)
            {
                Debug.LogWarning("UnityCar: front left wheel not assigned " + " (" + transform.name + ")");
            }

            if (FrontAxle.RightWheel == null)
            {
                Debug.LogWarning("UnityCar: front right wheel not assigned " + " (" + transform.name + ")");
            }

            if (RearAxle.LeftWheel == null)
            {
                Debug.LogWarning("UnityCar: rear left wheel not assigned " + " (" + transform.name + ")");
            }

            if (RearAxle.RightWheel == null)
            {
                Debug.LogWarning("UnityCar: rear right wheel not assigned " + " (" + transform.name + ")");
            }
        }

        #endregion

    }
}