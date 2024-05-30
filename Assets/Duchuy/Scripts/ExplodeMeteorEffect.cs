using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExplodeMeteorEffect : MonoBehaviour
{
    public float FLYING_FORCE = -100f;
    public Transform des;
    public Rigidbody2D rb2d;
    Vector3 destination;

    void Start()
    {
        //destination = Camera.main.ScreenToWorldPoint(des.position);
    }

    void Update()
    {
        Vector3 toDestination = transform.position - des.position;
        toDestination = toDestination.normalized * FLYING_FORCE;
        rb2d.velocity += (Vector2) toDestination;

        if (transform.position.y > des.position.y - 4f)// toDestination.magnitude < 1f)
            Destroy(gameObject);
    }
}
