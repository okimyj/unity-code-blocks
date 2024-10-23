using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public enum AssetBundleOperationType
{
    AssetBundleLoadLevelSimulationOperation_InEditor = 0,
    AssetBundleLoadLevelOperation,
    AssetBundleLoadAssetOperationSimulation,
    AssetBundleLoadAssetOperationFull,
}

public abstract class AssetBundleLoadOperation : IEnumerator
{
    public AssetBundleOperationType operationType { get; private set; }
    public AssetBundleLoadOperation(AssetBundleOperationType _type) { operationType = _type; }

    public object Current { get { return null; } }
    public bool MoveNext() { return !IsDone(); }
    public void Reset() { }

    abstract public bool Update();
    abstract public bool IsDone();
    abstract public float GetProgress();
}

#if UNITY_EDITOR
public class AssetBundleLoadLevelSimulationOperation : AssetBundleLoadOperation
{
    public AsyncOperation operation { get { return m_Operation; } }
    AsyncOperation m_Operation = null;

    /*
    //씬을 폴더별로 에셋번들로 묶었을 경우 실제 에셋 번들을 다운받아 사용할때는 이상이 없으나
    //에디터에서 AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName() 호출시 씬을 찾지 못해 테스트 할 수 가 없다.
    //그래서 강제로 폴더 지정하여 로드
    public AssetBundleLoadLevelSimulationOperation(string _bundle_name, string _scene_name, bool _is_addtive)
    {
        string[] levelPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(_bundle_name, _scene_name);
        if (levelPaths.Length == 0)
        {
            ///@TODO: The error needs to differentiate that an asset bundle name doesn't exist
            //        from that there right scene does not exist in the asset bundle...

            Debug.LogError("There is no scene with name \"" + _scene_name + "\" in " + _bundle_name);
            return;
        }

        if (_is_addtive)
            m_Operation = UnityEditor.EditorApplication.LoadLevelAdditiveAsyncInPlayMode(levelPaths[0]);
        else
            m_Operation = UnityEditor.EditorApplication.LoadLevelAsyncInPlayMode(levelPaths[0]);
    }
    /**/

    public AssetBundleLoadLevelSimulationOperation(string _scene_path, bool _is_addtive)
        : base(AssetBundleOperationType.AssetBundleLoadLevelSimulationOperation_InEditor)
    {
        if (string.IsNullOrEmpty(_scene_path))
        {
            Debug.LogError("There is no scene with name \"" + _scene_path);
            return;
        }

        if (_is_addtive)
            m_Operation = UnityEditor.EditorApplication.LoadLevelAdditiveAsyncInPlayMode(_scene_path);
        else
            m_Operation = UnityEditor.EditorApplication.LoadLevelAsyncInPlayMode(_scene_path);
    }

    public override bool Update()
    {
        return false;
    }

    public override bool IsDone()
    {
        return m_Operation == null || m_Operation.isDone;
    }

    public override float GetProgress()
    {
        return m_Operation.progress;
    }
}
#endif


public class AssetBundleLoadLevelOperation : AssetBundleLoadOperation
{
    public delegate AsyncOperation FuncLoadScene(string _bundle_name, string _scene_name, bool _is_additive);

    protected string m_BundleName;
    protected string m_SceneName;
    protected bool m_IsAdditive;
    protected string m_DownloadingError;

    public AsyncOperation operation { get { return m_Request; } }
    protected AsyncOperation m_Request;

    public AssetBundleLoadLevelOperation(FuncLoadScene _func, string _bundle_name, string _scene_name, bool _is_additive)
        : base(AssetBundleOperationType.AssetBundleLoadLevelOperation)
    {
        m_BundleName = _bundle_name;
        m_SceneName = _scene_name;
        m_IsAdditive = _is_additive;

        m_Request = _func(m_BundleName, m_SceneName, m_IsAdditive);
        //m_Request = BundleMgr.LoadSceneOperation(m_BundleName, m_SceneName, m_IsAdditive);
    }

    public override bool Update()
    {
        if (null != m_Request)
        {
            return false;
        }
        else
        {
            if (!string.IsNullOrEmpty(m_DownloadingError))
                return false;

            return true;
        }
    }

    public override bool IsDone()
    {
        // Return if meeting downloading error.
        // m_DownloadingError might come from the dependency downloading.
        if (m_Request == null && m_DownloadingError != null)
        {
            Debug.LogError(m_DownloadingError);
            return true;
        }

        return m_Request != null && m_Request.isDone;
    }

    public override float GetProgress()
    {
        if (IsDone())
            return 1.0f;

        if (null != m_Request)
            return m_Request.progress;

        return 0.0f;
    }
}

public abstract class AssetBundleLoadAssetOperation : AssetBundleLoadOperation
{
    public AssetBundleLoadAssetOperation(AssetBundleOperationType _type) : base(_type) { }

    public abstract T GetAsset<T>() where T : UnityEngine.Object;
}

public class AssetBundleLoadAssetOperationSimulation : AssetBundleLoadAssetOperation
{
    Object m_SimulatedObject;

    public AssetBundleLoadAssetOperationSimulation(Object simulatedObject)
        : base(AssetBundleOperationType.AssetBundleLoadAssetOperationSimulation)
    {
        m_SimulatedObject = simulatedObject;
    }

    public override T GetAsset<T>()
    {
        return m_SimulatedObject as T;
    }

    public override bool Update()
    {
        return false;
    }

    public override bool IsDone()
    {
        return true;
    }

    public override float GetProgress()
    {
        return 1.0f;
    }
}

public class AssetBundleLoadAssetOperationFull : AssetBundleLoadAssetOperation
{
    public delegate AssetBundleRequest FuncLoadScene(string _bundle_name, string _asset_name, System.Type _type);

    protected string m_BundleName;
    protected string m_AssetName;
    protected string m_DownloadingError;
    protected System.Type m_Type;
    protected AssetBundleRequest m_Request = null;

    public AssetBundleLoadAssetOperationFull(FuncLoadScene _func, string bundleName, string assetName, System.Type type, AssetBundleOperationType _operation_type = AssetBundleOperationType.AssetBundleLoadAssetOperationSimulation)
        : base(_operation_type)
    {
        m_BundleName = bundleName;
        m_AssetName = assetName;
        m_Type = type;

        m_Request = _func(m_BundleName, m_AssetName, m_Type);
    }

    public override T GetAsset<T>()
    {
        if (m_Request != null && m_Request.isDone)
            return m_Request.asset as T;
        else
            return null;
    }

    // Returns true if more Update calls are required.
    public override bool Update()
    {
        if (null != m_Request)
        {
            return false;
        }
        else
        {
            if (!string.IsNullOrEmpty(m_DownloadingError))
                return false;

            return true;
        }
    }

    public override bool IsDone()
    {
        // Return if meeting downloading error.
        // m_DownloadingError might come from the dependency downloading.
        if (m_Request == null && m_DownloadingError != null)
        {
            Debug.LogError(m_DownloadingError);
            return true;
        }

        return m_Request != null && m_Request.isDone;
    }

    public override float GetProgress()
    {
        if (IsDone())
            return 1.0f;

        if (null != m_Request)
            return m_Request.progress;

        return 0.0f;
    }
}

public class AssetBundleLoadManifestOperation : AssetBundleLoadAssetOperationFull
{
    public AssetBundleLoadManifestOperation(FuncLoadScene _func, string bundleName, string assetName, System.Type type)
        : base(_func, bundleName, assetName, type, AssetBundleOperationType.AssetBundleLoadAssetOperationFull)
    {
    }

    public override bool Update()
    {
        if (!base.Update())
        {
            //로드 실패
            if (m_Request == null)
                return false;
        }

        if (m_Request != null && m_Request.isDone)
        {
            BundleMgr.SetAssetBundleManifestObject(GetAsset<AssetBundleManifest>());
            return false;
        }
        else
            return true;
    }
}
