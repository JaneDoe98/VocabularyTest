namespace VocabularyTest
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            MainPage = new AppShell();
        }
    }
}
