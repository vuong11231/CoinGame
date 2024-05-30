using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = DataScriptableObject.FILE_NAME,
    menuName = "Create " + DataScriptableObject.FILE_NAME)]
public class DataScriptableObject : ScriptableObject
{
    private const string FILE_NAME = "DataFile"; // ONLY CHANGE THIS!

    private static DataScriptableObject _Instance = null;

    public static DataScriptableObject Instance
    {
        get
        {
            if (_Instance != null)
                return _Instance;
            
            _Instance = Resources.Load<DataScriptableObject>(FILE_NAME);
            return _Instance;
        }
    }


    [Header("distance")]

    public float sunDistance = 6;
    public float planetsDistance = 4;
    public float offsetEmptySpaceGameplay = 40f;

    [Header("other")]

    public bool notClearAttackedInfo = false;
    public GameObject sunExplosionGo = null;
    public float autoRestoreHours = 24;

}