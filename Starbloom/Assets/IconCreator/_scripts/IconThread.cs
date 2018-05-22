using System;
using UnityEngine;
using System.Collections.Generic;

public class IconThread
{
	private readonly int _iconSizeX;
	private readonly int _iconSizeY;

	private readonly List<OutlineColor> _outlineColors;

	private readonly bool _useOutline;
	private readonly int _outlineSize;
	private readonly bool _useBlur;
	private readonly int _blurSize;
	private readonly int _blurIterations;
	private readonly bool _fadeBottom;
	private readonly int _fadeBottomSize;

	private readonly bool _useBackground;
	private readonly bool _useForeground;

	private readonly Color32[] _backgroundColors;
	private readonly Color32[] _foregroundColors;
	private Color32[] _iconPixels;

	private readonly string _iconName;

	public IconThread(int iconSizeX, int iconSizeY, List<OutlineColor> outlineColors, bool useOutline, int outlineSize,
		bool useBlur, int blurSize, int blurIterations, bool fadeBottom, int fadeBottomSize, bool useBackground,
		bool useForeground, Color32[] backgroundColors, Color32[] foregroundColors, string iconName)
	{
		_iconSizeX = iconSizeX;
		_iconSizeY = iconSizeY;
		_outlineColors = outlineColors;
		_useOutline = useOutline;
		_outlineSize = outlineSize;
		_useBlur = useBlur;
		_blurSize = blurSize;
		_blurIterations = blurIterations;
		_fadeBottom = fadeBottom;
		_fadeBottomSize = fadeBottomSize;
		_useBackground = useBackground;
		_useForeground = useForeground;
		_backgroundColors = backgroundColors;
		_foregroundColors = foregroundColors;
		_iconName = iconName;
	}

	public Icon GenerateIcons(Color32[] iconColors)
	{
		var icon = new Icon {IconName = _iconName};

		var outlineLayer = OutlineIcon(iconColors);

		if (!_useOutline)
		{
			icon.IconPixels.Add(CombineLayers(_backgroundColors, outlineLayer, iconColors, _foregroundColors));
			icon.OutlineColorNames.Add("No outline");
		}
		else
		{
			foreach (var outlineColor in _outlineColors)
			{
				var coloredOutlineLayer = ColorOutlineLayer(outlineLayer, outlineColor.Color);
				icon.IconPixels.Add(CombineLayers(_backgroundColors, coloredOutlineLayer, iconColors, _foregroundColors));
				icon.OutlineColorNames.Add(outlineColor.ColorName);
			}
		}

		return icon;
	}

	private Color32[] CombineLayers(Color32[] background, Color32[] outline, Color32[] icon, Color32[] foreground)
	{
		var finalIcon = new Color32[_iconSizeX * _iconSizeY];

		var outlinedIcon = outline;

		if (_useBackground)
		{
			for (var i = 0; i < background.Length; i++)
			{
				if (background[i].a != 0)
				{
					finalIcon[i] = background[i];
				}
			}
		}

		for (var i = 0; i < icon.Length; i++)
		{
			if (icon[i].a != 0)
			{
				outlinedIcon[i] = icon[i];
			}
		}

		if (_fadeBottom)
		{
			FadeIconBottom(outlinedIcon);
		}

		for (var i = 0; i < outlinedIcon.Length; i++)
		{
			if (outlinedIcon[i].a == 0) continue;

			finalIcon[i] = Color32.Lerp(finalIcon[i], outlinedIcon[i], outlinedIcon[i].a / 255f);
			if (_useBackground)
			{
				finalIcon[i].a = 255;
			}
		}

		if (!_useForeground) return finalIcon;
		{
			for (var i = 0; i < foreground.Length; i++)
			{
				finalIcon[i] = Color32.Lerp(finalIcon[i], foreground[i], foreground[i].a / 255f);
				if (_useBackground)
				{
					finalIcon[i].a = 255;
				}
			}
		}

		return finalIcon;
	}

	private Color32[] ColorOutlineLayer(Color32[] outlineColors, Color outlineColor)
	{
		var outline = outlineColors;
		for (var index = 0; index < outline.Length; index++)
		{
			if (outline[index].a == 0) continue;

			var alpha = outline[index].a;
			outline[index] = outlineColor;
			outline[index].a = alpha;
		}
		return outline;
	}

	private Color32[] OutlineIcon(Color32[] iconColors)
	{
		var outlineColors = new Color32[_iconSizeX * _iconSizeY];

		if (!_useOutline) return outlineColors;

		for (var i = 0; i < iconColors.Length; i++)
		{
			if (iconColors[i].a == 0) continue;

			for (var j = -_outlineSize; j <= _outlineSize; j++)
			{
				for (var k = -_outlineSize; k <= _outlineSize; k++)
				{
					var index = i - _iconSizeX * j + k;
					if (index > 0 && index < iconColors.Length && outlineColors[index].a == 0)
						outlineColors[index] = Color.white;
				}
			}
		}
		return _useBlur ? BlurIcon(outlineColors) : outlineColors;
	}

	private Color32[] BlurIcon(Color32[] outlineColors)
	{
		for (var i = 0; i < _blurIterations; i++)
		{
			BoxBlurHorizontal(outlineColors, _blurSize);
			BoxBlurVertical(outlineColors, _blurSize);
		}
		return outlineColors;
	}

	private void FadeIconBottom(Color32[] colors)
	{
		var firstFoundIndex = int.MaxValue;
		for (var i = 0; i < colors.Length; i++)
		{
			if (colors[i].a == 0) continue;
			if (int.MaxValue == firstFoundIndex)
			{
				firstFoundIndex = i / _iconSizeX;
			}
		}

		for (var i = _iconSizeX * firstFoundIndex; i < colors.Length; i++)
		{
			if (i >= _iconSizeX * firstFoundIndex + _iconSizeX) continue;

			for (var j = 0; j < _iconSizeX; j++)
			{
				if (i + _iconSizeX * j >= colors.Length || colors[i + _iconSizeX * j].a == 0) continue;

				var a = j / (float) _fadeBottomSize * 255;
				var alpha = Math.Min(a, colors[i + _iconSizeX * j].a);
				colors[i + _iconSizeX * j].a = (byte) Mathf.Clamp(alpha, 0, 255);
			}
		}
	}

	private void BoxBlurHorizontal(Color32[] color32S, int range)
	{
		var pixels = color32S;
		var w = _iconSizeX;
		var h = _iconSizeY;
		var halfRange = range / 2;
		var index = 0;
		var newColors = new Color32[w];

		for (var y = 0; y < h; y++)
		{
			var hits = 0;
			var r = 0;
			var g = 0;
			var b = 0;
			var a = 0;
			for (var x = -halfRange; x < w; x++)
			{
				var oldPixel = x - halfRange - 1;
				if (oldPixel >= 0)
				{
					var col = pixels[index + oldPixel];
					if (col.a != 0)
					{
						r -= col.r;
						g -= col.g;
						b -= col.b;
						a -= col.a;
					}
					hits--;
				}

				var newPixel = x + halfRange;
				if (newPixel < w)
				{
					var col = pixels[index + newPixel];
					if (col.a != 0)
					{
						r += col.r;
						g += col.g;
						b += col.b;
						a += col.a;
					}
					hits++;
				}

				if (x < 0) continue;
				var color = new Color32((byte) (r / hits), (byte) (g / hits), (byte) (b / hits),
					(byte) (a / hits));

				newColors[x] = color;
			}

			for (var x = 0; x < w; x++)
			{
				pixels[index + x] = newColors[x];
			}

			index += w;
		}
	}

	private void BoxBlurVertical(Color32[] color32S, int range)
	{
		var pixels = color32S;
		var w = _iconSizeX;
		var h = _iconSizeY;
		var halfRange = range / 2;

		var newColors = new Color32[h];
		var oldPixelOffset = -(halfRange + 1) * w;
		var newPixelOffset = (halfRange) * w;

		for (var x = 0; x < w; x++)
		{
			var hits = 0;
			var r = 0;
			var g = 0;
			var b = 0;
			var a = 0;
			var index = -halfRange * w + x;
			for (var y = -halfRange; y < h; y++)
			{
				var oldPixel = y - halfRange - 1;
				if (oldPixel >= 0)
				{
					var col = pixels[index + oldPixelOffset];
					if (col.a != 0)
					{
						r -= col.r;
						g -= col.g;
						b -= col.b;
						a -= col.a;
					}
					hits--;
				}

				var newPixel = y + halfRange;
				if (newPixel < h)
				{
					var col = pixels[index + newPixelOffset];
					if (col.a != 0)
					{
						r += col.r;
						g += col.g;
						b += col.b;
						a += col.a;
					}
					hits++;
				}

				if (y >= 0)
				{
					var color =
						new Color32((byte) (r / hits), (byte) (g / hits), (byte) (b / hits), (byte) (a / hits));

					newColors[y] = color;
				}

				index += w;
			}

			for (var y = 0; y < h; y++)
			{
				pixels[y * w + x] = newColors[y];
			}
		}
	}
}