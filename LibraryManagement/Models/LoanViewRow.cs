using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Models
{
    public class LoanViewRow
    {
        public int Id { get; set; }
        public string Book { get; set; } = "";
        public string Member { get; set; } = "";
        public DateTime DueDate { get; set; }

        // Delete için gerçek Loan'u bulmakta işe yarar
        public int BookId { get; set; }
        public int MemberId { get; set; }
        public bool IsReturned { get; set; }
        public DateTime? ReturnedDate { get; set; }

        public string Status => IsReturned ? "Returned" : "Borrowed";

    }
}
