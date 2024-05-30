using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if SteveAdmob
using GoogleMobileAds.Api;
#endif

namespace SteveRogers
{
    public class AdmobBannerController : MonoBehaviour
    {
#if SteveAdmob

        public bool show = false;
        public AdPosition position = AdPosition.Bottom;


        private IEnumerator Start()
        {
            if (Application.isEditor)
                yield break;

            while (true)
            {
                if (AdmobMan.StateBanner == AdmobMan.BannerState.Showing)
                    AdmobMan.SetBannerPosition(position);

                yield return Utilities.WAIT_FOR_ONE_SECOND;
            }
        }

        private void Update()
        {
            if (Application.isEditor)
                return;

            if (AdmobMan.StateBanner == AdmobMan.BannerState.Showing)
            {
                if (!show)
                {
                    AdmobMan.HideBanner(true);
                }
            }
            else if (AdmobMan.StateBanner == AdmobMan.BannerState.Hiding)
            {
                if (show)
                {
                    AdmobMan.HideBanner(false);
                }
            }
        }

#endif // #if SteveAdmob
    }
}