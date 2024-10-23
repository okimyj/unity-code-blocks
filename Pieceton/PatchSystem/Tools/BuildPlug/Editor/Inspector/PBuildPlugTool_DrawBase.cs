using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

using Pieceton.Misc;

namespace Pieceton.BuildPlug
{
    public class PBuildPlugTool_DrawBase
    {
        protected static PBuildPlug objectInst = null;

        protected static bool DrawBegin(PBuildPlug _build_plug, string _title)
        {
            if (!IsValidSelection())
                return false;

            //EditorGUILayout.HelpBox(string.Format("[{0}]", _title), MessageType.None);
            EditorGUILayout.LabelField(string.Format("[{0}]", _title));

            if (null == _build_plug)
                return false;

            objectInst = _build_plug;

            return true;
        }

        public static bool TextField(PBuildPlug _build_plug, string _label, ref string _val)
        {
            string ori = _val;
            string nVal = EditorGUILayout.TextField(_label, _val);
            if (nVal != ori)
            {
                _val = nVal;
                //EditorUtility.SetDirty(_build_plug);
                _build_plug.SetHasDirty();
                return true;
            }

            return false;
        }

        public static bool TextField(PBuildPlug _build_plug, ref string _val)
        {
            string ori = _val;
            string nVal = EditorGUILayout.TextField(_val);
            if (nVal != ori)
            {
                _val = nVal;
                //EditorUtility.SetDirty(_build_plug);
                _build_plug.SetHasDirty();
                return true;
            }

            return false;
        }

        public static bool IntField(PBuildPlug _build_plug, string _label, ref int _val)
        {
            int ori = _val;
            int nVal = EditorGUILayout.IntField(_label, _val);
            if (nVal != ori)
            {
                _val = nVal;
                //EditorUtility.SetDirty(_build_plug);
                _build_plug.SetHasDirty();
                return true;
            }

            return false;
        }

        public static bool Toggle(PBuildPlug _build_plug, string _label, ref bool _val)
        {
            bool ori = _val;
            bool nVal = EditorGUILayout.Toggle(_label, _val);
            if (nVal != ori)
            {
                _val = nVal;
                //EditorUtility.SetDirty(_build_plug);
                _build_plug.SetHasDirty();
                return true;
            }

            return false;
        }

        public static bool IsValidSelection()
        {
            if (null == Selection.activeObject)
                return false;

            if (!(Selection.activeObject is PBuildPlug))
                return false;

            return true;
        }

        //public static void Resize<T>(this List<T> list, int size, T element = default(T))
        public static void Resize<T>(List<T> list, int size, T element = default(T))
        {
            int count = list.Count;

            if (size < count)
            {
                list.RemoveRange(size, count - size);
            }
            else if (size > count)
            {
                if (size > list.Capacity)   // Optimization
                    list.Capacity = size;

                list.AddRange(Enumerable.Repeat(element, size - count));
            }
        }
    }
}