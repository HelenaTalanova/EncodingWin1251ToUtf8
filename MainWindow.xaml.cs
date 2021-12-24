using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Text;
using System.Windows;

namespace EncodingWin1251ToUtf8
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string path;

        public MainWindow()
        {
            InitializeComponent();

            ButtonOpen.Click += (_, _) =>
            {
                var dialog = new CommonOpenFileDialog()
                {
                    IsFolderPicker = true
                };

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    path = dialog.FileName;
                    TextPath.Text = path;
                }
            };

            ButtonEncode.Click += (_, _) =>
            {
                TextFiles.Text = $"Files convert {Encode()}";
            };
        }

        private int Encode()
        {
            var count = 0;

            if (!string.IsNullOrWhiteSpace(path))
            {
                var converter = new ConverterWin1251ToUtf8();
                
                foreach (var f in new DirectoryInfo(path).GetFiles("*.cs", SearchOption.AllDirectories))
                {
                    count++;
                    converter.DecodeFile1251ToUTF(f.FullName);
                    Console.WriteLine($"File write: {f.FullName}");
                }
            }

            return count;
        }
    }

    internal class ConverterWin1251ToUtf8
    {
        protected Encoding Utf8;
        protected Encoding Win1251;

        public ConverterWin1251ToUtf8()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Utf8 = Encoding.GetEncoding("UTF-8");
            Win1251 = Encoding.GetEncoding("Windows-1251");
        }

        public void DecodeFile1251ToUTF(string path)
        {
            var text = File.ReadAllText(path, Win1251);

            var win1251Bytes = Win1251.GetBytes(text);
            var utf8Bytes = Encoding.Convert(Win1251, Utf8, win1251Bytes);

            var result = Utf8.GetString(utf8Bytes);
            File.WriteAllText(path, result, Utf8);
        }
    }
}
