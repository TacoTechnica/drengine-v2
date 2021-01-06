namespace DREngine.Editor.SubWindows.FieldWidgets
{
    public class IntegerTextFieldWidget : AbstractTextFieldWidget<int>
    {
        protected override int FromString(string value)
        {
            if (value == "") return 0;
            return int.Parse(value);
        }

        protected override string DataToString(int value)
        {
            return value.ToString();
        }

        protected override bool IsValidParse(string value)
        {
            if (value == "") return true;
            return int.TryParse(value, out _);
        }
    }
}
