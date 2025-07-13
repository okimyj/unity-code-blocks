using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace YJClient
{
    public class RedDotCheckerHero : RedDotCheckerBase
    {
        public RedDotCheckerHero() : base(EContentsType.Hero) { }
        public override void Refresh()
        {
            // Refresh RedDot.
            HasRedDot = true;
        }
    }

}
