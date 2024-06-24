using System.Diagnostics.Eventing.Reader;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EnglishHelper
{
    partial class Words : Window
    {
        public Words()
        {
            InitializeComponent();

            App.SQL.AllWordsToTextBox((uint)App.SQL.ReturnTotalRows(), InputBox);
        }

        public void CheckCrtlS(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.S)
            {
                e.Handled = true;
                InputBox_String_ToStingMatrix();
            }
        }

        private void InputBox_String_ToStingMatrix()
        {
            string[] words = InputBox.Text.Split(";");

            for (uint i = 0; i < (words.Length - 1); i++)
            {
                words[i].Replace("\r\n", "");
                App.SQL.WordsIntoBank(words[i], i);
            }
        }
    }
}