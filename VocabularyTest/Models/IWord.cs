namespace VocabularyTest.Models
{
    interface IWord
    {
        int Id { get; set; }
        string EnglishWord { get; set; }
        string HungarianWord { get; set; }
    }
}
