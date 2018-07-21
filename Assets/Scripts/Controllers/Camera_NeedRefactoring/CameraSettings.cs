using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Vigilant.Controllers.Camera
{ 
    /// <summary>
    /// 
    /// </summary>
    public sealed class CameraSettings : MonoBehaviour
    {
        #region Fields

        [Inject] private CameraController cameraController;

        [Header("Current camera settings")]
        
        [SerializeField] private CameraModes cameraMode;
        [SerializeField] private Transform cameraTarget;
        [SerializeField] private float cameraRotationDamping;
        [SerializeField] private float cameraDistance;
        [SerializeField] private float cameraHeightDamping;
        [SerializeField] private float cameraHeight;
        [SerializeField] private float cameraPitchAngle;
        [SerializeField] private float cameraYawAngle;
        [SerializeField] private float cameraDistanceMin;
        [SerializeField] private float cameraDistanceMax;
        [SerializeField] private float cameraMouseOrbitX;
        [SerializeField] private float cameraMouseOrbitY;
        
        [SerializeField] private bool dampFixedCamera;
        [SerializeField] private bool mouseOrbitFixedCamera;
        [SerializeField] private bool driverView;
        
        #endregion

        #region Properties
        #endregion

        #region Unity Events
        #endregion

        #region Methods

        public void ApplySetting()
        {
            if (cameraController == null)
            {
                return;
            }

            cameraController.DisableCamera();
            
            if (cameraTarget != null)
            {
                cameraController.TargetTransform = cameraTarget;
            }
            
            cameraController.MyCameraMode = cameraMode;

            cameraController.RotationDamping = cameraRotationDamping;
            cameraController.HeightDamping = cameraHeightDamping;
            cameraController.Distance = cameraDistance;
            cameraController.Height = cameraHeight;
            cameraController.PitchAngle = cameraPitchAngle;
            cameraController.YawAngle = cameraYawAngle;
            cameraController.DistanceMin = cameraDistanceMin;
            cameraController.DistanceMax = cameraDistanceMax;
            cameraController.MouseOrbitX = cameraMouseOrbitX;
            cameraController.MouseOrbitY = cameraMouseOrbitY;
            
            cameraController.DampFixedCamera = dampFixedCamera;
            cameraController.MouseOrbitFixedCamera = mouseOrbitFixedCamera;

            cameraController.EnableCamera();
        }

        #endregion
    }
}