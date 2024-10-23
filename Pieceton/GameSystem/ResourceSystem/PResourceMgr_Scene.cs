using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

namespace Pieceton.Misc.Resource
{
    public partial class PResourceMgr
    {
        public static void LoadScene(string _scene_name, LoadSceneMode _scene_mode)
        {
            SceneManager.LoadScene(_scene_name, _scene_mode);
        }

        public static void LoadSceneAsync(string _scene_name, LoadSceneMode _scene_mode)
        {
            SceneManager.LoadSceneAsync(_scene_name, _scene_mode);
        }

        //public void LoadSceneBundle(string _bundle_name, string _scene_name, bool _is_additive, params string[] _arg)
        //{
        //    BundleMgr.LoadScene(_bundle_name, _scene_name, _is_additive, _arg);
        //}

        //public IEnumerator LoadSceneBundleAsync(string _bundle_name, string _scene_name, bool _is_additive, params string[] _arg)
        //{
        //    yield return BundleMgr.LoadSceneAsync(_bundle_name, _scene_name, _is_additive, _arg);
        //}
    }
}