using System.Collections;
using System.Collections.Generic;
using UIFramework.Core;
using UnityEngine;
namespace UIFramework.Window
{
    public partial class WindowDisplayer : SingletonMonoDontDestroyBehaviour<WindowDisplayer>
    {
        private Transform holder;
        private Transform topHolder;
        public WindowDisplayer(Transform _holder, Transform _topHolder)
        {
            holder = _holder;
            topHolder = _topHolder;
        }
    }

}
