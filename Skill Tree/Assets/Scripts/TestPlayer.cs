using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public float Strength
        { get; private set; }
    public float Dodge = 5.0f;
    public float Health = 10;
    public int Damage = 1;
    public float Armor = 100f;
    private float baseArmor = 100f;
    private float mana = 11;

    PlayerInteractionHandler interactionHandler;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interactionHandler = GetComponent<PlayerInteractionHandler>();
        Debug.Log(Dodge);
        interactionHandler.ApplySkillEffects(this.gameObject, "Dodge", "+", 5f);
        Debug.Log(Dodge);
        interactionHandler.ApplySkillEffects(this.gameObject, "Dodge", "X", 5f);
        Debug.Log(Dodge);
        Dodge = 10;
        interactionHandler.CalculateAllSkillEffects(this.gameObject, "Dodge");
        Debug.Log(Dodge);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
