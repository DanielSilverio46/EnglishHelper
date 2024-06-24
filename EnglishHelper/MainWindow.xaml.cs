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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        App.DataUser user = new App.DataUser(0, 0);
        App.DataWords words;

        /// <summary>
        /// This method initializes the components of the main window, like sqlite data base and text of Labels
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            App.SQL.SetupSql();

            App.SQL.RandomWord(out string portuguese, out string english);
            words = new App.DataWords(portuguese, english);

            English.Content = words.English;
            user.OnLabels(PointsLabel, FailsLabel);

            user.UpdateLabel(before_points: "Points: ", before_fails: "Fails: ");
        }

        /// <summary>
        /// Process the data of text box (InputWordPortuguese) and modify the label PointsLabel and FailsLabel
        /// </summary>
        public void ProcessData()
        {
            bool match = words.PortugueseMatchWord(InputWordPortuguese.Text);

            if (match) user.AddPoint();
            else user.AddFail();

            App.SQL.RandomWord(out string portuguese, out string english);
            words.ChangeWords(portuguese, english);

            English.Content = words.English;
            user.UpdateLabel(before_points: "Points: ", before_fails: "Fails: ");

            InputWordPortuguese.Text = String.Empty;
        }

        /// <summary>
        /// Check if enter key is pressed on InputWordPortuguese text box and then call ProcessData function
        /// </summary>
        private void CatchKeyEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.ProcessData();
            }
        }

        /// <summary>
        /// Check if the ok button is pressed and then call ProcessData function 
        /// </summary>
        private void OkPressed(object sender, EventArgs e)
        {
            this.ProcessData();
        }

        /// <summary>
        /// Check if the words button is pressed and then call ProcessData function
        /// </summary>
        private void WordsPressed(object sender, EventArgs e)
        {
            new Words().Show();
        }
    }
}