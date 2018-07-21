using UnityEngine;
using Vigilant.Helpers;
using Vigilant.Interface.Managers;
using Vigilant.Managers.InputManagers;
using Zenject;

namespace Vigilant.Controllers.Camera
{
	public enum CameraModes
	{
		SmoothLookAt,
		MouseOrbit,
		FixedTo,
		Map
	};

	/// <summary>
	/// 
	/// </summary>
	public class CameraController : MonoBehaviour
	{
		#region Fields
		
		[SerializeField] public CameraModes MyCameraMode;

		[SerializeField] public float RotationDamping = 3f;
		[SerializeField] public float HeightDamping = 2f;
		[SerializeField] public float Distance = 10f;
		[SerializeField] public float Height = 2.5f;
		[SerializeField] public float YawAngle;
		[SerializeField] public float PitchAngle = -2.5f;

		[Inject] private IInputManager inputManager;
		
		//Mouse Orbit params
		private float xSpeed = 10f;
		private float ySpeed = 10f;
		private float yMinLimit = -20f;
		private float yMaxLimit = 80f;
		private float mouseDamping = 10;
		private float currentYawRotationAngle;
		private float wantedYawRotationAngle;
		private float currentHeight;
		private float wantedHeight;
		private float deltaPitchAngle;
		private float centrifugalAccel;

		private Transform targetTransform;
		private Rigidbody myTargetRigidbody;

		private Vector3 deltaMovement;
		private Vector3 oldVelocity;
		private Vector3 deltaVelocity;
		private Vector3 velocity;
		private Vector3 acceleration;

		private Quaternion rotationQuaternion;

		private CarDynamics carDynamics;

		#endregion

		#region Properties

		public bool DampFixedCamera { private get; set; }
		public bool MouseOrbitFixedCamera { private get; set; }
		
		public float MouseOrbitX { private get; set; }
		public float MouseOrbitY { private get; set; }
		public float DistanceMin { private get; set; }
		public float DistanceMax { private get; set; }

		public Transform MyTransform { private get; set; }

		public Transform TargetTransform
		{
			get { return targetTransform; }
			set { targetTransform = value; }
		}

		#endregion

		#region Unity Events

		private void LateUpdate()
		{
			if (!TargetTransform)
			{
				return;
			}

			switch (MyCameraMode)
			{
				case CameraModes.MouseOrbit:
					CameraMouseOrbitMode();
					break;
				case CameraModes.Map:
					CameraMapMode();
					break;
				case CameraModes.SmoothLookAt:
					CameraSmoothLookMode();
					break;
				case CameraModes.FixedTo:
					CameraFixedMode();
					break;
				default:
					break;
			}
		}

		private void FixedUpdate()
		{
			if (!DampFixedCamera)
			{
				return;
			}

			if (myTargetRigidbody != null)
			{
				oldVelocity = velocity;
				velocity = TargetTransform.InverseTransformDirection(myTargetRigidbody.velocity);
				deltaVelocity = velocity - oldVelocity;
			}
			else
			{
				//нужны проверки и проработка
				velocity = Vector3.one;
				oldVelocity = Vector3.one;
				deltaVelocity = Vector3.one;
			}

			if (carDynamics != null)
			{
				centrifugalAccel = carDynamics.GetCentrifugalAccel();//центростремительное ускорение
			}
			
			//Шайтанство не иначе
			//TODO:сделать адекватное вычисление перемещения камеры либо разобрать текущее и убрать магические числа
			acceleration = deltaVelocity / Time.deltaTime /* <- формула ускорения */ + centrifugalAccel * Vector3.right;

			//if (Mathf.Abs(accel.x)>100) accel.x=100*Mathf.Sign(accel.x);
			//if (Mathf.Abs(accel.y)>100) accel.y=100*Mathf.Sign(accel.y);
			//if (Mathf.Abs(accel.z)>100) accel.z=100*Mathf.Sign(accel.z);

			deltaMovement = acceleration * Time.deltaTime * Time.deltaTime * 5;
			
			deltaMovement.x = Mathf.Clamp(deltaMovement.x, -0.01f, 0.01f);
			deltaMovement.y = Mathf.Clamp(deltaMovement.y, -0.1f, 0.1f);
			deltaMovement.z = Mathf.Clamp(deltaMovement.z, -0.2f, 0.2f);

			deltaPitchAngle = deltaMovement.y * 20 - deltaMovement.z * 20;
			deltaPitchAngle = Mathf.Clamp(deltaPitchAngle, -5f, 5f);
		}

		#endregion

		#region Methods
		
		public void EnableCamera()
		{
			MyTransform = transform;

			if (!TargetTransform)
			{
				return;
			}

			carDynamics = TargetTransform.GetComponent<CarDynamics>();
			myTargetRigidbody = TargetTransform.GetComponent<Rigidbody>();

			inputManager.WeaponTowerDirectionChanged += OnTowerDirectionChanged;
		}

		public void DisableCamera()
		{
			TargetTransform = null;
		}
		
		/// <summary>
		/// Событие изменения орбиты камеры.
		/// </summary>
		/// <param name="direction"> new direction </param>
		private void OnTowerDirectionChanged(Vector2 direction)
		{
			//Нужно менять MouseOrbit(X,Y) отсюда,
			//но с клавиауры оно должно нарастать от времени удержания клавиши
		}

		/// <summary>
		/// Camera mode FixedTo.
		/// Камера прикрепленна к цели(машина) камеры и вращается вокруг цели. 
		/// </summary>
		private void CameraFixedMode()
		{
			if (MouseOrbitFixedCamera)
			{
				MouseOrbitX += Input.GetAxis("Mouse X") * xSpeed;
				MouseOrbitY -= Input.GetAxis("Mouse Y") * ySpeed;
				MouseOrbitY = ClampAngle(MouseOrbitY, yMinLimit, yMaxLimit);
			}
			else
			{
				MouseOrbitX = MouseOrbitY = 0;
			}

			rotationQuaternion = Quaternion.Slerp(MyTransform.rotation,
				Quaternion.Euler(MouseOrbitY + TargetTransform.eulerAngles.x + PitchAngle + deltaPitchAngle, MouseOrbitX + TargetTransform.eulerAngles.y + YawAngle,
					TargetTransform.eulerAngles.z), Time.time);
			MyTransform.rotation = rotationQuaternion;

			if (DampFixedCamera)
			{
				Vector3 nextPosition = MyTransform.position.SetVector3(null, TargetTransform.position.y + Height) -
				                       MyTransform.forward * Distance -
				                       (deltaMovement.z * TargetTransform.forward + deltaMovement.y * TargetTransform.up +
				                        deltaMovement.x * TargetTransform.right);
				MyTransform.position = nextPosition;
			}
			else
			{	
				MyTransform.eulerAngles.SetVector3(TargetTransform.eulerAngles.x + PitchAngle, TargetTransform.eulerAngles.y + YawAngle);
				MyTransform.position = new Vector3(TargetTransform.position.x, TargetTransform.position.y + Height, TargetTransform.position.z) -
				                       MyTransform.forward * Distance;
			}
		}

		/// <summary>
		/// Camera mode SmoothLookAt.
		/// Плавное следование за целью (машина) камеры. 
		/// </summary>
		private void CameraSmoothLookMode()
		{
			// Calculate the current yaw rotation angles
			wantedYawRotationAngle = TargetTransform.eulerAngles.y + YawAngle;
			currentYawRotationAngle = MyTransform.eulerAngles.y;

			wantedHeight = TargetTransform.position.y + Height;
			currentHeight = MyTransform.position.y;

			// Damp the rotation around the y-axis
			currentYawRotationAngle =
				Mathf.LerpAngle(currentYawRotationAngle, wantedYawRotationAngle, RotationDamping * Time.deltaTime);

			// Damp the height
			currentHeight = Mathf.Lerp(currentHeight, wantedHeight, HeightDamping * Time.deltaTime);

			// Convert the angle into a rotation
			Quaternion currentYawRotation = Quaternion.Euler(0, currentYawRotationAngle, 0);

			// Set the position of the camera on the x-z plane to distance meters behind the target
			MyTransform.position = TargetTransform.position;
			MyTransform.position += currentYawRotation * Vector3.back * Distance;

			// Set the height of the camera
			MyTransform.position = new Vector3(MyTransform.position.x, currentHeight, MyTransform.position.z);
//			MyTransform.position.SetVector3(null, currentHeight); //TODO: find why didn't work ??!@?!@?#!
//			Vector3 lookAtTargetVector =
//				new Vector3(MyTarget.position.x, MyTarget.position.y + Height + PitchAngle, MyTarget.position.z);
			
			// Always look at the target
			MyTransform.LookAt(new Vector3(TargetTransform.position.x, TargetTransform.position.y + Height + PitchAngle, TargetTransform.position.z));
		}

		/// <summary>
		/// Camera mode Map Camera.
		/// Заделка для миникартовой камеры.
		/// </summary>
		private void CameraMapMode()
		{
			MyTransform.position = new Vector3(TargetTransform.position.x, MyTransform.position.y, TargetTransform.position.z);
			MyTransform.eulerAngles =
				new Vector3(MyTransform.eulerAngles.x, TargetTransform.eulerAngles.y, MyTransform.eulerAngles.z);
		}
		
		/// <summary>
		/// Camera mode Mouse Orbit.
		/// Полный облет цели(машины) камерой.
		/// </summary>
		private void CameraMouseOrbitMode()
		{
			MouseOrbitX += Input.GetAxis("Mouse X") * xSpeed;
			MouseOrbitY -= Input.GetAxis("Mouse Y") * ySpeed;

			MouseOrbitY = ClampAngle(MouseOrbitY, yMinLimit, yMaxLimit);

			rotationQuaternion =
				Quaternion.Slerp(MyTransform.rotation, Quaternion.Euler(MouseOrbitY, MouseOrbitX, 0), Time.deltaTime * mouseDamping);

			Distance -= Input.GetAxis("Mouse ScrollWheel") * 5;
			Distance = Mathf.Clamp(Distance, DistanceMin, DistanceMax);

			Vector3 negDistance = new Vector3(0.0f, 0.0f, -Distance);
			Vector3 position = rotationQuaternion * negDistance + TargetTransform.position;

			MyTransform.rotation = rotationQuaternion;
			MyTransform.position = position;
		}

		/// <summary>
		/// Поправка угла в зависимости от стороны вращения.
		/// </summary>
		/// <param name="yawAngle"> угол вращения </param>
		/// <param name="min"> минимальное значение угла </param>
		/// <param name="max"> максимальное значение угла </param>
		/// <returns></returns>
		private static float ClampAngle(float yawAngle, float min, float max)
		{
			if (yawAngle < -360F)
			{
				yawAngle += 360F;
			}
			else if (yawAngle > 360F)
			{
				yawAngle -= 360F;
			}

			return Mathf.Clamp(yawAngle, min, max);
		}

		#endregion
	}
}