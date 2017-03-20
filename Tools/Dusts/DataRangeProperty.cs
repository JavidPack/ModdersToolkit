using Microsoft.Xna.Framework;
using ModdersToolkit.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;

namespace ModdersToolkit.Tools.Dusts
{
	class FloatDataRangeProperty
	{
		internal float min = 0f;
		internal float max = 5f;
		private float data = 1f;
		internal float Data
		{
			get { return data; }
			set
			{
				data = value;
				range.input.SetText(data.ToString("0.00"));
			}
		}

		public UIRange range;

		public FloatDataRangeProperty(string label, float defaultValue, float min, float max)
		{
			data = defaultValue;
			this.min = min;
			this.max = max;
			range = new UIRange(label, () => (Data - min) / (max - min), (s) => { Data = (s * (max - min) + min); }, ValidateInput);
			//range.Top.Set(top, 0f);
			range.Width.Set(0, 1f);
			Data = data;
		}

		private void ValidateInput()
		{
			float result;
			if (float.TryParse(range.input.Text, out result))
			{
				Data = result;
			}
			else
			{
				Data = data;
			}
		}
	}

	class IntDataRangeProperty
	{
		internal int max = 5;
		private int data = 1;
		internal int Data
		{
			get { return data; }
			set
			{
				data = value;
				range.input.SetText(data.ToString());
			}
		}

		public UIRange range;

		public IntDataRangeProperty(string label, int defaultValue, int max, bool fine = false)
		{
			data = defaultValue;
			this.max = max;
			range = new UIRange(label, () => (float)Data / max, (s) => { Data = (int)(s * max); }, ValidateInput, fine);
			range.intDataRangeProperty = this;
			//range.Top.Set(top, 0f);
			range.Width.Set(0, 1f);
			Data = data;
		}

		private void ValidateInput()
		{
			int result;
			if (int.TryParse(range.input.Text, out result))
			{
				Data = result;
			}
			else
			{
				Data = data;
			}
		}
	}

	class ColorDataRangeProperty
	{
		private Color data = Color.White;
		internal Color Data
		{
			get { return data; }
			set
			{
				data = value;
				//range.input.SetText($"{data.R}-{data.G}-{data.B}");
				range.input.SetText(data.Hex3());
			}
		}

		public UIRange range;

		public ColorDataRangeProperty(string label)
		{
			//Main.hslToRgb(amount, 1f, 0.5f)
			range = new UIRange(label, () => Main.rgbToHsl(Data).X, (s) => { Data = Main.hslToRgb(s, 1f, 0.5f); }, ValidateInput);
			range.Width.Set(0, 1f);
			range.slider.SetHueMode(true);
			Data = data;
		}

		private void ValidateInput()
		{
			//int result;
			//if (int.TryParse(range.input.Text, out result))
			//{
			//	Data = result;
			//}
			//else
			//{
			//	Data = data;
			//}
			Data = data;
		}
	}
}
