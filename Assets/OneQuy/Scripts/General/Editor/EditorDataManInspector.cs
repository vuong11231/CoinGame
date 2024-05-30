using UnityEngine;
using System.IO;
using UnityEditor;

namespace SteveRogers
{
    [CustomEditor(typeof(EditorDataMan))]
    public class EditorDataManInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorDataMan myTarget = (EditorDataMan)target;

            if (GUILayout.Button("Build"))
            {
                myTarget.Write();
            }
            else if (GUILayout.Button("Open File"))
            {
                if (Utilities.FileExist(EditorDataMan.PATH))
                    Utilities.OpenFileWithDefaultApp(EditorDataMan.PATH);
            }
            else if (GUILayout.Button("Delete File"))
            {
                if (Utilities.FileExist(EditorDataMan.PATH))
                    File.Delete(EditorDataMan.PATH);

                Utilities.WarningDone("Delete Editor Data File");
            }
        }
    }
}