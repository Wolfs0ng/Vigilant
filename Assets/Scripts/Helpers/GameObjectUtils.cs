using UnityEngine;

namespace Vigilant.Helpers 
{ 
    /// <summary>
    /// 
    /// </summary>
    public static class GameObjectUtils
    {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Methods

        public static Vector3 SetVector3(this Vector3 incomingVector, float? x = null, float? y = null, float? z = null)
        {
            Vector3 outComingVector = new Vector3(x ?? incomingVector.x, y ?? incomingVector.y,
                z ?? incomingVector.z);
            return outComingVector;
        }

        public static Vector3 SetX(this Vector3 incomingVector, float x)
        {
            Vector3 outComingVector = new Vector3(x, incomingVector.y, incomingVector.z);
            return outComingVector;
        }
        
        public static Vector3 SetY(this Vector3 incomingVector, float y)
        {
            Vector3 outComingVector = new Vector3(incomingVector.x, y, incomingVector.z);
            return outComingVector;
        }
        
        public static Vector3 SetZ(this Vector3 incomingVector, float z)
        {
            Vector3 outComingVector = new Vector3(incomingVector.x, incomingVector.y, z);
            return outComingVector;
        }
        
        #endregion
    }
}