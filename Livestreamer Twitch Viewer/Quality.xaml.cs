using System.Windows;

namespace LivestreamerTwitchViewer
{
    /// <summary>
    /// Interaction logic for Quality.xaml
    /// </summary>
    public partial class Quality : Window
    {
        private string m_result;

        public Quality()
        {
            InitializeComponent();
            SetFont();
        }

        private void SetFont()
        {
            sentence.FontFamily = Globals.OldNewspaperTypes;
            quality.FontFamily = Globals.OldNewspaperTypes;
            adder.FontFamily = Globals.OldNewspaperTypes;
        }

        public string Result
        {
            get { return m_result; }
        }

        private void adder_Click(object sender, RoutedEventArgs e)
        {
            m_result = quality.Text;
            this.DialogResult = true;
            this.Close();
        }
    }
}
