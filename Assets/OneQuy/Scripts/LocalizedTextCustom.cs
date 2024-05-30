using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SteveRogers
{
    [DisallowMultipleComponent]
    public class LocalizedTextCustom : MonoBehaviour
    {
        [Multiline]
        public List<string> messages;

        private bool updated = false;

        private void Update()
        {
            if (!TextMan.Inited || updated || TextMan.LangIndex > messages.Count-1)
                return;

            var content = messages[TextMan.LangIndex];

            if (content == null)
            {
                updated = true;
                return;
            }

            var text = GetComponent<Text>();

            if (text)
            {
                text.text = content;
                updated = true;
                return;
            }

            var textMeshPro = GetComponent<TextMeshProUGUI>();

            if (textMeshPro)
            {
                textMeshPro.text = content;
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