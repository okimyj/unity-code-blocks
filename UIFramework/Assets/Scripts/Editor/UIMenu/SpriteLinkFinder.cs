using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
public class SpriteLinkFinder : EditorWindowUtility
{
	[System.Serializable]
	private class ImageData
	{
		public Image ImageComp;
		public bool IsFoldOut;
		public string Path;
		public string SpriteAssetPath;
		public ImageData()
		{
			IsFoldOut = false;
			Path = "";
		}
	}

	[System.Serializable]
	private class ObjectData
	{
		public string ObjName;
		public GameObject Obj;
		public List<ImageData> ImageDataList;
		public bool IsFoldOut;
		public ObjectData()
		{
			ImageDataList = new List<ImageData>();
		}
		public void AddImageData(Image image, string assetPath)
		{
			ImageDataList.Add(new ImageData() { ImageComp = image, SpriteAssetPath = assetPath });
		}
	}
	
	private const string DEFAULT_SEARCH_PATH = "Assets/02_ResDependency";
	private string _strPath;
	private string _targetSpriteName;
	private List<ObjectData> _dataList = new List<ObjectData>();
	private Vector2 _mScroll = Vector2.zero;
	private GameObject _targetObj;
	private Sprite _targetSprite;
	private bool _onlyEqual = false;
	private bool _ignoreCase = true;
	private bool _shownHelp = false;
	private bool _findNoneSprite = false;

	private void OnGUI()
	{
		_shownHelp = EditorGUILayout.Foldout(_shownHelp, "HELP", true);
		if (_shownHelp)
		{
			EditorGUILayout.HelpBox("Link 버튼 : Project 창에서 폴더 선택 후 Link 버튼을 누르시면 Search Path 에 해당 경로가 입력됩니다." +
				"\nSelect 버튼 : Search Path에 해당하는 폴더를 선택된 상태로 바꿉니다." +
				"\nSearch Sprite Name : 찾고자 하는 sprite의 이름을 적습니다. " +
				"\nEquals 가 체크 되어있으면 입력된 값과 정확하게 동일한 것들을 찾고, 아닌 경우 해당 이름을 포함한 것을 모두 찾습니다." +
				"\nIgnore Case 가 체크 되어있으면 대소문자를 구분하지 않고, 아닌 경우 대소문자를 구분하여 찾습니다." +
				"\nTarget Object : 찾는 대상을 해당 오브젝트로 한정합니다." +
				"\nTarget Sprite : 스프라이트를 직접 링크해서 해당 스프라이트를 사용하는 요소들을 찾습니다.", MessageType.Info);
		}
		DrawSpaces(2);
		_strPath = string.IsNullOrEmpty(_strPath) ? DEFAULT_SEARCH_PATH : _strPath;
		const float TITLE_WIDTH = 150f;
		DrawHorizontalSection(()=> {
			DrawLabelField("Search Path : ", false, TITLE_WIDTH);
			_strPath = EditorGUILayout.TextField(_strPath);
			if (GUILayout.Button("Link", GUILayout.Width(80f)))
			{
				string result = DEFAULT_SEARCH_PATH;
				Object[] objs = Selection.GetFiltered<Object>(SelectionMode.Assets);
				foreach (Object obj in objs)
				{
					string path = AssetDatabase.GetAssetPath(obj);
					if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
					{
						result = path;
						break;
					}
				}
				_strPath = result;
			}
			if (GUILayout.Button("Select", GUILayout.Width(80f)))
			{
				Object obj = AssetDatabase.LoadAssetAtPath<Object>(_strPath);
				EditorGUIUtility.PingObject(obj);
			}
		});
		
		DrawHorizontalSection(() => {
			DrawLabelField("Search Sprite Name : ", false, TITLE_WIDTH);
			DrawVerticalSection(() => {
				var prevTargetSpriteName = _targetSpriteName;
				_targetSpriteName = EditorGUILayout.TextField(_targetSpriteName);
				if (prevTargetSpriteName != _targetSpriteName)
					_targetSprite = null;
				DrawHorizontalSection(() => {
					DrawLabelField("Equals : ");
					_onlyEqual = EditorGUILayout.Toggle(_onlyEqual, GUILayout.MaxWidth(20));
					DrawLabelField("Ignore Case : ");
					_ignoreCase = EditorGUILayout.Toggle(_ignoreCase, GUILayout.MaxWidth(20));
				});
			});
			
		});
		

		DrawHorizontalSection(() => {
			DrawLabelField("Target Sprite : ", false, TITLE_WIDTH);
			_targetSprite = EditorGUILayout.ObjectField(_targetSprite, typeof(Sprite), true) as Sprite;
		});

		DrawHorizontalSection(() => {
			DrawLabelField("Target Object : ", false, TITLE_WIDTH);
			_targetObj = EditorGUILayout.ObjectField(_targetObj, typeof(GameObject), true) as GameObject;
		});

		DrawHorizontalSection(() => {
			DrawLabelField("Find None Sprite : ", false, TITLE_WIDTH);
			_findNoneSprite = EditorGUILayout.Toggle(_findNoneSprite, GUILayout.MaxWidth(20));
		});
		
		DrawSpaces(2);

		DrawHorizontalSection(() => {
			if (GUILayout.Button("Load", GUILayout.Width(80f)))
			{
				_dataList.Clear();
				if (_targetObj == null)
					LoadForPath();
				else
					LoadForOneObj(_targetObj);
			}
			if (GUILayout.Button("Clear", GUILayout.Width(80f)))
			{
				_targetObj = null;
				_targetSprite = null;
				_dataList.Clear();
			}
		});
		
		_mScroll = EditorGUILayout.BeginScrollView(_mScroll);
		DrawVerticalSection(() => {
			if (_dataList.Count > 0)
			{
				for (int i = 0; i < _dataList.Count; ++i)
					DrawOneData(_dataList[i]);
			}
			else
			{
				EditorGUILayout.HelpBox("해당 sprite를 사용하는 요소가 없습니다.", MessageType.Info);
			}
		});
		EditorGUILayout.EndScrollView();
	}

    #region Load Datas
    private void LoadForOneObj(GameObject obj)
	{
		LoadOneObj(obj, true);
	}
	private void LoadForPath()
	{
		var assets = AssetDatabase.FindAssets("t:prefab", new string[] { _strPath });
		for (int i = 0; i < assets.Length; ++i)
		{
			var respositoryPath = AssetDatabase.GUIDToAssetPath(assets[i]);
			var obj = AssetDatabase.LoadAssetAtPath(respositoryPath, typeof(GameObject)) as GameObject;
			if(obj != null)
            {
				LoadOneObj(obj, true);
			}
		}
	}
	private void LoadOneObj(GameObject obj, bool isFoldout = false)
	{
		var images = obj.GetComponentsInChildren<Image>(true);
		var objData = new ObjectData() { ObjName = obj.name, Obj = obj, IsFoldOut = isFoldout };
		if (images != null && 0 < images.Length)
		{
			for (int k = 0; k < images.Length; ++k)
			{
				if (_findNoneSprite && images[k].sprite == null)
				{
					objData.AddImageData(images[k], "");
					continue;
				}
				if (images[k].sprite == null || (_targetSprite != null && !images[k].sprite.Equals(_targetSprite)))
					continue;
				if (!IsSpriteNameMatch(images[k].sprite))
					continue;

				var path = AssetDatabase.GetAssetPath(images[k].sprite);
				objData.AddImageData(images[k], path);
			}
		}
		if (objData != null && (objData.ImageDataList != null && objData.ImageDataList.Count > 0))
		{
			_dataList.Add(objData);
		}
	}
	private bool IsSpriteNameMatch(Sprite sprite)
	{
		if (string.IsNullOrEmpty(_targetSpriteName))
			return true;
		if (_onlyEqual)
			return sprite.name.Equals(_targetSpriteName);
		return sprite.name.Contains(_targetSpriteName, _ignoreCase ? System.StringComparison.OrdinalIgnoreCase : System.StringComparison.Ordinal);
	}
	#endregion

    #region Draw Data
    private void DrawOneData(ObjectData objData)
	{
		DrawSpaces();
		DrawHorizontalSection(() => {
			objData.IsFoldOut = EditorGUILayout.Foldout(objData.IsFoldOut, objData.ObjName, true);
			EditorGUILayout.ObjectField(objData.Obj, typeof(GameObject), true);
		});
		
		if (objData.IsFoldOut)
		{
			DrawVerticalSection(()=> {
				++EditorGUI.indentLevel;
				EditorGUILayout.LabelField("-------------------------------------------------------------------------------------------------------------------------------------");
				foreach (var imageData in objData.ImageDataList)
				{
					DrawImageData(imageData);
				}
				--EditorGUI.indentLevel;
				EditorGUILayout.LabelField("-------------------------------------------------------------------------------------------------------------------------------------");
			});
		}
		DrawSpaces();
	}

	private void DrawImageData(ImageData imageData)
    {
		Image image = imageData.ImageComp;
		if (image == null)
			return;
		DrawSpaces();
		DrawHorizontalSection(() => {
			imageData.IsFoldOut = EditorGUILayout.Foldout(imageData.IsFoldOut, image.name, true);
			EditorGUILayout.ObjectField(image, typeof(Image), true);
			EditorGUILayout.ObjectField("", image.sprite, typeof(Sprite), true);
		});
		
		if (imageData.IsFoldOut)
		{
			++EditorGUI.indentLevel;
			if (string.IsNullOrEmpty(imageData.Path))
				imageData.Path = GetPath(image.transform);

			EditorGUILayout.LabelField("Path In Object : " + imageData.Path);

			GUI.contentColor = Color.white;
			if (image.sprite != null)
			{
				EditorGUILayout.LabelField("Sprite Name : " + image.sprite.name);
				EditorGUILayout.LabelField("Asset Path : " + imageData.SpriteAssetPath);
			}
			--EditorGUI.indentLevel;
		}
	}
	private string GetPath(Transform trans)
	{
		if (trans.parent != null)
			return GetPath(trans.parent) + "/" + trans.name;
		else
			return trans.name;
	}
	#endregion


	[MenuItem("UI Menu/Sprite Link Finder")]
	static public void ShowSpriteLinkFinder()
	{
		EditorWindow.GetWindow<SpriteLinkFinder>(false, "Sprite Link Finder", true);
	}
}
