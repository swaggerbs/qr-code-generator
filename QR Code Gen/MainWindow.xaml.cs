using WpfApp2.Models;

namespace QR_Code_Gen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new FormViewModel();
        }
    }
}