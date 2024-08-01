using System.ComponentModel;
using VocabularyTest.Data;
using VocabularyTest.Models;

namespace VocabularyTest.ViewModels
{
    internal class MainPageViewModel : INotifyPropertyChanged
    {
        public List<Difficulty> Difficulties { get; set; }
        private List<Topic> topic;
        public List<Topic> Topics
        {
            get
            {
                return topic;
            }
            set
            {
                topic = value;
                OnPropertyChanged(nameof(Topics));
            }
        }

        internal WordDataContext _context = new WordDataContext();

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public MainPageViewModel()
        {
            Difficulties = new List<Difficulty>(_context.Difficulty.Select(x => DtoToModel(x)).ToList());

            Topics = new List<Topic>(_context.Topic.Select(x => DtoToModel(x)).ToList());
        }

        public static Difficulty DtoToModel(DifficultyDTO dto)
        {
            return new Difficulty(dto.DifficultyId, dto.DifficultyLevel);
        }

        public static Topic DtoToModel(TopicDTO dto)
        {
            return new Topic(dto.TopicId, dto.TopicName);
        }

        public void LoadTopics()
        {
            Topics = new List<Topic>(_context.Topic.Select(x => DtoToModel(x)).ToList());
        }
    }
}
