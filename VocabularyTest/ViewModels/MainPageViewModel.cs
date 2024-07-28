using VocabularyTest.Data;
using VocabularyTest.Models;

namespace VocabularyTest.ViewModels
{
    internal class MainPageViewModel
    {
        public List<Difficulty> Difficulties { get; set; }
        public List<Topic> Topics { get; set; }

        internal WordDataContext _context = new WordDataContext();

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
    }
}
