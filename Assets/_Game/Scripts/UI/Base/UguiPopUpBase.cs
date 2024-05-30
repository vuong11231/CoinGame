using System;
using UnityEngine;

namespace uGUI.PopUps
{
	public class UguiPopUpBase : MonoBehaviour
	{
        public PopupType Type;
        public GameObject Container;

        public virtual void Show(params object[] instantiateData)
        {
            this.Container.SetActive(true);
        }

        public virtual void HideOnAwake()
        {
            if (this.Container != null)
                this.Container.SetActive(false);
        }

        public virtual void Hide()
        {
            this.Container.SetActive(false);
        }
    }
}
