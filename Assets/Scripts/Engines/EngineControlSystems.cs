using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Vigilant.Engines
{ 
    /// <summary>
    /// 
    /// </summary>
    public sealed class EngineControlSystems
    {
        #region Fields
        
        protected bool TCS; // Traction Control System.
        protected bool ESP; // Electronic Stability Program.
        protected bool ABS; // Anti-lock braking.
        
        protected float minTCSVelocity;
        protected float minESPVelocity;
        protected float minABSVelocity;
	    
        protected float thresholdTCS;
        protected float thresholdABS;
        protected float strengthESP = 2f;

        protected EngineParams engineParams;
        protected EngineAssistant engineAssistant;
        protected Drivetrain drivetrain;
        
        protected float externalTCSThreshold;  // used to improve TCS behaviour with powerMultiplier>1
        
        #endregion

        #region Properties
        
        public bool Abs
        {
            get { return ABS; }
            set { ABS = value; }
        }

        public float MinAbsVelocity
        {
            get { return minABSVelocity; }
            set { minABSVelocity = value; }
        }

        public EngineParams EngineParams
        {
            get { return engineParams; }
            set { engineParams = value; }
        }

        public EngineAssistant EngineAssistant
        {
            get { return engineAssistant; }
            set { engineAssistant = value; }
        }

        public Drivetrain VehicleDrivetrain
        {
            get { return drivetrain; }
            set { drivetrain = value; }
        }

        #endregion

        #region Methods

        public EngineControlSystems(bool TCS, bool ESP, bool ABS, float minTCSVelocity, float minESPVelocity,
            float minABSVelocity,float thresholdTCS,float thresholdABS,float strengthESP)
        {
            this.TCS = TCS;
            this.ESP = ESP;
            this.ABS = ABS;
            this.minTCSVelocity = minTCSVelocity;
            this.minESPVelocity = minESPVelocity;
            this.minABSVelocity = minABSVelocity;
            this.thresholdTCS = thresholdTCS;
            this.thresholdABS = thresholdABS;
            this.strengthESP = strengthESP;
        }

        public float CheckControlSystem(float throttle, GameObject engineHolder, float brake, List<Wheel> allWheels)
        {
            float currentMaxThrottle = engineParams.MaxThrottle;
            bool onGround = drivetrain.OnGround();

            float ownerVelocityInKmh = engineParams.OwnerVelocity * 3.6f;// car's speed in kmh
            
            Transform holderTransform = engineHolder.transform;
            Rigidbody holderRigidbody = engineHolder.GetComponent<Rigidbody>();

            Axles axles = engineHolder.GetComponent<Axles>();
            
            if (TCS && drivetrain.ratio > 0 && drivetrain.clutch.GetClutchPosition() >= 0.9f && onGround &&
                throttle > drivetrain.idlethrottle && engineParams.OwnerVelocity > minTCSVelocity)
            {
                //we enable TCS only for speed > TCSMinVelocity (in km/h)
                engineAssistant.DoTCS(drivetrain.poweredWheels.ToList(), thresholdTCS, externalTCSThreshold,
                    ref currentMaxThrottle);
            }

            if (ESP && drivetrain.ratio > 0 && onGround && ownerVelocityInKmh > minESPVelocity)
            {
                //we enable ESP only for speed > ESPMinVelocity (in km/h)
                engineAssistant.DoESP(holderTransform, holderRigidbody, engineParams.OwnerVelocity, axles,
                    strengthESP, drivetrain, ref currentMaxThrottle);
            }

            if (ABS && brake > 0 && ownerVelocityInKmh > minABSVelocity && onGround)
            {
                //we enable ABS only for speed > ABSMinVelocity (in km/h)
                engineAssistant.DoABS(thresholdABS, brake, allWheels);
            }

            return throttle;
        }

        #endregion
    }
}