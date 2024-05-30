using UnityEngine;
using System.IO;
using UnityEditor;

namespace SteveRogers
{
    [CustomEditor(typeof(CheatManager))]
    public class CheatManInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            if (GUILayout.Button("Build"))
            {
                HelpersEditor.BuildCheatFilePC();

            }
            else if (GUILayout.Button("Default"))
            {
                CheatManager myTarget = (CheatManager)target;
                myTarget.buildData = myTarget.defaultValue;
                HelpersEditor.BuildCheatFilePC();
                Utilities.MarkAllScenesDirty();
            }
            //else if (GUILayout.Button("Open File"))
            //{
            //    if (Utilities.FileExist(EditorDataMan.PATH))
            //        Utilities.OpenFileWithDefaultApp(EditorDataMan.PATH);
            //}
            else if (GUILayout.Button("Delete"))
            {
                HelpersEditor.DeleteCheatFolderPC();
            }
        }
    }
}