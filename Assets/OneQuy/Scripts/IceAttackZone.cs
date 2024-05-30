using System;
using System.Collections.Generic;
using UnityEngine;
using SteveRogers;

public class IceAttackZone : MonoBehaviour
{
    public TypePlanet type = TypePlanet.Ice;
    public PlanetController owner = null;

    float timeDestroy = 0;
    HashSet<PlanetController> list = new HashSet<PlanetController>();

    private void Update()
    {
        float time = 0;

        if (type == TypePlanet.Ice)
            time = GameManager.Instance.iceAttackZoneExistTime;
        else if (type == TypePlanet.Fire)
            time = GameManager.Instance.fireAttackZoneExistTime;
        else if (type == TypePlanet.Antimatter)
            time = GameManager.Instance.antiMatterAttackZoneExistTime;
        else
            throw new Exception();

        timeDestroy += Time.deltaTime;

        if (timeDestroy > time)
        {
            foreach (var i in list)
                i.GotAttacked(type, false, owner, null);

            Destroy(transform.parent.parent.gameObject);
            list.Clear();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(GameConstants.PLANET_TAG))
            return;
        
        if (!LayerMask.LayerToName(collision.gameObject.layer).Equals("Enemy"))
            return;
        
        var enemy = collision.transform.parent.GetComponent<PlanetController>();

        if (!enemy)
            return;

        enemy.GotAttacked(type, true, owner, this);
        list.Add(enemy);
    }
}
