using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bl_Shaker : Singleton<bl_Shaker>
{
    [Header("Shake")]
    public Transform ShakeObject = null;
    private Vector3 originPosition;
    private Quaternion originRotation;
    private float shakeIntensity;
    [Header("Presents")]
    public List<Info> ShakesPresents = new List<Info>();

    void Start()
    {
        originPosition = ShakeObject.localPosition;
        originRotation = ShakeObject.localRotation;

        foreach(Info i in ShakesPresents) { i.Init(); }
    }

    public void Do()
    {
        StopAllCoroutines();
        ShakeObject.localPosition = originPosition;
        ShakeObject.localRotation = originRotation;
        StartCoroutine(Shake(ShakesPresents[0]));
    }

    public void Do(int index)
    {
        StopAllCoroutines();
        ShakeObject.localPosition = originPosition;
        ShakeObject.localRotation = originRotation;
        StartCoroutine(Shake(ShakesPresents[index]));
    }

    public void Do(Info inf)
    {
        StopAllCoroutines();
        ShakeObject.localPosition = originPosition;
        ShakeObject.localRotation = originRotation;
        StartCoroutine(Shake(inf));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IEnumerator Shake(Info _info)
    {
        yield return new WaitForSeconds(_info.Delay);
        shakeIntensity = _info.ShakeIntensity;
        Transform t = (_info.ShakeObject == null) ? ShakeObject : _info.ShakeObject;
        bool overrid = (_info.ShakeObject != null);
        while (shakeIntensity > 0)
        {
            Vector3 rp = originPosition + Random.insideUnitSphere * shakeIntensity;
            if (_info.is2D)
            {
                rp.z = (overrid) ? _info.OriginPosition.z : originPosition.z;
            }

            t.localPosition = rp;
            if (_info.useRotation)
            {
                Quaternion originRot = (overrid) ? _info.OriginRotation : originRotation;
                if (_info.is2D)
                {
                   t.localRotation = new Quaternion(
                   originRot.x ,
                   originRot.y,
                   originRot.z + Random.Range(-shakeIntensity, shakeIntensity) * _info.ShakeAmount,
                   originRot.w);
                }
                else
                {
                    t.localRotation = new Quaternion(
                    originRot.x + Random.Range(-shakeIntensity, shakeIntensity) * _info.ShakeAmount,
                    originRot.y + Random.Range(-shakeIntensity, shakeIntensity) * _info.ShakeAmount,
                    originRot.z + Random.Range(-shakeIntensity, shakeIntensity) * _info.ShakeAmount,
                    originRot.w + Random.Range(-shakeIntensity, shakeIntensity) * _info.ShakeAmount);
                }
            }
            shakeIntensity -= _info.ShakeDecay;
            yield return false;
        }

        if (!overrid)
        {
            ShakeObject.localPosition = originPosition;
            if (_info.useRotation)
            {
                ShakeObject.localRotation = originRotation;
            }
        }

        _info.Reset();
    }

    [System.Serializable]
    public class Info
    {
        public string Name = "Shake Name";
        [Range(0.001f, 0.01f)]
        public float ShakeDecay = 0.002f;
        [Range(0.01f, 0.2f)]
        public float ShakeIntensity = 0.02f;
        [Range(0.01f, 0.5f)]
        public float ShakeAmount = 0.2f;
        [Range(0f, 5f)]
        public float Delay = 0f;
        public bool useRotation = true;
        public bool is2D = false;
        public Transform ShakeObject = null;

        [HideInInspector]public  Vector3 OriginPosition;
        [HideInInspector]public Quaternion OriginRotation;

        public void Init()
        {
            if(ShakeObject != null)
            {
                OriginPosition = ShakeObject.localPosition;
                OriginRotation = ShakeObject.localRotation;
            }
        }

        public void Reset()
        {
            if (ShakeObject != null)
            {
                ShakeObject.localPosition = OriginPosition;
                ShakeObject.localRotation = OriginRotation;
            }
        }
    }
}