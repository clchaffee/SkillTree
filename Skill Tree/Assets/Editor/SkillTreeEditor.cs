using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkillTree))]
public class SkillTreeEditor : Editor
{
    private bool currentNodeFoldout;
    private bool previousNodeFoldout;
    private bool nextNodeFoldout;
    int? selectedNodeIndex = null;
    int? pickedHandle = null;

    /// <summary>
    /// Provides handles for each node point
    /// </summary>
    private void OnSceneGUI()
    {
        SkillTree tree = (SkillTree)target;
        //Catch errors in which the number of skill nodes is null (when the tree script is first applied)
        try
        {
            for (int i = 0; i < tree.NumberOfSkillNodes; i++)
            {
                Vector3 position = tree.GetSkillNodePosition(i);
                if (tree.nodePoints[i].IsAcquired)
                {
                    Handles.color = Color.green;
                }
                else
                {
                    Handles.color = Color.white;
                }

                if (Handles.Button(position, Quaternion.identity, .25f, .5f, Handles.SphereHandleCap))
                {
                    pickedHandle = i;
                    Repaint();
                }
            }
        }
        catch (NullReferenceException)
        {
            throw;
        }

        

        if (pickedHandle.HasValue)
        {
            EditorGUI.BeginChangeCheck();

            Vector3 position = tree.GetSkillNodePosition(pickedHandle.Value);

            Vector3 newPosition = Handles.PositionHandle(position, Quaternion.identity);

            Handles.Label(newPosition, tree.nodePoints[(int)pickedHandle].NodeNumber.ToString());

            if (EditorGUI.EndChangeCheck())
            {
                //Undo (CTRL+Z support)
                Undo.RecordObject(tree, "Change point position");

                tree.SetSkillNodePosition(pickedHandle.Value, newPosition);
                SceneView.RepaintAll();
            }
        }
    }
    /// <summary>
    /// Used to check changes made in the editor similar to the OnValidate method, but is only called when the editor itself is changed
    /// </summary>
    public override void OnInspectorGUI()
    {
        SkillTree tree = (SkillTree)target;

        //Drag and drop field and label
        EditorGUILayout.LabelField("JSON File Asset Field:", EditorStyles.boldLabel);
        TextAsset jsonFile = (TextAsset)EditorGUILayout.ObjectField(null, typeof(TextAsset), false);
        //sets the Skill Tree variable
        if (jsonFile != null)
        {
            tree.JSONFileTextField = AssetDatabase.GetAssetPath(jsonFile);
            EditorUtility.SetDirty(tree);
        }
        
        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();
        if (EditorGUI.EndChangeCheck())
        {
            tree.OnInspectorChanged();
        }
        //Checks changes of the JSON for a compiler button
        if (GUILayout.Button(new GUIContent("Compile JSON", "WARNING: Compiling JSON will replace any manually added nodes.")))
        {
            tree.CompileJson();

            SceneView.RepaintAll();
        }

        if (tree.nodePoints == null || tree.nodePoints.Length == 0)
        {
            EditorGUILayout.HelpBox("No skill nodes available. Add nodes to edit.", MessageType.Warning);
            return;
        }

        if (!pickedHandle.HasValue) return;

        int currentIndex = pickedHandle.Value;

        // Display Previous Node (if it exists)
        if (currentIndex > 0)
        {
            previousNodeFoldout = EditorGUILayout.Foldout(previousNodeFoldout, $"Previous Node (Index {currentIndex - 1})");
            if (previousNodeFoldout)
            {
                EditorGUI.indentLevel++;
                DisplayNode(tree, currentIndex - 1);
                EditorGUI.indentLevel--;
            }
        }

        // Display Current Node
        currentNodeFoldout = EditorGUILayout.Foldout(currentNodeFoldout, $"Current Node (Index {currentIndex})");
        if (currentNodeFoldout)
        {
            EditorGUI.indentLevel++;
            DisplayNode(tree, currentIndex);
            EditorGUI.indentLevel--;
        }

        // Display Next Node (if it exists)
        if (currentIndex < tree.nodePoints.Length - 1)
        {
            nextNodeFoldout = EditorGUILayout.Foldout(nextNodeFoldout, $"Next Node (Index {currentIndex + 1})");
            if (nextNodeFoldout)
            {
                EditorGUI.indentLevel++;
                DisplayNode(tree, currentIndex + 1);
                EditorGUI.indentLevel--;
            }
        }

        // Save changes if anything was modified
        if (GUI.changed)
        {
            Undo.RecordObject(tree, "Modify Skill Tree");
            EditorUtility.SetDirty(tree);
        }
    }

    /// <summary>
    /// Display the fields of a single node in the Inspector.
    /// </summary>
    private void DisplayNode(SkillTree tree, int index)
    {
        var node = tree.nodePoints[index];

        node.IsAcquired = EditorGUILayout.Toggle("Is Acquired", node.IsAcquired);
        node.ResourceCost = EditorGUILayout.IntField("Resource Cost", node.ResourceCost);
        node.RequiredStatName = EditorGUILayout.TextField("Required Stat Name", node.RequiredStatName);

        // Mark the SkillTree as dirty to save changes
        if (GUI.changed)
        {
            Undo.RecordObject(tree, "Modify Skill Node");
            EditorUtility.SetDirty(tree);
        }
    }

}
