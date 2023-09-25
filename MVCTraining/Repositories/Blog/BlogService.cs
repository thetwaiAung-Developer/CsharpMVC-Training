﻿using MvcTraining.Models;

namespace MvcTraining.Repositories.Blog
{
    public class BlogService
    {
        private readonly BlogDao _blogDAO;

        public BlogService(BlogDao blogDAO)
        {
            _blogDAO = blogDAO;
        }

        public int SaveBlog(BlogDto dto)
        {
            dto.Is_Deleted = false;
            int resut = _blogDAO.Create(dto);
            return resut;
        }

        public BlogDto GetBlogById(int id)
        {
            BlogDto blogDto = _blogDAO.GetById(id);
            return blogDto;
        }

        public BlogResponseFilter GetAllBlog(DataTablesRequest requestModel)
        {
            BlogResponseFilter response = new BlogResponseFilter();
            string searchParam = string.Empty;
            string sortColumnParam = string.Empty;
            string pagination = string.Empty;
            string sortColumn = requestModel.SortColumn.Trim();
            string sortColumnDirection = requestModel.SortColumnDirection.Trim();
            string search = requestModel.Search.Trim();
            string pageStartAndSize = " OFFSET " + requestModel.Start + " ROWS FETCH NEXT " + requestModel.Length + " ROWS ONLY";
            if (!string.IsNullOrEmpty(search))
            {
                searchParam = @" and Blog_Title like '%" + search + "%' or Blog_Author like '%" + search + "%'";
            }
            if (!string.IsNullOrEmpty(sortColumn))
            {
                switch (sortColumn)
                {
                    case "Blog_Title":
                        sortColumnParam = " order by Blog_Title" + sortColumnDirection;
                        break;
                    case "Blog_Author":
                        sortColumnParam = " order by Blog_Author" + sortColumnDirection;
                        break;
                    default:
                        sortColumnParam = " Order By Blog_Id desc ";
                        break;
                }
            }
            pagination = sortColumnParam + pageStartAndSize;
            var blogList = _blogDAO.GetAll(searchParam, pagination);
            int blogCount = _blogDAO.ListCount();
            int blogFilterCount = _blogDAO.FilterListCount(searchParam);
            response.BlogList = blogList;
            response.RequestTotal = blogCount;
            response.RequestFilter = blogFilterCount;
            return response;
        }
    }
}
