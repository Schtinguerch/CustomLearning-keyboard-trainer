namespace WPFMeteroWindow.Controls
{
    public interface ITextInputControl
    {
        string DoneText { get; set; }
        string LeftText { get; set; }
        string AllText { get; set; }
        string ErrorText { get; set; }
        void LoadText(string text);
    }
}
