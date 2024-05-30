using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SteveRogers;

public class FollowHP : MonoBehaviour
{
    public CameraFollow follow = null;

    public PlanetController planet = null;
    public Image hpFill = null;

    // Start is called before the first frame update
    void Start()
    {
        follow = GetComponent<CameraFollow>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!planet || !planet.gameObject.activeSelf)
        {
            Destroy(gameObject);
            return;
        }

        follow.followObject = planet.transform;

        //follow.SetOffset(new Vector3(0, (planet.size - 10) / 19f * TempVar.f1));
        follow.SetOffset(new Vector3(0, 1));
        var data = DataGameSave.dataServer;
        hpFill.fillAmount = data.ListPlanet[planet.Manager.IndexSpace].HP / planet.startHP;
    }
}
