using VocabularyTest.Models;
using VocabularyTest.ViewModels;

namespace VocabularyTest.Views;

public partial class WordsPage : ContentPage
{
    private Locale? engLocale;

    public WordsPage()
    {
        InitializeComponent();
        FindTTS();
    }

    private void TopicPckr_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (topicPckr.SelectedItem != null)
        {
            listViewModel.LoadWords(((Topic)topicPckr.SelectedItem).Id);
        }

        BasicChckBx.IsChecked = true;
        IntermediateChckBx.IsChecked = true;
        AdvancedChckBx.IsChecked = true;
    }
    private void BasicChckBx_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        foreach (var word in listViewModel.Words)
        {
            if (word.Difficulty == "Basic")
            {
                word.IsCorrectDifficulty = e.Value;
            }
        }
    }

    private void IntermediateChckBx_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        foreach (var word in listViewModel.Words)
        {
            if (word.Difficulty == "Intermediate")
            {
                word.IsCorrectDifficulty = e.Value;
            }
        }
    }

    private void AdvancedChckBx_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        foreach (var word in listViewModel.Words)
        {
            if (word.Difficulty == "Advanced")
            {
                word.IsCorrectDifficulty = e.Value;
            }
        }
    }

    private void HideKnownWordsChckBx_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        foreach (var word in listViewModel.Words)       //ebbõl static class member-t
        {
            word.HideSolved = e.Value;
        }
    }

    private async void newWordsBtn_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//AddWords");
    }

    private async void newTopicBtn_Clicked(object sender, EventArgs e)
    {
        string topicName = await DisplayPromptAsync("Új Téma", "Adja meg a témakör nevét!", "Létrehozás", "Mégse");
        if (topicName != null)
        {
            listViewModel.AddTopic(topicName);
        }
    }

    private async void updateTopicBtn_Clicked(object sender, EventArgs e)
    {
        if (topicPckr.SelectedItem != null)
        {
            string newTopicName = await DisplayPromptAsync("Téma módosítása", $"Adja meg a \"{((Topic)topicPckr.SelectedItem).Name}\" nevezetû témakör új nevét!", "Tovább", "Mégse");

            if (newTopicName != null)
            {
                bool isConfirmed = await DisplayAlert("Megerõsítés", $"Eredeti név: {((Topic)topicPckr.SelectedItem).Name} \nÚj név: {newTopicName}\nFolytatja?", "Igen", "Nem");

                if (isConfirmed)
                {
                    listViewModel.UpdateTopic(((Topic)topicPckr.SelectedItem).Id, newTopicName);

                    topicPckr.ItemsSource = listViewModel.Topics;
                    topicPckr.SelectedItem = newTopicName;
                }
            }
        }
        else
        {
            await DisplayAlert("Nincs kiválasztott téma", "Kérem, válassza ki, hogy melyik témát szeretné módosítani a lenti témakör választóból!", "OK");
        }
    }

    private async void deleteWordBtn_Clicked(object sender, EventArgs e)
    {
        if (wordsView.SelectedItem != null)
        {
            bool answer = await DisplayAlert("Törlés megerõsítése", $"Biztos törölni szeretné a kiválasztott szót?\n{((DetailedWord)wordsView.SelectedItem).EnglishWord} - {((DetailedWord)wordsView.SelectedItem).HungarianWord}", "Igen", "Nem");

            if (answer)
            {
                listViewModel.DeleteWord((DetailedWord)wordsView.SelectedItem);
            }
        }
        else
        {
            await DisplayAlert("Nincs kiválasztott szó", "Kérem, válassza ki, hogy melyik szavat szeretné törölni a listából!", "OK");
        }
    }

    private async void deleteTopicBtn_Clicked(object sender, EventArgs e)
    {
        if (topicPckr.SelectedItem != null)
        {
            bool answer = await DisplayAlert("Törlés megerõsítése", $"Biztos törölni szeretné a kiválasztott témát? Ezzel a mûvelettel a hozzátartozó szavak is törlésre kerülnek!\n{((Topic)topicPckr.SelectedItem).Name}", "Igen", "Nem");

            if (answer)
            {
                listViewModel.DeleteTopic((Topic)topicPckr.SelectedItem);
            }
        }
        else
        {
            await DisplayAlert("Nincs kiválasztott téma", "Kérem, válassza ki, hogy melyik témát szeretné törölni a lenti témakör választóból!", "OK");
        }
    }

    private void mainPageBtn_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//MainPage");
    }

    private void wordsView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (wordsView.SelectedItem != null)
        {
            PronunceWord(((DetailedWord)wordsView.SelectedItem).EnglishWord);
        }
    }



    CancellationTokenSource cts;

    private async Task PronunceWord(string word)
    {
        if (engLocale != null)
        {
            CancelSpeech();

            SpeechOptions options = new SpeechOptions()
            {
                Locale = engLocale
            };
            cts = new CancellationTokenSource();
            await TextToSpeech.Default.SpeakAsync(word, options, cts.Token);
        }
    }

    public void CancelSpeech()
    {
        if (cts?.IsCancellationRequested ?? true)
        {
            return;
        }

        cts.Cancel();
    }

    private async Task FindTTS()
    {
        var locales = await TextToSpeech.GetLocalesAsync();

        engLocale = locales.FirstOrDefault(x => x.Name == "Microsoft Zira");

        if (engLocale == null)
        {
            bool answer = await DisplayAlert("Beszédhang nem található", "A program által használt beszédhang nem található a számítógépen. Amennyiben kijelöléskor szeretné hallani a szavak kiejtését, kérem töltse le a kívánt beszédhangot a weboldal alapján (hangcsomag neve: \"angol (egyesült államokbeli)\")!", "Weboldal Megnyitása", "Mégse");
            if (answer)
            {
                Uri uri = new Uri("https://support.microsoft.com/hu-hu/topic/nyelvek-%C3%A9s-hangok-let%C3%B6lt%C3%A9se-modern-olvas%C3%B3-olvas%C3%A1si-m%C3%B3d-%C3%A9s-felolvas%C3%A1s-c%C3%A9lj%C3%A1b%C3%B3l-4c83a8d8-7486-42f7-8e46-2b0fdf753130");
                await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            }
        }
    }
}