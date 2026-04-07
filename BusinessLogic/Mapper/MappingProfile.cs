using AutoMapper;
using BusinessLogic.DTOs.Book;
using BusinessLogic.DTOs.Branch;
using BusinessLogic.DTOs.Fine;
using BusinessLogic.DTOs.Loan;
using BusinessLogic.DTOs.User;
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
                .ForMember(dest => dest.Genres, opt =>
                    opt.MapFrom(src => src.Genres.Select(g => g.Genre).ToList()))
                .ForMember(dest => dest.LoanDurationDays, opt =>
                    opt.Equals(null))
                .ForMember(dest => dest.CanBeReserved, opt =>
                    opt.Equals(null));
            CreateMap<BookCreateDto, Book>()
                .ForMember(dest => dest.Genres, opt => opt.UseDestinationValue())
                .ForMember(dest => dest.Branches, opt => opt.UseDestinationValue())
                .ForMember(dest => dest.Loans, opt => opt.UseDestinationValue());
            CreateMap<BookUpdateDto, Book>()
                .ForMember(dest => dest.Genres, opt => opt.UseDestinationValue())
                .ForMember(dest => dest.Branches, opt => opt.UseDestinationValue())
                .ForMember(dest => dest.Loans, opt => opt.UseDestinationValue());

            // Branch Mappings
            CreateMap<Branch, BranchReadDto>()
                .ForMember(dest => dest.EmployeeIds, opt =>
                    opt.MapFrom(src => src.Librarians.Select(l => l.Id).ToList()))
                .ForMember(dest => dest.BookRelations, opt =>
                    opt.MapFrom(src => src.Books.Select(b =>
                        new BookRelationDto(b.BookISBN, b.Count)).ToList()));
            CreateMap<BranchCreateDto, Branch>()
                .ForMember(dest => dest.Librarians, opt => opt.UseDestinationValue())
                .ForMember(dest => dest.Books, opt => opt.UseDestinationValue());
            CreateMap<BranchUpdateDto, Branch>()
                .ForMember(dest => dest.Librarians, opt => opt.UseDestinationValue())
                .ForMember(dest => dest.Books, opt => opt.UseDestinationValue());

            // Loan Mappings
            CreateMap<Loan, LoanReadDto>()
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
