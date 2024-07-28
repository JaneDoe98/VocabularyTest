using VocabularyTest.Data;
using VocabularyTest.Models;

namespace VocabularyTest.ViewModels
{
    internal class WordViewModel
    {
        public List<Word> Words { get; set; }
        internal WordDataContext _context = new WordDataContext();

        public bool isSubmitted = false;

        public WordViewModel(int wordNumber, int difficultyId, int topicId, bool onlyUnsolved)
        {
            if(difficultyId != -1 && topicId != -1)
            {
                Words = new List<Word>(_context.Word.Where(x => x.DifficultyId == difficultyId &&
                    x.TopicId == topicId)
                    .Select(x => DtoToModel(x)).ToList());
            }
            else if(difficultyId != -1 && topicId == -1)
            {
                Words = new List<Word>(_context.Word.Where(x => x.DifficultyId == difficultyId)
                    .Select(x => DtoToModel(x)).ToList());
            }
            else if(difficultyId == -1 && topicId != -1)
            {
                Words = new List<Word>(_context.Word.Where(x => x.TopicId == topicId)
                    .Select(x => DtoToModel(x)).ToList());
            }
            else
            {
                Words = new List<Word>(_context.Word.Select(x => DtoToModel(x)).ToList());
            }
            
            var rng = new Random();
            Words.Shuffle(rng);

            if (onlyUnsolved)
            {
                Words = Words.Where(x => x.IsUnsolved == true).Take(wordNumber).Select((x, i) =>
                {
                    x.IsEvenIndex = i % 2 == 0;
                    return x;
                }).ToList();
            }
            else
            {
                Words = Words.Take(wordNumber).Select((x, i) =>
                {
                    x.IsEvenIndex = i % 2 == 0;
                    return x;
                }).ToList();
            }
            
        }

        public static Word DtoToModel(WordDTO dto)
        {
            return new Word(dto.WordId, dto.EnglishWord, dto.HungarianWord, dto.IsUnsolved == 0 ? false : true);
        }
    }
    static class WordListExtensions
    {
        public static void Shuffle<T>(this IList<T> list, Random rng)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
