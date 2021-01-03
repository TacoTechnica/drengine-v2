namespace DREngine.Editor.SubWindows.FieldWidgets
{
    public class IntegerTextFieldWidget : AbstractTextFieldWidget<int>
    {
        protected override int FromString(string value)
        {
            return int.Parse(value);
        }

        protected override string DataToString(int value)
        {
            return value.ToString();
        }

        protected override bool IsValidParse(string value)
        {
            return int.TryParse(value, out _);
        }
    }
}
