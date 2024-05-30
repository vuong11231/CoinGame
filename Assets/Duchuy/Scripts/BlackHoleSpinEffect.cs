using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class BlackHoleSpinEffect : MonoBehaviour {
    public Transform pivot;
    public GameObject destroyrEffect;
    public BlackHole blackHole;
    public GameObject particalSystem;

    private float START_SPEED = 500f;
    private float ACCELERATE = 70f;
    private float SMALLING_SPEED = 0.1f;

    private float GRAVITY_RATE_START = 0.4f;
    private float GRAVITY_RATE_ACCELERATE = 0.01f;

    private float START_SIZE = 1f;
    public float DESROY_RANGE = 1.2f;
    public float MAX_EFFECT_SPINNING_TIME = 10f;

    public DataReward reward;

    bool isSpinning = false;
    float speed = 0f;
    float gravityRate = 0f;
    float effectTime = Mathf.Infinity;

    void Update() {
        if (isSpinning) {
            if (Time.time > effectTime) {
                effectTime = Mathf.Infinity;
                DoBlackHoleEffect();
            }

            speed += ACCELERATE * Time.deltaTime;
            gravityRate += GRAVITY_RATE_ACCELERATE * Time.deltaTime;
            Vector2 vec = transform.position - pivot.position;
            Vector2 perpendicular = -Vector2.Perpendicular(vec).normalized * Time.deltaTime * speed;
            Vector2 gravity = -vec.normalized * Time.deltaTime * speed * gravityRate;
            GetComponent<Rigidbody2D>().velocity = perpendicular + gravity;

            transform.localScale = transform.localScale - new Vector3(SMALLING_SPEED, SMALLING_SPEED) * Time.deltaTime;
            if (Vector3.Distance(transform.position, pivot.transform.position) < DESROY_RANGE) {
                DoBlackHoleEffect();
            }
        }
    }

    public void DoBlackHoleEffect() {
        particalSystem.SetActive(true);
        particalSystem.GetComponent<ParticleSystem>().Play();

        LeanTween.delayedCall(3f, () => {
            if (particalSystem)
                particalSystem.SetActive(false);
        });

        Destroy(gameObject);
        GameObject effect = LeanPool.Spawn(destroyrEffect);
        effect.transform.position = transform.position;

        BlackHole.Instance.SpawnRewardVisual(reward);
        BlackHole.flyingRewards.Remove(reward);

        StartCoroutine(DestroyGameObject(effect));
    }

    public void StartSpinning()
    {
        isSpinning = true;
        effectTime = Time.time + MAX_EFFECT_SPINNING_TIME;
        speed = START_SPEED;
        gravityRate = GRAVITY_RATE_START;
        transform.localScale = new Vector3(START_SIZE, START_SIZE);

        reward = BlackHole.Instance.ConsumeStone();
        BlackHole.flyingRewards.Add(reward);
    }

    public IEnumerator DestroyGameObject(GameObject gameObject)
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(3f, 6f));
        Destroy(gameObject);
    }


}
