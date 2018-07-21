using UnityEngine;

namespace Vigilant.Engines
{ 
    /// <summary>
    /// 
    /// </summary>
    public class EngineParams
    {
        #region Fields
        
        protected float throttleTime = 0.1f;// How long it takes to fully engage the throttle.
        protected float throttleReleaseTime = 0.1f;// How long it takes to fully release the throttle.
        protected float brakesTime = 0.1f;// How long it takes to fully engage the brakes
        protected float brakesReleaseTime = 0.1f;// How long it takes to fully release the brakes
        protected float veloSteerTime = 0.05f;// This is added to steerTime per m/s of velocity,
                                              // so steering is slower when the car is moving faster.
        protected float velocitySteerReleaseTime = 0.05f;// This is added to steerReleaseTime per m/s of velocity,
                                                         // so steering is slower when the car is moving faster.
        protected float steerReleaseTime = 0.1f;// How long it takes to fully turn the steering
                                                // wheel from full lock to center.
        protected float steerTime = 0.1f;// How long it takes to fully turn the steering
                                         // wheel from center to full lock.
        protected float steerCorrectionFactor = 0;// When detecting a situation where the player tries
                                                  // to counter steer to correct an oversteer situation,
                                                  // steering speed will be multiplied by the difference
                                                  // between steerInput and current steering times this 
                                                  // factor, to make the correction easier.
        
        #endregion

        #region Properties
        
        public float SteerReleaseTime
        {
            get { return steerReleaseTime; }
        }

        public float SteerTime
        {
            get { return steerTime; }
        }

        public float VelocitySteerReleaseTime
        {
            get { return velocitySteerReleaseTime; }
        }

        public float VeloSteerTime
        {
            get { return veloSteerTime; }
        }

        public float SteerCorrectionFactor
        {
            get { return steerCorrectionFactor; }
        }

        public float BrakesReleaseTime
        {
            get { return brakesReleaseTime; }
        }

        public float BrakesTime
        {
            get { return brakesTime; }
        }

        public float ThrottleReleaseTime
        {
            get { return throttleReleaseTime; }
        }

        public float ThrottleTime
        {
            get { return throttleTime; }
        }

        #endregion

        #region Unity Events
        #endregion

        #region Methods

        public EngineParams(float throttleTime = .1f, float throttleReleaseTime = .1f, float brakesTime = .1f,
            float brakesReleaseTime = .1f, float steerTime = .1f, float steerReleaseTime = .1f,
            float veloSteerTime = .05f, float velocitySteerReleaseTime = .05f, float steerCorrectionFactor = 0)
        {

        }

        #endregion
    }
}