////////////////////////////////////////////////////////////////////////////
// bl_FloatingText
//
//
//                    Lovatto Studio 2016
////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class bl_FloatingText : MonoBehaviour
{
    [SerializeField]private Text m_Text;
    private Canvas m_Canvas;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_info"></param>
    public void Instance(Info _info)
    {
        m_Canvas = transform.root.GetComponent<Canvas>();
        if(m_Canvas == null)
        {
            Destroy(gameObject);
        }

        transform.position = m_Canvas.CalculatePositionFromTransformToRectTransform(_info.InitPosition, Camera.main);
        m_Text.text = _info.Text;
        StartCoroutine(OnUpdate(_info));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_info"></param>
    /// <returns></returns>
    IEnumerator OnUpdate(Info _info)
    {
        while (_info.Delay > 0)
        {
            transform.position += _info.FloatDirection * (Time.deltaTime * _info.Speed);
            _info.Delay -= Time.deltaTime;
            yield return null;
        }
        float alpha = m_Text.color.a;
        Color c = m_Text.color;
        while(alpha > 0)
        {
            transform.position += _info.FloatDirection * Time.deltaTime * _info.Speed;
            alpha -= Time.deltaTime;
            c.a = alpha;
            m_Text.color = c;
            yield return null;
        }
        Destroy(gameObject);
    }

    [System.Serializable]
    public class Info
    {
        public string Text;
        public Vector3 InitPosition;
        public Vector3 FloatDirection;
        public float Speed;
        public float Delay;
    }

}