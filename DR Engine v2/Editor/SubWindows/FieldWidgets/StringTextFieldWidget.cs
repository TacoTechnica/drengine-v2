namespace DREngine.Editor.SubWindows.FieldWidgets
{
    public class StringTextFieldWidget : AbstractTextFieldWidget<string>
    {
        protected override string FromString(string value)
        {
            return value;
        }

        protected override string DataToString(string value)
        {
            return value;

        }

        protected override bool IsValidParse(string value)
        {
            return true;
        }
    }
}