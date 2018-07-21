using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vigilant.Enums;
using Vigilant.Helpers;
using Vigilant.Interface.Engines;
using Vigilant.Managers.InputManagers;

namespace Vigilant.Engines
{
    /// <summary>
    /// Default car engine.
    /// </summary>
    public class BaseEngine : MonoBehaviour, IEngine
    {
        #region Fields

        //TODO: возможно стоит назначать через di
        // Cached References
        [SerializeField] protected Rigidbody tempRigidbody;
	    [SerializeField] protected Transform myTransform;
	    [SerializeField] protected CarDynamics carDynamics;
	    [SerializeField] protected Drivetrain drivetrain;
	    [SerializeField] protected Axles axles;
	    [SerializeField] protected EngineAssistant engineAssistant;

	    [SerializeField] protected OnlyKeyboardInputManager inputManager;
	    
	    [SerializeField] protected float steerAssistanceMinVelocity;
	    [SerializeField] protected float minTCSVelocity;
	    [SerializeField] protected float minESPVelocity;
	    [SerializeField] protected float minABSVelocity;
	    
	    [SerializeField] protected float thresholdTCS;
	    [SerializeField] protected float thresholdABS;
	    [SerializeField] protected float strengthESP = 2f;
	    
	    [SerializeField] protected bool smoothInput;
	    [SerializeField] protected bool TCS; // Traction Control System.
	    [SerializeField] protected bool ESP; // Electronic Stability Program.
	    [SerializeField] protected bool ABS; // Electronic Stability Program.
	    
        [SerializeField] List<Wheel> allWheels;

        protected Vector2 moveDirection;
        
        protected float ownerVelocity;// car's speed
	    protected float maxThrottle;
	    protected float maxThrottleInReverse;//maximum allowed throttle in reverse
	    protected float oldSteering;
	    protected float steering;
	    protected float deltaSteering;
	    protected float brake;
	    protected float throttle;
	    protected float steerTimer;
	    protected float maxSteer;
	    
        protected int targetGear;

	    protected float externalTCSThreshold;  // used to improve TCS behaviour with powerMultiplier>1
	    
	    protected bool steerAssistance;
	    protected bool triggeredTCS;
	    protected bool triggeredESP;
	    protected bool triggeredABS;
	    protected bool engineStart;
	    protected float clutchInput;//TODO: Сцепление, заменить на бул
	    protected float handbrakeInput;//TODO: ручной тормоз, заменить на бул


	    private EngineParams engineParams;
	    
        #endregion

        #region Properties

        public bool IsBrake
        {
            get
            {
                try
                {
                    return moveDirection.y < 0;
                }
                catch (NullReferenceException exception)
                {
                    Debug.LogError("NullReference on MoveDirection vector:" + exception);
                    throw;
                }
                catch (Exception exception)
                {
                    Debug.LogError("Some unexpected error: " + exception);
                    throw;
                }
            }
        }

        public bool IsAcceleration
        {
            get
            {
                try
                {
                    return moveDirection.y > 0;
                }
                catch (NullReferenceException exception)
                {
                    Debug.LogError("NullReference on MoveDirection vector:" + exception);
                    throw;
                }
                catch (Exception exception)
                {
                    Debug.LogError("Some unexpected error: " + exception);
                    throw;
                }
            }
        }

	    public bool IsSmoothInput
	    {
		    get
		    {
			    try
			    {
				    return smoothInput;
			    }
			    catch (Exception exception)
			    {
				    Debug.LogError("Some unexpected error: " + exception);
				    throw;
			    }
		    }

		    set { smoothInput = value; }
	    }

	    public float SteerReleaseTime
	    {
		    get
		    {
			    try
			    {
				    if (engineParams != null)
				    {
					    return engineParams.SteerReleaseTime;
				    }
				    else
				    {
					    //TODO:Add null ref check and error.
					    Debug.LogError("null");
				    }
			    }
			    catch (Exception exception)
			    {
				    Debug.LogError("Some unexpected error: " + exception);
				    throw;
			    }
		    }
	    }

	    public float SteerTime
	    {
		    get
		    {
			    try
			    {
				    if (engineParams != null)
				    {
					    return engineParams.SteerTime;
				    }
				    else
				    {
					    //TODO:Add null ref check and error.
					    Debug.LogError("null");
				    }
			    }
			    catch (Exception exception)
			    {
				    Debug.LogError("Some unexpected error: " + exception);
				    throw;
			    }
		    }
	    }

	    #endregion

        #region Unity Events

        private void Start()
        {
            if (tempRigidbody == null)
            {
                tempRigidbody = GetComponent<Rigidbody>();
            }

            if (carDynamics == null)
            {
                carDynamics = GetComponent<CarDynamics>();
            }

            if (drivetrain == null)
            {
                drivetrain = GetComponent<Drivetrain>();
            }

            if (axles == null)
            {
                axles = GetComponent<Axles>();
            }

            if (myTransform == null)
            {
                myTransform = transform;
            }

            if (allWheels == null && allWheels.Count <= 0)
            {
                //TODO:Change to list in axles
                allWheels = axles.allWheels.ToList();
            }

            if (inputManager != null)
            {
                inputManager.MoveDirectionChanged += OnMoveDirectionChange;
                inputManager.Button += OnButtonClick;
            }

	        if (engineParams == null)
	        {
		        engineParams = new EngineParams();
	        }

	        moveDirection = Vector2.zero;
        }

	    private void Update()
	    {
		    if (!drivetrain.changingGear && targetGear != drivetrain.gear)
		    {
			    drivetrain.Shift(targetGear);
		    }

		    if (drivetrain.automatic && drivetrain.autoReverse)
		    {
			    if (moveDirection.y < 0 && ownerVelocity <= 0.5f)
			    {
				    if (drivetrain.gear != drivetrain.firstReverse)
				    {
					    drivetrain.Shift(drivetrain.firstReverse);
				    }
			    }

			    if (moveDirection.y > 0 && ownerVelocity <= 0.5f)
			    {
				    if (drivetrain.gear != drivetrain.first)
				    {
					    drivetrain.Shift(drivetrain.first);
				    }
			    }
		    }
	    }

	    private void FixedUpdate()
	    {
		    maxThrottle = 1;
		    oldSteering = steering;
		    ownerVelocity = carDynamics.velo;
		    var ownerVelocityInKmh = carDynamics.velo * 3.6f;// car's speed in kmh
		    bool onGround = drivetrain.OnGround();

		    if (smoothInput)
		    {
			    engineAssistant.SmoothSteer(moveDirection.x,ownerVelocity, engineParams, ref steering);
			    
			    engineAssistant.SmoothThrottle(moveDirection.y, engineParams, drivetrain.changingGear,
				    drivetrain.automatic, ref throttle);
			    engineAssistant.SmoothBrakes(moveDirection.y,engineParams,ref brake);
		    }
		    else
		    {
			    steering = moveDirection.x;
			    brake = moveDirection.y < 0 ? moveDirection.y : 0;
			    throttle = moveDirection.y > 0 ? moveDirection.y : 0;
			    if (drivetrain.changingGear && drivetrain.automatic)
			    {
				    throttle = 0;
			    }
		    }

		    if (steerAssistance && drivetrain.ratio > 0 && ownerVelocityInKmh > steerAssistanceMinVelocity)
		    {
			    float averageLateralSlip = 0;
			    foreach (Wheel w in allWheels)
			    {
				    averageLateralSlip += w.lateralSlip;
			    }

			    averageLateralSlip /= allWheels.Count;

			    //we enablesteerAssistance only for speed > SteerAssistanceMinVelocity (in km/h)
			    engineAssistant.SteerAssistance(averageLateralSlip, oldSteering, carDynamics.fixedTimeStepScalar,
				    ref steerTimer, ref steering);
		    }
		    else
		    {
			    steerTimer = 0;
			    maxSteer = 1;
		    }


		    triggeredTCS = false;
		    if (TCS && drivetrain.ratio > 0 && drivetrain.clutch.GetClutchPosition() >= 0.9f && onGround &&
		        throttle > drivetrain.idlethrottle && ownerVelocityInKmh > minTCSVelocity)
		    {

			    //we enable TCS only for speed > TCSMinVelocity (in km/h)
			    engineAssistant.DoTCS(drivetrain.poweredWheels.ToList(), thresholdTCS, externalTCSThreshold, ref maxThrottle,
				    ref triggeredTCS);
		    }

		    triggeredESP = false;
		    if (ESP && drivetrain.ratio > 0 && onGround && ownerVelocityInKmh > minESPVelocity)
		    {
			    //we enable ESP only for speed > ESPMinVelocity (in km/h)
			    engineAssistant.DoESP(myTransform, tempRigidbody, ownerVelocity, axles, strengthESP, drivetrain, ref maxThrottle,
				    ref triggeredESP);
		    }

		    triggeredABS = false;
		    if (ABS && brake > 0 && ownerVelocityInKmh > minABSVelocity && onGround)
		    {
			    //we enable ABS only for speed > ABSMinVelocity (in km/h)
			    engineAssistant.DoABS(thresholdABS, brake, allWheels, ref triggeredABS);
		    }

		    float mmaxThrottle;
		    if (drivetrain.gearRatios[drivetrain.gear] > 0)
		    {
			    mmaxThrottle = maxThrottle;
		    }
		    else
		    {
			    mmaxThrottle = maxThrottleInReverse;
		    }

		    if (drivetrain.revLimiterTriggered) throttle = 0;
		    else if (drivetrain.revLimiterReleased)
		    {
			    throttle = moveDirection.y;
		    }
		    else
		    {
			    throttle = Mathf.Clamp(throttle, drivetrain.idlethrottle, mmaxThrottle);
		    }

		    brake = Mathf.Clamp01(brake);
		    steering = Mathf.Clamp(steering, -1, 1);
		    deltaSteering = steering - oldSteering;

		    // Apply inputs
		    foreach (Wheel w in allWheels)
		    {
			    if (!(ABS && ownerVelocityInKmh > minABSVelocity && moveDirection.y < 0))
			    {
				    w.brake = brake; // if ABS is on, brakes are applied by ABS directly
			    }

			    w.handbrake = handbrakeInput;
			    w.steering = steering;
			    w.deltaSteering = deltaSteering;
		    }

		    drivetrain.throttle = throttle;
		    if (drivetrain.clutch != null)
		    {
			    if (clutchInput != 0 || drivetrain.autoClutch == false)
			    {
				    drivetrain.clutch.SetClutchPosition(1 - clutchInput);
			    }
		    }

		    drivetrain.startEngine = engineStart;
	    }

	    private void OnDestroy()
        {
            if (inputManager == null) return;
            
            inputManager.MoveDirectionChanged -= OnMoveDirectionChange;
            inputManager.Button -= OnButtonClick;
        }

        #endregion

        #region Methods

        private void OnMoveDirectionChange(Vector2 direction)
        {
            moveDirection = direction;
        }

        private void OnButtonClick(ButtonCode buttonCode, ButtonState buttonState)
        {
            switch (buttonCode)
            {
                case ButtonCode.GearShiftUp:
                    targetGear++;
                    break;
                case ButtonCode.GearShiftDown:
                    targetGear--;
                    break;
                
                default:
                    break;
            }
        }

        #endregion
    }
}