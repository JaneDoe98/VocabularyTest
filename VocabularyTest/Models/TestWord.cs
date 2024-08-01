using System.ComponentModel;

namespace VocabularyTest.Models
{
    internal class TestWord : IWord, INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string EnglishWord { get; set; }
        public string HungarianWord { get; set; }

        public string UserAnswer { get; set; }

        public bool IsUnsolved { get; set; }
        public bool IsCorrect { get; set; } = false;
        public bool IsEvenIndex { get; set; }

        public TestWord(int id, string english, string hungarian, bool isUnsolved = true)
        {
            Id = id;
            EnglishWord = english;
            HungarianWord = hungarian;
            IsUnsolved = isUnsolved;
        }

        public void CheckAnswers()
        {
            IsCorrect = string.Equals(UserAnswer, EnglishWord, StringComparison.CurrentCultureIgnoreCase);

            OnPropertyChanged(nameof(IsCorrect));

            if (IsCorrect)
            {
                IsUnsolved = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
