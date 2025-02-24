using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillUIManager : MonoBehaviour
{
    public GameObject skillMenuPanel = null; // Assign the UI panel in the inspector

    private bool isMenuOpen = false;

    SkillTree tree;

    private int selectedSkillIndex = 0;
    private float? selectedSkillCost = null;
    private string selectedSkillName = null;
    private string selectedSkillDescription = null;

    //private Button purchaseButton = null;
    private Button purchaseButton;

    public GameObject skillTitleObject = null;
    public GameObject skillCostObject = null;
    public GameObject skillDescriptionObject = null;
    public GameObject purchaseButtonObject = null;

    private TextMeshProUGUI skillTitleText = null;
    private TextMeshProUGUI skillDescriptionText = null;
    private TextMeshProUGUI skillCostText = null;

    public GameObject player = null;

    private void Awake()
    {
        skillTitleText = skillTitleObject.GetComponent<TextMeshProUGUI>();
        skillDescriptionText = skillDescriptionObject.GetComponent<TextMeshProUGUI>();
        skillCostText = skillCostObject.GetComponent<TextMeshProUGUI>();
        skillMenuPanel.SetActive(false);
        purchaseButton = purchaseButtonObject.GetComponent<Button>();

        //purchaseButton.onClick.AddListener(() => PlayerInteractionHandler.Instance.PurchaseSkillNode(selectedSkillIndex, player));
    }

    void Update()
    {
        //Open/close the menu with a key press (e.g., 'P')
        if (Input.GetKeyDown(KeyCode.P))
        {
            ToggleSkillMenu();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                SkillNodePrefab node = hit.collider.GetComponent<SkillNodePrefab>();
                if (node != null)
                {
                    UpdateSkillData((int)node.skillNodeIndex);
                    selectedSkillIndex = (int)node.skillNodeIndex;
                }
            }
        }


        //if (!PlayerInteractionHandler.Instance.CanPurchaseSkillNode(selectedSkillIndex, player))
        //{
        //    purchaseButtonObject.SetActive(false);
        //}


    }

    public void BuySelected()
    {
        PlayerInteractionHandler.Instance.PurchaseSkillNode(selectedSkillIndex, player);
    }

    private void UpdateSkillData(int index)
    {
        selectedSkillCost = SkillTree.Instance.nodePoints[index].ResourceCost;
        selectedSkillName = SkillTree.Instance.nodePoints[index].Name;
        selectedSkillDescription = SkillTree.Instance.nodePoints[index].Description;

        skillTitleText.text = selectedSkillName;
        skillDescriptionText.text = selectedSkillDescription;
        skillCostText.text = $"Cost: {selectedSkillCost}";
        //Hides the purchase button if you've already acquired it
        purchaseButtonObject.SetActive(!SkillTree.Instance.nodePoints[index].IsAcquired);

    }

    public void ToggleSkillMenu()
    {
        isMenuOpen = !isMenuOpen;
        skillMenuPanel.SetActive(isMenuOpen);
        SkillTree.Instance.gameObject.SetActive(isMenuOpen);
        //Support to pause the game when the menu is open, optional
        //Time.timeScale = isMenuOpen ? 0 : 1;
    }
}
