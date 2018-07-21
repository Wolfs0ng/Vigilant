using UnityEngine;
using UnityEngine.UI;

namespace Vigilant.Helpers 
{ 
    /// <summary>
    /// 
    /// </summary>
    public sealed class FPSCounter : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Text textFrame;

        private float updateInterval = 1f;
        private float accumulate; // FPS accumulated over the interval
        private float timeLeft; // Left time for current interval
        private float fps = 15f; // Current FPS
        private float lastTimeSample;
        
        private int frames; // Frames drawn over the interval
        private int gotIntervals;

        #endregion

        #region Properties

        public float GetFps()
        {
            return fps;
        }

        public bool HasFps()
        {
            return gotIntervals > 2;
        }
        
        #endregion

        #region Unity Events

        private void Start()
        {
            timeLeft = updateInterval;
            lastTimeSample = Time.realtimeSinceStartup;
        }

        private void Update()
        {
            frames++;
            float newSample = Time.realtimeSinceStartup;
            float deltaTime = newSample - lastTimeSample;
            lastTimeSample = newSample;

            timeLeft -= deltaTime;
            accumulate += 1f / deltaTime;

            if (!(timeLeft <= 0f))
            {
                return;
            }
            
            // Interval ended - update GUI text and start new interval
            // display two fractional digits (f2 format)
            fps = accumulate / frames;
            timeLeft = updateInterval;
            accumulate = 0f;
            frames = 0;
            gotIntervals++;

            textFrame.text = fps.ToString();
        }

        #endregion

        #region Methods

        #endregion
    }
}