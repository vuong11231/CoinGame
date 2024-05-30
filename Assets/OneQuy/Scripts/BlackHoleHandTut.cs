using UnityEngine;
using SteveRogers;

public class BlackHoleHandTut : MonoBehaviour
{
    public RectTransform hand;
    public RectTransform startPoint;
    public RectTransform targetPoint;

    void Start()
    {
        if (TutMan.IsDone(TutMan.TUT_KEY_HAND_BLACK_HOLE))
        {
            Destroy(gameObject);
            return;
        }

        Play();
    }

    private void Play()
    {
        hand.localPosition = startPoint.localPosition;

        hand.gameObject.LeanMoveLocal(targetPoint.localPosition, Cheat.Get("tmp", 1))
            .setOnComplete(() =>
            {
                Play();
            });
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            LeanTween.cancel(gameObject);
            Destroy(gameObject);
            TutMan.SetTutDone(TutMan.TUT_KEY_HAND_BLACK_HOLE);
        }
    }
}
