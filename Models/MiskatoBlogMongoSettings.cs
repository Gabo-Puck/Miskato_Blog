namespace Miskato_Blog.Models
{
    public class MiskatoBlogMongoSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string PostCollectionName { get; set; } = null!;
    }
}