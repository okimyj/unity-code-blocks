using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

using Pieceton.Configuration;
using Pieceton.Misc;
using Pieceton.BuildPlug;

namespace Pieceton.PatchSystem
{
    public static class PiecetonBuilder
    {
        public static void Package(string _pieceton_object_name)
        {
            PBuildPlug objectInst = PBuildPlug.LoadObject(_pieceton_object_name);
            Package(objectInst);
        }
        public static void Package(PBuildPlug _build_plug)
        {
            PiecetonPackageBuilder.Build(_build_plug);
        }


        public static void Bundle(string _pieceton_object_name)
        {
            PBuildPlug objectInst = PBuildPlug.LoadObject(_pieceton_object_name);
            Bundle(objectInst);
        }
        public static void Bundle(PBuildPlug _build_plug)
        {
            PiecetonBundleBuilder.Build(_build_plug);
        }
    }
}