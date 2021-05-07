using System;
using System.IO;
using LmlLibrary;
using Localization = WPFMeteroWindow.Resources.localizations.Resources;

namespace WPFMeteroWindow
{
    public class KeyboardLayoutEditor
    {
        private string _filePath;

        private bool _isNewLayout;

        public string[][] LayoutKeys { get; set; }

        public KeyboardLayoutEditor()
        {
            _isNewLayout = true;
            NullUpKeys();
        }

        public KeyboardLayoutEditor(string filePath)
        {
            _filePath = filePath;
            _isNewLayout = false;

            NullUpKeys();
            if (File.Exists(filePath))
            {
                var keyboardData = new Lml(filePath, Lml.Open.FromFile);
                for (int i = 0; i < 61; i++)
                {
                    LayoutKeys[i] = keyboardData.GetArray($"Layout>k{i}");

                    for (int j = 0; j < 4; j++)
                        LayoutKeys[i][j] = LayoutKeys[i][j].ToBeCorrected();
                }
            }
        }

        public void WriteDataOnFile()
        {
            if (!_isNewLayout)
            {
                if (!string.IsNullOrEmpty(_filePath))
                    File.WriteAllText(_filePath, ToLml());
                else
                    throw new NotImplementedException("Empty data");
            }

            else
            {
                var writer = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "LML-files | *.lml",
                    DefaultExt = ".lml"
                };

                if (writer.ShowDialog() == true)
                {
                    _isNewLayout = false;
                    _filePath = writer.FileName;

                    File.WriteAllText(_filePath, ToLml());
                    Intermediary.LayoutPage.EditorTitleTextBox.Text = $"{Path.GetFileName(_filePath)} - {Localization.uKbLayoutEditor}";
                }
            }
        }

        private void NullUpKeys()
        {
            LayoutKeys = new string[61][];
            for (int i = 0; i< 61; i++)
                LayoutKeys[i] = new string[] { "", "", "", "" };
        }

        private string ToLml()
        {
            var data = "<<Layout:\n";
            for (int i = 0; i < 61; i++)
            {
                if (i == 56)
                {
                    data += "    <k56 { ;space; ;nth; ;nth; ;nth; }>>\n";
                    continue;
                }

                data += $"    <k{i} {{ ";
                data += $"{LayoutKeys[i][0].ToLmlFormat()} {LayoutKeys[i][1].ToLmlFormat()} {LayoutKeys[i][2].ToLmlFormat()} {LayoutKeys[i][3].ToLmlFormat()}";
                data += " }>>\n";

                if ((i == 13) || (i == 27) || (i == 50) || (i == 52))
                    data += '\n';
            }

            data += ":Layout>>";
            return data;
        }

        public override string ToString() =>
            _filePath;
    }
}
