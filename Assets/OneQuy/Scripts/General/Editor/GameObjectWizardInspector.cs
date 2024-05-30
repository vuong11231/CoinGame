using UnityEngine;
using System.IO;
using UnityEditor;

namespace SteveRogers
{
    [CustomEditor(typeof(GameObjectWizard))]
    public class GameObjectWizardInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            #region world position

            GUILayout.Space(10);
            GUILayout.Label("World Position", new GUIStyle { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 15 });
            GUILayout.Space(10);

            if (GUILayout.Button("Print World Position"))
            {
                GameObjectWizard myTarget = (GameObjectWizard)target;
                Debug.Log(myTarget.transform.position);
            }
            else if (GUILayout.Button("Copy World Position"))
            {
                GameObjectWizard myTarget = (GameObjectWizard)target;
                Utilities.Clipboard = myTarget.transform.position.ToString();
                Utilities.WarningDone("Copied");
            }
            else if (GUILayout.Button("Set World Position From Clipboard"))
            {
                GameObjectWizard myTarget = (GameObjectWizard)target;
                myTarget.transform.position = Utilities.StringToVector3(Utilities.Clipboard);
            }

            #endregion

            #region audio source

            GUILayout.Space(10);
            GUILayout.Label("Audio", new GUIStyle { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 15 });
            GUILayout.Space(10);

            if (GUILayout.Button("Check AudioSource Is Playing"))
            {
                GameObjectWizard myTarget = (GameObjectWizard)target;
                Debug.Log(Utilities.CreateLogContent("clip: " + myTarget.GetComponent<AudioSource>().clip?.name, "is playing: " + myTarget.GetComponent<AudioSource>().isPlaying));
            }

            #endregion

            #region spine
#if SPINE
            GUILayout.Space(10);
            GUILayout.Label("Spine", new GUIStyle { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 15 });
            GUILayout.Space(10);

            if (GUILayout.Button("Log Info"))
            {
                GameObjectWizard myTarget = (GameObjectWizard)target;
                var spine = myTarget.GetComponent<Spine.Unity.SkeletonGraphic>();
                var controller = new SpineController(spine);

                Debug.Log(Utilities.CreateLogContent(
                    "curent anim: " + controller.Current,
                    "curent anim percent: " + controller.CurrentAnimationPercent,
                    "is pausing: " + controller.IsPausing,
                    "is completed: " + controller.IsComplete));

            }
            else if (GUILayout.Button("SetToFirstFrameAndTrackTimeScaleZero"))
            {
                GameObjectWizard myTarget = (GameObjectWizard)target;
                var spine = myTarget.GetComponent<Spine.Unity.SkeletonGraphic>();
                var controller = new SpineController(spine);
                controller.SetToFirstFrameAndTrackTimeScaleZero(true);
                Utilities.WarningDone();
            }
            else if (GUILayout.Button("Set Setup Pose"))
            {
                GameObjectWizard myTarget = (GameObjectWizard)target;
                var spine = myTarget.GetComponent<Spine.Unity.SkeletonGraphic>();
                var controller = new SpineController(spine);
                controller.SetSetupPose();
                Utilities.WarningDone();
            }
#endif // #if SPINE
            #endregion
        }
    }
}