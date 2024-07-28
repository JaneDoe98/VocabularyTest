using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VocabularyTest.Data
{
    [Table("word")]
    internal record WordDTO
    {
        [Column("word_id")]
        [Key]
        public int WordId { get; set; }
        [Column("english_word")]
        public string EnglishWord { get; set; }
        [Column("hungarian_word")]
        public string HungarianWord { get; set; }
        [Column("is_unsolved")]
        public int IsUnsolved { get; set; } = 1;
        [Column("topic_id")]
        public int TopicId { get; set; }
        [Column("difficulty_id")]
        public int DifficultyId { get; set; }
    }

    [Table("topic")]
    internal record TopicDTO
    {
        [Column("topic_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int TopicId { get; set; }
        [Column("topic_name")]
        public string TopicName { get; set; }

    }

    [Table("difficulty")]
    internal record DifficultyDTO
    {
        [Column("difficulty_id")]
        [Key]
        public int DifficultyId { get; set; }
        [Column("difficulty_level")]
        public string DifficultyLevel { get; set; }
    }
}
