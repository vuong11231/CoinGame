using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SteveRogers;


public class ChangeImageByLanguage : MonoBehaviour
{
    public List<Sprite> imgs;

    void Start()
    {
        if (imgs.Count >= TextMan.LangIndex - 1) {
            GetComponent<Image>().sprite = imgs[TextMan.LangIndex];
        }
    }
}
