# SkillTree
## Description
This skill tree is designed to be reminiscent of those in games such as Path of Exile. Each node provides passive bonuses that a player purchases and will increase or decrease a stat that the player has such as health, speed, damage, etc. While it can support the unlocking of activated abilities such as unlocking a double jump, it is not designed with this in mind. The UI is primarily designed to be an editor tool: friendly to you, the developer, not the player. As such, you will be able to see how the skill tree will look in the scene view and change values without leaving the engine via the inspector. The nodes that the skill tree uses are 3D so they can work in both 3D and 2D menues.
## Setup

1. Drag and drop the Skill Tree game object prefab from the "prefab" folder. Make sure it contains a Player Interaction Handler and Skill Tree script as components. In the Skill Tree component on the inspector make sure the Skill Node Prefab has a valid game object you'd like to use, or modify the SkillNodePrefab object to suit your needs.

     It should look like this:

      ![Screenshot 2025-02-23 180906](https://github.com/user-attachments/assets/c1c72528-b69a-4d1e-bf7d-28e7d41606fc)


2.  Drag and drop the Canvas prefab from the "prefab" folder, or create your own canvas. It's easier to use the prefab, as the canvas requires a Panel child which then needs a button and three text field children. Make sure the Skill UI Manager script is attached as a component and contains the proper public fields populated, including providing your player object.

    It should look like this in the inspector panel:

    ![Screenshot 2025-02-23 181305](https://github.com/user-attachments/assets/16260a49-f367-4a38-a556-8ea3c56c11da)

    It should look like this in the scene panel:

    ![Screenshot 2025-02-23 182026](https://github.com/user-attachments/assets/62eb5a71-3c23-415e-aebf-e0bf9db8e8bf)

3. If you don't already have it, you may need to install the 2D sprite package as part of using the canvas system.
4. You will have to change the name of the player component within the Player Interaction Manager script twice to the name of your player script. Due to the usage of reflection, that script requires the player script, not just an editor-assigned game object. Once you do that, you should not have to modify any code unless you want to.

## The Skill Nodes
The skill nodes of the tree store 14 values, listed with their types and descriptions below:
1. string Name - The name of the skill, which will be visible in the skill tree's list.
2. string Description - A short description of the skill, designed to tell the player what it does.
3. Vector3 Position - The position in 3D space of the node. Typically local to the skill tree object.
4. string StatAugmented - The player variable that is being changed. Must be the name of the desired variable on the player.
5. string MathDone - The kind of math that is done to the augmented stat. Below is the list of supported entries:

      - Addition: "+", "add", "addition", "plus"
   
      - Subtraction: "-", "subtract", "subtraction", "minus"
   
      - Multiplication: "x","*", "multiplication", "multiply", "times"
   
      - Division: "/", "divide", "division", "divided by"
   
      - Percentage changes: "%", "percentile"
6. int Value - The number the stat is being changed by.
7. int NodeNumber - The number or index of the current node.
8. int PreviousNode - The number or index of the current node's predecessor.
9. int ResourceCost - The cost of unlocking the skill. If you want it to be skill point-based, you can set this to smaller or consistent values such as 1-3 or larger values if you want it to be based on another resource.
10. int[] RequiredNodes - The node indices, if any, that the player must own to purchase the skill.
11. string RequiredStatName - The name of the required stat, if any, that the player must own to purchase the skill. 
12. int RequiredStatValue - The stat prerequisite value, if any, that the player must have to purchase the skill.
13. int RequiredSpentResource - The number of spent skill resources, if any, that the player must have previously spent to unlock the skill.
14. bool IsAcquired - Whether or not the skill has been acquired or is owned.

## The Skill Tree
The skill tree script contains three important features: the Node Points array, the JSON field, and the Skill Node prefab, among others.
You can reference, add, remove, and modify each skill node and its data from the inspector when you have the skill tree selected. You can also see the position it will be in and what nodes it will be connected to, both of which can be modified either in the scene view or in the inspector. If you click on a node, the inspector will tell you which node you have selected.
There are two ways to provide JSON data: a text field and a drag-and-drop field. The text field will always assume that the file is located within the "Assets" folder, and if it is, you can simply type the name of the file (i.e. no directory prefix, no .json suffix). If the file is not in the Assets folder, you can instead type out the file path or drag and drop the file into the related field to autofill the text field. Finally, you can click the "Compile JSON" button to populate the Node Points array with the selected JSON data. It's important to know that doing so will override any nodes you've added or modified manually.
The Skill Node Prefab field is a field for a game object or prefab that the tree will automatically Instantiate, or create, on runtime during Unity's Start function
