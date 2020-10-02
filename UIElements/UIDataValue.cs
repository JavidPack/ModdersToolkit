using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModdersToolkit.UIElements
{
	/// <summary>
	/// Purely a data storage
	/// </summary>
    internal class UIDataValue<T>
	{
		internal string label;
		internal Func<string> GetValueString;

		internal Func<T> DataGetter;
		internal Action<T> DataSetter;
		internal Action<T> SetValue;

		protected T defaultValue = default(T);
		// use data as memory when datasetter/getter are active to notify ui of changes
		protected T data = default(T);
		public T Data
		{
			get
			{
				if (DataGetter != null)
				{
					if (!data.Equals(DataGetter()))
					{
						data = DataGetter();
						OnValueChanged?.Invoke();
						return data;
					}
					return data;
				}
				return data;
			}
			protected set
			{
				if (DataSetter != null)
				{
					DataSetter(value);
					data = DataGetter();
				}
				else
				{
					data = value;
				}
				OnValueChanged?.Invoke();
			}
		}

		protected virtual void DefaultSetValue(T value)
		{
			Data = value;
		}

		protected virtual T DefaultGetValue(T value)
		{
			return Data;
		}

		protected virtual string DefaultGetValueString()
		{
			return Data.ToString();
		}

		public event Action OnValueChanged;

		public UIDataValue()
		{
			GetValueString = () => DefaultGetValueString();
			SetValue = (value) => DefaultSetValue(value);
			SetDefaultValue = (value) => DefaultSetDefaultValue(value);
			ResetToDefaultValue = () => DefaultResetToDefaultValue();
		}

		public UIDataValue(string label, T defaultValue) : this()
		{
			this.label = label;
			this.defaultValue = defaultValue;
			ResetToDefaultValue();
		}

		internal Action<T> SetDefaultValue;
		protected virtual void DefaultSetDefaultValue(T value)
		{
			defaultValue = value;
		}

		internal Action ResetToDefaultValue;
		protected virtual void DefaultResetToDefaultValue()
		{
			SetValue(defaultValue);
		}
	}

    internal class UIBoolDataValue : UIDataValue<bool>
	{
		public UIBoolDataValue(string label = "", bool defaultValue = false) : base(label, defaultValue)
		{
		}
	}

    internal class UIBoolNDataValue : UIDataValue<bool?>
	{
		public UIBoolNDataValue(string label = "", bool? defaultValue = null) : base(label, defaultValue)
		{
		}
	}

	// Add support for ranges, sliders
    internal abstract class UIRangedDataValue<T> : UIDataValue<T>
	{
		protected bool enforceMin;
		protected bool enforceMax;
		protected T min;
		protected T max;
		internal Func<float> GetProportion;
		internal Action<float> SetProportion;
		internal Action<string> ParseValue;
		internal Action Increment;
		internal Action Decrement;


		public UIRangedDataValue(string label = "", T defaultValue = default(T), T min = default(T), T max = default(T), bool enforceMin = false, bool enforceMax = false) : base(label, defaultValue)
		{
			this.min = min;
			this.max = max;
			this.enforceMin = enforceMin;
			this.enforceMax = enforceMax;
			GetProportion = () => DefaultGetProportion();
			SetProportion = (proportion) => DefaultSetProportion(proportion);
			ParseValue = (text) => DefaultParseValue(text);
			Increment = () => DefaultIncrement();
			Decrement = () => DefaultDecrement();

		}

		protected abstract float DefaultGetProportion();
		protected abstract void DefaultSetProportion(float proportion);
		protected abstract void DefaultParseValue(string value);
		protected abstract void DefaultIncrement();
		protected abstract void DefaultDecrement();
	}

	/// <summary>
	/// Float implementation
	/// </summary>
    internal class UIFloatRangedDataValue : UIRangedDataValue<float>
	{
		// ctor with alt Func and Action vs override.
		public UIFloatRangedDataValue(string label = "", float defaultValue = 0f, float min = 0f, float max = 1f, bool enforceMin = false, bool enforceMax = false) : base(label, defaultValue, min, max, enforceMin, enforceMax)
		{
		}

		//public UIFloatRangedDataValue(string label = "", float defaultValue = 0f, float min = 0f, float max = 1f, bool enforceMin = false, bool enforceMax = false) : base(label, min, max, enforceMin, enforceMax)
		//{
		//}

		// returns 
		protected override float DefaultGetProportion()
		{
			return (Data - min) / (max - min);
		}

		protected override void DefaultDecrement()
		{
			SetValue(Data - 0.05f * (max - min));
		}

		protected override void DefaultIncrement()
		{
			SetValue(Data + 0.05f * (max - min));
		}

		protected override void DefaultSetProportion(float proportion)
		{
			SetValue((proportion * (max - min) + min));
		}

		protected override void DefaultParseValue(string value)
		{
			float result;
			if (float.TryParse(value, out result))
			{
				SetValue(result);
			}
			else
			{
				SetValue(Data);
			}
		}

		protected override string DefaultGetValueString()
		{
			return Data.ToString("0.00");
		}

		/// <summary>
		/// All value changes pass through this, for range validation
		/// </summary>
		protected override void DefaultSetValue(float value)
		{
			if (value < min && enforceMin)
			{
				Data = min;
				return;
			}
			if (value > max && enforceMax)
			{
				Data = max;
				return;
			}
			Data = value;
		}
	}

	/// <summary>
	/// Float implementation
	/// </summary>
    internal class UIIntRangedDataValue : UIRangedDataValue<int>
	{
		// ctor with alt Func and Action vs override.
		public UIIntRangedDataValue(string label = "", int defaultValue = 0, int min = 0, int max = 100, bool enforceMin = false, bool enforceMax = false) : base(label, defaultValue, min, max, enforceMin, enforceMax)
		{
		}

		// returns 
		protected override float DefaultGetProportion()
		{
			return (float)(Data - min) / (max - min);
		}

		protected override void DefaultDecrement()
		{
			SetValue(Data - 1);
		}

		protected override void DefaultIncrement()
		{
			SetValue(Data + 1);
		}

		protected override void DefaultSetProportion(float proportion)
		{
			SetValue((int)(proportion * (max - min) + min));
		}

		protected override void DefaultParseValue(string value)
		{
			int result;
			if (int.TryParse(value, out result))
			{
				SetValue(result);
			}
			else
			{
				SetValue(Data);
			}
		}

		/// <summary>
		/// All value changes pass through this, for range validation
		/// </summary>
		protected override void DefaultSetValue(int value)
		{
			if (value < min && enforceMin)
			{
				Data = min;
				return;
			}
			if (value > max && enforceMax)
			{
				Data = max;
				return;
			}
			Data = value;
		}
	}

    internal class UIFloatRangedDataValueLogit : UIFloatRangedDataValue
	{
		public UIFloatRangedDataValueLogit(string label = "", float defaultValue = 0f, float min = 0, float max = 1, bool enforceMin = false, bool enforceMax = false) : base(label, defaultValue, min, max, enforceMin, enforceMax)
		{
		}

		// todo, scale by min max.

		protected override float DefaultGetProportion()
		{
			return (float)(Math.Exp(Data) / (1 + Math.Exp(Data)));
		}

		protected override void DefaultSetProportion(float proportion)
		{
			SetValue((float)Math.Log(proportion / (1 - proportion)));
		}
	}
}
