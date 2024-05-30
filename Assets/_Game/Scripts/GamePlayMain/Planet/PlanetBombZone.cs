using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
public class PlanetBombZone : MonoBehaviour
{

    public List<PlanetController> ListPlanet = new List<PlanetController>();

    public MainPlanetController SunEnemy;

    public PlanetController Data;
    public GameObject FxBomb;
    public bool IsDameSun = false;
    bool IsDame = false;
    float Size = 1;

    public void SetZone(int level, float _Size)
    {
        Size = .8f/ _Size  * DatabasePlanet.Instance.ListAntimatterialPlanet[level].UpSizeDame + .5f;//-1
        transform.LeanScale(Vector3.one * Size, 0);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == GameConstants.PLANET_TAG )
        {
            if(!IsDame)
            ListPlanet.Add(collision.transform.parent.gameObject.GetComponent<PlanetController>());
        }
        if(collision.gameObject.tag == GameConstants.SUN_TAG)
        {
            IsDameSun = true;
            SunEnemy = collision.GetComponent<MainPlanetController>();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == GameConstants.PLANET_TAG)
        {
            if(!IsDame)
            ListPlanet.Remove(collision.transform.parent.gameObject.GetComponent<PlanetController>());
        }
    }
    public void DameEnemy()
    {
        GameObject temp = LeanPool.Spawn(FxBomb, transform.position, Quaternion.identity);
        temp.transform.localScale = Vector3.one * ((float)Size/2);
        IsDame = true;

        for (int i =0;i< ListPlanet.Count; i++)
        {
            if (ListPlanet[i].gameObject !=transform.parent.gameObject)
            ListPlanet[i].GetDame(Data);
        }
        if(IsDameSun)
        {
            SunEnemy.GetDamage(Data);
        }
    }


}
