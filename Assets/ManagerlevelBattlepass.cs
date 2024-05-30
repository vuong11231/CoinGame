using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelBattlepass : MonoBehaviour
{
    public int level;
    public int WinToNextLevel;
    public LevelBattlepass()
    {
        level = 1;
        WinToNextLevel = 1;
    }
    public void AddLevel(int amout)
    {
        level += amout;
        if (level <= WinToNextLevel)
        {
            // tăng cấp cho màn chơi 
            WinToNextLevel++;
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}