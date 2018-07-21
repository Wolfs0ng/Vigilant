using System;
using UnityEngine;

namespace Vigilant.Helpers
{ 
    /// <summary>
    /// 
    /// </summary>
    public static class MathUtils
    {
        #region Fields
        
        public static float Atan(this float x)
        {
            float absX = Mathf.Abs(x);
            return x * (1f + 1.1f * absX) / (1f + 2 * (1.6f * absX + 1.1f * x * x) / Mathf.PI);
        }

        public static double Atan(this double x)
        {
            double absX = Math.Abs(x);
            return x * (1f + 1.1f * absX) / (1f + 2 * (1.6f * absX + 1.1f * x * x) / Mathf.PI);
        }

        #endregion

        #region Properties
        #endregion

        #region Unity Events
        #endregion

        #region Methods
        #endregion
    }
}