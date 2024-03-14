using Application.Common.DTOs.Books;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Application.Interfaces.Services;

public interface IBookService
{
    Task CreateBookAsync(string email, BookInDto bookInDto);
    Task<IList<BookOutDto>> GetBooksAsync(string email);
    Task DeleteBooksAsync(string email, IEnumerable<Guid> guids);
    Task UpdateBookAsync(string email, BookForUpdateDto bookUpdateDto);
    Task AddBookBinaryData(string email, Guid guid, MultipartReader reader);
    Task<Stream> GetBookBinaryData(string email, Guid guid);
    Task ChangeBookCover(string email, Guid guid, MultipartReader reader);
    Task<Stream> GetBookCover(string email, Guid guid);
    Task DeleteBookCover(string email, Guid guid);
    Task<string> GetFormatForBook(string email, Guid guid);
    Task<string> GetExtensionForBook(string email, Guid guid);
}