using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Threading;

#if UNITY_EDITOR

using UnityEditor;
using Timer = System.Timers.Timer;



[ExecuteInEditMode]
public class IconCreator : MonoBehaviour
{
	/*****************************************************************
	Variables
	*****************************************************************/

	[Header("Icon size")] [SerializeField] private int _iconSizeX;

	[SerializeField] private int _iconSizeY;

	[Header("Increased icon quality")] [SerializeField] private bool _increasedQuality;



	[Header("Outline")] [SerializeField] private bool _useOutline;
	[SerializeField] private List<OutlineColor> _outlineColors;
	[SerializeField] private int _outlineSize;

	[Header("Blur")] [SerializeField] private bool _blurOutline;
	[SerializeField] private int _blurSize;
	[SerializeField] private int _blurIterations;

	[Header("Bottom fade")] [SerializeField] private bool _fadeBottom;
	[SerializeField] private int _fadeBottomSize;

	[Header("Background & foreground")] [SerializeField] private bool _useBackground;
	[SerializeField] private bool _useForeground;


	[Header("Offset correction")] [SerializeField] private Vector2 _offsetCorrection;

    public IconValues IconValues;

    public int _currentIndex;

	/*****************************************************************
	Getter & Setters
	*****************************************************************/

	private int IconSizeX
	{
		get
		{
			if (_iconSizeX < 16) _iconSizeX = 16;
			return _iconSizeX;
		}
		set { _iconSizeX = value; }
	}

	private int IconSizeY
	{
		get
		{
			if (_iconSizeY < 16) _iconSizeY = 16;
			return _iconSizeY;
		}
		set { _iconSizeY = value; }
	}

	private Vector2 OffsetCorrection
	{
		get { return _offsetCorrection; }
	}

	private List<IconValues.Pivots> Models
	{
		get { return IconValues.ModelPivots; }
	}

	private bool IncreasedQuality
	{
		get { return _increasedQuality; }
	}

	private bool UseOutline
	{
		get
		{
			if (OutlineColors == null || OutlineColors.Count == 0) _useOutline = false;
			return _useOutline;
		}
	}

	private List<OutlineColor> OutlineColors
	{
		get { return _outlineColors; }
	}

	private int OutlineSize
	{
		get
		{
			if (_outlineSize < 0) _outlineSize = 0;
			return _outlineSize;
		}
	}

	private bool BlurOutline
	{
		get { return _blurOutline; }
	}

	private int BlurSize
	{
		get
		{
			if (_blurSize < 0) _blurSize = 0;
			return _blurSize;
		}
	}

	private int BlurIterations
	{
		get
		{
			if (_blurIterations < 0) _blurIterations = 0;
			return _blurIterations;
		}
	}

	private bool FadeBottom
	{
		get { return _fadeBottom; }
	}

	private int FadeBottomSize
	{
		get
		{
			if (_fadeBottomSize < 0) _fadeBottomSize = 0;
			return _fadeBottomSize;
		}
	}

	private Vector3 PivotPosition
	{
		get { return IconValues.ModelPivots[_currentIndex]._pivotPosition; }
		set
		{
            IconValues.ModelPivots[_currentIndex]._pivotPosition = value;
			PivotTransform.localPosition = IconValues.ModelPivots[_currentIndex]._pivotPosition;
		}
	}

	private Vector3 PivotRotation
	{
		get { return IconValues.ModelPivots[_currentIndex]._pivotRotation; }
		set
		{
            IconValues.ModelPivots[_currentIndex]._pivotRotation = value;
			PivotTransform.localEulerAngles = IconValues.ModelPivots[_currentIndex]._pivotRotation;
		}
	}

	private Vector3 PivotScale
	{
		get { return IconValues.ModelPivots[_currentIndex]._pivotScale; }
		set
		{
            IconValues.ModelPivots[_currentIndex]._pivotScale = value;
			PivotTransform.localScale = IconValues.ModelPivots[_currentIndex]._pivotScale;
		}
	}

	private bool UseBackground
	{
		get { return _useBackground; }
	}

	private bool UseForeground
	{
		get { return _useForeground; }
	}

	private Transform BackgroundTransform
	{
		get { return transform.Find("Background"); }
	}

	private Transform PivotTransform
	{
		get { return transform.Find("Pivot"); }
	}

	private Transform ForegroundTransform
	{
		get { return transform.Find("Foreground"); }
	}

	private Camera CurrentCamera
	{
		get { return GetComponent<Camera>(); }
	}

	private Texture2D BackgroundTexture { get; set; }

	private Texture2D ForegroundTexture { get; set; }

	private Texture2D IconTexture { get; set; }

	private List<Icon> GeneratedIcons { get; set; }

	private Rect RenderRect
	{
		get
		{
			return new Rect(OffsetCorrection.x, OffsetCorrection.y, IconSizeX + OffsetCorrection.x,
				IconSizeY + OffsetCorrection.y);
		}
	}

	public int CurrentIndex
	{
		get
		{

			return _currentIndex;
		}
		set
		{
			_currentIndex = value;
			if (_currentIndex >= Models.Count - 1) _currentIndex = Models.Count - 1;
			if (_currentIndex < 0) _currentIndex = 0;
		}
	}

	/*****************************************************************
	Functions
	*****************************************************************/

	public void Update()
	{
		UpdateModelLocation();
	}

	public void ChangePreviewModel()
	{
		while (PivotTransform.childCount != 0)
		{
			DestroyImmediate(PivotTransform.GetChild(0).gameObject);
		}
		if (Models.Count > 0 && Models[CurrentIndex] != null)
		{
			((GameObject) Instantiate(Models[CurrentIndex]._models, PivotTransform.position, Quaternion.identity, PivotTransform))
				.transform.localEulerAngles = Vector3.zero;
		}
	}

	private void UpdateModelLocation()
	{
		CurrentCamera.pixelRect = new Rect(
			OffsetCorrection.x,
			OffsetCorrection.y,
			IconSizeX + OffsetCorrection.x,
			IconSizeY + OffsetCorrection.y
		);
		PivotTransform.localEulerAngles = PivotRotation;
		PivotTransform.localPosition = PivotPosition;
		PivotTransform.localScale = PivotScale;
		BackgroundTransform.gameObject.SetActive(UseBackground);
		ForegroundTransform.gameObject.SetActive(UseForeground);
	}

	public void CreateIcons(bool SingleTarget)
	{
        _useBackground = false;

		var savedIconSizeX = IconSizeX;
		var savedIconSizeY = IconSizeY;
		if (IncreasedQuality)
		{
			savedIconSizeX = IconSizeX;
			savedIconSizeY = IconSizeY;
			IconSizeX *= 2;
			IconSizeY *= 2;
		}

		if (IconSizeX > CurrentCamera.pixelWidth / CurrentCamera.rect.width || IconSizeY > CurrentCamera.pixelHeight / CurrentCamera.rect.height)
		{
			if (EditorUtility.DisplayDialog("IconThread size is greater than the size of the screen",
				"Please decrease the icon size or increase your game window size.", "Ok"))
			{
				IconSizeX = savedIconSizeX;
				IconSizeY = savedIconSizeY;
				return;
			}
		}

		UpdateModelLocation();

		GeneratedIcons = new List<Icon>();

		RenderBackground();
		var backgroundPixels = BackgroundTexture.GetPixels32();
		RenderForeground();
		var foregroundPixels = ForegroundTexture.GetPixels32();

		var threads = new List<Thread>();

        List<IconValues.Pivots> Pivots = new List<IconValues.Pivots>();
        if (SingleTarget)
            Pivots.Add(Models[CurrentIndex]);
        else
            Pivots = Models;

        foreach (var model in Pivots)
		{
			if(model == null) continue;
			EditorUtility.DisplayProgressBar("Rendering models",
				"Rendering model " + (Models.IndexOf(model) + 1) + " of " + Models.Count,
				Models.IndexOf(model) / (float)Models.Count);

			RenderIcon(model);

			var iconPixels = IconTexture.GetPixels32();
			var modelName = model._models.name;
			var thread = new Thread(() =>
			{
				var iconThread = new IconThread(
					IconSizeX,
					IconSizeY,
					OutlineColors,
					UseOutline,
					OutlineSize,
					BlurOutline,
					BlurSize,
					BlurIterations,
					FadeBottom,
					FadeBottomSize,
					UseBackground,
					UseForeground,
					backgroundPixels,
					foregroundPixels,
					modelName
				);
				lock (GeneratedIcons)
				{
					GeneratedIcons.Add(iconThread.GenerateIcons(iconPixels));
				}
			});
			lock (threads)
			{
				threads.Add(thread);
			}
			thread.Start();
		}

		for (var index = 0; index < threads.Count; index++)
		{
			EditorUtility.DisplayProgressBar("Creating icons", "Creating icon " + (index + 1) + " of " + threads.Count,
				index / (float) threads.Count);
			var thread = threads[index];
			thread.Join();
		}

		CreateFinalIcons();

		IconSizeX = savedIconSizeX;
		IconSizeY = savedIconSizeY;

		EditorUtility.ClearProgressBar();

		AssetDatabase.Refresh();

        _useBackground = true;
    }

	private void CreateFinalIcons()
	{
		foreach (var generatedIcon in GeneratedIcons)
		{
			foreach (var icon in generatedIcon.IconPixels)
			{
				var bigTexture = new Texture2D(IconSizeX, IconSizeY, TextureFormat.ARGB32, true);
				bigTexture.SetPixels32(icon);
				if (IncreasedQuality)
				{
					bigTexture.Apply(true);
					var iconTexture = new Texture2D(IconSizeX / 2,
						IconSizeY / 2, TextureFormat.ARGB32, true);
					iconTexture.SetPixels32(bigTexture.GetPixels32(1));
					iconTexture.Apply(true);
					generatedIcon.FinalIcons.Add(iconTexture);
				}
				else
				{
					generatedIcon.FinalIcons.Add(bigTexture);
				}
			}
			SaveIcon(generatedIcon);
		}
	}

	private void SaveIcon(Icon icon)
	{
		for (var i = 0; i < icon.FinalIcons.Count; i++)
		{
			if (
				!Directory.Exists(Application.dataPath + "/GeneratedIcons/" +
				                  (IncreasedQuality ? IconSizeX / 2 : IconSizeX) +
				                  "x" + (IncreasedQuality ? IconSizeY / 2 : IconSizeY) + "/" +
				                  icon.OutlineColorNames[i]))
			{
				Directory.CreateDirectory(Application.dataPath + "/GeneratedIcons/" +
				                          (IncreasedQuality ? IconSizeX / 2 : IconSizeX) + "x" +
				                          (IncreasedQuality ? IconSizeY / 2 : IconSizeY) + "/" +
				                          icon.OutlineColorNames[i]);
			}
			File.WriteAllBytes(
				Path.Combine(
					Application.dataPath + "/GeneratedIcons/" + (IncreasedQuality ? IconSizeX / 2 : IconSizeX) + "x" +
					(IncreasedQuality ? IconSizeY / 2 : IconSizeY) + "/" +
					icon.OutlineColorNames[i], icon.IconName + ".png"),
				icon.FinalIcons[i].EncodeToPNG());
		}
	}

	private void RenderBackground()
	{
		BackgroundTexture = new Texture2D(IconSizeX, IconSizeY);

		BackgroundTransform.gameObject.SetActive(true);
		PivotTransform.gameObject.SetActive(false);
		ForegroundTransform.gameObject.SetActive(false);

		CurrentCamera.Render();

		BackgroundTexture.ReadPixels(RenderRect, 0, 0);
	}

	private void RenderForeground()
	{
		ForegroundTexture = new Texture2D(IconSizeX, IconSizeY);

		BackgroundTransform.gameObject.SetActive(false);
		PivotTransform.gameObject.SetActive(false);
		ForegroundTransform.gameObject.SetActive(true);

		CurrentCamera.Render();

		ForegroundTexture.ReadPixels(RenderRect, 0, 0);
	}

	private void RenderIcon(IconValues.Pivots model)
	{
		UpdateModelLocation();

		IconTexture = new Texture2D(IconSizeX, IconSizeY);

		BackgroundTransform.gameObject.SetActive(false);
		PivotTransform.gameObject.SetActive(true);
		ForegroundTransform.gameObject.SetActive(false);

		while (PivotTransform.childCount != 0)
		{
			DestroyImmediate(PivotTransform.GetChild(0).gameObject);
		}

        PivotTransform.position = model._pivotPosition;
        PivotTransform.eulerAngles = model._pivotRotation;
        PivotTransform.localScale = model._pivotScale;

        GameObject GO = ((GameObject)Instantiate(model._models, PivotTransform.position, Quaternion.identity, PivotTransform));
        GO.transform.localEulerAngles = Vector3.zero;
        if (model.AdjustMaterialForIcon)
            GO.GetComponent<MeshRenderer>().material = model.IconMaterial;


        CurrentCamera.Render();
		IconTexture.ReadPixels(new Rect(0, 0, IconSizeX, IconSizeY), 0, 0);
		IconTexture.ReadPixels(new Rect(0, 0, IconSizeX, IconSizeY), 0, 0);

		IconTexture.Apply();
	}
}
#endif