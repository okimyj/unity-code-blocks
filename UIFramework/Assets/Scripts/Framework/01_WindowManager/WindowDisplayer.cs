using System.Collections;
using System.Collections.Generic;
using UIFramework.Core;
using UnityEngine;
namespace UIFramework.Window
{
    public partial class WindowDisplayer : SingletonMonoDontDestroyBehaviour<WindowDisplayer>
    {
        private bool isInitialized = false;
        public void Initialize()
        {
            if (isInitialized)
                return;
            isInitialized = true;
            gameObject.layer = LayerMask.NameToLayer("UI");
        }
    }

}
