namespace DREngine.Editor.SubWindows.FieldWidgets
{
    public class FloatTextFieldWidget : AbstractTextFieldWidget<float>
    {
        protected override float FromString(string value)
        {
            return float.Parse(value);
        }

        protected override string DataToString(float value)
        {
            return value.ToString();
        }

        protected override bool IsValidParse(string value)
        {
            return float.TryParse(value, out _);
        }
    }
}
