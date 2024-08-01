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
        foreach (var word in listViewModel.Words)       //ebb�l static class member-t
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
        string topicName = await DisplayPromptAsync("�j T�ma", "Adja meg a t�mak�r nev�t!", "L�trehoz�s", "M�gse");
        if (topicName != null)
        {
            listViewModel.AddTopic(topicName);
        }
    }

    private async void updateTopicBtn_Clicked(object sender, EventArgs e)
    {
        if (topicPckr.SelectedItem != null)
        {
            string newTopicName = await DisplayPromptAsync("T�ma m�dos�t�sa", $"Adja meg a \"{((Topic)topicPckr.SelectedItem).Name}\" nevezet� t�mak�r �j nev�t!", "Tov�bb", "M�gse");

            if (newTopicName != null)
            {
                bool isConfirmed = await DisplayAlert("Meger�s�t�s", $"Eredeti n�v: {((Topic)topicPckr.SelectedItem).Name} \n�j n�v: {newTopicName}\nFolytatja?", "Igen", "Nem");

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
            await DisplayAlert("Nincs kiv�lasztott t�ma", "K�rem, v�lassza ki, hogy melyik t�m�t szeretn� m�dos�tani a lenti t�mak�r v�laszt�b�l!", "OK");
        }
    }

    private async void deleteWordBtn_Clicked(object sender, EventArgs e)
    {
        if (wordsView.SelectedItem != null)
        {
            bool answer = await DisplayAlert("T�rl�s meger�s�t�se", $"Biztos t�r�lni szeretn� a kiv�lasztott sz�t?\n{((DetailedWord)wordsView.SelectedItem).EnglishWord} - {((DetailedWord)wordsView.SelectedItem).HungarianWord}", "Igen", "Nem");

            if (answer)
            {
                listViewModel.DeleteWord((DetailedWord)wordsView.SelectedItem);
            }
        }
        else
        {
            await DisplayAlert("Nincs kiv�lasztott sz�", "K�rem, v�lassza ki, hogy melyik szavat szeretn� t�r�lni a list�b�l!", "OK");
        }
    }

    private async void deleteTopicBtn_Clicked(object sender, EventArgs e)
    {
        if (topicPckr.SelectedItem != null)
        {
            bool answer = await DisplayAlert("T�rl�s meger�s�t�se", $"Biztos t�r�lni szeretn� a kiv�lasztott t�m�t? Ezzel a m�velettel a hozz�tartoz� szavak is t�rl�sre ker�lnek!\n{((Topic)topicPckr.SelectedItem).Name}", "Igen", "Nem");

            if (answer)
            {
                listViewModel.DeleteTopic((Topic)topicPckr.SelectedItem);
            }
        }
        else
        {
            await DisplayAlert("Nincs kiv�lasztott t�ma", "K�rem, v�lassza ki, hogy melyik t�m�t szeretn� t�r�lni a lenti t�mak�r v�laszt�b�l!", "OK");
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
            bool answer = await DisplayAlert("Besz�dhang nem tal�lhat�", "A program �ltal haszn�lt besz�dhang nem tal�lhat� a sz�m�t�g�pen. Amennyiben kijel�l�skor szeretn� hallani a szavak kiejt�s�t, k�rem t�ltse le a k�v�nt besz�dhangot a weboldal alapj�n (hangcsomag neve: \"angol (egyes�lt �llamokbeli)\")!", "Weboldal Megnyit�sa", "M�gse");
            if (answer)
            {
                Uri uri = new Uri("https://support.microsoft.com/hu-hu/topic/nyelvek-%C3%A9s-hangok-let%C3%B6lt%C3%A9se-modern-olvas%C3%B3-olvas%C3%A1si-m%C3%B3d-%C3%A9s-felolvas%C3%A1s-c%C3%A9lj%C3%A1b%C3%B3l-4c83a8d8-7486-42f7-8e46-2b0fdf753130");
                await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            }
        }
    }
}