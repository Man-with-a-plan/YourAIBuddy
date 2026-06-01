using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Profiling;

public class PerformanceLogger : MonoBehaviour
{
    private List<string> lines = new List<string>();
    private float testDuration = 10f;
    private float elapsed = 0f;
    private bool finished = false;
    private int framesBelowTarget = 0;
    private float totalFps = 0f;
    private int frameCount = 0;
    private float targetFps = 60f;

    [SerializeField] private string fileName = "ragdoll_test.csv";

    void Start()
    {
        lines.Add("Frame,FPS,MemoryMB,BelowTarget");
    }

    void Update()
    {
        if (finished) return;

        elapsed += Time.deltaTime;
        float fps = 1f / Time.deltaTime;
        float memoryMB = Profiler.GetTotalAllocatedMemoryLong() / 1048576f;
        bool belowTarget = fps < targetFps;

        if (belowTarget) framesBelowTarget++;
        totalFps += fps;
        frameCount++;

        lines.Add($"{Time.frameCount},{fps:F2},{memoryMB:F2},{(belowTarget ? 1 : 0)}");

        if (elapsed >= testDuration)
        {
            finished = true;
            SaveResults(memoryMB);
        }
    }

    void SaveResults(float finalMemory)
    {
        float averageFps = totalFps / frameCount;
        lines.Add("");
        lines.Add($"AverageFPS,{averageFps:F2}");
        lines.Add($"FramesBelowTarget,{framesBelowTarget}");
        lines.Add($"FinalMemoryMB,{finalMemory:F2}");

        File.WriteAllLines(fileName, lines);
        Debug.Log($"Test complete. Saved to {fileName}");
    }
}