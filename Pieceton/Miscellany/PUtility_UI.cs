using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Pieceton.Misc.UI
{
    public static class PUI
    {
        public static bool IsActive(Component _obj)
        {
            if (null != _obj)
            {
                return IsActive(_obj);
            }

            return false;
        }
        public static bool IsActive(GameObject _obj)
        {
            if (null != _obj)
            {
                return _obj.activeSelf;
            }

            return false;
        }

        public static void SetActive(Component _obj, bool _active)
        {
            if (null != _obj)
            {
                SetActive(_obj.gameObject, _active);
            }
        }

        public static void SetActive(MaskableGraphic _obj, bool _active)
        {
            if (null != _obj)
            {
                SetActive(_obj.gameObject, _active);
            }
        }

        public static void SetActive(Selectable _obj, bool _active)
        {
            if (null != _obj)
            {
                SetActive(_obj.gameObject, _active);
            }
        }

        public static void SetActive(GameObject _obj, bool _active)
        {
            if (null != _obj)
            {
                if (_obj.activeSelf != _active)
                {
                    _obj.SetActive(_active);
                }
            }
        }

        public static void SetEnable(MaskableGraphic _obj, bool _enable)
        {
            if (null != _obj)
            {
                _obj.enabled = _enable;
            }
        }

        public static void SetColor(MaskableGraphic _obj, Color _color)
        {
            if (null != _obj)
            {
                _obj.color = _color;
            }
        }

        public static void SetColor(Button _obj, Color _color)
        {
            if (null != _obj)
            {
                if (null != _obj.targetGraphic)
                {
                    _obj.targetGraphic.color = _color;
                }
            }
        }
        
        public static void SetInteractive(Selectable _obj, bool _interactive)
        {
            if (null != _obj)
            {
                if (_obj.interactable != _interactive)
                {
                    _obj.interactable = _interactive;
                }
            }
        }

        public static bool IsInteractive(Selectable _obj, bool _default = false)
        {
            if (null != _obj)
            {
                return _obj.interactable;
            }

            return _default;
        }

        public static void SetToggleOn(Toggle _obj, bool _is_on)
        {
            if (null != _obj)
            {
                if (_obj.isOn != _is_on)
                {
                    _obj.isOn = _is_on;
                }
            }
        }

        public static bool IsToggleOn(Toggle _obj, bool _default = false)
        {
            if (null == _obj)
                return _default;

            return _obj.isOn;
        }

        public static void SetSliderValue(Slider _obj, float _val)
        {
            if (null != _obj)
            {
                if (_obj.value != _val)
                {
                    _obj.value = _val;
                }
            }
        }

        public static void SetImageFillAmount(Image _obj, float _ratio)
        {
            if (null != _obj)
            {
                if (_obj.fillAmount != _ratio)
                {
                    _obj.fillAmount = _ratio;
                }
            }
        }

        public static void SetScrollbarSize(Scrollbar _obj, float _val)
        {
            if (null != _obj)
            {
                if (_obj.size != _val)
                {
                    _obj.size = _val;
                }
            }
        }

        public static void SetChildAlignment(LayoutGroup _obj, TextAnchor _anc)
        {
            if (null != _obj)
            {
                if (_obj.childAlignment != _anc)
                {
                    _obj.childAlignment = _anc;
                }
            }
        }

        public static void SetPaddingRight(LayoutGroup _obj, int _val)
        {
            if (null != _obj)
            {
                if (_obj.padding.right != _val)
                {
                    _obj.padding.right = _val;
                }
            }
        }

        public static string GetText(Text _obj, string _default)
        {
            if (null != _obj)
            {
                return _obj.text;
            }

            return _default;
        }

        public static void SetText(Text _obj, string _val)
        {
            if (null != _obj)
            {
                _obj.text = _val;
            }
        }

        public static void AddText(Text _obj, string _val)
        {
            if (null != _obj)
            {
                _obj.text += _val;
            }
        }

        public static string GetText(InputField _obj, string _default)
        {
            if (null != _obj)
            {
                return _obj.text;
            }

            return _default;
        }

        public static void SetText(InputField _obj, string _val)
        {
            if (null != _obj)
            {
                _obj.text = _val;
            }
        }

        public static void SetSprite(Image _obj, Sprite _sp)
        {
            if (null != _obj)
            {
                _obj.sprite = _sp;
            }
        }

        public static Sprite GetSprite(Image _obj)
        {
            if (null != _obj)
            {
                return _obj.sprite;
            }

            return null;
        }
    }

    public static class GraphicRaycasterEx
    {
        public static void SetBlockingMask(this GraphicRaycaster _ray_caster, int _mask_layer)
        {
            if (null != _ray_caster)
            {
                var fieldInfo = _ray_caster.GetType().GetField("m_BlockingMask", BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.Instance);
                if (null != fieldInfo)
                {
                    LayerMask layerMask = new LayerMask();
                    layerMask.value = _mask_layer;
                    fieldInfo.SetValue(_ray_caster, layerMask);
                }
            }
        }
    }
}