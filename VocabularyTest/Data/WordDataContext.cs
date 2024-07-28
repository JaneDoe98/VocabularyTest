using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace VocabularyTest.Data
{
    class WordDataContext : DbContext
    {
        internal DbSet<WordDTO> Word { get; set; }
        internal DbSet<TopicDTO> Topic { get; set; }
        internal DbSet<DifficultyDTO> Difficulty { get; set; }

        internal string DbPath { get; }

        public WordDataContext()
        {
            SQLitePCL.Batteries_V2.Init();

            string dbName = "word.db";

#if ANDROID
    DbPath = Path.Combine(FileSystem.Current.AppDataDirectory, dbName);
#elif IOS
    DbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "..", "Library", dbName);
#elif WINDOWS
            DbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), dbName);
#else
            throw new PlatformNotSupportedException("Platform nem támogatott");
#endif
            if (!File.Exists(DbPath))
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourcePath = $"VocabularyTest.Resources.{dbName}";

                var stream = assembly.GetManifestResourceStream(resourcePath);

                if (stream == null)
                {
                    throw new FileNotFoundException("Nem találhtó: ", dbName);
                }

                using (var fileStream = new FileStream(DbPath, FileMode.Create, FileAccess.Write))
                {
                    stream.CopyTo(fileStream);
                }
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={DbPath}");
        }
    }
}
