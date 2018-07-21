using UnityEngine;

public class SteeringWheel : MonoBehaviour {
	public float maxSteeringAngle=270;
	public bool rotateAroundY=false;
	public bool invertRotation=false;
	Transform myTransform;
	[HideInInspector]
	public CarController carController;
	float z;
	int sign=1;
	
	void Start () {
		myTransform=transform;
		if (rotateAroundY==true) z=myTransform.localEulerAngles.y;
		else z=myTransform.localEulerAngles.z;
		if (invertRotation==true) sign=-1;
	}
	
	void Update () {
		if (carController) {
			sign=1;
			if (invertRotation==true) sign=-1;
			if (rotateAroundY==true) myTransform.localEulerAngles=new Vector3 (myTransform.localEulerAngles.x, z + sign*carController.steering*maxSteeringAngle, myTransform.localEulerAngles.z);
			else myTransform.localEulerAngles=new Vector3 (myTransform.localEulerAngles.x, myTransform.localEulerAngles.y, z + sign*carController.steering*maxSteeringAngle);
		}
	}
}
