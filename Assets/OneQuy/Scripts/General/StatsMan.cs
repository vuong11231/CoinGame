using UnityEngine;
using System.Text;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_5
using UnityEngine.Profiling;
#endif

namespace SteveRogers
{
    public class StatsMan : MonoBehaviour
    {
        StringBuilder tx;

        float updateInterval = 1.0f;
        float lastInterval; // Last interval end time
        float frames = 0; // Frames over current interval

        float framesavtick = 0;
        float framesav = 0.0f;
        GUIContent gui = new GUIContent();
        GUIContent guiButton = new GUIContent("Stats");
        Rect rect = new Rect(0, 70, 400, 130);
        Rect rectButton = new Rect(0, 40, 50, 25);
        bool showStats = true;

        void Start()
        {
            lastInterval = Time.realtimeSinceStartup;
            frames = 0;
            framesav = 0;
            tx = new StringBuilder();
            tx.Capacity = 200;
        }

        void Update()
        {
            ++frames;

            var timeNow = Time.realtimeSinceStartup;

            if (timeNow > lastInterval + updateInterval)
            {
                float fps = frames / (timeNow - lastInterval);
                float ms = 1000.0f / Mathf.Max(fps, 0.00001f);

                ++framesavtick;
                framesav += fps;
                float fpsav = framesav / framesavtick;

                tx.Clear();

                tx.AppendFormat("Time : {0} ms     FPS: {1}     AvgFPS: {2}\nGPU memory : {3}    Sys Memory : {4}\n", ms, fps, fpsav, SystemInfo.graphicsMemorySize, SystemInfo.systemMemorySize)

                .AppendFormat("TotalAllocatedMemory : {0}mb\nTotalReservedMemory : {1}mb\nTotalUnusedReservedMemory : {2}mb",
                UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / 1048576,
                UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong() / 1048576,
                UnityEngine.Profiling.Profiler.GetTotalUnusedReservedMemoryLong() / 1048576
                );

#if UNITY_EDITOR
                tx.AppendFormat("\nDrawCalls : {0}\nUsed Texture Memory : {1}\nrenderedTextureCount : {2}", UnityStats.drawCalls, UnityStats.usedTextureMemorySize / 1048576, UnityStats.usedTextureCount);
#endif

                gui.text = tx.ToString();
                frames = 0;
                lastInterval = timeNow;
            }
        }

        private void OnGUI()
        {
            if (GUI.Button(rectButton, guiButton))
            {
                showStats = !showStats;
            }

            if (showStats)
            {
                GUI.Box(rect, gui);
            }
        }
    }
}