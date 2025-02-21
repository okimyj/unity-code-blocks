using UIFramework.Core;
using UnityEngine;
namespace UIFramework.Resource
{
    public class ResourceManager : SingletonMonoDontDestroyBehaviour<ResourceManager>
    {
        public void Initialize()
        {

        }    
        public T Load<T>(ResKey key, string assetName) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(assetName))
            {
                GameLogger.LogError("[ResourceManager]::Load() - AssetName is empty.");
                return null;
            }
            if (key.UsePackage)
            {
                var path = $"{key.BaseDir}{assetName}";
                var res = Resources.Load<T>(path);
                if(res == null)
                {
                    GameLogger.LogError($"[ResourceManager]::Load() - Failed. path : {path}");
                }
                return res;
            }
            // TODO : simulate mode, bundle load.
            return null;
        }
        
    }

}
