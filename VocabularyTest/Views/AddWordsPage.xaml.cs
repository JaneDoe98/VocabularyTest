using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using ExcelDataReader;
using System.Data;
using System.Reflection;
using VocabularyTest.Models;

namespace VocabularyTest.Views;

public partial class AddWordsPage : ContentPage
{
    public AddWordsPage()
    {
        InitializeComponent();
    }

    private void addNewWordBtn_Clicked(object sender, EventArgs e)
    {
        if (hungarianEntr.Text == "" ||
            englishEntr.Text == "" ||
            difficultyPckr.SelectedItem == null ||
            topicPckr.SelectedItem == null)
        {
            DisplayAlert("Hi�nyz� adatok", "Adjon meg minden adatot e gomb f�l�tt!", "OK");
        }
        else
        {
            viewModel.AddSingleWord(hungarianEntr.Text, englishEntr.Text, ((Difficulty)difficultyPckr.SelectedItem).Id,
            ((Topic)topicPckr.SelectedItem).Id);

            DisplayAlert("Sikeres ment�s", "Az �j sz� beker�lt a sz�t�rba.", "OK");
        }
    }

    private async void addExcelDataBtn_Clicked(object sender, EventArgs e)
    {
        if (excelTopicPckr.SelectedItem == null)
        {
            await DisplayAlert("Nincs kiv�lasztott t�me", "El�bb v�lassza a ki a hozz�adand� adatok t�mak�r�t!", "OK");
        }
        else
        {
            var chosenFile = await PickFile();

            if (chosenFile != null)
            {
                if (chosenFile.FileName.EndsWith("xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    var excelTable = ReadExcel(chosenFile);

                    if (excelTable == null)
                    {
                        return;
                    }

                    if (excelTable.Rows.Count != 0)
                    {
                        if ((string)excelTable.Rows[0][0] != "Basic_eng" &&
                                    (string)excelTable.Rows[0][1] != "Basic_hun" &&
                                    (string)excelTable.Rows[0][2] != "Intermediate_eng" &&
                                    (string)excelTable.Rows[0][3] != "Intermediate_hun" &&
                                    (string)excelTable.Rows[0][4] != "Advanced_eng" &&
                                    (string)excelTable.Rows[0][5] != "Advanced_hun")
                        {
                            await DisplayAlert("Hib�s t�bla", "A t�bla val�sz�n�leg nem a megfelel� form�tumban van.", "OK");
                        }
                        else
                        {
                            viewModel.AddWords(excelTable, ((Topic)excelTopicPckr.SelectedItem).Id);

                            await DisplayAlert("Sikeres ment�s", "Az �j szavak beker�ltek a sz�t�rba.", "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Hiba", "A kiv�lasztott f�jl val�s, de nem tartalmaz sorokat.", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Hib�s f�jl form�tum", "A kiv�lasztott f�jl val�sz�n�leg nem Excel f�jl.", "OK");
                }
            }
        }
    }

    private async Task<FileResult?> PickFile()
    {
        var result = await FilePicker.Default.PickAsync();

        return result;

    }

    private DataTable? ReadExcel(FileResult pickedFile)
    {
        try
        {
            using (var stream = File.Open(pickedFile.FullPath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet().Tables[0];

                    return result;
                }
            }
        }
        catch (IOException)
        {
            DisplayAlert("Olvas�si hiba", "A kiv�lasztott f�jl val�sz�n�leg meg van nyitva, vagy valami m�s haszn�lja a sz�m�t�g�pen. El�sz�r z�rja be azt a folyamatot!", "OK");

            return null;
        }
        
    }

    private void downloadExcelSampleBtn_Clicked(object sender, EventArgs e)
    {
        CancellationTokenSource source = new CancellationTokenSource();
        CancellationToken token = source.Token;

        GetSampleFile(token);
    }

    private async Task GetSampleFile(CancellationToken cancellationToken)
    {
        string fileName = "data_sample.xlsx";

        var assembly = Assembly.GetExecutingAssembly();
        var resourcePath = $"VocabularyTest.Resources.{fileName}";

        using (var stream = assembly.GetManifestResourceStream(resourcePath))
        {

            var fileSaverResult = await FileSaver.Default.SaveAsync(fileName, stream, cancellationToken);
            if (fileSaverResult.IsSuccessful)
            {
                await Toast.Make($"Minta Excel mentve ide:\n{fileSaverResult.FilePath}").Show(cancellationToken);
            }
            else
            {
                await Toast.Make($"Ment�si hiba:\n{fileSaverResult.Exception.Message}").Show(cancellationToken);
            }
        }
    }

    private void goBackBtn_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//Words");
    }
}