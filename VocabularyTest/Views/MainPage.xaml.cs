using VocabularyTest.Models;

namespace VocabularyTest.Views
{
    public partial class MainPage : ContentPage
    {
        private bool anyDifficulty;
        private bool anyTopic;
        private bool onlyUnsolved;

        public MainPage()
        {
            RequestPermission();

            InitializeComponent();
        }

        private void StartBtn_Clicked(object sender, EventArgs e)
        {
            if ((DifficultyChckBx.IsChecked || DifficultyPckr.SelectedItem != null) &&
                (TopicChckBx.IsChecked || TopicPckr.SelectedItem != null))
            {
                int wordNumber = Int32.Parse(sliderLbl.Text);
                int difficultyId = anyDifficulty == true ? -1 : ((Difficulty)DifficultyPckr.SelectedItem).Id;
                int topicId = anyTopic == true ? -1 : ((Topic)TopicPckr.SelectedItem).Id;

                Shell.Current.GoToAsync($"{nameof(TestPage)}?WordNumber={wordNumber}&Difficulty={difficultyId}&Topic={topicId}&Unsolved={onlyUnsolved}");
            }
            else
            {
                DisplayAlert("Adatok nincsenek megadva", "Nincs kiválasztva nehézség és/vagy témakör.", "OK");
            }
        }

        private void DifficultyChckBx_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            anyDifficulty = e.Value;
            DifficultyPckr.IsEnabled = !e.Value;

            if (e.Value == true)
            {
                DifficultyPckr.SelectedItem = null;
            }
        }

        private void TopicChckBx_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            anyTopic = e.Value;
            TopicPckr.IsEnabled = !e.Value;

            if (e.Value == true)
            {
                TopicPckr.SelectedItem = null;
            }
        }

        private void WordNumberSldr_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (sender is Slider slider)
            {
                double newStep = 1.0;
                double newValue = Math.Round(e.NewValue / newStep) * newStep;
                slider.Value = newValue;

                sliderLbl.Text = slider.Value.ToString();
            }
        }

        private void onlyUnsolvedChckBx_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            onlyUnsolved = e.Value;
        }

        private void AllWordsBtn_Clicked(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync("//Words");
        }

        private async void RequestPermission()
        {
            PermissionStatus statusRead = await Permissions.RequestAsync<Permissions.StorageRead>();
            PermissionStatus statusWrite = await Permissions.RequestAsync<Permissions.StorageWrite>();
        }
    }
}
