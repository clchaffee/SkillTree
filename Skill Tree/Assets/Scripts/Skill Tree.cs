using UnityEngine;
using System.IO;

public class SkillTree : MonoBehaviour
{
    public static SkillTree Instance { get; private set; } 
    [SerializeField]
    [Tooltip("Case sensitive path to JSON file to populate the Skill Tree. If the file is in the Assets folder, simply type the name of the file." +
    "\nWhenever this value is set to a valid file path, the skill tree node list will update to match the file, possibly deleting manually added nodes.")]
    public string JSONFileTextField = null;
    [SerializeField]
    [Tooltip("All Nodes for the Skill Tree")]
    public SkillNode[] nodePoints = null;
    [SerializeField]
    [Tooltip("Use world space for position data. May change the results of a node's placement if the SkillTree object is not at (0,0,0)")]
    private bool useWorldSpace = false;
    public int NumberOfSkillNodes { get { return nodePoints.Length; } }
    [HideInInspector]
    public int SelectedNodeIndex = -1; // -1 means no selection
    [Tooltip("Game Object to populate the Skill Tree with on runtime")]
    public GameObject skillNodePrefab;

    /// <summary>
    /// Is called every time the inspector is changed, but not when the game is compiled or ran.
    /// </summary>
    public void OnInspectorChanged()
    {
        //If the list is ever null, make there have one entry
        if (nodePoints == null)
        {
            nodePoints = new SkillNode[1];
            nodePoints[0] = new SkillNode();
        }
        //Automatically updates the node's number to match the array
        for (int i = 0; i < nodePoints.Length; i++)
        {
            nodePoints[i].NodeNumber = i;
        }
    }

    /// <summary>
    /// Populates the nodes for the editor from a given data field when the compile button is pressed
    /// </summary>
    public void CompileJson()
    {
        //Handles getting the JSON data from the editor field

        string JSONPath = null;
        if (JSONFileTextField.Contains('/'))
        {
            JSONPath = JSONFileTextField;
        }
        else
        {
            JSONPath = $"Assets/{JSONFileTextField}.json";
        }
        if (!File.Exists(JSONPath))
        {
            Debug.LogWarning($"JSON file not found at {JSONPath}");
            return;
        }

        string json = File.ReadAllText(JSONPath); // Read the file contents
        SkillNodeContainer container = JsonUtility.FromJson<SkillNodeContainer>(json);

        if (container != null && container.skillNodes != null)
        {
            nodePoints = container.skillNodes;
            nodePoints[0].IsAcquired = true;
            Debug.Log("Skill Tree compiled from JSON.");
        }
        else
        {
            Debug.LogWarning("Invalid JSON format.");
        }
    }

    /// <summary>
    /// Gets the Node of the given index. At current, only provides position data
    /// </summary>
    /// <param name="index"></param>
    /// <returns>the position of the skill node depending on whether world space is being used</returns>
    public Vector3 GetSkillNodePosition(int index)
    {
        return useWorldSpace ? nodePoints[index].Position : this.transform.TransformPoint(nodePoints[index].Position);
    }
    /// <summary>
    /// Sets the position data for a given skill node
    /// </summary>
    /// <param name="index"></param>
    /// <param name="position"></param>
    public void SetSkillNodePosition(int index, Vector3 position)
    {
        nodePoints[index].Position = useWorldSpace ? position : this.transform.InverseTransformPoint(position);
    }
    /// <summary>
    /// Acquires a skill node and triggers the math calculation
    /// </summary>
    /// <param name="index">The index of the skill being acquired</param>
    public void AcquireSkillNode(int index)
    {
        //if the node is acquired, exit
        if (nodePoints[index].IsAcquired) return;

        int requirementTracker = 0;
        //Loops through all requirements to see if they have been met
        for (int i = 0; i < nodePoints[index].RequiredNodes.Length; i++)
        {
            //Check the node at the index found in the required nodes array and check if it's acquired
            if (IsRequiredNodeAcquired(index, nodePoints[index].RequiredNodes[i]))
            {
                requirementTracker++;
            }
        }
        //If all requirements have been met
        if (requirementTracker == nodePoints[index].RequiredNodes.Length)
        {
            nodePoints[index].IsAcquired = true;
            //fire event
        }
    }
    /// <summary>
    /// Checks to see if a skill node has been acquired yet
    /// </summary>
    /// <param name="selfIndex">Index of the node in question</param>
    /// <param name="requiredNodeIndex">Index of the required node</param>
    /// <returns></returns>
    private bool IsRequiredNodeAcquired(int selfIndex, int requiredNodeIndex)
    {
        // Ensure indices are valid
        if (selfIndex < 0 || selfIndex >= nodePoints.Length)
            return false;

        if (requiredNodeIndex < 0 || requiredNodeIndex >= nodePoints.Length)
            return false;

        // Check if the required node is acquired
        return nodePoints[requiredNodeIndex].IsAcquired;
    }

    private void OnDrawGizmos()
    {
        if (nodePoints == null)
        {
            return;
        }
        for (int i = 0; i < nodePoints.Length; i++)
        {
            Vector3 node = GetSkillNodePosition(i);
            Vector3 previousNode = GetSkillNodePosition(nodePoints[i].PreviousNode);

            Gizmos.color = Color.white;
            Gizmos.DrawLine(previousNode, node);
        }
    }

    /// <summary>
    /// Instantiates new prefab objects so that clickable MonBehavior objects are able to be interacted with
    /// </summary>
    private void Start()
    {
        //Destroy any nodes if there are objects already in the scene
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < nodePoints.Length; i++)
        {
            SkillNode node = nodePoints[i];

            // Instantiate the prefab
            GameObject skillNode = Instantiate(skillNodePrefab, GetSkillNodePosition(i), Quaternion.identity, this.transform);
            // Set the name of the GameObject for clarity in the hierarchy
            skillNode.name = $"Skill Node {i}";

            SkillNodePrefab prefab = skillNode.GetComponent<SkillNodePrefab>();
            prefab.Initialize(node, i);
        }
        this.gameObject.SetActive(false);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Ensure only one instance exists
            return;
        }

        Instance = this;
    }

}
