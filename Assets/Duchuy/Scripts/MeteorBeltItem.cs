using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MeteorBeltItem : MonoBehaviour {
    public enum METEOR_TYPE { METEORITE, MONO_COLOR, MULTI_COLOR, DIAMOND, BIG_METEOR_1, BIG_METEOR_2 }
    public METEOR_TYPE meteorType;

    public string color;
    public string effectType;

    //public GameObject EffectDestroy;
    public float dame = 2;
    public float hp = 2;
    public float gravityRange = 5f;
    public DataReward reward;

    private float SPEED = 0.5f;

    [HideInInspector]
    public MeteorBelt meteorBelt;

    Vector3 direction;

    void Start()
    {
        GetComponent<Rigidbody2D>().gravityScale = 0;
        direction = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1)).normalized * SPEED;
        UpdateHPBar();
    }

    void Update()
    {
        if (!MeteorBelt.isMeteorFall) {
            GetComponent<Rigidbody2D>().velocity = direction;
        }
    }

    public void GetHit(float dame, GameObject[] EffectDestroy)
    {
        hp -= dame;

        //if (transform.childCount > 0)
        //    transform.GetChild(0).gameObject.SetActive(true);

        //UpdateHPBar();

        if (meteorType == METEOR_TYPE.BIG_METEOR_1)
        {
            GameManager.bigMeteorHp1 = hp;
        }
        else if (meteorType == METEOR_TYPE.BIG_METEOR_2) {
            GameManager.bigMeteorHp2 = hp;
        }

        if (hp < 0)
        {
            var destroyvfx = EffectDestroy[UnityEngine.Random.Range(0, EffectDestroy.Length)];
            LeanPool.Spawn(destroyvfx, transform.position, destroyvfx.transform.rotation);// destroy user planet when user attack

            if (meteorType == METEOR_TYPE.BIG_METEOR_1)
            {
                GameManager.bigMeteorHp1 = GameManager.BIG_METEOR_MAX_HP;
            }
            else if (meteorType == METEOR_TYPE.BIG_METEOR_2)
            {
                GameManager.bigMeteorHp2 = GameManager.BIG_METEOR_MAX_HP;
            }

            if (meteorType != METEOR_TYPE.BIG_METEOR_1 && meteorType != METEOR_TYPE.BIG_METEOR_2)
            {
                meteorBelt.AddReward(this);
            }
            else {
                LeanPool.Spawn(MeteorBelt.Instance.explodeObject, transform.position, destroyvfx.transform.rotation);

                MeteorBelt.Instance.AddSpecificReward("material",   reward.material, transform.position, "bigmeteor");
                MeteorBelt.Instance.AddSpecificReward("air",        reward.air,     transform.position, "bigmeteor");
                MeteorBelt.Instance.AddSpecificReward("antimater",  reward.antimater, transform.position, "bigmeteor");
                MeteorBelt.Instance.AddSpecificReward("fire",       reward.fire,    transform.position, "bigmeteor");
                MeteorBelt.Instance.AddSpecificReward("gravity",    reward.gravity, transform.position, "bigmeteor");
                MeteorBelt.Instance.AddSpecificReward("ice",        reward.ice,     transform.position, "bigmeteor");
                MeteorBelt.Instance.AddSpecificReward("diamond",    reward.diamond, transform.position, "bigmeteor");
                MeteorBelt.Instance.AddSpecificReward("unknown",    reward.unknown, transform.position, "bigmeteor");
            }
           
            gameObject.SetActive(false);
        }
    }

    public void UpdateHPBar()
    {
        Transform fill = transform.childCount > 0 ? transform.GetChild(0) : null;

        if (!fill)
            return;

        if (fill.GetComponent<UnityEngine.UI.Image>())
            fill.GetComponent<UnityEngine.UI.Image>().fillAmount = hp * 1f / startHP;
    }

    private int startHP = 1;

    public void SetData(int hp, int dame, DataReward reward)
    {
        startHP = hp;
        this.hp = hp;
        UpdateHPBar();
        this.dame = dame;
        this.reward = reward;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Sun")) {
            MainPlanetController.Instance.GetDamage(dame, PlanetController.Owner.Enemy);

            //var destroyvfx = MeteorBelt.Instance.effectDestroys[UnityEngine.Random.Range(0, MeteorBelt.Instance.effectDestroys.Count)];
            var destroyvfx = MeteorBelt.Instance.effectDestroys[Random.Range(0, MeteorBelt.Instance.effectDestroys.Count)];
            GameObject go = LeanPool.Spawn(destroyvfx, transform.position, destroyvfx.transform.rotation);// destroy user planet when user attack
            //go.transform.localScale = new Vector3(10, 10);

            Vector3 rand = new Vector3(Random.Range(-MeteorBelt.METEOR_FALL_LIMIT, MeteorBelt.METEOR_FALL_LIMIT), Random.Range(-MeteorBelt.METEOR_FALL_LIMIT, MeteorBelt.METEOR_FALL_LIMIT));
            transform.position = rand + new Vector3(MeteorBelt.METEOR_FALL_LIMIT, MeteorBelt.METEOR_FALL_LIMIT);

            //LeanPool.Spawn(MeteorBelt.Instance.explodeObject, transform.position, destroyvfx.transform.rotation);

            float hp = GameManager.sunHp < 0 ? 0 : GameManager.sunHp;
            MeteorBelt.Instance.hpValue.transform.localScale = new Vector3(hp / GameManager.METEOR_FALL_HP, MeteorBelt.Instance.hpValue.transform.localScale.y);
        }
    }
}
