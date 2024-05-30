using UnityEngine;

namespace SteveRogers
{
    /// <summary>
    /// ~  Fps counter for unity  ~
    /// Brief : Calculate the FPS and display it on the screen 
    /// HowTo : Create empty object at initial scene and attach this script!!!
    /// </summary>
    public class FPSCounter : SingletonPersistent<FPSCounter>
    {
        private Rect boxRect = new Rect(50, 0, 300, 30);
        private Rect rectButton = new Rect(0, 0, 40, 25);

        GUIContent guiButton = new GUIContent("FPS");
        bool showStats = true;

        // for fps calculation.
        private int frameCount;
        private float elapsedTime;
        private double frameRate;
        private double minFrameRate = 100;
        private double maxFrameRate = 0;
        private double averageFramRate = 0;
        private long count = 0;
        private double sum = 0;

        public double FPS { get { return frameRate; } }

        /// <summary>
        /// Monitor changes in resolution and calcurate FPS
        /// </summary>
        private void Update()
        {
            // FPS calculation
            frameCount++;
            elapsedTime += Time.deltaTime;

            if (elapsedTime > 0.5f)
            {
                frameRate = System.Math.Round(frameCount / elapsedTime, 1, System.MidpointRounding.AwayFromZero);
                minFrameRate = System.Math.Min(frameRate, minFrameRate);
                maxFrameRate = System.Math.Max(frameRate, maxFrameRate);

                sum += frameRate;
                count++;
                averageFramRate = System.Math.Round(sum / count, 1);

                frameCount = 0;
                elapsedTime = 0;
            }
        }

        /// <summary>
        /// Display FPS
        /// </summary>
        private void OnGUI()
        {
            if (GUI.Button(rectButton, guiButton))
            {
                showStats = !showStats;
            }

            if (showStats)
            {
                GUI.Box(boxRect, string.Format(" FPS: {0}  Min: {1}  Max: {2}  Avg: {3}", frameRate, minFrameRate, maxFrameRate, averageFramRate));
            }
        }
    }
}