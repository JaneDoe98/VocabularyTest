using VocabularyTest.Views;

namespace VocabularyTest
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(TestPage), typeof(TestPage));
            //Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            //Routing.RegisterRoute(nameof(WordsPage), typeof(WordsPage));
            //Routing.RegisterRoute(nameof(AddWordsPage), typeof(AddWordsPage));
        }
    }
}
