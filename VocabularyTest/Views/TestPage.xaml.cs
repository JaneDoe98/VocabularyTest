using VocabularyTest.Data;
using VocabularyTest.Models;
using VocabularyTest.ViewModels;

namespace VocabularyTest.Views;

[QueryProperty(nameof(ChosenWordNumber), "WordNumber")]
[QueryProperty(nameof(ChosenDifficulty), "Difficulty")]
[QueryProperty(nameof(ChosenTopic), "Topic")]
[QueryProperty(nameof(OnlyUnsolved), "Unsolved")]
public partial class TestPage : ContentPage
{
    public int ChosenWordNumber { get; set; }
    public int ChosenDifficulty { get; set; }
    public int ChosenTopic { get; set; }
    public bool OnlyUnsolved { get; set; }

    private WordViewModel VM;

    public TestPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        VM = new WordViewModel(ChosenWordNumber, ChosenDifficulty, ChosenTopic, OnlyUnsolved);
        wordList.ItemsSource = VM.Words;

        base.OnAppearing();
    }


    private void CancelBtn_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//MainPage");
    }

    private void SubmitBtn_Clicked(object sender, EventArgs e)
    {
        foreach (Word word in this.VM.Words)
        {
            word.CheckAnswers();
            if (word.IsCorrect)
            {
                WordDTO wordToUpdate = VM._context.Word.Where(x => x.WordId == word.Id).Single();
                if (wordToUpdate.IsUnsolved == 1)
                {
                    wordToUpdate.IsUnsolved = 0;
                    VM._context.Update(wordToUpdate);
                }
            }
        }

        VM._context.SaveChanges();

        VM.isSubmitted = true;

        int correctAnswers = this.VM.Words.Count(x => x.IsCorrect);

        double percentage = Math.Round((double)correctAnswers / this.VM.Words.Count * 100);

        DisplayAlert("Eredmények", $"{correctAnswers}/{this.VM.Words.Count}\n{percentage}%", "OK");
    }
}