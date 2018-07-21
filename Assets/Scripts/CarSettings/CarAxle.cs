using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vigilant.CarSettings
{
    /// <summary>
    /// Contains axle parameters.
    /// </summary>
    [System.Serializable]
    public sealed class CarAxle
    {
        #region Fields

        [Header("Wheels")] [SerializeField] private Wheel leftWheel;
        [SerializeField] private Wheel rightWheel;

        [Header("Axle parameters")] [SerializeField]
        private bool powered;

        [SerializeField] private float suspensionTravel = 0.2f;
        [SerializeField] private float suspensionRate = 20000;
        [SerializeField] private float bumpRate = 4000;
        [SerializeField] private float reboundRate = 4000;
        [SerializeField] private float fastBumpFactor = 0.3f;
        [SerializeField] private float fastReboundFactor = 0.3f;
        [SerializeField] private float antiRollBarRate = 10000;
        [SerializeField] private float brakeFrictionTorque = 1500;
        [SerializeField] private float handbrakeFrictionTorque = 0;
        [SerializeField] private float maxSteeringAngle = 0;
        [SerializeField] private float forwardGripFactor = 1;
        [SerializeField] private float sidewaysGripFactor = 1;
        [SerializeField] private float camber = 0;

        // WTF ? что делают параметры шин в оси ?
        // В данный момент не вижу причин держать тут, переносим в колеса.
        [Header("Tires parameters")] [SerializeField]
        private CarDynamics.Tires tires;

        [SerializeField] private float tiresPressure = 200;
        [SerializeField] private float optimalTiresPressure = 200;

        //Разобраться и сделать адекватно
        private List<Wheel> wheels;

        private float deltaCamber;
        private float oldCamber;

        #endregion

        #region Properties

        public Wheel LeftWheel
        {
            get { return leftWheel; }
            set
            {
                if (value == null)
                {
                    throw new NullReferenceException("Left Wheel can't be null");
                }

                leftWheel = value;
            }
        }

        public Wheel RightWheel
        {
            get { return rightWheel; }
            set
            {
                if (value == null)
                {
                    throw new NullReferenceException("Right Wheel can't be null");
                }

                rightWheel = value;
            }
        }

        public bool Powered
        {
            get { return powered; }
            set { powered = value; }
        }

        public float SuspensionTravel
        {
            get { return suspensionTravel; }
            set { suspensionTravel = value; }
        }

        public float SuspensionRate
        {
            get { return suspensionRate; }
            set { suspensionRate = value; }
        }

        public float BumpRate
        {
            get { return bumpRate; }
            set { bumpRate = value; }
        }

        public float ReboundRate
        {
            get { return reboundRate; }
            set { reboundRate = value; }
        }

        public float FastBumpFactor
        {
            get { return fastBumpFactor; }
            set { fastBumpFactor = value; }
        }

        public float FastReboundFactor
        {
            get { return fastReboundFactor; }
            set { fastReboundFactor = value; }
        }

        public float AntiRollBarRate
        {
            get { return antiRollBarRate; }
            set { antiRollBarRate = value; }
        }

        public float BrakeFrictionTorque
        {
            get { return brakeFrictionTorque; }
            set { brakeFrictionTorque = value; }
        }

        public float HandbrakeFrictionTorque
        {
            get { return handbrakeFrictionTorque; }
            set { handbrakeFrictionTorque = value; }
        }

        public float MaxSteeringAngle
        {
            get { return maxSteeringAngle; }
            set { maxSteeringAngle = value; }
        }

        public float ForwardGripFactor
        {
            get { return forwardGripFactor; }
            set { forwardGripFactor = value; }
        }

        public float SidewaysGripFactor
        {
            get { return sidewaysGripFactor; }
            set { sidewaysGripFactor = value; }
        }

        public float Camber
        {
            get { return camber; }
            set { camber = value; }
        }

        public float DeltaCamber
        {
            get { return deltaCamber; }
            set { deltaCamber = value; }
        }

        public float OldCamber
        {
            get { return oldCamber; }
            set { oldCamber = value; }
        }

        public CarDynamics.Tires Tires
        {
            get { return tires; }
            set { tires = value; }
        }

        public float TiresPressure
        {
            get { return tiresPressure; }
            set { tiresPressure = value; }
        }

        public float OptimalTiresPressure
        {
            get { return optimalTiresPressure; }
            set { optimalTiresPressure = value; }
        }

        public List<Wheel> Wheels
        {
            get
            {
                if (wheels == null)
                {
                    return wheels;
                }

                if (LeftWheel != null && !wheels.Contains(LeftWheel))
                {
                    wheels.Add(LeftWheel);
                }

                if (RightWheel != null && !wheels.Contains(RightWheel))
                {
                    wheels.Add(rightWheel);
                }

                return wheels;
            }
            set { wheels = value; }
        }

        #endregion

        #region Methods

        #endregion
    }
}