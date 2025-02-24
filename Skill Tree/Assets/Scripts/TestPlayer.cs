using UnityEngine;
public class TestPlayer : MonoBehaviour
{
    public float Strength
        { get; private set; }
    public float Dodge = 5.0f;
    public float Agility = 1;
    public int Damage = 1;
    public float Armor = 100f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Strength = 5f;

        //Debug.Log(Dodge);
        //interactionHandler.ApplySkillEffects(this.gameObject, "Dodge", "+", 5f);
        //Debug.Log(Dodge);
        //interactionHandler.ApplySkillEffects(this.gameObject, "Dodge", "X", 5f);
        //Debug.Log(Dodge);
        //Dodge = 10;
        //interactionHandler.CalculateAllSkillEffects(this.gameObject, "Dodge");
        //Debug.Log(Dodge);

        //Proof of concept
        Debug.Log($"Agility: {Agility}");
        Debug.Log($"Dodge: {Dodge}");

        //Try to buy node 4
        Debug.Log($"Want to buy Skill: {SkillTree.Instance.nodePoints[4].Name}");
        Debug.Log($"Skill Description: {SkillTree.Instance.nodePoints[4].Description}");
        PlayerInteractionHandler.Instance.PurchaseSkillNode(4, this.gameObject);

        //Try to buy node 2 (succeed)
        PlayerInteractionHandler.Instance.AcquireSkillResource(1);
        Debug.Log($"Want to buy Skill: {SkillTree.Instance.nodePoints[2].Name}");
        Debug.Log($"Skill Description: {SkillTree.Instance.nodePoints[2].Description}");
        PlayerInteractionHandler.Instance.PurchaseSkillNode(2, this.gameObject);
        Debug.Log($"New Agility: {Agility}");

        //Try to buy node 4 now (succeed)
        PlayerInteractionHandler.Instance.AcquireSkillResource(2);
        Debug.Log($"Want to buy Skill: {SkillTree.Instance.nodePoints[4].Name}");
        Debug.Log($"Skill Description: {SkillTree.Instance.nodePoints[4].Description}");

        PlayerInteractionHandler.Instance.PurchaseSkillNode(4, this.gameObject);
        Debug.Log($"Agility: {Agility}");
        Debug.Log($"New Dodge: {Dodge}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
