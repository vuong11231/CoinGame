using UnityEngine;
using UnityEngine.UI;

namespace Assets.SimpleAndroidNotifications.Helpers
{
	/// <summary>
	/// Used to show debug messages.
	/// </summary>
	public class Popup : MonoBehaviour
	{
		public CanvasGroup CanvasGroup;
		public Text Message;
		public static Popup Instance;

		public void Awake()
		{
			Instance = this;
		}

		public void ShowMessage(string pattern, params object[] args)
		{
			if (args.Length > 0)
			{
				pattern = string.Format(pattern, args);
			}

			Message.text = pattern;
			CanvasGroup.blocksRaycasts = true;
			CanvasGroup.alpha = 1;
		}

		public void Hide()
		{
			CanvasGroup.blocksRaycasts = false;
			CanvasGroup.alpha = 0;
		}
	}
}