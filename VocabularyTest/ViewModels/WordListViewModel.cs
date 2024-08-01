using CommunityToolkit.Maui.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using VocabularyTest.Data;
using VocabularyTest.Models;

namespace VocabularyTest.ViewModels
{
    internal class WordListViewModel : ObservableCollection<DetailedWord>, INotifyPropertyChanged
    {
        private ObservableCollection<DetailedWord> words;

        public ObservableCollection<DetailedWord> Words
        {
            get
            {
                return words;
            }
            set
            {
                words = value;
                OnPropertyChanged(nameof(Words));
            }
        }
        private ObservableCollection<Topic> topics;

        private int nextTopicId;

        public ObservableCollection<Topic> Topics
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

        public WordDataContext _context = new WordDataContext();

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public WordListViewModel()
        {
            Topics = new ObservableCollection<Topic>(_context.Topic.Select(x => DtoToModel(x)).ToList());
            foreach (var topic in Topics)
            {
                nextTopicId = Math.Max(nextTopicId, topic.Id + 1);
            }
        }

        public static DetailedWord DtoToModel(WordDTO dto, string difficulty)
        {
            return new DetailedWord()
            {
                Id = dto.WordId,
                EnglishWord = dto.EnglishWord,
                HungarianWord = dto.HungarianWord,
                IsUnsolved = dto.IsUnsolved == 0 ? false : true,
                Difficulty = difficulty
            };
        }

        public static Difficulty DtoToModel(DifficultyDTO dto)
        {
            return new Difficulty(dto.DifficultyId, dto.DifficultyLevel);
        }

        public static Topic DtoToModel(TopicDTO dto)
        {
            return new Topic(dto.TopicId, dto.TopicName);
        }

        public void LoadWords(int topicId)
        {
            if (Words != null)
            {
                Words.Clear();
            }
            Words = new ObservableCollection<DetailedWord>(_context.Word.Where(x => x.TopicId == topicId)
                .Select(x => DtoToModel(x, _context.Difficulty.Where(y => y.DifficultyId == x.DifficultyId)
                .Select(y => y.DifficultyLevel).Single()))
                .ToObservableCollection().OrderBy(x => x.Difficulty == "Advanced").ThenBy(x => x.Difficulty));

            AddRange(Words);
        }
        public void AddRange(IEnumerable<DetailedWord> items)  //!
        {
            this.CheckReentrancy();
            foreach (var item in items)
            {
                this.Items.Add(item);
            }
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void AddTopic(string topicName)
        {
            TopicDTO topicToAdd = new TopicDTO
            {
                TopicId = nextTopicId++,
                TopicName = topicName
            };

            _context.Topic.Add(topicToAdd);

            Topic topicToAddView = new Topic(topicToAdd.TopicId, topicToAdd.TopicName);
            Topics.Add(topicToAddView);

            _context.SaveChanges();
        }

        public void UpdateTopic(int topicId, string newTopicName)
        {
            _context.Topic.Where(x => x.TopicId == topicId).ExecuteUpdate(x => x.SetProperty(y => y.TopicName, newTopicName));

            _context.ChangeTracker.Clear();     //!

            Topics.Clear();
            Topics = new ObservableCollection<Topic>(_context.Topic.Select(x => DtoToModel(x)));
        }

        public async void DeleteWord(DetailedWord wordToDelete)
        {
            await _context.Word.Where(x => x.WordId == wordToDelete.Id).ExecuteDeleteAsync();
            Words.Remove(wordToDelete);
        }

        public async void DeleteTopic(Topic topicToDelete)
        {
            await _context.Topic.Where(x => x.TopicId == topicToDelete.Id).ExecuteDeleteAsync();
            Topics.Remove(topicToDelete);
        }
    }


    public class DetailedWord : IWord, INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string EnglishWord { get; set; }
        public string HungarianWord { get; set; }
        public string Difficulty { get; set; }
        public bool IsUnsolved { get; set; }

        public bool IsSupposedToShow
        {
            get
            {
                if (HideSolved)
                {
                    return IsCorrectDifficulty && IsUnsolved;
                }
                else
                {
                    return IsCorrectDifficulty;
                }
            }
        }

        private bool isCorrectDifficulty = true;
        public bool IsCorrectDifficulty
        {
            get
            {
                return isCorrectDifficulty;
            }
            set
            {
                isCorrectDifficulty = value;
                OnPropertyChanged(nameof(IsCorrectDifficulty));
                OnPropertyChanged(nameof(IsSupposedToShow));
            }
        }

        private bool hideSolved = false;
        public bool HideSolved
        {
            get
            {
                return hideSolved;
            }
            set
            {
                hideSolved = value;
                OnPropertyChanged(nameof(HideSolved));
                OnPropertyChanged(nameof(IsSupposedToShow));
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
