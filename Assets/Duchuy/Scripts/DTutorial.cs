using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DTutorial : MonoBehaviour
{
    public static float TUTORIAL_DURATION = 5f;

    public static DTutorial instance = null;
    public GameObject hand = null;
    public GameObject spaceManBattle = null;

    GameObject spawn;

    private void Awake()
    {
        instance = this;
    }

    public void Show(PlanetController _planet = null)  
    {
        PlanetController planet = _planet;

        if (!planet)
        {            
            planet = spaceManBattle.GetComponentInChildren<PlanetController>();
        }

        if (!planet)
            return;

        spawn = Instantiate(hand, planet.transform);
        spawn.transform.localPosition = new Vector3(1, 0, 0);
        DataHelper.SetBool(TextConstants.IsTutorial, true);
    }

    public void Hide()
    {
        Destroy(spawn);
    }

    private void Update()
    {
        if (spawn && Input.anyKeyDown)
        {
            Hide();
        }
    }
}
