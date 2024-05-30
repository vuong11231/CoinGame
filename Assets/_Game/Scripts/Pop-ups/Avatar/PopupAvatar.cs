using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hellmade.Sound;
using SteveRogers;

public class PopupAvatar : MonoBehaviour
{
    public static PopupAvatar _Instance;

    public Color inactiveColor = Color.white;
    public GameObject Panel = null;

    private int curTickIdx = -1;
    private int startIdx = -1;

    public void SetData()
    {
        curTickIdx = -1;

        foreach (Transform t in Panel.transform.GetChild(1))
        {
            var btn = t.GetChild(0).GetComponent<Button>();
            int index = t.GetSiblingIndex();
            
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnPressed_Avatar(index));

            if (index < SpaceManager.Instance.ListSpace.Count)
            {
                btn.GetComponent<Image>().color = Color.white;
                btn.interactable = true;
            }
            else
            {
                btn.GetComponent<Image>().color = inactiveColor;
                btn.interactable = false;
            }
        }

        if (SpaceManager.Instance.PlanetSelect)
        {
            startIdx = (int)SpaceManager.Instance.PlanetSelect.DataPlanet.skin;
            SetTick(startIdx);
        }
    }

    void SetTick(int idx)
    {
        if (curTickIdx == idx)
            return;

        foreach (Transform t in Panel.transform.GetChild(1))
        {
            var tick = t.GetChild(1).gameObject;
            tick.SetActive(t.GetSiblingIndex() == idx);
        }

        curTickIdx = idx;

        // save

        SpaceManager.Instance.PlanetSelect.DataPlanet.skin = (PLanetName)curTickIdx;
        DataGameSave.SaveToServer();

        // update avatar

        if (SpaceManager.Instance.PlanetSelect)
            SpaceManager.Instance.PlanetSelect.UpdateSkin();
    }

    public void OnPressed_Avatar(int idx)
    {
        SetTick(idx);
    }
}
