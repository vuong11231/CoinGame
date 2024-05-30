using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace SteveRogers
{
    public class SpriteAnim : MonoBehaviour
    {
        public Image image = null;
        public SpriteRenderer spriteRenderer = null;

        [SerializeField]
        private List<Anim> anims = null;

        [SerializeField]
        private bool log = false;

        private int currentAnimIndex = -1;
        private string currentAnimName = null;

        private void Start()
        {
            // check if this is sprite

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            // check if this is image

            if (spriteRenderer == null)
            {
                if (image == null)
                {
                    image = GetComponent<Image>();
                }
            }

            if (!image && !spriteRenderer)
                throw new System.Exception("SpriteAnim: not found component image / sprite renderer on go: " + name);
        }

        public void ApplyData(List<Anim> anims)
        {
            this.anims = anims;
        }

        public void Load(string name)
        {
            foreach (var i in anims)
                if (i.name.Equals(name))
                {
                    i.Load();
                    return;
                }
        }

        public void Set(string name) // main
        {
            if (name.Equals(currentAnimName))
                return;

            foreach (var i in anims)
                if (i.name.Equals(name))
                {
                    currentAnimIndex = anims.IndexOf(i);
                    currentAnimName = name;
                    i.Load();

                    if (log)
                        print("current SpriteAnim: " + name);

                    return;
                }

            return;
        }
        
        public void Pause()
        {
            currentAnimIndex = -1;
            currentAnimName = null;
        }

#if UNITY_EDITOR
        private void FixedUpdate()
        {
            foreach (var i in anims)
            {
                if (i.forcePlay)
                {
                    Set(i.name);
                    return;
                }
            }
        }
#endif

        private void Update()
        {
            if (currentAnimIndex == -1)
                return;

            var spr = anims[currentAnimIndex].OnUpdate(Time.deltaTime);

            if (spr)
            {
                if (image)
                    image.sprite = spr;
                else
                    spriteRenderer.sprite = spr;
            }
        }
    }

    [Serializable]
    public class Anim
    {
        [Header("required")]

        public string name = null;

        [SerializeField, Range(0.001f, 1f)]
        private float time = 0.2f;
        private float current = 0;

        [Header("load by path (optional)")]

        [SerializeField]
        private string resourceFilepathPattern = null;

        [SerializeField, Range(1, 3)]
        private int formatNumber = 2;

        [SerializeField, Range(0, 1000)]
        private int startNumber = 0;

        [SerializeField, Range(0, 1000)]
        private int endNumber = 0;

        [Header("load manual (optional)")]

        [SerializeField]
        private List<Sprite> sprites = null;
        private int currentIndex = -1;

#if UNITY_EDITOR
        [Header("debug")]
        public bool forcePlay = false;
#endif

        public Anim(string _name, float _time, string _resourceFilepathPattern, int _formatNumber, int _startNumber, int _endNumber)
        {
            name = _name;
            time = _time;
            resourceFilepathPattern = _resourceFilepathPattern;
            formatNumber = _formatNumber;
            startNumber = _startNumber;
            endNumber = _endNumber;
        }

        public void Load()
        {
            if (sprites != null && sprites.Count > 0)
            {
                sprites.Clear();
                //return;
            }

            sprites = new List<Sprite>();
            var arr = resourceFilepathPattern.Split('#');

            string formatNum = "0";

            for (int i = 1; i < formatNumber; i++)
                formatNum += "0";

            for (int i = startNumber; i <= endNumber; i++)
            {
                string path = string.Format("{0}{1}{2}",
                    arr[0],
                    i.ToString(formatNum),
                    arr[1]);

                var spr = Resources.Load<Sprite>(path);

                if (spr == null)
                    throw new FileNotFoundException(path);

                sprites.Add(spr);
            }
        }

        public Sprite OnUpdate(float dt)
        {
            if (sprites == null || sprites.Count == 0)
                return null;

            current += dt;

            if (current >= time)
            {
                current = 0;
                currentIndex++;
                return sprites[currentIndex % sprites.Count];
            }
            else
                return null;
        }
    }
}