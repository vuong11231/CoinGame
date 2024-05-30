using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SteveRogers
{
    [DisallowMultipleComponent]
    public class LocalizedText : MonoBehaviour
    {
        public string id = null;

        private bool updated = false;

        private void Update()
        {
            if (!TextMan.Inited || updated)
                return;

            var content = TextMan.Get(id);

            if (content == null)
            {
                updated = true;
                return;
            }

            var text = GetComponent<Text>();

            if (text)
            {
                text.text = TextMan.Get(id);

                //Debug.Log(text.text, text.gameObject);

                updated = true;
                return;
            }

            var textMeshPro = GetComponent<TextMeshProUGUI>();

            if (textMeshPro)
            {
                textMeshPro.text = TextMan.Get(id);
                
                //Debug.Log(textMeshPro.text, textMeshPro.gameObject);

                updated = true;
                return;
            }
        }

        public void ReUpdate()
        {
            updated = false;
        }
    }
}