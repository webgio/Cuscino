using System;

namespace Cuscino.SpecTests
{
    public class BlogPost: CouchDoc
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }

        public BlogPost()
        {
            Created = DateTime.UtcNow;
        }
    }
}