using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace SteveRogers
{
    public class AtlasHub : System.IDisposable
    {
        private SpriteAtlas atlas = null;
        private Dictionary<string, Sprite> loadedSprites = null;

        public void Dispose()
        {
            GameObject.Destroy(atlas);
            atlas = null;

            if (loadedSprites != null)
            {
                loadedSprites.Clear();
                loadedSprites = null;
            }
        }

        public AtlasHub(SpriteAtlas atlas)
        {
            if (atlas == null)
            {
                throw new Exception("Atlas null!");
            }

            this.atlas = atlas;
        }


        public Sprite GetSprite(string name)
        {
            if (name.IsNullOrEmpty())
                return null;

            Sprite rs = null;

            // Already has in dic

            if (loadedSprites != null && loadedSprites.TryGetValue(name, out rs))
                return rs;

            // Not has in dic yet

            rs = atlas.GetSprite(name);

            if (loadedSprites == null)
                loadedSprites = new Dictionary<string, Sprite>();

            loadedSprites[name] = rs;

            return rs;
        }
    }
}