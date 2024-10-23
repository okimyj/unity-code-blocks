using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pieceton.Misc;

#region PResourcesInfo
public partial class PResourcesInfo
{
    public EPResourcesType resoucesType { get; private set; }

    public string resoucesPath { get; private set; }
    public string assetExtension { get; private set; }

    private PResourcesInfo(EPResourcesType _resources_type, string _resources_path, string _asset_extension)
    {
        resoucesType = _resources_type;
        resoucesPath = _resources_path;
        assetExtension = _asset_extension;
    }

    public string MakePath(string _asset_name)
    {
        if (string.IsNullOrEmpty(_asset_name))
            return string.Empty;

        if (string.IsNullOrEmpty(resoucesPath))
            return _asset_name;

        return resoucesPath + "/" + _asset_name;
    }
}
#endregion PResourcesInfo

#region PBundleInfo
public partial class PBundleInfo
{
    public EPBundleType bundleType { get; private set; }

    public string bundleName { get; private set; }
    public string assetExtension { get; private set; }
    public string assetPath { get; private set; }

    private PBundleInfo(EPBundleType _bundle_type, string _bundle_name, string _asset_extension, string _asset_editor_path)
    {
        bundleType = _bundle_type;
        bundleName = _bundle_name;
        assetExtension = _asset_extension;
        assetPath = _asset_editor_path;
    }
}
#endregion PBundleInfo

public class ResExtension
{
    public const string BUNDLE_PACK = "pak";
    public const string BUNDLE_DOT_PACK = "." + BUNDLE_PACK;

    public const string PREFAB = "prefab";
    public const string SCENE = "unity";
    public const string DB_SCRIPT = "bytes";
    public const string UI_SPRITE = "png";
    public const string TEXTURE_2D = "png";
    public const string BYTES = "bytes";
    public const string JSON_DATA = "json";
    public const string PROJECTILE_DATA = "bytes";
    public const string SOUND_EFFECT = "mp3";
    public const string LOCALIZE_FILE = "bytes";
    public const string TUTORIAL_DATA = "txt";
    public const string BGM = "ogg";
    public const string FONT = "ttf";
    public const string CSV = "csv";
    public const string ASSETS = "asset";
    public const string ANIMATION_CLIP = "anim";
    public const string ANIM_CONTROLLER = "controller";
    public const string ANIM_OV_CONTROLLER = "overrideController";
}

public enum EPResourcesType
{
    Localization = 0,
    Font,
    SoundComponent,
    Sound,
    BGM,
    UIPrefab,
    CodeBlockPrefab,

    End
}

public enum EPBundleType
{
    Localization = 0,
    UIWindow,
    EnvPlants,
    EnvProps,
    EnvRocks,
    EnvTerrain,
    EnvTrees,
    BlockObjects,
    Weapons,
    Etc,

    End
}

public partial class PResourcesInfo
{
    private static Dictionary<EPResourcesType, PResourcesInfo> _dic = new Dictionary<EPResourcesType, PResourcesInfo>();

    public static void Initialize()
    {
        _dic.Clear();
        Add(EPResourcesType.Localization, "Localization", ResExtension.LOCALIZE_FILE);
        Add(EPResourcesType.Font, "Fonts", ResExtension.FONT);
        Add(EPResourcesType.SoundComponent, "Prefab", ResExtension.PREFAB);
        Add(EPResourcesType.Sound, "Sound", ResExtension.SOUND_EFFECT);
        Add(EPResourcesType.BGM, "BGM", ResExtension.SOUND_EFFECT);
    }

    public static void Add(EPResourcesType _resources_type, string _resources_path, string _asset_extension)
    {
        if (_dic.ContainsKey(_resources_type))
        {
            PLog.AnyLogError("Duplicated PResourcesInfo. EPResourcesType = '{0}'", _resources_type);
            return;
        }

        _dic.Add(_resources_type, new PResourcesInfo(_resources_type, _resources_path, _asset_extension));
    }
    public static PResourcesInfo Get(EPResourcesType _resources_type)
    {
        PResourcesInfo info;
        if (_dic.TryGetValue(_resources_type, out info))
            return info;

        PLog.AnyLogError("Not found PResourcesInfo. EPResourcesType = '{0}'", _resources_type);
        return null;
    }
}

public partial class PBundleInfo
{
    private static Dictionary<EPBundleType, PBundleInfo> _dic = new Dictionary<EPBundleType, PBundleInfo>();

    public static void Initialize()
    {
        _dic.Clear();
    }

    public static void Add(EPBundleType _bundle_type, string _bundle_name, string _asset_extension, string _asset_path)
    {
        if (_dic.ContainsKey(_bundle_type))
        {
            PLog.AnyLogError("Duplicated PBundleInfo. EPBundleType = '{0}'", _bundle_type);
            return;
        }

        _dic.Add(_bundle_type, new PBundleInfo(_bundle_type, _bundle_name, _asset_extension, _asset_path));
    }

    public static PBundleInfo Get(EPBundleType _bundle_type)
    {
        PBundleInfo info;
        if (_dic.TryGetValue(_bundle_type, out info))
            return info;

        PLog.AnyLogError("Not found PBundleInfo. EPBundleType = '{0}'", _bundle_type);
        return null;
    }
}