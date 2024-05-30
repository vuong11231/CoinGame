using UnityEngine;
using System.Collections;

public class DestroyObjectInTime : MonoBehaviour
{
    public float time;
    public bool isAnim;
    public bool isDestroy;
    // Use this for initialization
    void OnEnable()
    {
        if (!isAnim)
        {
            this.StartDelayMethod(time, DestroyGameObject);
        }
    }

    public void DestroyGameObject()
    {
        try
        {
            if (isDestroy)
            {
                Destroy(gameObject);
            }
            else
            {
                //SmartPool.Instance.Despawn(gameObject);
            }
        }
        catch
        {

        }
    }


}
