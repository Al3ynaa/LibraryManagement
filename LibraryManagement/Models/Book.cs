using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Models
{
    public class Book
    {
        public int Id { get; set; }

        public string Title { get; set; } = "";
        public string Author { get; set; } = "";

        public int Year { get; set; }          
        public string ISBN { get; set; } = ""; 
    }

}
