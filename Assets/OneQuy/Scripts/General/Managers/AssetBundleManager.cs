using UnityEngine;
using System.IO;
using UnityEngine.U2D;
using System;
using System.Collections.Generic;

namespace SteveRogers
{
    public partial class AssetBundleManager : SingletonTiny<AssetBundleManager>
    {
        public enum LoadError
        {
            None,
            FileNotFound,
            LoadBundleFail,
            AssetNotFound
        }
    }

    public partial class AssetBundleManager : SingletonTiny<AssetBundleManager>
    {
        #region Core (Private)

        private static string dir = null;
        private Dictionary<string, AssetBundle> loadeds = null; // Path can be full filepath / resource path / filename; Bundle is not null
        private Dictionary<string, Action<UnityEngine.Object>> onDoneList = null; // Path can be full filepath / resource path / filename; 

        public void Dispose(bool unloadAllObjects) 
        {
            if (loadeds.IsValid())
                loadeds.Clear();

            if (onDoneList.IsValid())
                onDoneList.Clear();

            AssetBundle.UnloadAllAssetBundles(unloadAllObjects);
        }

        public AssetBundleManager()
        {
            dir = Utilities.GetProjectPath(true) + Utilities.ASSET_BUNDLE_FOLDER_NAME;
        }

        private string GetRealFilepath(string filepath_or_filename) // Resource path will return null 
        {
            if (!File.Exists(filepath_or_filename)) // Filename / Resource
            {
                string path = path = GetDir(true) + filepath_or_filename;

                if (!File.Exists(path)) // Resource
                    return null;
                else // Filename
                    return path;
            }
            else // Filepath
                return filepath_or_filename;
        }

        private void InvokeOnDoneList(UnityEngine.Object o, string realpath_filename_or_resourcepath)
        {
            if (onDoneList == null || !onDoneList.ContainsKey(realpath_filename_or_resourcepath))
                return;
            else
            {
                onDoneList[realpath_filename_or_resourcepath].Invoke(o);
                onDoneList.Remove(realpath_filename_or_resourcepath);
            }
        }

        private void LoadAssetAsync(AssetBundle bundle, string assetName, string realpath_filename_or_resourcepath)
        {
            if (bundle == null)
            {
                if (onDoneList == null || !onDoneList.ContainsKey(realpath_filename_or_resourcepath))
                    return;
                else
                    onDoneList[realpath_filename_or_resourcepath].Invoke(null);
            }
            else
            {
                var load = bundle.LoadAssetAsync(assetName);

                load.completed +=
                    (i) =>
                    {
                        InvokeOnDoneList(load.asset, realpath_filename_or_resourcepath);
                    };
            }
        }

        private void AddLoadedBundle(AssetBundle bundle, string filepath_or_filename_or_resourcepath)
        {
            filepath_or_filename_or_resourcepath = filepath_or_filename_or_resourcepath.ToLower();

            if (string.IsNullOrEmpty(filepath_or_filename_or_resourcepath) ||
                bundle == null ||
                (loadeds != null && loadeds.ContainsKey(filepath_or_filename_or_resourcepath)))
            {
                Debug.LogError("AddLoadedBundle fail (already contains this bundle): " + bundle + " | " + filepath_or_filename_or_resourcepath);
                return;
            }

            if (loadeds == null)
                loadeds = new Dictionary<string, AssetBundle>();

            loadeds[filepath_or_filename_or_resourcepath] = bundle;
        }

        private AssetBundle GetLoadedBundle(string filepath_or_filename_or_resourcepath)
        {
            if (loadeds == null || string.IsNullOrEmpty(filepath_or_filename_or_resourcepath))
            {
                return null;
            }

            return loadeds.TryGetFromDictionary(filepath_or_filename_or_resourcepath.ToLower());
        }

        #endregion

        #region Load (Public)

        // Sync

        public UnityEngine.Object LoadAsset(string filepath_or_filename_or_resourcepath, string assetName, out LoadError error) // Main 
        {
            string path = filepath_or_filename_or_resourcepath;
            AssetBundle bundle = GetLoadedBundle(path);
            TextAsset textAsset = null;

            if (bundle == null)
                path = GetRealFilepath(filepath_or_filename_or_resourcepath);

            if (string.IsNullOrEmpty(path)) // Resource path
            {
                textAsset = Resources.Load<TextAsset>(filepath_or_filename_or_resourcepath);

                if (textAsset != null)
                    path = filepath_or_filename_or_resourcepath;
            }

            if (string.IsNullOrEmpty(path))
            {
                error = LoadError.FileNotFound;
                return null;
            }
            else
            {
                if (!bundle)
                {
                    if (textAsset == null)
                        bundle = AssetBundle.LoadFromFile(path);
                    else
                        bundle = AssetBundle.LoadFromMemory(textAsset.bytes);

                    if (bundle)
                    {
                        AddLoadedBundle(bundle, filepath_or_filename_or_resourcepath);
                    }
                }

                if (bundle)
                {
                    if (bundle.Contains(assetName))
                    {
                        error = LoadError.None;
                        return bundle.LoadAsset<UnityEngine.Object>(assetName);
                    }
                    else
                    {
                        error = LoadError.AssetNotFound;
                        return null;
                    }
                }
                else
                {
                    error = LoadError.LoadBundleFail;
                    return null;
                }
            }
        }

        //public bool LoadBundle(string filepath_or_filename_or_resourcepath)
        //{
        //    string path = filepath_or_filename_or_resourcepath;
        //    AssetBundle bundle = GetLoadedBundle(path);
        //    TextAsset textAsset = null;

        //    if (bundle == null)
        //        path = GetRealFilepath(filepath_or_filename_or_resourcepath);

        //    if (string.IsNullOrEmpty(path)) // Resource path
        //    {
        //        textAsset = Resources.Load<TextAsset>(filepath_or_filename_or_resourcepath);

        //        if (textAsset != null)
        //            path = filepath_or_filename_or_resourcepath;
        //    }

        //    if (string.IsNullOrEmpty(path))
        //    {
        //        Debug.LogError("Filepath not existed: " + filepath_or_filename_or_resourcepath);
        //        return false;
        //    }
        //    else
        //    {
        //        if (!bundle)
        //        {
        //            if (textAsset == null)
        //                bundle = AssetBundle.LoadFromFile(path);
        //            else
        //                bundle = AssetBundle.LoadFromMemory(textAsset.bytes);

        //            if (bundle)
        //                AddLoadedBundle(bundle, filepath_or_filename_or_resourcepath);
        //            else
        //                return false;
        //        }

        //        return true;
        //    }
        //}

        public TextAsset LoadTextAsset(string filepath_or_filename_or_resourcepath, string assetName, out LoadError error) // Sub 
        {
            return LoadAsset(filepath_or_filename_or_resourcepath, assetName, out error) as TextAsset;
        }

        public GameObject LoadGameObject(string filepath_or_filename_or_resourcepath, string assetName, out LoadError error) // Sub 
        {
            return LoadAsset(filepath_or_filename_or_resourcepath, assetName, out error) as GameObject;
        }

        public SpriteAtlas LoadSpriteAtlas(string filepath_or_filename_or_resourcepath, string assetName, out LoadError error) // Sub 
        {
            return LoadAsset(filepath_or_filename_or_resourcepath, assetName, out error) as SpriteAtlas;
        }

        // Async

        public void LoadAssetAsync(string filepath_or_filename_or_resourcepath, string assetName, Action<UnityEngine.Object> onDone) // Main 
        {
            // Check if this is loading?

            if (onDone == null)
                onDone = (o) => {};

            if (onDoneList != null && onDoneList.ContainsKey(filepath_or_filename_or_resourcepath)) // Is loading 
            {
                onDoneList[filepath_or_filename_or_resourcepath] += onDone;
                return;
            }
            else if (onDoneList == null) // Not loading
            {
                onDoneList = new Dictionary<string, Action<UnityEngine.Object>>();
            }

            onDoneList[filepath_or_filename_or_resourcepath] = onDone;

            // Load bundle

            TextAsset textAsset = null;
            string realpath = filepath_or_filename_or_resourcepath;
            AssetBundle bundle = GetLoadedBundle(realpath);

            if (bundle == null)
                realpath = GetRealFilepath(filepath_or_filename_or_resourcepath);

            if (string.IsNullOrEmpty(realpath)) // This maybe Resource path?
            {
                textAsset = Resources.Load<TextAsset>(filepath_or_filename_or_resourcepath);

                if (textAsset != null)
                    realpath = filepath_or_filename_or_resourcepath;
            }

            if (string.IsNullOrEmpty(realpath))
            {
                Debug.LogError("Filepath not existed at LoadAssetAsync! " + filepath_or_filename_or_resourcepath);
                InvokeOnDoneList(null, filepath_or_filename_or_resourcepath);
            }
            else
            {
                if (bundle)
                    LoadAssetAsync(bundle, assetName, filepath_or_filename_or_resourcepath);
                else
                {
                    AssetBundleCreateRequest load;

                    if (textAsset)
                        load = AssetBundle.LoadFromMemoryAsync(textAsset.bytes);
                    else
                        load = AssetBundle.LoadFromFileAsync(realpath);

                    load.completed += (i) =>
                    {
                        if (load.assetBundle)
                        {
                            AddLoadedBundle(load.assetBundle, filepath_or_filename_or_resourcepath);
                            LoadAssetAsync(load.assetBundle, assetName, filepath_or_filename_or_resourcepath);
                        }
                        else
                        {
                            Debug.LogError("Load bundle fail at LoadAssetAsync! " + realpath);
                            InvokeOnDoneList(null, filepath_or_filename_or_resourcepath);
                        }
                    };
                }
            }
        }

        public void LoadGameObjectAsync(string filepath_or_filename_or_resourcepath, string assetName, Action<UnityEngine.GameObject> onDone) // Sub 
        {
            LoadAssetAsync(filepath_or_filename_or_resourcepath, assetName, (o) =>
            {
                if (onDone != null)
                    onDone(o as GameObject);
            });
        }

        public void LoadSpriteAtlasAsync(string filepath_or_filename_or_resourcepath, string assetName, Action<UnityEngine.U2D.SpriteAtlas> onDone) // Sub 
        {
            LoadAssetAsync(filepath_or_filename_or_resourcepath, assetName, (o) =>
            {
                if (onDone != null)
                    onDone(o as UnityEngine.U2D.SpriteAtlas);
            });
        }

        #endregion

        #region Others (Public)

        public string GetDir(bool withSeparator)
        {
            return withSeparator ? dir + System.IO.Path.DirectorySeparatorChar : dir;
        }

        public string[] GetAllAssetNames(string filepath_or_filename_or_resourcepath)
        {
            var bundle = GetLoadedBundle(filepath_or_filename_or_resourcepath);

            if (bundle == null)
            {
                Debug.LogError(Utilities.CreateLogContent("GetAllAssetNames", "Bundle not loaded!", filepath_or_filename_or_resourcepath));
                return null;
            }
            else
                return bundle.GetAllAssetNames();
        }

        public void Unload(string filepath_or_filename_or_resourcepath, bool unloadAllLoadedObjects)
        {
            var bundle = GetLoadedBundle(filepath_or_filename_or_resourcepath);

            if (!bundle)
                return;

            loadeds.Remove(filepath_or_filename_or_resourcepath);
            bundle.Unload(unloadAllLoadedObjects);
        }

        #endregion
    }
}