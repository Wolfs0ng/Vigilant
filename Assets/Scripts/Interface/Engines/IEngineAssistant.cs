using System.Collections.Generic;
using UnityEngine;
using Vigilant.Engines;

namespace Vigilant.Interface.Engines
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEngineAssistant
    {
        #region Properties

        float SteerTimer { get; set; }

        #endregion

        #region Methods

        void SteerAssistance(float averageLateralSlip, float oldSteering, float fixedTimeStepScalar,
            ref float steering);

        void SmoothSteer(float steerInput, float velo, EngineParams engineParams,
            ref float steering);

        void SmoothThrottle(float throttleInput, EngineParams engineParams,
            bool isChangingGear, bool isAutomaticTransmission,
            ref float throttle);

        void SmoothBrakes(float brakeInput, EngineParams engineParams, ref float brake);

        void DoABS(float ABSThreshold, float brake, List<Wheel> allWheels);

        void DoTCS(List<Wheel> poweredWheels, float TCSThreshold, float externalTCSThreshold, ref float maxThrottle);

        void DoESP(Transform playerTransform, Rigidbody playeRigidbody, float velocity, Axles axles,
            float ESPStrength, Drivetrain drivetrain, ref float maxThrottle);

        #endregion
    }
}