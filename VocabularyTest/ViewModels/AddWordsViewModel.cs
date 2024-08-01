using System.ComponentModel;
using System.Data;
using VocabularyTest.Data;
using VocabularyTest.Models;

namespace VocabularyTest.ViewModels
{
    internal class AddWordsViewModel : INotifyPropertyChanged
    {
        private List<Topic> topics;
        public List<Topic> Topics
        {
            get
            {
                return topics;
            }
            set
            {
                topics = value;
                OnPropertyChanged(nameof(Topics));
            }
        }
        public List<Difficulty> Difficulties { get; set; }

        internal WordDataContext _context = new WordDataContext();


        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        public AddWordsViewModel()
        {
            Difficulties = new List<Difficulty>(_context.Difficulty.Select(x => DtoToModel(x)).ToList());
        }

        public static Topic DtoToModel(TopicDTO dto)
        {
            return new Topic(dto.TopicId, dto.TopicName);
        }

        public static Difficulty DtoToModel(DifficultyDTO dto)
        {
            return new Difficulty(dto.DifficultyId, dto.DifficultyLevel);
        }

        public void LoadTopics()
        {
            Topics = new List<Topic>(_context.Topic.Select(x => DtoToModel(x)).ToList());
        }

        public void AddSingleWord(string hunWord, string engWord, int difficultyId, int topicId)
        {
            WordDTO newWord = new WordDTO()
            {
                EnglishWord = engWord,
                HungarianWord = hunWord,
                DifficultyId = difficultyId,
                TopicId = topicId
            };

            _context.Word.Add(newWord);

            _context.SaveChanges();
        }

        public void AddWords(DataTable table, int chosenTopicId)
        {

            for (int i = 1; i < table.Rows.Count; i++)
            {
                if (table.Rows[i][0] != DBNull.Value &&
                    table.Rows[i][1] != DBNull.Value &&
                    (string)table.Rows[i][0] != "" &&
                    (string)table.Rows[i][1] != "")
                {
                    WordDTO newWord = new WordDTO()
                    {
                        EnglishWord = (string)table.Rows[i][0],
                        HungarianWord = (string)table.Rows[i][1],
                        DifficultyId = 1,
                        TopicId = chosenTopicId
                    };

                    _context.Word.Add(newWord);
                }

                if (table.Rows[i][3] != DBNull.Value &&
                    table.Rows[i][4] != DBNull.Value &&
                    (string)table.Rows[i][3] != "" &&
                    (string)table.Rows[i][4] != "")
                {
                    WordDTO newWord = new WordDTO()
                    {
                        EnglishWord = (string)table.Rows[i][3],
                        HungarianWord = (string)table.Rows[i][4],
                        DifficultyId = 2,
                        TopicId = chosenTopicId
                    };

                    _context.Word.Add(newWord);
                }

                if (table.Rows[i][6] != DBNull.Value &&
                    table.Rows[i][7] != DBNull.Value &&
                    (string)table.Rows[i][6] != "" &&
                    (string)table.Rows[i][7] != "")
                {
                    WordDTO newWord = new WordDTO()
                    {
                        EnglishWord = (string)table.Rows[i][6],
                        HungarianWord = (string)table.Rows[i][7],
                        DifficultyId = 3,
                        TopicId = chosenTopicId
                    };

                    _context.Word.Add(newWord);
                }
            }
            _context.SaveChanges();
        }
    }
}