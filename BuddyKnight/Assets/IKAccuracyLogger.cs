using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class IKAccuracyLogger : MonoBehaviour
{
    [SerializeField] private ChainIKConstraint leftArmConstraint;
    [SerializeField] private ChainIKConstraint rightArmConstraint;
    [SerializeField] private TwoBoneIKConstraint leftLegConstraint;
    [SerializeField] private TwoBoneIKConstraint rightLegConstraint;

    private List<string> lines = new List<string>();
    private float testDuration = 10f;
    private float elapsed = 0f;
    private bool finished = false;

    private float totalLeftArm = 0f;
    private float totalRightArm = 0f;
    private float totalLeftLeg = 0f;
    private float totalRightLeg = 0f;
    private int frameCount = 0;

    void Start()
    {
        lines.Add("Frame,LeftArmError,RightArmError,LeftLegError,RightLegError");
    }

    void Update()
    {
        if (finished) return;
        elapsed += Time.deltaTime;

        float leftArm = Vector3.Distance(
            leftArmConstraint.data.tip.position,
            leftArmConstraint.data.target.position);

        float rightArm = Vector3.Distance(
            rightArmConstraint.data.tip.position,
            rightArmConstraint.data.target.position);

        float leftLeg = Vector3.Distance(
            leftLegConstraint.data.tip.position,
            leftLegConstraint.data.target.position);

        float rightLeg = Vector3.Distance(
            rightLegConstraint.data.tip.position,
            rightLegConstraint.data.target.position);

        totalLeftArm += leftArm;
        totalRightArm += rightArm;
        totalLeftLeg += leftLeg;
        totalRightLeg += rightLeg;
        frameCount++;

        lines.Add($"{Time.frameCount},{leftArm:F4},{rightArm:F4},{leftLeg:F4},{rightLeg:F4}");

        if (elapsed >= testDuration)
        {
            finished = true;
            SaveResults();
        }
    }

    void SaveResults()
    {
        lines.Add("");
        lines.Add($"AverageLeftArm,{totalLeftArm / frameCount:F4}");
        lines.Add($"AverageRightArm,{totalRightArm / frameCount:F4}");
        lines.Add($"AverageLeftLeg,{totalLeftLeg / frameCount:F4}");
        lines.Add($"AverageRightLeg,{totalRightLeg / frameCount:F4}");

        File.WriteAllLines("C:/Users/Vartotojas/UnityProjects6/YourAIBuddy/BuddyKnight/ik_accuracy.csv", lines);
        Debug.Log("IK accuracy test complete.");
    }
}