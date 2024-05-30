using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SteveRogers
{
    public class DataMaster
    {
        private static DataMasterModel m_DataMasterModel = null;
        private static List<List<string>> m_PlanetUpgradeMaterialNeeded = null;
        private static List<AddEffectCost> m_AddEffectCostList = null;

        public const int MAX_PLANETS_NUMBER = 10;

        public static AddEffectCost GetAddEffectCost(TypePlanet type)
        {
            if (m_AddEffectCostList == null)
            {
                var t = Utilities.ReadAllText_Resources("Add-Effect-Cost-List");
                Utilities.SafeJsonDeserializeObject(out m_AddEffectCostList, ref t);
            }

            return m_AddEffectCostList.Find(i => i.typePlanet == type.ToString());
        }

        public static int PlanetUpgradeMaterialNeeded_MaxUpgradeIndex
        {
            get
            {
                if (m_PlanetUpgradeMaterialNeeded == null)
                    m_PlanetUpgradeMaterialNeeded = Utilities.ReadExcelFormat_FromResources("Planet-Upgrade-Material-Needed");

                return m_PlanetUpgradeMaterialNeeded[0].Count - 2;
            }
        }

        public static int PlanetUpgradeMaterialNeeded_Get(PLanetName planet, int upgradeIdx)
        {
            if (m_PlanetUpgradeMaterialNeeded == null)
                m_PlanetUpgradeMaterialNeeded = Utilities.ReadExcelFormat_FromResources("Planet-Upgrade-Material-Needed");

            foreach (var line in m_PlanetUpgradeMaterialNeeded)
            {
                if (line[0].Equals(planet.ToString().ToUpper()))
                {

                    var val = line.Get(upgradeIdx + 1).Parse(-1);
                    return val;
                }
            }

            throw new Exception("not found planet in file data: Planet-Upgrade-Material-Needed.txt, " + planet.ToString().ToUpper());
        }

        private static string DataMasterCSVPath => Utilities.GetRuntimeDataFolderPath(true) + "DataMaster.csv";

        public static void DownloadDataMaster(Action doneCb)
        {
            GameManager.Instance.StartCoroutine(Utilities.DownloadGoogleSheet_CRT(
                "1X3Ah5-IiUq9DvN8CWHZTJLMb2MvFm1lgy5dz79wU53E",
                (res, s) =>
                {
                    if (res || File.Exists(DataMasterCSVPath))
                        doneCb();
                    else
                    {
                        PopupConfirm.ShowOK(
                            "Oops!",
                            TextMan.Get("Download Data Failed"),
                            TextMan.Get("Try Again"),
                            () => DownloadDataMaster(doneCb));
                    }
                },
                DataMasterCSVPath,
                "110423127"));
        }

        public static DataMasterModel DataMasterModel
        {
            get
            {
                if (m_DataMasterModel == null)
                {
                    var c = File.ReadAllText(DataMasterCSVPath);
                    var dic = Utilities.CreateDictionaryFromCSVContent(ref c);

                    m_DataMasterModel = new DataMasterModel
                    {
                        SizeStart = Utilities.GetValueFromCSVDictionary(dic, "SizeStart", 10f),
                        SizePerUpgrade = Utilities.GetValueFromCSVDictionary(dic, "SizePerUpgrade", 10f),

                        HpStart = Utilities.GetValueFromCSVDictionary(dic, "HpStart", 10f),
                        HpPerUpgrade = Utilities.GetValueFromCSVDictionary(dic, "HpPerUpgrade", 10f),

                        DameStart = Utilities.GetValueFromCSVDictionary(dic, "DameStart", 10f),
                        DamePerUpgrade = Utilities.GetValueFromCSVDictionary(dic, "DamePerUpgrade", 10f),

                        SpeedStart = Utilities.GetValueFromCSVDictionary(dic, "SpeedStart", 10f),
                        SpeedPerUpgrade = Utilities.GetValueFromCSVDictionary(dic, "SpeedPerUpgrade", 10f),

                        GravityStart = Utilities.GetValueFromCSVDictionary(dic, "GravityStart", 10f),
                        GravityPerUpgrade = Utilities.GetValueFromCSVDictionary(dic, "GravityPerUpgrade", 10f),

                        CollectMaterialStart = Utilities.GetValueFromCSVDictionary(dic, "CollectMaterialStart", 1),
                        CollectMaterialPerUpgrade = Utilities.GetValueFromCSVDictionary(dic, "CollectMaterialPerUpgrade", 1),
                    };
                }
                
                return m_DataMasterModel;
            }
        }

        public static float GetUpgradeValue(PLanetName planet, float startValue, int upgradeIdx, float stepValue)
        {
            return startValue + stepValue * upgradeIdx + (int)planet * (stepValue * 10);
        }

#if UNITY_EDITOR
        
        private static string GenerateID(Transform t, out string content)
        {
            var text = t.GetComponent<Text>();
            content = null;

            if (text)
                content = text.text;
            else
            {
                TextMeshProUGUI textMeshPro = t.GetComponent<TextMeshProUGUI>();

                if (textMeshPro)
                    content = textMeshPro.text;
            }

            if (content.IsNullOrEmpty())
                return null;
            else
            {
                var id = string.Empty;

                for (int i = 0; i < content.Length && i < 50; i++)
                {
                    if (char.IsLetter(content[i]))
                        id += content[i];
                }

                return id;
            }
        }

        [MenuItem("Cheat/Add 1000 Mats")]
        private static void AddMat()
        {
            if (Application.isPlaying)
                DataGameSave.dataLocal.M_Material += 1000;
            else
                EditorUtility.DisplayDialog("Oops", "Press Play and try again!", "OK");
        }

        [MenuItem("Cheat/Add 1000000 Mats")]
        private static void AddMat_100000000()
        {
            if (Application.isPlaying)
                DataGameSave.dataLocal.M_Material += 100000000;
            else
                EditorUtility.DisplayDialog("Oops", "Press Play and try again!", "OK");
        }

        [MenuItem("Cheat/Set 0 Material", priority = 100)]
        private static void ResetMat()
        {
            if (Application.isPlaying)
                DataGameSave.dataLocal.M_Material = 0;
            else
                EditorUtility.DisplayDialog("Oops", "Press Play and try again!", "OK");
        }

        [MenuItem("Cheat/Full Stone")]
        private static void FullStone()
        {
            DataGameSave.dataLocal.M_Material += 1000;
            DataGameSave.dataLocal.M_AirStone += 1000;
            DataGameSave.dataLocal.M_AntimatterStone += 1000;
            DataGameSave.dataLocal.M_FireStone += 1000;
            DataGameSave.dataLocal.M_GravityStone += 1000;
            DataGameSave.dataLocal.M_IceStone += 1000;
            DataGameSave.dataLocal.M_ColorfulStone += 1000;
            DataGameSave.dataLocal.M_ToyStone1 += 1000;
            DataGameSave.dataLocal.M_ToyStone2 += 1000;
            DataGameSave.dataLocal.M_ToyStone3 += 1000;
            DataGameSave.dataLocal.M_ToyStone4 += 1000;
            DataGameSave.dataLocal.M_ToyStone5 += 1000;
        }


        [MenuItem("Cheat/Full + 1 Stone")]
        private static void FullOneStone()
        {
            DataGameSave.dataLocal.M_Material = 1;
            DataGameSave.dataLocal.M_AirStone = 1;
            DataGameSave.dataLocal.M_AntimatterStone = 1;
            DataGameSave.dataLocal.M_FireStone = 1;
            DataGameSave.dataLocal.M_GravityStone = 1;
            DataGameSave.dataLocal.M_IceStone = 1;
            DataGameSave.dataLocal.M_ColorfulStone = 1;
            DataGameSave.dataLocal.M_ToyStone1 = 1;
            DataGameSave.dataLocal.M_ToyStone2 = 1;
            DataGameSave.dataLocal.M_ToyStone3 = 1;
            DataGameSave.dataLocal.M_ToyStone4 = 1;
            DataGameSave.dataLocal.M_ToyStone5 = 1;
        }

        [MenuItem("Cheat/Increase 1 Level ")]
        private static void LevelUp()
        {
            if (Application.isPlaying)
                DataGameSave.dataServer.level += 1;
            else
                EditorUtility.DisplayDialog("Oops", "Press Play and try again!", "OK");
        }

        [MenuItem("Cheat/Win Game ")]
        private static void WinGame() {
            DataGameSave.dataEnemy.sunHp = -1f;
        }

        [MenuItem("Cheat/Reset Daily Battle ")]
        private static void ResetDailyBattle()
        {
            GameManager.DailyBattleAttackCount = GameManager.MAX_DAILY_ATTACK_COUNT;
        }

        [MenuItem("Cheat/Destroy 1000 ngoi sao")]
        private static void Destroy1000Star() {
            DataGameSave.dataLocal.destroyedSolars += 1000;
        }

        [MenuItem("Cheat/Destroy 1000 planet")]
        private static void Destroy1000Planet() {
            DataGameSave.dataLocal.destroyPlanet += 1000;
        }

        [MenuItem("Cheat/Destroy 1000 meteor ")]
        private static void Destroy1000Meteor() {
            DataGameSave.dataLocal.meteorPlanetHitCount += 1000;
        }

        [MenuItem("Cheat/Open event")]
        private static void OpenEvent() {
            UIMultiScreenCanvasMan.Instance.OnPressed_Event();
        }

        [MenuItem("Cheat/Use 10 auto attack")]
        private static void Use10AutoAttack() {
            DataGameSave.autoShootCount -= 10;
        }

        [MenuItem("Cheat/Clear meta data (skin,đếm theo ngày,...)")]
        private static void ClearMetaData() {
            DataGameSave.metaData = new Dictionary<string, string>();
        }

        [MenuItem("Project/Clear All %q")]
        private static void ClearAll()
        {
            if (Directory.Exists("obj/UserData"))
                Directory.Delete("obj/UserData", true);

            PlayerPrefs.DeleteAll();
            Utilities.WarningDone("Clear");
        }

        [MenuItem("Project/Open Language Sheet", priority = 200)]
        private static void landsset()
        {
            Application.OpenURL("https://docs.google.com/spreadsheets/d/1X3Ah5-IiUq9DvN8CWHZTJLMb2MvFm1lgy5dz79wU53E/edit#gid=0");
        }
#endif
    }

    [Serializable]
    public struct AddEffectCost
    {
        public string typePlanet;
        public int antimatter;
        public int air;
        public int ice;
        public int gravity;
        public int fire;
    }

    [Serializable]
    public class DataMasterModel
    {
        public float SizeStart = 10;
        public float SizePerUpgrade = 1;

        public float HpStart = 100;
        public float HpPerUpgrade = 10;

        public float DameStart = 150;
        public float DamePerUpgrade = 10;

        public float SpeedStart = 400;
        public float SpeedPerUpgrade = 50;

        public float CollectMaterialStart = 1;
        public float CollectMaterialPerUpgrade = 1;

        public float GravityStart = 10;
        public float GravityPerUpgrade = 5;
    }



    #region cheat full money

#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public static class MenuItem_Debug_FullMoney // CHANGE THIS (1/3)
    {
        public static bool IsTrue(bool valueIfNotEditor)
        {
#if UNITY_EDITOR
            return EditorPrefs.GetBool(MENU_NAME, false);
#else
        return valueIfNotEditor;
#endif
        }

#if UNITY_EDITOR

        private static bool enabled_;

        private const string MENU_NAME = "Cheat/Full Money"; // CHANGE THIS (2/3)

        /// Called on load thanks to the InitializeOnLoad attribute
        static MenuItem_Debug_FullMoney() // CHANGE THIS (3/3)
        {
            enabled_ = EditorPrefs.GetBool(MENU_NAME, false);

            /// Delaying until first editor tick so that the menu
            /// will be populated before setting check state, and
            /// re-apply correct action
            EditorApplication.delayCall += () =>
            {
                PerformAction(enabled_);
            };
        }

        [MenuItem(MENU_NAME)]
        private static void ToggleAction()
        {

            /// Toggling action
            PerformAction(!enabled_);
        }

        public static void PerformAction(bool enabled)
        {

            /// Set checkmark on menu item
            Menu.SetChecked(MENU_NAME, enabled);

            /// Saving editor state
            EditorPrefs.SetBool(MENU_NAME, enabled);
            enabled_ = enabled;
        }

#endif
    }

    #endregion
}