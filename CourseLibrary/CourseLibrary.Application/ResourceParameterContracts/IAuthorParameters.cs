﻿namespace CourseLibrary.Services.ResourceParameterContracts
{
    public interface IAuthorParameters
    {
        public string MainCategory { get; set; }
        public string SearchQuery { get; set; }
    }
}