using AutoMapper;
using BusinessLogic.DTOs.Book;
using BusinessLogic.DTOs.Branch;
using BusinessLogic.DTOs.Fine;
using BusinessLogic.DTOs.Loan;
using BusinessLogic.DTOs.User;
using Microsoft.AspNetCore.Routing.Constraints;
using Repository.Enums.Types;
using Repository.Tables;

namespace BusinessLogic.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User Mappings
            CreateMap<User, UserReadDto>();
            CreateMap<UserCreateDto, User>()
                .ForMember(dest => dest.PasswordHash, opt =>
                    opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.Branch, opt => opt.UseDestinationValue())
                .ForMember(dest => dest.Loans, opt => opt.UseDestinationValue());
            CreateMap<UserUpdateDto, User>()
                .ForMember(dest => dest.PasswordHash, opt =>
                    opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.Branch, opt => opt.UseDestinationValue())
                .ForMember(dest => dest.Loans, opt => opt.UseDestinationValue());

            // Book Mappings
            CreateMap<Book, BookReadDto>()
                .ForCtorParam("Genres", opt => opt.MapFrom(src =>
                    src.Genres != null
                    ? src.Genres.Select(g => g.Genre).ToList()
                    : new List<BookGenreType>()))
                .ForMember(dest => dest.Genres, opt => opt.MapFrom(src =>
                    src.Genres.Select(g => g.Genre)));
            CreateMap<BookCreateDto, Book>()
                .ForMember(dest => dest.Genres, opt => opt.MapFrom(src =>
                    src.Genres != null
                    ? src.Genres.Select(g => new BookGenre { Genre = g })
                    : new List<BookGenre>()))
                .ForMember(dest => dest.Branches, opt => opt.Ignore())
                .ForMember(dest => dest.Loans, opt => opt.Ignore());
            CreateMap<BookUpdateDto, Book>()
                .ForMember(dest => dest.ISBN, opt => opt.Ignore())
                .ForMember(dest => dest.Genres, opt => opt.MapFrom(src =>
                    src.Genres != null
                    ? src.Genres.Select(g => new BookGenre { Genre = g, BookISBN = src.ISBN })
                    : new List<BookGenre>()))
                .ForMember(dest => dest.Branches, opt => opt.Ignore())
                .ForMember(dest => dest.Loans, opt => opt.Ignore());

            // Branch Mappings
            CreateMap<Branch, BranchReadDto>()
                .ForCtorParam("EmployeeIds", opt => opt.MapFrom(src =>
                    src.Librarians != null
                    ? src.Librarians.Select(l => l.Id).ToList()
                    : new List<int>()))
                .ForCtorParam("BookRelations", opt => opt.MapFrom(src =>
                    src.Books != null
                    ? src.Books.Select(b =>
                        new BookRelationDto(b.BookISBN, b.Count)).ToList()
                    : new List<BookRelationDto>()))
                .ForMember(dest => dest.EmployeeIds, opt =>
                    opt.MapFrom(src => src.Librarians.Select(l => l.Id)))
                .ForMember(dest => dest.BookRelations, opt =>
                    opt.MapFrom(src => src.Books.Select(b =>
                        new BookRelationDto(b.BookISBN, b.Count))));
            CreateMap<BranchCreateDto, Branch>()
                .ForMember(dest => dest.Librarians, opt => opt.UseDestinationValue())
                .ForMember(dest => dest.Books, opt => opt.UseDestinationValue());
            CreateMap<BranchUpdateDto, Branch>()
                .ForMember(dest => dest.Librarians, opt => opt.UseDestinationValue())
                .ForMember(dest => dest.Books, opt => opt.UseDestinationValue());

            // Loan Mappings
            CreateMap<Loan, LoanReadDto>()
                .ForCtorParam("BookRelations", opt => opt.MapFrom(src =>
                    src.Books != null
                    ? src.Books.Select(b =>
                        new BookRelationDto(b.BookISBN, b.Count)).ToList()
                    : new List<BookRelationDto>()))
                .ForMember(dest => dest.LoanerName, opt =>
                    opt.MapFrom(src => src.User != null ?
                        src.User.Name : "Unknown"))
                .ForMember(dest => dest.BookRelations, opt =>
                    opt.MapFrom(src => src.Books.Select(b =>
                        new BookRelationDto(b.BookISBN, b.Count)).ToList()));
            CreateMap<LoanCreateDto, Loan>()
                .ForMember(dest => dest.User, opt => opt.UseDestinationValue())
                .ForMember(dest => dest.Fine, opt => opt.UseDestinationValue());
            CreateMap<LoanUpdateDto, Loan>()
                .ForMember(dest => dest.User, opt => opt.UseDestinationValue())
                .ForMember(dest => dest.Fine, opt => opt.UseDestinationValue());

            // Fine Mappings
            CreateMap<Fine, FineReadDto>();
            CreateMap<FineCreateDto, Fine>()
                .ForMember(dest => dest.Loan, opt => opt.UseDestinationValue());
            CreateMap<FineUpdateDto, Fine>()
                .ForMember(dest => dest.Loan, opt => opt.UseDestinationValue());
        }
    }
}
