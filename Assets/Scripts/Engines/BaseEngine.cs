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
	    [SerializeField] protected Drivetrain drivetrain;
	    [SerializeField] protected Axles axles;
	    [SerializeField] protected CarDynamics carDynamics;
	    
	    [SerializeField] protected OnlyKeyboardInputManager inputManager;
	    
	    [SerializeField] protected bool smoothInput;
	    
        [SerializeField] List<Wheel> allWheels;

        protected Vector2 moveDirection;
        
	    protected float oldSteering;
	    protected float steering;
	    protected float deltaSteering;
	    
	    protected float brake;
	    protected float throttle;
	    
	    protected float steerTimer;
	    
        protected int targetGear;
	    
	    protected bool steerAssistance;
	    
	    protected float steerAssistanceMinVelocity;
	    
	    //Need only for dashboard, so no need at all.
//	    protected bool triggeredTCS;
//	    protected bool triggeredESP;
//	    protected bool triggeredABS;
	    
	    //Inputs
	    protected bool engineStart;
	    protected float clutchInput;//TODO: Сцепление, заменить на бул
	    protected float handbrakeInput;//TODO: ручной тормоз, заменить на бул


	    //New field, testing....
	    protected EngineParams engineParams;
	    protected EngineControlSystems engineControlSystems;
	    protected IEngineAssistant engineAssistant;
	    
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
				    return engineParams.SteerReleaseTime;
			    }
			    catch (NullReferenceException exception)
			    {
				    Debug.LogError("NullReference on EngineParams:" + exception);
				    throw;
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
				    return engineParams.SteerTime;
			    }
			    catch (NullReferenceException exception)
			    {
				    Debug.LogError("NullReference on EngineParams:" + exception);
				    throw;
			    }
			    catch (Exception exception)
			    {
				    Debug.LogError("Some unexpected error: " + exception);
				    throw;
			    }
		    }
	    }

	    public EngineParams EngineParams
	    {
		    set
		    {
			    if (value != null)
			    {
				    engineParams = value;
			    }
		    }
	    }

	    public IEngineAssistant EngineAssistant
	    {
		    set
		    {
			    if (value != null)
			    {
				    engineAssistant = value;
			    }
		    }
	    }

	    public EngineControlSystems EngineControlSystems
	    {
		    set
		    {
			    if (value != null)
			    {
				    engineControlSystems = value;
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

            if (drivetrain == null)
            {
                drivetrain = GetComponent<Drivetrain>();
            }

            if (axles == null)
            {
                axles = GetComponent<Axles>();
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

	        if (engineAssistant == null)
	        {
		        engineAssistant = new EngineAssistant();
	        }
	        
            if (carDynamics == null)
            {
                carDynamics = GetComponent<CarDynamics>();
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
			    if (moveDirection.y < 0 && engineParams.OwnerVelocity <= 0.5f)
			    {
				    if (drivetrain.gear != drivetrain.firstReverse)
				    {
					    drivetrain.Shift(drivetrain.firstReverse);
				    }
			    }

			    if (moveDirection.y > 0 && engineParams.OwnerVelocity <= 0.5f)
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
		    engineParams.MaxThrottle = 1;
		    oldSteering = steering;
		    
            
		    engineParams.OwnerVelocity = carDynamics.velo;
		    float ownerVelocityInKmh = engineParams.OwnerVelocity * 3.6f;// car's speed in kmh
		    bool onGround = drivetrain.OnGround();

		    if (smoothInput)
		    {
			    engineAssistant.SmoothSteer(moveDirection.x,engineParams.OwnerVelocity, engineParams, ref steering);
			    
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
				    ref steering);
		    }
		    else
		    {
			    engineAssistant.SteerTimer = 0;
		    }

		    throttle = engineControlSystems.CheckControlSystem(throttle, gameObject, brake, allWheels);

		    if (drivetrain.gearRatios[drivetrain.gear] <= 0)
		    {
				engineParams.MaxThrottle = engineParams.MaxThrottleInReverse;
		    }

		    if (drivetrain.revLimiterTriggered)
		    {
			    throttle = 0;
		    }
		    else if (drivetrain.revLimiterReleased)
		    {
			    throttle = moveDirection.y;
		    }
		    else
		    {
			    throttle = Mathf.Clamp(throttle, drivetrain.idlethrottle, engineParams.MaxThrottle);
		    }

		    brake = Mathf.Clamp01(brake);
		    steering = Mathf.Clamp(steering, -1, 1);
		    deltaSteering = steering - oldSteering;

		    // Apply inputs
		    foreach (Wheel w in allWheels)
		    {
			    if (!(engineControlSystems.Abs && ownerVelocityInKmh > engineControlSystems.MinAbsVelocity && moveDirection.y < 0))
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