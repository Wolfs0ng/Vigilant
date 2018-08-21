using UnityEngine;
using Vigilant.Engines;

namespace Vigilant.Helpers
{ 
    /// <summary>
    /// 
    /// </summary>
    public sealed class EngineSpecialParams : MonoBehaviour
    {
        #region Fields

        [Header("Engine Class ",order = 0)]
        [Tooltip("Engine class wich control current vehicle")]
        
        [SerializeField] private BaseEngine engine;

        [Header("Parameters for engine", order = 1)]
        [Tooltip("Main engine params")]
        
        [SerializeField] private float throttleTime = .1f;
        [SerializeField] private float throttleReleaseTime = .1f;
        [SerializeField] private float brakesTime = .1f;
        [SerializeField] private float brakesReleaseTime = .1f;
        [SerializeField] private float steerTime = .1f;
        [SerializeField] private float steerReleaseTime = .1f;
        [SerializeField] private float veloSteerTime = .05f;
        [SerializeField] private float velocitySteerReleaseTime = .05f;
        [SerializeField] private float steerCorrectionFactor = 0f;
        [SerializeField] private float maxThrottle = 1f;
        [SerializeField] private float maxThrottleInReverse = 1f;

        [Header("Parameters for engine control systems", order = 2)]
        [Tooltip("Engine control systems param")]
        
        [SerializeField] private bool TCS;
        [SerializeField] private bool ESP;
        [SerializeField] private bool ABS;
        [SerializeField] private float minTCSVelocity;
        [SerializeField] private float minESPVelocity;
        [SerializeField] private float minABSVelocity;
        [SerializeField] private float thresholdTCS;
        [SerializeField] private float thresholdABS;
        [SerializeField] private float strengthESP;
        
        #endregion

        #region Properties
        #endregion

        #region Unity Events
        #endregion

        #region Methods

        private void Awake()
        {
            EngineParams engineParams = new EngineParams(throttleTime, throttleReleaseTime, brakesTime,
                brakesReleaseTime, steerTime, steerReleaseTime, veloSteerTime, velocitySteerReleaseTime,
                steerCorrectionFactor, maxThrottle, maxThrottleInReverse);
            
            EngineControlSystems engineControlSystems = new EngineControlSystems(TCS, ESP, ABS, minTCSVelocity,
                minESPVelocity, minABSVelocity, thresholdTCS, thresholdABS, strengthESP);

            engine.EngineParams = engineParams;
            engine.EngineControlSystems = engineControlSystems;
        }
        
        #endregion
    }
}