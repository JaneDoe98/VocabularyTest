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
            DisplayAlert("Hiányzó adatok", "Adjon meg minden adatot e gomb fölött!", "OK");
        }
        else
        {
            viewModel.AddSingleWord(hungarianEntr.Text, englishEntr.Text, ((Difficulty)difficultyPckr.SelectedItem).Id,
            ((Topic)topicPckr.SelectedItem).Id);

            DisplayAlert("Sikeres mentés", "Az új szó bekerült a szótárba.", "OK");
        }
    }

    private async void addExcelDataBtn_Clicked(object sender, EventArgs e)
    {
        if (excelTopicPckr.SelectedItem == null)
        {
            await DisplayAlert("Nincs kiválasztott téme", "Elõbb válassza a ki a hozzáadandó adatok témakörét!", "OK");
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
                            await DisplayAlert("Hibás tábla", "A tábla valószínûleg nem a megfelelõ formátumban van.", "OK");
                        }
                        else
                        {
                            viewModel.AddWords(excelTable, ((Topic)excelTopicPckr.SelectedItem).Id);

                            await DisplayAlert("Sikeres mentés", "Az új szavak bekerültek a szótárba.", "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Hiba", "A kiválasztott fájl valós, de nem tartalmaz sorokat.", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Hibás fájl formátum", "A kiválasztott fájl valószínûleg nem Excel fájl.", "OK");
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
            DisplayAlert("Olvasási hiba", "A kiválasztott fájl valószínûleg meg van nyitva, vagy valami más használja a számítógépen. Elõször zárja be azt a folyamatot!", "OK");

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
                await Toast.Make($"Mentési hiba:\n{fileSaverResult.Exception.Message}").Show(cancellationToken);
            }
        }
    }

    private void goBackBtn_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//Words");
    }
}