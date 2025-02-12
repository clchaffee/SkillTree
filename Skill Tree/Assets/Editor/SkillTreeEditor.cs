using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static Unity.VisualScripting.LudiqRootObjectEditor;

[CustomEditor(typeof(SkillTree))]
public class SkillTreeEditor : Editor
{
    int? pickedHandle = null;
    /// <summary>
    /// Provides handles for each node point
    /// </summary>
    private void OnSceneGUI()
    {
        SkillTree tree = (SkillTree)target;

        for (int i = 0; i < tree.NumberOfSkillNodes; i++)
        {
            Vector3 position = tree.GetSkillNodePosition(i);
            if(Handles.Button(position, Quaternion.identity, .25f, .5f, Handles.SphereHandleCap))
            {
                pickedHandle = i;
            } 
        }  
        if(pickedHandle.HasValue)
        {
            EditorGUI.BeginChangeCheck();

            Vector3 position = tree.GetSkillNodePosition(pickedHandle.Value);

            Vector3 newPosition = Handles.PositionHandle(position, Quaternion.identity);
            
            if (EditorGUI.EndChangeCheck())
            {
                //Undo (CTRL+Z support)
                Undo.RecordObject(tree, "Change point position");

                tree.SetSkillNodePosition(pickedHandle.Value, newPosition);
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
        //checks changes of the JSON for a compiler button
        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();
        if (EditorGUI.EndChangeCheck())
        {
            tree.OnInspectorChanged();
        }
        if (GUILayout.Button(new GUIContent("Compile JSON", "WARNING: Compiling JSON will replace any manually added nodes.")))
        {
            tree.CompileJson();
            
            SceneView.RepaintAll();
        }
    }


}
