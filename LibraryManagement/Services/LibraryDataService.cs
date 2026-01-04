using LibraryManagement.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Services
{
    public class LibraryDataService
    {
        private int _bookId = 1;
        private int _memberId = 1;
        private int _loanId = 1;

        public List<Book> Books { get; } = new();
        public List<Member> Members { get; } = new();
        public List<Loan> Loans { get; } = new();

        public LibraryDataService()
        {
            //  örnek veriler 
            AddBook(new Book { Title = "Clean Code", Author = "Robert C. Martin", Year = 2008, ISBN = "9780132350884" });
            AddBook(new Book { Title = "The Pragmatic Programmer", Author = "Andrew Hunt", Year = 1999, ISBN = "9780201616224" });

            AddMember(new Member { Name = "Ali", Email = "ali@gmail.com", Phone = "555-111-2233" });
            AddMember(new Member { Name = "aleyna", Email = "aleyna@gmail.com", Phone = "05554889623" });

            AddLoan(new Loan { BookId = 2, MemberId = 2, DueDate = DateTime.Today.AddDays(7) });
        }

        // ---------- BOOK ----------
        public Book AddBook(Book b)
        {
            b.Id = _bookId++;
            Books.Add(b);
            return b;
        }

        public void UpdateBook(Book updated)
        {
            var b = Books.FirstOrDefault(x => x.Id == updated.Id);
            if (b == null) return;

            b.Title = updated.Title;
            b.Author = updated.Author;
            b.Year = updated.Year;
            b.ISBN = updated.ISBN;
        }

        public void DeleteBook(int id)
        {
            var book = Books.FirstOrDefault(x => x.Id == id);
            if (book == null) return;

            // kitap silinince ona bağlı loan'lar da silinsin
            Loans.RemoveAll(l => l.BookId == id);
            Books.Remove(book);
        }

        public Book? GetBookById(int id) => Books.FirstOrDefault(x => x.Id == id);

        // ---------- MEMBER ----------
        public Member AddMember(Member m)
        {
            m.Id = _memberId++;
            Members.Add(m);
            return m;
        }

        public void UpdateMember(Member updated)
        {
            var m = Members.FirstOrDefault(x => x.Id == updated.Id);
            if (m == null) return;

            m.Name = updated.Name;
            m.Email = updated.Email;
            m.Phone = updated.Phone;
        }

        public void DeleteMember(int id)
        {
            var member = Members.FirstOrDefault(x => x.Id == id);
            if (member == null) return;

            // üye silinince ona bağlı loan'lar da silinsin
            Loans.RemoveAll(l => l.MemberId == id);
            Members.Remove(member);
        }

        public Member? GetMemberById(int id) => Members.FirstOrDefault(x => x.Id == id);

        // ---------- LOAN ----------
        
        public Loan AddLoan(Loan l)
        {
            // Aynı BookId için IsReturned = false olan bir kayıt varsa ekleme
            bool alreadyBorrowed = Loans.Any(x => x.BookId == l.BookId && x.IsReturned == false);

            if (alreadyBorrowed)
                throw new InvalidOperationException("Bu kitap şu anda başka bir üyede. \n " +
                    "Önce iade edilmeden tekrar verilemez.");

            l.Id = _loanId++;
            l.IsReturned = false;
            l.ReturnedDate = null;

            Loans.Add(l);
            return l;
        }
        public void ReturnLoan(int loanId)
        {
            var loan = Loans.FirstOrDefault(x => x.Id == loanId);
            if (loan == null) return;

            if (loan.IsReturned)
                throw new Exception("Bu ödünç zaten iade edilmiş.");

            loan.IsReturned = true;
            loan.ReturnedDate = DateTime.Now;
        }


        public void UpdateLoan(Loan updated)
        {
            var l = Loans.FirstOrDefault(x => x.Id == updated.Id);
            if (l == null) return;

            l.BookId = updated.BookId;
            l.MemberId = updated.MemberId;
            l.DueDate = updated.DueDate;
        }

        public void DeleteLoan(int id)
        {
            var loan = Loans.FirstOrDefault(x => x.Id == id);
            if (loan == null) return;

            Loans.Remove(loan);
        }



    }

}
