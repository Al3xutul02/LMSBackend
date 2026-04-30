using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.DTOs.Loan
{
    public record LoanDetailsDto(
         int RequestId = 0,
         DateTime RequestDate = new DateTime(),
         DateTime? ReturnDeadline = null,

         string UserName = "",
         string UserEmail = "",
         string UserPhone = "N/A", 
         string UserRole = "Student",
         int ActiveLoansCount = 0,
         IEnumerable<RequestedBookDto>? RequestedBooks = null,

         string OverallStatus = "Pending"
     );
}
