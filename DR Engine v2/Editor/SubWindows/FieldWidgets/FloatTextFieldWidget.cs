using System.Globalization;

namespace DREngine.Editor.SubWindows.FieldWidgets
{
    public class FloatTextFieldWidget : AbstractTextFieldWidget<float>
    {
        protected override float FromString(string value)
        {
            if (value == "") return 0;
            return float.Parse(value);
        }

        protected override string DataToString(float value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        protected override bool IsValidParse(string value)
        {
            if (value == "") return true;
            return float.TryParse(value, out _);
        }
    }
}
