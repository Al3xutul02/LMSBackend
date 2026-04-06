using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DTOs.Book
{
    public record BookRelationDto(
        int ISBN = 0,
        int Count = 0
        );
}
