using Unity.Properties;
using UnityEngine;
using System.Reflection;
using System;

public class PlayerInteractionHandler : MonoBehaviour
{
    [SerializeField]
    SkillTree skillTree = null;

    //Event for when isAcquired changed to true

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
        foreach (SkillNode node in skillTree.nodePoints)
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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
