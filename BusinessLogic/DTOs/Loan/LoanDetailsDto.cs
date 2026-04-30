using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.DTOs.Loan
{
    public record LoanDetailsDto(
         int RequestId = 0,
         DateTime RequestDate = new DateTime(),
         DateTime? ReturnDeadline = null,

         // Datele utilizatorului pentru sidebar-ul stâng
         string UserName = "",
         string UserEmail = "",
         string UserPhone = "N/A", // Poți adăuga câmpul în entitatea User
         string UserRole = "Student",
         int ActiveLoansCount = 0,

         // Lista de cărți din tabelul central
         IEnumerable<RequestedBookDto>? RequestedBooks = null,

         string OverallStatus = "Pending"
     );
}
