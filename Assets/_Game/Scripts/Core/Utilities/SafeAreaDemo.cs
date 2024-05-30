using UnityEngine;
using System;


public class SafeAreaDemo : SingletonMonoDontDestroy<SafeAreaDemo>
{
    [SerializeField] KeyCode KeySafeArea = KeyCode.A;
    SafeArea.SimDevice[] Sims;
    int SimIdx;

    void Awake()
    {
        if (!Application.isEditor)
            Destroy(gameObject);

        Sims = (SafeArea.SimDevice[])Enum.GetValues(typeof(SafeArea.SimDevice));
    }

    public void Init()
    {
        className = "Safe Area Demo";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeySafeArea))
        {
            ToggleSafeArea();
            this.PostEvent(EventID.UpdateSafeArea);
        }
    }

    /// <summary>
    /// Toggle the safe area simulation device.
    /// </summary>
    void ToggleSafeArea()
    {
        SimIdx++;

        if (SimIdx >= Sims.Length)
            SimIdx = 0;

        SafeArea.Sim = Sims[SimIdx];
        Debug.LogFormat("Switched to sim device {0} with debug key '{1}'", Sims[SimIdx], KeySafeArea);
    }
}

