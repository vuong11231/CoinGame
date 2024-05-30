using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupMeteorBuff : MonoBehaviour {
    public static bool selectedBuff = false;

    public List<GameObject> buffs;
    public List<bool> selects;
    public GameObject blackCover;

    //private void Start() {
    //    gameObject.SetActive(false);
    //}

    private void OnEnable() {
        blackCover.SetActive(true);
        selects = new List<bool>();
        for (int i = 0; i < buffs.Count; i++) {
            selects.Add(false);
        }

        //selects = new List<bool>(buffs.Count);

        List<int> showBuff = new List<int>();

        showBuff.Add(0);

        while (showBuff.Count < 5) {
            int rand = Random.Range(0, buffs.Count);
            if (!showBuff.Contains(rand)) {
                showBuff.Add(rand);
            }
        }

        for (int i = 0; i < buffs.Count; i++) {
            buffs[i].SetActive(showBuff.Contains(i));
        }
    }

    int CountSelect() {
        int count = 0;
        selects.ForEach((item) => {
            if (item == true) count++;
        });
        return count;
    }

    public void OnClickSelectBuff(int index) {
        if (!selects[index] && CountSelect() >= 2) {
            return;
        }

        selects[index] = !selects[index];
        buffs[index].GetComponent<Image>().color = selects[index] ? Color.yellow : Color.white;
    }

    public void OnSelectBuffFinish() {
        if (CountSelect() < 2) {
            return;
        }

        for (int i = 0; i < selects.Count; i++) {
            if (selects[0]) {
                MeteorBelt.Instance.MeteorFallBuff_RestoreAllPlanet();
            }
            if (selects[1]) {
                MeteorBelt.Instance.MeteorFallBuff_IncreaseHp();
            }
            if (selects[2]) {
                GameManager.planetSpeedRatio *= 4f;
            }
            if (selects[3]) {
                MeteorBelt.Instance.MeteorFallBuff_TransformAllPlanet(TypePlanet.Air);
            }
            if (selects[4]) {
                MeteorBelt.Instance.MeteorFallBuff_TransformAllPlanet(TypePlanet.Ice);
            }
            if (selects[5]) {
                MeteorBelt.Instance.MeteorFallBuff_TransformAllPlanet(TypePlanet.Antimatter);
            }
            if (selects[6]) {
                MeteorBelt.Instance.MeteorFallBuff_TransformAllPlanet(TypePlanet.Fire);
            }
            if (selects[7]) {
                MeteorBelt.Instance.MeteorFallBuff_TransformAllPlanet(TypePlanet.Gravity);
            }
            if (selects[8]) {
                MeteorBelt.Instance.btnTakeNoDame.SetActive(true);
            }
        }
        selectedBuff = true;
        GetComponent<PopupCustom>().Onclose();
    }

    public void WatchAdsToUnlockBuff() {
        GoogleMobileAdsManager.Instance.ShowRewardedVideo(() => {
            blackCover.SetActive(false);
        });
    }

    //public void RestoreAllPlanet(){
    //    MeteorBelt.Instance.MeteorFallBuff_RestoreAllPlanet();
    //    selectedBuff = true;
    //    GetComponent<PopupCustom>().Onclose();
    //}

    //public void IncreaseHp() {
    //    MeteorBelt.Instance.MeteorFallBuff_IncreaseHp();
    //    selectedBuff = true;
    //    GetComponent<PopupCustom>().Onclose();
    //}

    //public void IncreaseSpeed() {
    //    GameManager.planetSpeedRatio *= 4f;
    //    selectedBuff = true;
    //    GetComponent<PopupCustom>().Onclose();
    //}

    //public void TransformToAir() {
    //    MeteorBelt.Instance.MeteorFallBuff_TransformAllPlanet(TypePlanet.Air);
    //    selectedBuff = true;
    //    GetComponent<PopupCustom>().Onclose();
    //}

    //public void TransformToIce() {
    //    MeteorBelt.Instance.MeteorFallBuff_TransformAllPlanet(TypePlanet.Ice);
    //    selectedBuff = true;
    //    GetComponent<PopupCustom>().Onclose();
    //}

    //public void TransformToGravity() {
    //    MeteorBelt.Instance.MeteorFallBuff_TransformAllPlanet(TypePlanet.Gravity);
    //    selectedBuff = true;
    //    GetComponent<PopupCustom>().Onclose();
    //}

    //public void TransformToFire() {
    //    MeteorBelt.Instance.MeteorFallBuff_TransformAllPlanet(TypePlanet.Fire);
    //    selectedBuff = true;
    //    GetComponent<PopupCustom>().Onclose();
    //}

    //public void TransformToAntiMater() {
    //    MeteorBelt.Instance.MeteorFallBuff_TransformAllPlanet(TypePlanet.Antimatter);
    //    selectedBuff = true;
    //    GetComponent<PopupCustom>().Onclose();
    //}

    //public void TakeNoDame() {
    //    MeteorBelt.Instance.btnTakeNoDame.SetActive(true);
    //    selectedBuff = true;
    //    GetComponent<PopupCustom>().Onclose();
    //}
}
