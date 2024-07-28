using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VocabularyTest.Models
{
    internal class Difficulty
    {
        public int Id { get; set; }
        public string Level { get; set; }

        public Difficulty(int id, string level)
        {
            Id = id;
            Level = level;
        }
    }
}
