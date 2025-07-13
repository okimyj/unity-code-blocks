using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace YJClient
{
    public class RedDotCheckerShop : RedDotCheckerBase
    {
        public RedDotCheckerShop() : base(EContentsType.Shop) { }
        public override void Refresh()
        {
            // Refresh RedDot.
            HasRedDot = true;
        }
    }

}
