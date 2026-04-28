using Repository.Enums.Types;

namespace Repository.Tables
{
    public class BorrowRequest
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public User? User { get; set; }
        public int BookISBN { get; set; }
        public Book? Book { get; set; }
        public int Count { get; set; } = 1;
        public DateTime RequestDate { get; set; }
        public RequestStatus Status { get; set; } = RequestStatus.Pending;
    }
}