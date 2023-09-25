﻿using MvcTraining.Models;

namespace MvcTraining.Repositories.Blog
{
    public class BlogRequestModel
    {
        public long BlogId { get; set; }
        public string BlogTitle { get; set; }
        public string BlogAuthor { get; set; }
        public string BlogContent { get; set; }
    }

    public class BlogDataRequestModel
    {
        public DataTablesRequest DataTablesRequest { get; set; }
    }
}
