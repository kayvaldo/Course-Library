﻿using System;
using System.Collections.Generic;
using System.Linq;
using CourseLibrary.Domain;
using CourseLibrary.Services;
using CourseLibrary.Services.ResourceParameterContracts;
using Microsoft.EntityFrameworkCore;

namespace CourseLibrary.Persistence.EFCore
{
    public class CourseLibraryRepository : ICourseLibraryRepository, IDisposable
    {
        public CourseLibraryRepository(CourseLibraryContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        private readonly CourseLibraryContext _context;

        public void AddAuthor(Author author)
        {
            if (author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            // the repository fills the id (instead of using identity columns)
            author.Id = Guid.NewGuid();

            foreach (var course in author.Courses)
            {
                course.Id = Guid.NewGuid();
            }

            _context.Authors.Add(author);
        }

        public void AddCourse(Guid authorId, Course course)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            if (course == null)
            {
                throw new ArgumentNullException(nameof(course));
            }

            // always set the AuthorId to the passed-in authorId
            course.AuthorId = authorId;
            _context.Courses.Add(course);
        }

        public bool AuthorExists(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return _context.Authors.Any(author => author.Id == authorId);
        }

        public void DeleteAuthor(Author author)
        {
            if (author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            _context.Authors.Remove(author);
        }

        public void DeleteCourse(Course course)
        {
            _context.Courses.Remove(course);
        }

        public Author GetAuthor(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return _context.Authors.FirstOrDefault(author => author.Id == authorId);
        }

        public IEnumerable<Author> GetAuthors()
        {
            return _context.Authors.ToList();
        }

        public IEnumerable<Author> GetAuthors(IAuthorParameters authorParameters)
        {
            if (authorParameters == null
                || string.IsNullOrWhiteSpace(authorParameters.MainCategory)
                && string.IsNullOrWhiteSpace(authorParameters.SearchQuery))
            {
                return GetAuthors();
            }

            var authors = _context.Authors.AsQueryable();

            if (!string.IsNullOrWhiteSpace(authorParameters.MainCategory))
            {
                var mainCategory = authorParameters.MainCategory.Trim();
                authors = authors.Where(author => author.MainCategory.Equals(mainCategory));
            }

            if (!string.IsNullOrWhiteSpace(authorParameters.SearchQuery))
            {
                var searchQuery = authorParameters.SearchQuery.Trim();
                authors = authors.Where(
                    author => author.FirstName.Contains(searchQuery)
                        || author.LastName.Contains(searchQuery)
                        || author.MainCategory.Contains(searchQuery));
            }

            var enumerable = authors.ToList();
            return enumerable;
        }

        public IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorIds)
        {
            if (authorIds == null)
            {
                throw new ArgumentNullException(nameof(authorIds));
            }

            return _context.Authors.Where(a => authorIds.Contains(a.Id))
                .OrderBy(a => a.FirstName)
                .ThenBy(a => a.LastName)
                .ToList();
        }

        public Course GetCourse(Guid authorId, Guid courseId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            if (courseId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(courseId));
            }

            return _context.Courses.FirstOrDefault(course => course.AuthorId == authorId && course.Id == courseId);
        }

        public IEnumerable<Course> GetCourses(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return _context.Courses.Where(course => course.AuthorId == authorId).OrderBy(c => c.Title).ToList();
        }

        public bool Save()
        {
            return _context.SaveChanges() >= 0;
        }

        public void UpdateAuthor(Author author)
        {
            // no code in this implementation
        }

        public void UpdateCourse(Course course)
        {
            // no code in this implementation
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose resources when needed
            }
        }
    }
}