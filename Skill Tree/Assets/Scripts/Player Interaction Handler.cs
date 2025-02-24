using UnityEngine;
using System.Reflection;
using System;

public class PlayerInteractionHandler : MonoBehaviour
{
    public static PlayerInteractionHandler Instance { get; private set; }

    [SerializeField]

    //Can represent anything. Skill points, shards, etc. Required for purchasing skills with a non-zero price
    public float OwnedSkillResource = 0;
    [SerializeField]
    private string resourceAcquireDescription = null;
    private float totalSpentSkillResource = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Ensure only one instance exists
            return;
        }

        Instance = this;
    }


    /// <summary>
    /// Applies the math to the player given a certain skill and mathematical type. Skills modified this way must be public floats. Reflects the player's fields and properties with the skill's name
    /// </summary>
    /// <param name="player">What player or game object are you modifying</param>
    /// <param name="statAugmented">What's the name of the skill?</param>
    /// <param name="mathDone">(Optional) Does it add, subtract, multiply, or divide?</param>
    /// <param name="modifyByValue">(Optional) What value is being applied?</param>
    /// <param name="baseStat">(Optional) If the calculation is being done on a base stat, apply math to the base stat first</param>
    public void ApplySkillEffects(GameObject player, string statAugmented, string mathDone = "+", float modifyByValue = 1f, float? baseStat = null)
    {
        //Rename the value within GetComponent<T> to match your player script
        var playerComponent = player.GetComponent<TestPlayer>();
        if (playerComponent == null)
        {
            Debug.Log("Player component not found on player game object!");
            return;
        }
        Type playerType = playerComponent.GetType();
        FieldInfo field = playerType.GetField(statAugmented, BindingFlags.Public | BindingFlags.Instance);
        PropertyInfo property = playerType.GetProperty(statAugmented, BindingFlags.Public | BindingFlags.Instance);

        //If the skill name is a field
        if (field != null)
        {
            if (field.FieldType == typeof(float))
            {
                float currentValue = (float)field.GetValue(playerComponent);
                float calculatedValue = currentValue;

                switch (mathDone.ToLower())
                {
                    //All supported addition parameters
                    case "+":
                    case "add":
                    case "addition":
                    case "plus":
                        calculatedValue = baseStat.HasValue ? baseStat.Value + modifyByValue + (currentValue - baseStat.Value) : currentValue + modifyByValue;
                        break;
                    //All supported subtraction parameters
                    case "-":
                    case "subtract":
                    case "subtraction":
                    case "minus":
                        calculatedValue = baseStat.HasValue ? baseStat.Value - modifyByValue + (currentValue - baseStat.Value) : currentValue - modifyByValue;
                        break;
                    //All supported multiplication parameters
                    case "x":
                    case "*":
                    case "multiplication":
                    case "multiply":
                    case "times":
                        calculatedValue = baseStat.HasValue ? baseStat.Value * modifyByValue + (currentValue - baseStat.Value) : currentValue * modifyByValue;
                        break;
                    //All supported division parameters
                    case "/":
                    case "divide":
                    case "division":
                    case "divided by":
                        calculatedValue = baseStat.HasValue ? baseStat.Value / modifyByValue + (currentValue - baseStat.Value) : currentValue / modifyByValue;
                        break;
                    //All supported percentile parameters
                    case "%":
                    case "percentile":
                        calculatedValue = baseStat.HasValue ? baseStat.Value * (1 + modifyByValue / 100) + (currentValue - baseStat.Value) : currentValue * (1 + modifyByValue / 100);
                        break;

                    default:
                        Debug.Log("Could not apply math, unsupported parameter found!");
                        break;
                }

                if (baseStat.HasValue)
                {

                }
                field.SetValue(playerComponent, calculatedValue);
            }
        }
        //If the skill name is a property
        if (property != null)
        {
            if (property.PropertyType == typeof(float))
            {
                float currentValue = (float)property.GetValue(playerComponent);
                float calculatedValue = currentValue;

                switch (mathDone.ToLower())
                {
                    //All supported addition parameters
                    case "+":
                    case "add":
                    case "addition":
                    case "plus":
                        calculatedValue = currentValue + modifyByValue;
                        break;
                    //All supported subtraction parameters
                    case "-":
                    case "subtract":
                    case "subtraction":
                    case "minus":
                        calculatedValue = currentValue - modifyByValue;
                        break;
                    //All supported multiplication parameters
                    case "x":
                    case "*":
                    case "multiplication":
                    case "multiply":
                    case "times":
                        calculatedValue = currentValue * modifyByValue;
                        break;
                    //All supported division parameters
                    case "/":
                    case "divide":
                    case "division":
                    case "divided by":
                        calculatedValue = currentValue / modifyByValue;
                        break;
                    //All supported percentile parameters
                    case "%":
                    case "percentile":
                        calculatedValue = currentValue * (1 + modifyByValue);
                        break;

                    default:
                        Debug.Log("Could not apply math, unsupported parameter found!");
                        break;
                }


                property.SetValue(playerComponent, calculatedValue);
            }
        }
        if (field == null && property == null)
        {
            Debug.Log("Invalid field or property to modify");
        }
    }

    /// <summary>
    /// (Re)Calculates every skill effect of the acquired nodes. Useful if the base values of the stats modified are changed
    /// </summary>
    /// <param name="player">What player or game object are you modifying</param>
    /// <param name="statAugmented">(Optional) What stat is specifically being recalculated</param>
    public void CalculateAllSkillEffects(GameObject player, string statAugmented = null)
    {
        foreach (SkillNode node in SkillTree.Instance.nodePoints)
        {
            //only does the recalculations of a specific stat
            if (node.IsAcquired && statAugmented == node.StatAugmented)
            {
                ApplySkillEffects(player, node.StatAugmented, node.MathDone, node.Value);
            }
            //recalculates all stats
            else if (node.IsAcquired && statAugmented == null)
            {
                ApplySkillEffects(player, node.StatAugmented, node.MathDone, node.Value);
            }
        }
    }

    public void PurchaseSkillNode(int index, GameObject player)
    {
        float skillCost = SkillTree.Instance.nodePoints[index].ResourceCost;

        //If the node has a required prerequisite
        if (isRequiredStatSufficient(index, player, SkillTree.Instance.nodePoints[index].RequiredStatName))
        {
            //If you've spent enough resources
            if (isEnoughResourceSpent(index))
            {
                Debug.Log("You've spent enough.");
                //If you own enough resources to purchase the skill
                if (OwnedSkillResource >= skillCost)
                {
                    OwnedSkillResource -= skillCost;
                    if (OwnedSkillResource < 0)
                    {
                        OwnedSkillResource = 0;
                    }
                    totalSpentSkillResource += skillCost;
                    SkillTree.Instance.AcquireSkillNode(index);
                    ApplySkillEffects(player, SkillTree.Instance.nodePoints[index].StatAugmented, SkillTree.Instance.nodePoints[index].MathDone, SkillTree.Instance.nodePoints[index].Value);
                }
                else
                {
                    Debug.Log("You do not meet the resource requirement for this skill.");
                }
            }
            else
            {
                Debug.Log("You do not meet the prerequisite spending requirement for this skill.");
            }
        }
        else
        {
            Debug.Log("You do not meet the stat requirement for this skill.");
            return;
        }



    }

    public bool CanPurchaseSkillNode(int index, GameObject player)
    {
        float skillCost = SkillTree.Instance.nodePoints[index].ResourceCost;
        bool statPrereqMet = false;
        //If the node has a required prerequisite
        if (SkillTree.Instance.nodePoints[index].RequiredStatName != "")
        {
            statPrereqMet = isRequiredStatSufficient(index, player, SkillTree.Instance.nodePoints[index].RequiredStatName);
            //If you've spent enough resources
            if (isEnoughResourceSpent(index))
            {
                //If you own enough resources to purchase the skill
                if (OwnedSkillResource >= skillCost)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {

            return false;
        }
    }

    public void AcquireSkillResource(float numberAcquired)
    {
        Debug.Log(resourceAcquireDescription);
        OwnedSkillResource += numberAcquired;
    }

    private bool isRequiredStatSufficient(int selfIndex, GameObject player, string statName)
    {
        if (statName == "") return true;
        var playerComponent = player.GetComponent<TestPlayer>();
        if (playerComponent == null)
        {
            Debug.Log("Player component not found on player game object!");
            return false;
        }
        Type playerType = playerComponent.GetType();
        FieldInfo field = playerType.GetField(statName, BindingFlags.Public | BindingFlags.Instance);
        PropertyInfo property = playerType.GetProperty(statName, BindingFlags.Public | BindingFlags.Instance);

        if (field != null)
        {
            if (SkillTree.Instance.nodePoints[selfIndex].RequiredStatName == statName && SkillTree.Instance.nodePoints[selfIndex].RequiredStatValue < (float)field.GetValue(playerComponent))
            {
                return true;
            }
        }
        if (property != null)
        {
            if (SkillTree.Instance.nodePoints[selfIndex].RequiredStatName == statName && SkillTree.Instance.nodePoints[selfIndex].RequiredStatValue < (float)property.GetValue(playerComponent))
            {
                return true;
            }
        }

        return false;
    }

    private bool isEnoughResourceSpent(int selfIndex)
    {
        float requirement = SkillTree.Instance.nodePoints[selfIndex].RequiredSpentResource;

        return totalSpentSkillResource >= requirement;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
