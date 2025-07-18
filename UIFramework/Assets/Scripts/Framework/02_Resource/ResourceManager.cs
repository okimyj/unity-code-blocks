using YJFramework.Core;
using UnityEngine;

namespace YJFramework.Resource
{
    public class ResourceManager : SingletonMonoDontDestroyBehaviour<ResourceManager>
    {
        public void Initialize()
        {

        }
        public T LoadAsset<T>(ResKey key, string assetName) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(assetName))
            {
                GameLogger.LogError("[ResourceManager]::LoadAsset() - AssetName is empty.");
                return null;
            }
            if (key.UsePackage)
            {
                var path = $"{key.BaseDir}{assetName}";
                var res = Resources.Load<T>(path);
                if(res == null)
                {
                    GameLogger.LogError($"[ResourceManager]::LoadAsset() - Failed. path : {path}");
                }
                return res;
            }
            // TODO : simulate mode, bundle load.
            return null;
        }
        
    }

}
