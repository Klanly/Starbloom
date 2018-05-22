using System.Collections.Generic;
using UnityEngine;

public class Icon
{
	private List<Color32[]> _iconPixels;
	private string _iconName;
	private List<string> _outlineColorNames;
	private List<Texture2D> _finalIcons;

	public List<Color32[]> IconPixels
	{
		get { return _iconPixels; }
		set { _iconPixels = value; }
	}

	public string IconName
	{
		get { return _iconName; }
		set { _iconName = value; }
	}

	public List<string> OutlineColorNames
	{
		get { return _outlineColorNames; }
		set { _outlineColorNames = value; }
	}

	public List<Texture2D> FinalIcons
	{
		get { return _finalIcons; }
		set { _finalIcons = value; }
	}

	public Icon()
	{
		IconPixels = new List<Color32[]>();
		FinalIcons = new List<Texture2D>();
		OutlineColorNames = new List<string>();
	}
}