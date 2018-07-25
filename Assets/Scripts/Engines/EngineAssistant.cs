using System;
using System.Collections.Generic;
using UnityEngine;
using Vigilant.Interface.Engines;

namespace Vigilant.Engines
{
	/// <summary>
	/// Contains methods to help control car. Mostly uses from engine classes.
	/// </summary>
	public sealed class EngineAssistant : IEngineAssistant
	{
		#region Fields

		private const float DampRateOffset = 40;

		private float steerTimer = 0;
		
		#endregion

		#region Properties

		public float SteerTimer
		{
			get { return steerTimer; }
			set { steerTimer = value; }
		}
		
		#endregion

		#region Methods

		/// <summary>
		/// Full steer speed calculation, with correction.
		/// Полная формула рассчета скорости поворота(при повороте ?)
		/// с проверкой и использованием коррекции скорости
		/// </summary>
		/// <param name="useFirstFormula"> Choose which formula we need. Выбираем формула расчета скорости. </param>
		/// <param name="steerReleaseTime"></param>
		/// <param name="steerTime"></param>
		/// <param name="veloSteerReleaseTime"></param>
		/// <param name="velo"></param>
		/// <param name="veloSteerTime"></param>
		/// <param name="steerInput"></param>
		/// <param name="steering"></param>
		/// <param name="steerCorrectionFactor"></param>
		/// <returns></returns>
		private float GetSteerSpeedWithCorrection(bool useFirstFormula, float steerReleaseTime, float steerTime,
			float veloSteerReleaseTime, float velo, float veloSteerTime, float steerInput, float steering,
			float steerCorrectionFactor)
		{
			float steerSpeed = SteerSpeedCalculate(useFirstFormula, steerReleaseTime, steerTime, veloSteerReleaseTime, velo,
				veloSteerTime);

			if (steering > 0)
			{
				if (steerInput < 0 && steering > 0)
				{
					steerSpeed *= GetSteerSpeedCorrection(steerInput, steering, steerCorrectionFactor);
				}
			}
			else if (steering < 0)
			{
				if (steerInput > 0 && steering < 0)
				{
					steerSpeed *= GetSteerSpeedCorrection(steerInput, steering, steerCorrectionFactor);
				}
			}

			return steerSpeed;
		}

		private float SteerSpeedCalculate(bool useFirstFormula, float steerReleaseTime, float steerTime,
			float veloSteerReleaseTime, float velo, float veloSteerTime)
		{
			return useFirstFormula
				? 1 / (steerReleaseTime + veloSteerReleaseTime * velo)
				: 1 / (steerTime + veloSteerTime * velo);
		}

		private float GetSteerSpeedCorrection(float steerInput, float steering, float steerCorrectionFactor)
		{
			return 1 + Mathf.Abs(Mathf.Abs(steering) - Mathf.Abs(steerInput)) * steerCorrectionFactor;
		}

		#region IEngineAssistant

		/// <summary>
		/// Help player with steering.
		/// </summary>
		/// <param name="averageLateralSlip"></param>
		/// <param name="oldSteering"></param>
		/// <param name="fixedTimeStepScalar"></param>
		/// <param name="steerTimer"></param>
		/// <param name="steering"></param>
		public void SteerAssistance(float averageLateralSlip, float oldSteering, float fixedTimeStepScalar,
			ref float steering)
		{
			float sign, steeringVelocity, dampRate, dampingForce, acceleration;
			float maxSteer = Mathf.Clamp(1 - Mathf.Abs(averageLateralSlip), -1, 1);

			try
			{
				if (SteerTimer <= 1)
				{
					SteerTimer += Time.deltaTime;
					maxSteer = 1 - SteerTimer + SteerTimer * maxSteer;
				}
			}
			catch (OverflowException exception)
			{
				Debug.LogError("Overflow in param values: " + exception);
				throw;
			}
			catch (Exception exception)
			{
				Debug.LogError("Some unexpected error: " + exception);
				throw;
			}

			sign = Mathf.Sign(averageLateralSlip);

			steering = maxSteer > 0
				? Mathf.Clamp(steering, -maxSteer, maxSteer)
				: Mathf.Clamp(steering, sign * maxSteer / 2, sign * maxSteer / 2);

			//damp steer correction oscillation. (~Уменьшение коррекции ударного усилия, сомнительный перевод) 
			try
			{
				steeringVelocity = steering - oldSteering;
			}
			catch (OverflowException exception)
			{
				Debug.LogError("Overflow in param values: " + exception);
				throw;
			}
			catch (Exception exception)
			{
				Debug.LogError("Some unexpected error: " + exception);
				throw;
			}

			try
			{
				dampRate = DampRateOffset * fixedTimeStepScalar;
			}
			catch (OverflowException exception)
			{
				Debug.LogError("Overflow in param values: " + exception);
				throw;
			}
			catch (Exception exception)
			{
				Debug.LogError("Some unexpected error: " + exception);
				throw;
			}

			try
			{
				dampingForce = steeringVelocity * dampRate;
			}
			catch (OverflowException exception)
			{
				Debug.LogError("Overflow in param values: " + exception);
				throw;
			}
			catch (Exception exception)
			{
				Debug.LogError("Some unexpected error: " + exception);
				throw;
			}

			acceleration = dampingForce;

			try
			{
				steering -= acceleration * Time.deltaTime;
			}
			catch (OverflowException exception)
			{
				Debug.LogError("Overflow in param values: " + exception);
				throw;
			}
			catch (Exception exception)
			{
				Debug.LogError("Some unexpected error: " + exception);
				throw;
			}
		}

		/// <summary>
		/// Calculate smooth steer.
		/// </summary>
		/// <param name="steerInput"></param>
		/// <param name="steerReleaseTime"></param>
		/// <param name="steerTime"></param>
		/// <param name="veloSteerReleaseTime"></param>
		/// <param name="velo"></param>
		/// <param name="veloSteerTime"></param>
		/// <param name="steerCorrectionFactor"></param>
		/// <param name="steering"></param>
		public void SmoothSteer(float steerInput, float velo, EngineParams engineParams,
			ref float steering)
		{
			if (steerInput < steering)
			{
				steering -= GetSteerSpeedWithCorrection(steering > 0, engineParams.SteerReleaseTime, engineParams.SteerTime,
					            engineParams.VelocitySteerReleaseTime,
					            velo, engineParams.VeloSteerTime, steerInput, steering, engineParams.SteerCorrectionFactor) *
				            Time.deltaTime;

				if (steerInput > steering)
				{
					steering = steerInput;
				}
			}
			else if (steerInput > steering)
			{
				steering += GetSteerSpeedWithCorrection(steering < 0, engineParams.SteerReleaseTime, engineParams.SteerTime,
					            engineParams.VelocitySteerReleaseTime, velo,
					            engineParams.VeloSteerTime, steerInput, steering, engineParams.SteerCorrectionFactor) * Time.deltaTime;

				if (steerInput < steering)
				{
					steering = steerInput;
				}
			}
		}

		public void SmoothThrottle(float throttleInput, EngineParams engineParams,
			bool isChangingGear, bool isAutomaticTransmission,
			ref float throttle)
		{
			if (throttleInput > 0 && (!isChangingGear || !isAutomaticTransmission))
			{
				if (throttleInput < throttle)
				{
					throttle -= Time.deltaTime / engineParams.ThrottleReleaseTime;

					if (throttleInput > throttle)
						throttle = throttleInput;
				}
				else if (throttleInput > throttle)
				{
					throttle += Time.deltaTime / engineParams.ThrottleTime;

					if (throttleInput < throttle)
						throttle = throttleInput;
				}
			}
			else
			{
				throttle -= Time.deltaTime / engineParams.ThrottleReleaseTime;
			}
		}

		public void SmoothBrakes(float brakeInput, EngineParams engineParams, ref float brake)
		{
			if (brakeInput > 0)
			{
				if (brakeInput < brake)
				{
					brake -= Time.deltaTime / engineParams.BrakesReleaseTime;

					if (brakeInput > brake)
						brake = brakeInput;
				}
				else if (brakeInput > brake)
				{
					brake += Time.deltaTime / engineParams.BrakesTime;

					if (brakeInput < brake)
						brake = brakeInput;
				}
			}
			else
			{
				brake -= Time.deltaTime / engineParams.BrakesReleaseTime;
			}
		}

		public void DoABS(float ABSThreshold, float brake, List<Wheel> allWheels)
		{
			foreach (Wheel w in allWheels)
			{
				float slip = -w.longitudinalSlip;
				if (slip >= 1 + ABSThreshold)
				{
					// wheels locked
					w.brake = 0;
				}
				else
				{
					w.brake = brake;
				}
			}
		}

		public void DoTCS(List<Wheel> poweredWheels, float TCSThreshold, float externalTCSThreshold, ref float maxThrottle)
		{
			float maxSlip = 0;
			/* 		foreach(Wheel w in drivetrain.poweredWheels){
						maxSlip+= Mathf.Max(w.longitudinalSlip,Mathf.Abs(w.lateralSlip));
					}
					maxSlip*=drivetrain.drivetrainFraction; */

			float temp = 0;
			foreach (Wheel w in poweredWheels)
			{
				temp = Mathf.Max(w.longitudinalSlip, Mathf.Abs(w.lateralSlip));
				if (temp > maxSlip)
				{
					maxSlip = temp;
				}
			}

			float threshold = maxSlip - TCSThreshold - externalTCSThreshold;
			if (threshold > 1)
			{
				maxThrottle = Mathf.Clamp(2 - threshold, 0, 1);
				if (maxThrottle > 0.9f)
				{
					maxThrottle = 1;
				}
			}
		}

		public void DoESP(Transform playerTransform, Rigidbody playeRigidbody, float velocity, Axles axles,
			float ESPStrength, Drivetrain drivetrain, ref float maxThrottle)
		{
			Vector3 driveDir = playerTransform.forward;
			Vector3 veloDir = playeRigidbody.velocity;
			veloDir -= playerTransform.up * Vector3.Dot(veloDir, playerTransform.up);
			veloDir.Normalize();

			float angle = 0;
			if (velocity > 1)
			{
				angle = -Mathf.Asin(Vector3.Dot(Vector3.Cross(driveDir, veloDir), playerTransform.up));
			}

			if (angle > 0.1f)
			{
				//turning right and fishtailing
				if (axles.frontAxle.leftWheel)
					axles.frontAxle.leftWheel.brake =
						Mathf.Clamp01(axles.frontAxle.leftWheel.brake + Mathf.Abs(angle) * ESPStrength);
				maxThrottle = Mathf.Max(maxThrottle - angle * ESPStrength, drivetrain.idlethrottle);
				//brakeKey=true;
			}
			else if (angle < -0.1f)
			{
				//turning left and fishtailing
				if (axles.frontAxle.rightWheel)
					axles.frontAxle.rightWheel.brake =
						Mathf.Clamp01(axles.frontAxle.rightWheel.brake + Mathf.Abs(angle) * ESPStrength);
				maxThrottle = Mathf.Max(maxThrottle + angle * ESPStrength, drivetrain.idlethrottle);
				//brakeKey=true;
			}
		}

		#endregion

		#endregion
	}
}