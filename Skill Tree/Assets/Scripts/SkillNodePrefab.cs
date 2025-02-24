using UnityEngine;
using UnityEngine.UIElements;

public class SkillNodePrefab : MonoBehaviour
{
    public int? skillNodeIndex = null;
    public Image nodeImage = null;
    public GameObject nodePrefab = null;

    public void Awake()
    {

        //nodeImage = GetComponent<Image>();
        //nodePrefab = GetComponent<GameObject>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Initialize(SkillNode node, int index)
    {
        this.skillNodeIndex = index;
        //Draws lines between nodes and their previous ones
        if (index > 0)
        {
            Vector3[] positions = new Vector3[2];
            positions[1] = SkillTree.Instance.GetSkillNodePosition(SkillTree.Instance.nodePoints[index].PreviousNode) - SkillTree.Instance.nodePoints[index].Position;
            positions[0] = Vector3.zero;
            GetComponent<LineRenderer>().SetPositions(positions);
        }
        else
        {
            Destroy(GetComponent<LineRenderer>());          
        }

    }
}
