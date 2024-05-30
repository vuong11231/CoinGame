using Lean.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx.Async.Triggers;
using UnityEngine;
using TMPro;
using SteveRogers;

[RequireComponent(typeof(Rigidbody2D))]
public class DCamera : MonoBehaviour
{
    public static float MAX_SHOOT_RADIUS = 580f;

    public GameObject pivot;

    public GameObject target;
    public float maxDistance = 4f;
    public float speed = 50f;
    public GameObject returnToSunButton;

    public GameObject background;

    private Vector3 vectorToTarget;
    private Rigidbody2D rigidbody2D;

    void Start()
    {
        var renderer = GetComponent<SpriteRenderer>();
        if (renderer != null) renderer.enabled = false;
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.isKinematic = true;
        rigidbody2D.gravityScale = 0;
        returnToSunButton.SetActive(false);
    }

    void Update()
    {
        float distanceToPivot = Vector3.Distance(transform.position, pivot.transform.position);

        if (distanceToPivot > MAX_SHOOT_RADIUS)
        {
            ReturnToSun();
        }

        if (distanceToPivot < 3f)
        {
            target = null;
        }

        if (target != null)
            GoToTargetPosition(maxDistance);
    }

    public void DestroyShootingMeteor()
    {
        DataGameSave.dataServer.ListPlanet[MeteorBelt.Instance.shootingIndex].Type = TypePlanet.Destroy;
        PlayfabManager.GetServerTime(
            0,
            (result, index) =>
            {
                DataGameSave.dataServer.ListPlanet[MeteorBelt.Instance.shootingIndex].ShootTime = result;
            });

        GameStatics.IsAnimating = false;

        GameObject shootingPlanet = (transform.parent != null) ? transform.parent.gameObject : null;
        MeteorBelt.Instance.ShootMeteorBeltEnd(shootingPlanet);
    }

    void GoToTargetPosition(float maxDistanceToTarget)
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        if (distanceToTarget == Mathf.Abs(transform.position.z) && target.CompareTag("Pivot"))
        {
            rigidbody2D.velocity = new Vector3(0f, 0f);
            target = null;
            return;
        }

        if (distanceToTarget <= maxDistanceToTarget)
        {
            rigidbody2D.velocity = new Vector3(0f, 0f);
            return;
        }
        else
        {
            vectorToTarget = target.transform.position - transform.position;
            vectorToTarget = vectorToTarget * speed / distanceToTarget;
            rigidbody2D.velocity = vectorToTarget;
        }
    }

    public void MoveTo(Transform target)
    {
        Vector3 pos = transform.position;
        pos.x = target.position.x;
        pos.y = target.position.y;
        transform.position = pos;
    }

    public void ReturnToSun()
    {
        transform.SetParent(null);
        target = pivot;

        if (MeteorBelt.Instance != null)
            DestroyShootingMeteor();

        returnToSunButton.SetActive(false);
    }


}
