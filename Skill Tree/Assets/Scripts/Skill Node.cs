using UnityEngine;

[System.Serializable]
public class SkillNode
{
    public string Name = null;
    public string Description = null;
    public Vector3 Position = Vector3.zero;

    public string StatAugmented = null;
    public string MathDone = null;
    public int Value = 0;

    public int NodeNumber = 0;
    public int PreviousNode = 0;

    public int ResourceCost = 0;
    public int[] RequiredNodes = null;
    public string RequiredStatName = null;
    public int RequiredStatValue = 0;
    public int RequiredSpentResource = 0;

    public bool IsAcquired = false;

}
