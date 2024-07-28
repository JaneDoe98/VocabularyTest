namespace VocabularyTest.Models
{
    internal class Topic
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Topic(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
