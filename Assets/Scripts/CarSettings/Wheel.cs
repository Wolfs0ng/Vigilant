using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vigilant.Enums;
using Vigilant.Helpers;

namespace Vigilant.CarSettings
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class Wheel : MonoBehaviour
	{
		
		//Need split all this field/properties to multiple wheel helper classes. Like 4 or 5 O_o ? mb even more.
		
		#region Fields

		private WheelPos wheelPos;
		private PhysicMaterial physicMaterial;
		private SurfaceType surfaceType;

		// Graphical wheel representation (to be rotated accordingly)
		[SerializeField] private GameObject model;
		[SerializeField] private GameObject caliperModel;

		private LineRenderer suspensionLineRenderer;

		private bool showForces;
		private bool isPowered;

		private float relaxLong;
		private float relaxLat;
		private float tireDeflection; // vertical deflection of the tire
		private float lateralTireDeflection; // lateral deflection of the tire

		[SerializeField] private float lateralTireStiffness; // lateral stiffness of the tire. 
		// The higher is the lateral stiffness,
		// the lower is the lateral deflection of
		// the tire in curves (tire is more stable).
		// This influence force feedback too.

		private float longitudinalTireStiffness;
		private float overTurningMoment;

		private float pressure = 200; // wheel inflation pressure in kPa

		private float optimalPressure = 200; // tire pressure where grip is maximum.
		// If pressure is lower, grip is decreased proportionally.
		// If pressure is higher, grip is always maximum.
		// (in real life sport tires optimal
		// pressure is 200 kPa, truck tires 800 kPa)

		[SerializeField] private bool tirePuncture;

		private float pressureFactor;

		[SerializeField] private bool rimScraping;

		private float verticalTireStiffness;
		private float tireDampingRate;

		[SerializeField] private bool tirePressureEnabled = true;

		private float cos;
		private float differentialSlipRatio;
		private float tanSlipAngle;
		private float deltaRatio1;
		private float deltaAngle1;
		private float localScale;

		private float lateralSlipVelo;
		private float longitunalSlipVelo;
		private List<float> slipRatio_hat;
		private List<float> slipAngle_hat;

		private Vector3 force;
		private Vector3 totalForce;

		[SerializeField] private Vector3 pos;

		private Vector3 modelPosition;

		[SerializeField] private float fx;
		[SerializeField] private float maxFx;
		[SerializeField] private float fy;
		[SerializeField] private float maxFy;
		[SerializeField] private float mz;
		[SerializeField] private float maxMz;

		private Vector3 latForce;
		private Vector3 longForce;


		private int layerMask;
		private int increment;

		[SerializeField] private bool onGroundDown;
		
		private RaycastHit hitDown;
		private float originalMass;

		// Wheel mass in kg
		[SerializeField] private float mass = 50;

		// Wheel radius in meters
		[SerializeField] private float radius = 0.34f;

		// Rim radius in meters
		[SerializeField] private float rimRadius;

		private float sidewallHeight = 0;

		// Wheel width in meters
		[SerializeField] private float width = 0.2f;

		// Wheel suspension travel in meters (we use suspensionTravel as it
		// was the suspension length, so min_len=0 and max_len=suspensionTravel)
		private float suspensionTravel = 0.2f;

		// suspensionRate is the spring rate (in newtons per meter)
		// of the suspension weighted respect the position of the wheel
		private float suspensionRate = 20000;
		private float bumpRate = 4000;
		private float reboundRate = 4000;
		private float fastBumpFactor = 0.3f;
		private float fastReboundFactor = 0.3f;

		// Wheel rotational inertia (moment of inertia) in kg*m^2
		// (for a wheel the moment of inertia is 1/2M*r^2  so 10* 0.37^2 = 10*0.1369 = 1.369
		[SerializeField] private float rotationalInertia; //1.8f;
		private float totalRotationalInertia;
		private float brakeFrictionTorque = 1500; // Maximal braking torque (in Nm)
		private float handbrakeFrictionTorque = 0; // Maximal handbrake torque (in Nm)

		// Rolling Resistance (friction torque in Nm) 
		private float rollingResistanceTorque;
		
		private float rollingFrictionCoefficient = 0.018f; //tarmac	

		//total friction torque (brake, handbrake, rollingResistanceTorque, wheelFrictionTorque)
		private float totalFrictionTorque;
		private float totalFrictionTorqueImpulse;

		private float frictionAngularDelta;

		// Coefficient of static friction of rubber on
		// asphalt/concrete  http://en.wikipedia.org/wiki/Friction#Static_friction
		private float staticFrictionCoefficient = 1f;
		
		private bool isDirty = false;

		//friction coefficient used to simulate different surfaces (asphalt,grass, ecc.)
		private float gripMaterial = 1f;
		private float sidewaysGripFactor = 1;
		private float forwardGripFactor = 1;
		private float gripPressure = 1;
		private float gripSlip;
		private float gripVelo;

		private float m_gripMaterial;

		private float maxSteeringAngle = 33f; // Maximal steering angle (in degrees)

		//Pacejka coefficients
		private List<float> lateralCoefficients = new List<float>
		{
			1.5f,
			-40f,
			1600f,
			2600f,
			8.7f,
			0.014f,
			-0.24f,
			1.0f,
			-0.03f,
			-0.0013f,
			-0.06f,
			-8.5f,
			-0.29f,
			17.8f,
			-2.4f
		}; //lateral values

		private List<float> longitudinalCoefficients = new List<float>
		{
			1.5f,
			-80f,
			1950f,
			23.3f,
			390f,
			0.05f,
			0f,
			0.055f,
			-0.024f,
			0.014f,
			0.26f
		}; // longitudinal values

		private List<float> aligningValues = new List<float>
		{
			2.2f,
			-3.9f,
			-3.9f,
			-1.26f,
			-8.2f,
			0.025f,
			0f,
			0.044f,
			-0.58f,
			0.18f,
			0.043f,
			0.048f,
			-0.0035f,
			-0.18f,
			0.14f,
			-1.029f,
			0.27f,
			-1.1f
		}; // aligning values

		private CarDynamics cardynamics;
		private Drivetrain drivetrain;
		private Axles axles;
		private Transform myTransform;

		//friction torque relative to the wheel, its used to stop the wheel when the car its upside down and the wheel its rotating
		private float wheelFrictionTorque = 0.5f;
		
		private float lockingTorqueImpulse; // locking torque impulse applied to this wheel
		private float roadTorqueImpulse; // road torque impulse applied to this wheel
		private float drivetrainInertia; // drivetrain rotationalInertia as currently connected to this wheel

		// brake input
		[SerializeField] private float brake;

		// handbrake input
		[SerializeField] private float handbrake;

		// steering input
		[SerializeField] private float steering;
		
		private float deltaSteering;
		private float antiRollBarForce; // suspension force externally applied by anti-roll bars

		private float wheelImpulse;
		private float angularVelocity;
		
		private float oldAngularVelocity;
		
		private float slipVelo;
		
		private float slipVeloSmoke;
		private float slipVeloThreshold = 4;

		private float slipRatio;
		private float slipAngle;
		private float longitudinalSlip;
		private float lateralSlip;
		private float idealSlipRatio;
		private float idealSlipAngle;
		
		private float slipSkidAmount;
		private float slipSmokeAmount;

		[SerializeField] private float compression;
		
		private float overTravel;

		// state
		private float wheelTireVelo;
		private float wheelRoadVelo;
		private float absRoadVelo;
		private float dampAbsRoadVelo;
		private float wheelRoadVeloLat;
		private Vector3 wheelVelo;
		private Vector3 groundNormal;
		
		private float rotation;

		[SerializeField] private float normalForce;
		
		private float suspensionForce;
		private float tireForce;
		private float bumpStopForce;
		private float springForce;
		private float criticalDamping;
		private float radialDampingRatio;

		private float normalVelocity;
		private float oldNormalVelocity;
		private float deltaVelocity;
		private float accel;
		private float nextVelocity;
		private float nextCompression;
		private float deflectionVelocity;

		private float inclination;
		
		private float camber;
		
		private Quaternion camberRotation;
		
		private float deltaCamber;
		
		private float inclination_sin;
		private float inclination_rad;

		private float roadDistance;
		private float springLength;
		private float radiusLoaded;

		private Vector3 roadForce;
		private Vector3 up, right, forwardNormal, rightNormal; //,forward;
		private Quaternion localRotation = Quaternion.identity;
		
		private int lastSkid = -1;

		// cached values
		private Rigidbody body;
		private Transform trs;
		private Transform modelTransform;
		private Transform caliperModelTransform;

		private Skidmarks skidmarks;
		private ParticleEmitter skidSmoke;
		private bool isSkidSmoke = true;
		private bool isSkidMark = true;
		private int axleWheelsLength;
		private int axlesNumber;
		private int time = 3;

		private float velo;
		private float longRelaxation;
		private float relaxationLat;

		#endregion

		#region Properties

		public WheelPos WheelPos
		{
			get { return wheelPos; }
			set { wheelPos = value; }
		}

		public PhysicMaterial PhysicMaterial
		{
			get { return physicMaterial; }
			set { physicMaterial = value; }
		}

		public SurfaceType SurfaceType
		{
			get { return surfaceType; }
			set { surfaceType = value; }
		}

		public GameObject Model
		{
			get { return model; }
			set { model = value; }
		}

		public GameObject CaliperModel
		{
			get { return caliperModel; }
			set { caliperModel = value; }
		}

		public bool ShowForces
		{
			get { return showForces; }
			set { showForces = value; }
		}

		public bool IsPowered
		{
			get { return isPowered; }
			set { isPowered = value; }
		}

		public float LateralTireStiffness
		{
			get { return lateralTireStiffness; }
			set { lateralTireStiffness = value; }
		}

		public float Pressure
		{
			get { return pressure; }
			set { pressure = value; }
		}

		public float OptimalPressure
		{
			get { return optimalPressure; }
			set { optimalPressure = value; }
		}

		public bool TirePuncture
		{
			get { return tirePuncture; }
			set { tirePuncture = value; }
		}

		public bool RimScraping
		{
			get { return rimScraping; }
			set { rimScraping = value; }
		}

		public bool TirePressureEnabled
		{
			get { return tirePressureEnabled; }
			set { tirePressureEnabled = value; }
		}

		public Vector3 Pos
		{
			get { return pos; }
			set { pos = value; }
		}

		public float Fx
		{
			get { return fx; }
			set { fx = value; }
		}

		public float MaxFx
		{
			get { return maxFx; }
			set { maxFx = value; }
		}

		public float Fy
		{
			get { return fy; }
			set { fy = value; }
		}

		public float MaxFy
		{
			get { return maxFy; }
			set { maxFy = value; }
		}

		public float Mz
		{
			get { return mz; }
			set { mz = value; }
		}

		public float MaxMz
		{
			get { return maxMz; }
			set { maxMz = value; }
		}

		public bool OnGroundDown
		{
			get { return onGroundDown; }
			set { onGroundDown = value; }
		}

		public RaycastHit HitDown
		{
			get { return hitDown; }
			set { hitDown = value; }
		}

		public float OriginalMass
		{
			get { return originalMass; }
			set { originalMass = value; }
		}

		public float Mass
		{
			get { return mass; }
			set { mass = value; }
		}

		public float Radius
		{
			get { return radius; }
			set { radius = value; }
		}

		public float RimRadius
		{
			get { return rimRadius; }
			set { rimRadius = value; }
		}

		public float Width
		{
			get { return width; }
			set { width = value; }
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

		public float RotationalInertia
		{
			get { return rotationalInertia; }
			set { rotationalInertia = value; }
		}

		public float TotalRotationalInertia
		{
			get { return totalRotationalInertia; }
			set { totalRotationalInertia = value; }
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

		public float RollingFrictionCoefficient
		{
			get { return rollingFrictionCoefficient; }
			set { rollingFrictionCoefficient = value; }
		}

		public float GripVelo
		{
			get { return gripVelo; }
			set { gripVelo = value; }
		}

		public float GripSlip
		{
			get { return gripSlip; }
			set { gripSlip = value; }
		}

		public float GripPressure
		{
			get { return gripPressure; }
			set { gripPressure = value; }
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

		public float GripMaterial
		{
			get { return gripMaterial; }
			set { gripMaterial = value; }
		}

		public float MaxSteeringAngle
		{
			get { return maxSteeringAngle; }
			set { maxSteeringAngle = value; }
		}

		public List<float> LateralCoefficients
		{
			get { return lateralCoefficients; }
			set { lateralCoefficients = value; }
		}

		public List<float> LongitudinalCoefficients
		{
			get { return longitudinalCoefficients; }
			set { longitudinalCoefficients = value; }
		}

		public List<float> AligningValues
		{
			get { return aligningValues; }
			set { aligningValues = value; }
		}

		public float LockingTorqueImpulse
		{
			get { return lockingTorqueImpulse; }
			set { lockingTorqueImpulse = value; }
		}

		public float RoadTorqueImpulse
		{
			get { return roadTorqueImpulse; }
			set { roadTorqueImpulse = value; }
		}

		public float DrivetrainInertia
		{
			get { return drivetrainInertia; }
			set { drivetrainInertia = value; }
		}

		public float Brake
		{
			get { return brake; }
			set { brake = value; }
		}

		public float Handbrake
		{
			get { return handbrake; }
			set { handbrake = value; }
		}

		public float Steering
		{
			get { return steering; }
			set { steering = value; }
		}

		public float DeltaSteering
		{
			get { return deltaSteering; }
			set { deltaSteering = value; }
		}

		public float AntiRollBarForce
		{
			get { return antiRollBarForce; }
			set { antiRollBarForce = value; }
		}

		public float WheelImpulse
		{
			get { return wheelImpulse; }
			set { wheelImpulse = value; }
		}

		public float AngularVelocity
		{
			get { return angularVelocity; }
			set { angularVelocity = value; }
		}

		public float SlipVelo
		{
			get { return slipVelo; }
			set { slipVelo = value; }
		}

		public float SlipRatio
		{
			get { return slipRatio; }
			set { slipRatio = value; }
		}

		public float SlipAngle
		{
			get { return slipAngle; }
			set { slipAngle = value; }
		}

		public float LongitudinalSlip
		{
			get { return longitudinalSlip; }
			set { longitudinalSlip = value; }
		}

		public float LateralSlip
		{
			get { return lateralSlip; }
			set { lateralSlip = value; }
		}

		public float IdealSlipRatio
		{
			get { return idealSlipRatio; }
			set { idealSlipRatio = value; }
		}

		public float IdealSlipAngle
		{
			get { return idealSlipAngle; }
			set { idealSlipAngle = value; }
		}

		public float Compression
		{
			get { return compression; }
			set { compression = value; }
		}

		public float WheelTireVelo
		{
			get { return wheelTireVelo; }
			set { wheelTireVelo = value; }
		}

		public float WheelRoadVelo
		{
			get { return wheelRoadVelo; }
			set { wheelRoadVelo = value; }
		}

		public float AbsRoadVelo
		{
			get { return absRoadVelo; }
			set { absRoadVelo = value; }
		}

		public float DampAbsRoadVelo
		{
			get { return dampAbsRoadVelo; }
			set { dampAbsRoadVelo = value; }
		}

		public float WheelRoadVeloLat
		{
			get { return wheelRoadVeloLat; }
			set { wheelRoadVeloLat = value; }
		}

		public Vector3 WheelVelo
		{
			get { return wheelVelo; }
			set { wheelVelo = value; }
		}

		public Vector3 GroundNormal
		{
			get { return groundNormal; }
			set { groundNormal = value; }
		}

		public float NormalForce
		{
			get { return normalForce; }
			set { normalForce = value; }
		}

		public float Camber
		{
			get { return camber; }
			set { camber = value; }
		}

		public int AxleWheelsLength
		{
			get { return axleWheelsLength; }
			set { axleWheelsLength = value; }
		}

		public int AxlesNumber
		{
			get { return axlesNumber; }
			set { axlesNumber = value; }
		}

		#endregion

		#region Unity Events

		#endregion

		#region Methods

		private float CalculateLongitudinalForce(float force, float slipRatio)
		{
			float doubleForce = force * force;
			float shapeFactor = LongitudinalCoefficients.First(); //shape factor
			float peakFactor = maxFy = (LongitudinalCoefficients[1] * doubleForce + LongitudinalCoefficients[2] * force) *
			                           m_gripMaterial * forwardGripFactor; // peak factor
			float BCD = (LongitudinalCoefficients[3] * doubleForce + LongitudinalCoefficients[4] * force) *
			            Mathf.Exp(-LongitudinalCoefficients[5] * force);
			float stiffnessFactor = BCD / (shapeFactor * peakFactor); // stiffness factor
			float curvatureFactor = LongitudinalCoefficients[6] * doubleForce + LongitudinalCoefficients[7] * force +
			                        LongitudinalCoefficients[8]; // curvature factor
			float horizontalShift = 0; // horizontal shift

			relaxLong = 0;
			slipRatio *= 100f; //covert to %

			float composite = slipRatio + horizontalShift; // composite
			float compositeStiffness = stiffnessFactor * composite;
			float longitudinalForce = peakFactor * Mathf.Sin(shapeFactor *
			                                                 (compositeStiffness - curvatureFactor *
			                                                  (compositeStiffness - compositeStiffness.Atan()))
			                                                 .Atan()); // longitudinal force

			if (pressure != 0 && tirePressureEnabled && longitudinalTireStiffness > 0)
			{
				relaxLong = BCD / longitudinalTireStiffness;
			}

			return longitudinalForce;
		}

		private float CalculateLateralForce(float force, float slipAngle, float inclination)
		{
			float doubleForce = force * force;
			float shapeFactor = LateralCoefficients.First(); //shape factor
			float peakFactor = maxFx = (LateralCoefficients[1] * doubleForce +
			                            LateralCoefficients[2] * force) *
			                           m_gripMaterial * sidewaysGripFactor; // peak factor
			float BCD = LateralCoefficients[3] * Mathf.Sin(2 * (force / LateralCoefficients[4]).Atan()) *
			            (1 - LateralCoefficients[5] * Mathf.Abs(inclination));
			float stiffnessFactor = BCD / (shapeFactor * peakFactor); // stiffness factor
			float curvatureFactor = LateralCoefficients[6] * force +
			                        LateralCoefficients[7]; // curvature factor
			float horizontalShift = 0; // horizontal shift
			float verticalShift =
				-LateralCoefficients[11] * force * Mathf.Abs(inclination * Mathf.Clamp(absRoadVelo * 100, 0, 1)) +
				LateralCoefficients[12] * force +
				LateralCoefficients[13]; // vertical shift. Pacejka89 with fix to avoid lateral sliding with two wheels on a kerb
			float composite = slipAngle + horizontalShift; // composite
			float compositeStiffness = stiffnessFactor * composite;

			float lateralForce =
				peakFactor * Mathf.Sin(shapeFactor *
				                       (compositeStiffness - curvatureFactor *
				                        (compositeStiffness -
				                         (compositeStiffness).Atan())).Atan()) + verticalShift; // lateral force
			relaxLat = 0;

			if (pressure != 0 && tirePressureEnabled && lateralTireStiffness > 0)
			{
				relaxLat = BCD / lateralTireStiffness;
			}

			return lateralForce;
		}

		// aligning force(steering wheel force feedback)
		private float CalculateAligningForce(float force, float slipAngle, float inclination)
		{
			float doubleForce = force * force;
			float shapeFactor = AligningValues.First(); //shape factor
			float peakFactor = maxMz = (AligningValues[1] * doubleForce + AligningValues[2] * force) * m_gripMaterial *
			                           sidewaysGripFactor; // peak factor
			float BCD = (AligningValues[3] * doubleForce + AligningValues[4] * force) *
			            (1 - AligningValues[6] * Mathf.Abs(inclination)) * Mathf.Exp(-AligningValues[5] * force);
			float stiffnessFactor = BCD / (shapeFactor * peakFactor); // stiffness factor
			float curvatureFactor = (AligningValues[7] * doubleForce + AligningValues[8] * force + AligningValues[9]) *
			                        (1 - AligningValues[10] * Mathf.Abs(inclination)); // curvature factor
			float horizontalShift = 0; // horizontal shift (c[11]*inclination + c[12]*Fz + c[13]; какой-то апдейт) 
			float verticalShift = (AligningValues[14] * doubleForce + AligningValues[15] * force) * inclination +
			                      AligningValues[16] * force + AligningValues[17]; // vertical shift
			float composite = slipAngle + horizontalShift; // composite
			float compositeStiffness = stiffnessFactor * composite;
			float aligningForce =
				peakFactor * Mathf.Sin(shapeFactor *
				                       (compositeStiffness + curvatureFactor * ((compositeStiffness).Atan() - compositeStiffness))
				                       .Atan()) + verticalShift;

			return aligningForce;
		}

		private Vector3 CalcForces(float force)
		{

			force *= 0.001f; //convert to kN

			//Clamp normal load
			float forcesScale = 1;
			float clampForce = 20f;

			if (force > clampForce)
			{
				forcesScale = Mathf.Clamp(force / clampForce, 1, 2);
				force = clampForce;
			}

			// get ideal slip ratio
			if (slipRatio_hat != null && slipAngle_hat != null)
			{
				LookupIdealSlipRatioIdealSlipAngle(force);
			}

			wheelTireVelo = angularVelocity * radiusLoaded;
			absRoadVelo = Mathf.Abs(wheelRoadVelo);

			longRelaxation = relaxLong * 100; //relaxation long
			relaxationLat = relaxLat * 100; //relaxation lat

			if (longRelaxation < 0.35f * cardynamics.invFixedTimeStepScalar)
			{
				longRelaxation = 0.35f * cardynamics.invFixedTimeStepScalar;
			}

			if (relaxationLat < 0.5f * cardynamics.invFixedTimeStepScalar)
			{
				relaxationLat = 0.5f * cardynamics.invFixedTimeStepScalar;
			}

			// damp sliAngle and slipRatio oscilation
			float factor = velo * 0.02f; // divided by 50
			if (factor < 1)
			{
				factor = 1;
			}

			longRelaxation *= factor;
			relaxationLat *= factor;

			float max_dampAbsRoadVelo_absRoadVelo = Mathf.Max(absRoadVelo, dampAbsRoadVelo);
			deltaRatio1 = (wheelTireVelo - wheelRoadVelo) - max_dampAbsRoadVelo_absRoadVelo * differentialSlipRatio;
			deltaRatio1 /= longRelaxation;
			differentialSlipRatio += deltaRatio1 * Time.deltaTime;
			slipRatio = differentialSlipRatio;
			slipRatio = Mathf.Clamp(slipRatio, -1.5f, 1.5f);

			float slipAngleFactor = 1;
			float absSlipAngle = Mathf.Abs(slipAngle);
			float halfIdeal = idealSlipAngle * 0.5f;
			if (absSlipAngle < halfIdeal && (wheelPos == WheelPos.REAR_RIGHT || wheelPos == WheelPos.REAR_LEFT))
			{
				slipAngleFactor = (absSlipAngle - cardynamics.factor * absSlipAngle + halfIdeal * cardynamics.factor) / halfIdeal;
			}

			deltaAngle1 = wheelRoadVeloLat - max_dampAbsRoadVelo_absRoadVelo * tanSlipAngle;
			deltaAngle1 /= relaxationLat;
			tanSlipAngle += deltaAngle1 * Time.deltaTime;
			slipAngle = -tanSlipAngle.Atan() * Mathf.Rad2Deg / slipAngleFactor;
			slipAngle = Mathf.Clamp(slipAngle, -90f, 90f);

			longitudinalSlip = slipRatio / idealSlipRatio;
			lateralSlip = slipAngle / idealSlipAngle;

//		if(Score.me != null){
//			if(Mathf.Abs(lateralSlip) > .9f){
//				Score.me.ScoreAdd();
//				time = 3;
//			}
//
//			if(this.name == "wheelRL" || this.name == "wheelRR"){
//				if(Mathf.Abs(lateralSlip) < 0.1 && time <= 0){
//					Score.me.SetScore();
//				}else if(Mathf.Abs(lateralSlip) < 0.1 && time > 0){
//					time--;
//				}
//			}
//		}

			m_gripMaterial = (gripMaterial + gripSlip + gripVelo) * gripPressure * forcesScale;

			float rho = Mathf.Max(Mathf.Sqrt(longitudinalSlip * longitudinalSlip + lateralSlip * lateralSlip),
				0.0001f); // avoid divide-by-zero		
			Fx = (longitudinalSlip / rho) * CalculateLongitudinalForce(force, rho * idealSlipRatio);
			Fy = (lateralSlip / rho) * CalculateLateralForce(force, rho * idealSlipAngle, inclination);
			if (cardynamics.enableForceFeedback && maxSteeringAngle != 0)
				Mz = CalculateAligningForce(force, slipAngle, inclination);
			else Mz = 0;

			if (float.IsInfinity(Fx) || float.IsNaN(Fx))
			{
				Fx = 0;
			}

			if (float.IsInfinity(Fy) || float.IsNaN(Fy))
			{
				Fy = 0;
			}

			return new Vector3(Fx, Fy, Mz);
		}
		
		private void LookupIdealSlipRatioIdealSlipAngle(float load)
		{
			int HAT_ITERATIONS = slipRatio_hat.Count;
			float HAT_LOAD = 0.5f;
			float nf = load;
			if (nf < HAT_LOAD)
			{
				idealSlipRatio = slipRatio_hat[0];
				idealSlipAngle = slipAngle_hat[0];
			}
			else if (nf >= HAT_LOAD*HAT_ITERATIONS)
			{
				idealSlipRatio = slipRatio_hat[HAT_ITERATIONS-1];
				idealSlipAngle = slipAngle_hat[HAT_ITERATIONS-1];
			}
			else
			{
				int lbound = (int)(nf/HAT_LOAD);
				
				lbound--;
				
				if (lbound < 0)
				{
					lbound = 0;
				}

				if (lbound >= slipRatio_hat.Count)
				{
					lbound = slipRatio_hat.Count - 1;
				}

				float blend = (nf-HAT_LOAD*(lbound+1))/HAT_LOAD;
				idealSlipRatio = slipRatio_hat[lbound]*(1.0f-blend)+slipRatio_hat[lbound+1]*blend;
				idealSlipAngle = slipAngle_hat[lbound]*(1.0f-blend)+slipAngle_hat[lbound+1]*blend;
			}
		}

		#endregion
	}
}