﻿using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Exceptions;
using Entities.Models;
using Entities.RequestFeatures;
using Repositories.Contracts;
using Services.Contracts;
using System.Dynamic;
using static Entities.Exceptions.BadRequestException;

namespace Services
{
    public class BookManager : IBookService
    {
        private readonly IRepositoryManager _manager;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;
        private readonly IDataShaper<BookDto> _shaper;

        public BookManager(IRepositoryManager manager, ILoggerService logger, IMapper mapper, IDataShaper<BookDto> shaper)
        {
            _manager = manager;
            _logger = logger;
            _mapper = mapper;
            _shaper = shaper;
        }

        private async Task<Book> GetOneBookByIdAndCheckExists(int id, bool trackChanges)
        {
            var entity = await _manager.Book.GetOneBookbyIdAsync(id, trackChanges);
            if (entity is null)
                throw new BookNotFoundException(id);
            return entity;
        }

        public async Task<BookDto> CreateOneBookAsync(BookDtoForInsertion bookDto)
        {
            var entity = _mapper.Map<Book>(bookDto);
            _manager.Book.CreateOneBook(entity);
            await _manager.SaveAsync();
            return _mapper.Map<BookDto>(entity);
        }

        public async Task DeleteOneBookAsync(int id, bool trackChanges)
        {
            var entity = await GetOneBookByIdAndCheckExists(id, trackChanges);
            _manager.Book.DeleteOneBook(entity);
            await _manager.SaveAsync();
        }

        public async Task<(IEnumerable<ExpandoObject> books, MetaData metaData)> GetAllBooksAsync(BookParameters bookParameters, bool trackChanges)
        {
            if (!bookParameters.ValidPriceRange)
                throw new InvalidPriceFiltersBadRequestException();

            var booksWithMetaData = await _manager.Book.GetAllBooksAsync(bookParameters, trackChanges);

            var booksDto = _mapper.Map<IEnumerable<BookDto>>(booksWithMetaData);
            var shapedData = _shaper.ShapeData(booksDto, bookParameters.Fields);

            return (books: shapedData, metaData: booksWithMetaData.MetaData);
        }

        public async Task<BookDto> GetOneBookByIdAsync(int id, bool trackChanges)
        {
            var entity = await GetOneBookByIdAndCheckExists(id, trackChanges);
            return _mapper.Map<BookDto>(entity);
        }

        public async Task<(BookDtoForUpdate bookDtoForUpdate, Book book)> GetOneBookForPatchAsync(int id, bool trackChanges)
        {
            var entity = await GetOneBookByIdAndCheckExists(id, trackChanges);
            var bookDtoForUpdate = _mapper.Map<BookDtoForUpdate>(entity);
            return (bookDtoForUpdate, entity);
        }

        public async Task SaveChangesForPatchAsync(BookDtoForUpdate bookDtoForUpdate, Book book)
        {
            _mapper.Map(bookDtoForUpdate, book);
            await _manager.SaveAsync();
        }

        public async Task UpdateOneBookAsync(int id, BookDtoForUpdate bookDto, bool trackChanges)
        {
            var entity = await GetOneBookByIdAndCheckExists(id, trackChanges);
            entity = _mapper.Map<Book>(bookDto);
            _manager.Book.Update(entity);
            await _manager.SaveAsync();
        }
    }
}
