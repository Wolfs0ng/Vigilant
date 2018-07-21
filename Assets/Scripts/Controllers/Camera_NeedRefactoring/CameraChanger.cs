using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vigilant.Enums;
using Vigilant.Helpers;
using Vigilant.Interface.Managers;
using Vigilant.Managers.InputManagers;
using Zenject;

namespace Vigilant.Controllers.Camera
{
	/// <summary>
	/// 
	/// </summary>
	public class CameraChanger : MonoBehaviour
	{
		#region Fields
		
		//Машина игрока создается динамически, и лист должен динамически изменятся. Как ?
		[SerializeField] private List<CameraSettings> settingsList;
		[SerializeField] private int StartCameraNumber;

		[Inject] private IInputManager InputManager;
		
		private int cameraNumber;

		#endregion

		#region Properties

		#endregion

		#region Unity Events

		private void Awake()
		{
			if (InputManager != null)
			{
				InputManager.Button += OnButtonClick;
			}
		}

		private void Start()
		{
			SetCamera(settingsList[StartCameraNumber]);
		}

		#endregion

		#region Methods

		private void ChangeCamera()
		{
			cameraNumber++;
			if (cameraNumber >= settingsList.Count)
			{
				cameraNumber = 0;
			}
			SetCamera(settingsList[cameraNumber]);
		}

		public void SetCamera(CameraSettings cameraSettings)
		{
			cameraSettings.ApplySetting();
		}

		private void OnButtonClick(ButtonCode buttonCode, ButtonState buttonState)
		{
			switch (buttonCode)
			{
				case ButtonCode.CameraChange:
						ChangeCamera();
					break;
				default:
					break;
			}
		}

//		private float FindMaxBoundingSize(List<Collider> colliders)
//		{
//			collidersSize = Vector3.zero;
//
//			sizeVector = Vector3.zero;
//			if (colliders.Count != 0)
//			{
//				foreach (Collider myCollider in colliders)
//				{
//					if (myCollider.gameObject.layer != LayerMask.NameToLayer("Wheel") &&
//					    myCollider.transform.GetComponent<FuelTank>() == null)
//					{
//						collidersSize += myCollider.bounds.size;
//					}
//				}
//			}
//
//			return Mathf.Max(collidersSize.x + ExternalSize.x, collidersSize.y + ExternalSize.y,
//				collidersSize.z + ExternalSize.z);
//		}

//		public void SetCamera(int index, Transform target, bool newTarget)
//		{
//			cameraNumber = index;
//			if (newTarget)
//			{
//				cameraController.MyTarget = target;
//			}
//			cameraController.DampFixedCamera = false;
//			cameraController.MouseOrbitFixedCamera = false;
//			cameraController.DriverView = false;
//
//			if (index == 0 && target != null)
//			{
//				//external
//				cameraController.MyCameraMode = CameraModes.SmoothLookAt;
//				cameraController.TargetTransform = target;
//				cameraController.RotationDamping = 3.0f;
//				cameraController.HeightDamping = 100f;
//				List<Collider> colliders = target.gameObject.GetComponentsInChildren<Collider>().ToList();
//				cameraController.Distance = FindMaxBoundingSize(colliders) * 1.5f;
//				if (cameraController.Distance < 4)
//				{
//					cameraController.Distance = 4;
//				}
//
//				cameraController.Height = cameraController.Distance / 2;
//				cameraController.PitchAngle = -cameraController.Height / 1.5f;
//				cameraController.YawAngle = 0;
//			}
//			else if (index == 1 && target != null)
//			{
//				//onboard
//				cameraController.MyCameraMode = CameraModes.FixedTo;
//				foreach (Transform child in cameraController.MyTarget.gameObject.GetComponentsInChildren<Transform>())
//				{
//					if (child.gameObject.tag == "Fixed_Camera_Driver_View" || child.gameObject.name == "Fixed_Camera_Driver_View")
//					{
//						cameraController.TargetTransform = child;
//					}
//				}
//
//				cameraController.Distance = 0;
//				cameraController.Height = 0;
//				cameraController.PitchAngle = 0;
//				cameraController.YawAngle = 0;
//				cameraController.DampFixedCamera = true;
//				cameraController.MouseOrbitFixedCamera = true;
//				cameraController.DriverView = true;
//				cameraController.MouseOrbitX = 0;
//				cameraController.MouseOrbitY = 0;
//			}
//			else if (index == 2 && target != null)
//			{
//				//flybird
//				cameraController.MyCameraMode = CameraModes.SmoothLookAt;
//				cameraController.TargetTransform = target;
//				cameraController.RotationDamping = 3.0f;
//				cameraController.HeightDamping = 100f;
//				List<Collider> colliders = target.gameObject.GetComponentsInChildren<Collider>().ToList();
//				cameraController.Distance = FindMaxBoundingSize(colliders);
//				if (cameraController.Distance < 4)
//				{
//					cameraController.Distance = 4;
//				}
//
//				cameraController.Distance *= 2;
//				cameraController.Height = cameraController.Distance / 2;
//				cameraController.PitchAngle = -cameraController.Height / 2.5f;
//				cameraController.YawAngle = 0;
//			}
//			else if (index == 3 && target != null)
//			{
//				//look at the rear left wheel
//				cameraController.MyCameraMode = CameraModes.FixedTo;
//				foreach (Transform child in cameraController.MyTarget.gameObject.GetComponentsInChildren<Transform>())
//				{
//					if (child.gameObject.tag == "Fixed_Camera_1" || child.gameObject.name == "Fixed_Camera_1")
//					{
//						cameraController.TargetTransform = child;
//					}
//				}
//
//				cameraController.Distance = 0;
//				cameraController.Height = 0;
//				cameraController.PitchAngle = 0;
//				cameraController.YawAngle = 0;
//			}
//			else if (index == 4 && target != null)
//			{
//				//look from back
//				cameraController.MyCameraMode = CameraModes.FixedTo;
//				foreach (Transform child in cameraController.MyTarget.gameObject.GetComponentsInChildren<Transform>())
//				{
//					if (child.gameObject.tag == "Fixed_Camera_2" || child.gameObject.name == "Fixed_Camera_2")
//					{
//						cameraController.TargetTransform = child;
//					}
//				}
//
//				cameraController.Distance = 0;
//				cameraController.Height = 0;
//				cameraController.PitchAngle = 0;
//				cameraController.YawAngle = 0;
//			}
//			else if (index == 5 && target != null)
//			{
//				//lateral
//				cameraController.MyCameraMode = CameraModes.SmoothLookAt;
//				cameraController.RotationDamping = 3.0f;
//				cameraController.HeightDamping = 50f;
//				cameraController.TargetTransform = target;
//				List<Collider> colliders = target.gameObject.GetComponentsInChildren<Collider>().ToList();
//				cameraController.Distance = FindMaxBoundingSize(colliders);
//				if (cameraController.Distance < 4) cameraController.Distance = 4;
//				cameraController.Height = cameraController.Distance / 2;
//				cameraController.PitchAngle = -cameraController.Height;
//				cameraController.YawAngle = 90;
//			}
//			else if (index == 6 && target != null)
//			{
//				//mouse orbit
//				cameraController.MyCameraMode = CameraModes.MouseOrbit;
//				cameraController.TargetTransform = target;
//				if (cameraController.Distance < cameraController.DistanceMin || cameraController.Distance > cameraController.DistanceMax)
//				{
//					cameraController.Distance = 5f;
//				}
//
//				cameraController.MouseOrbitX = cameraController.MyTransform.eulerAngles.y;
//				cameraController.MouseOrbitY = cameraController.MyTransform.eulerAngles.x;
//			}
//			else if (index == 7 && target != null)
//			{
//				//Map Camera
//				cameraController.MyCameraMode = CameraModes.Map;
//				cameraController.TargetTransform = target;
//				cameraController.Distance = 0;
//				cameraController.Height = 0;
//				cameraController.PitchAngle = 0;
//				cameraController.YawAngle = 0;
//			}
//		}

		#endregion
	}
}