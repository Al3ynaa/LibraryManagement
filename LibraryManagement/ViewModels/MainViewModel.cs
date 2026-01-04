using LibraryManagement.Models;
using LibraryManagement.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace LibraryManagement.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly LibraryDataService _service = new LibraryDataService();

        // Koleksiyonlar (değişmeyecek, sadece içi güncellenecek)
        public ObservableCollection<Book> Books { get; } = new();
        public ObservableCollection<Member> Members { get; } = new();
        public ObservableCollection<Loan> Loans { get; } = new();
        public ObservableCollection<LoanViewRow> LoanViews { get; } = new();

        public ICollectionView BooksView { get; }
        public ICollectionView MembersView { get; }
        public ICollectionView LoanViewsView { get; }

        // Search
        private string _bookSearch = "";
        public string BookSearch { get => _bookSearch; set { _bookSearch = value; OnPropertyChanged(); BooksView.Refresh(); } }

        private string _memberSearch = "";
        public string MemberSearch { get => _memberSearch; set { _memberSearch = value; OnPropertyChanged(); MembersView.Refresh(); } }

        private string _loanSearch = "";
        public string LoanSearch { get => _loanSearch; set { _loanSearch = value; OnPropertyChanged(); LoanViewsView.Refresh(); } }

        // Selected
        private Book? _selectedBook;
        public Book? SelectedBook
        {
            get => _selectedBook;
            set
            {
                _selectedBook = value;
                OnPropertyChanged();

                if (_selectedBook != null)
                {
                    BookTitle = _selectedBook.Title;
                    BookAuthor = _selectedBook.Author;
                    BookYear = _selectedBook.Year;
                    BookISBN = _selectedBook.ISBN;
                }

                UpdateCommandStates();
            }
        }

        private Member? _selectedMember;
        public Member? SelectedMember
        {
            get => _selectedMember;
            set
            {
                _selectedMember = value;
                OnPropertyChanged();

                if (_selectedMember != null)
                {
                    MemberName = _selectedMember.Name;
                    MemberEmail = _selectedMember.Email;
                    MemberPhone = _selectedMember.Phone;
                }

                UpdateCommandStates();
            }
        }

        private LoanViewRow? _selectedLoanView;
        public LoanViewRow? SelectedLoanView
        {
            get => _selectedLoanView;
            set
            {
                _selectedLoanView = value;
                OnPropertyChanged();

                if (_selectedLoanView != null)
                {
                    LoanBook = Books.FirstOrDefault(b => b.Id == _selectedLoanView.BookId);
                    LoanMember = Members.FirstOrDefault(m => m.Id == _selectedLoanView.MemberId);
                    DueDate = _selectedLoanView.DueDate;
                }

                UpdateCommandStates();
            }
        }

        // UI message (Loans)
        private string _loanMessage = "";
        public string LoanMessage { get => _loanMessage; set { _loanMessage = value; OnPropertyChanged(); } }

        // Form fields (Books)
        private string _bookTitle = "";
        public string BookTitle { get => _bookTitle; set { _bookTitle = value; OnPropertyChanged(); UpdateCommandStates(); } }

        private string _bookAuthor = "";
        public string BookAuthor { get => _bookAuthor; set { _bookAuthor = value; OnPropertyChanged(); } }

        private int _bookYear = DateTime.Now.Year;
        public int BookYear { get => _bookYear; set { _bookYear = value; OnPropertyChanged(); } }

        private string _bookISBN = "";
        public string BookISBN { get => _bookISBN; set { _bookISBN = value; OnPropertyChanged(); } }

        // Form fields (Members)
        private string _memberName = "";
        public string MemberName { get => _memberName; set { _memberName = value; OnPropertyChanged(); UpdateCommandStates(); } }

        private string _memberEmail = "";
        public string MemberEmail { get => _memberEmail; set { _memberEmail = value; OnPropertyChanged(); } }

        private string _memberPhone = "";
        public string MemberPhone { get => _memberPhone; set { _memberPhone = value; OnPropertyChanged(); } }

        // Form fields (Loans)
        private Book? _loanBook;
        public Book? LoanBook { get => _loanBook; set { _loanBook = value; OnPropertyChanged(); UpdateCommandStates(); } }

        private Member? _loanMember;
        public Member? LoanMember { get => _loanMember; set { _loanMember = value; OnPropertyChanged(); UpdateCommandStates(); } }

        private DateTime _dueDate = DateTime.Today.AddDays(7);
        public DateTime DueDate { get => _dueDate; set { _dueDate = value; OnPropertyChanged(); } }

        // Commands
        public RelayCommand AddBookCommand { get; }
        public RelayCommand UpdateBookCommand { get; }
        public RelayCommand DeleteBookCommand { get; }

        public RelayCommand AddMemberCommand { get; }
        public RelayCommand UpdateMemberCommand { get; }
        public RelayCommand DeleteMemberCommand { get; }

        public RelayCommand AddLoanCommand { get; }      // Borrow
        public RelayCommand UpdateLoanCommand { get; }
        public RelayCommand DeleteLoanCommand { get; }
        public RelayCommand ReturnLoanCommand { get; }

        public MainViewModel()
        {
            // Views
            BooksView = CollectionViewSource.GetDefaultView(Books);
            BooksView.Filter = BookFilter;

            MembersView = CollectionViewSource.GetDefaultView(Members);
            MembersView.Filter = MemberFilter;

            LoanViewsView = CollectionViewSource.GetDefaultView(LoanViews);
            LoanViewsView.Filter = LoanFilter;

            // Commands
            AddBookCommand = new RelayCommand(_ => AddBook(), _ => !string.IsNullOrWhiteSpace(BookTitle));
            UpdateBookCommand = new RelayCommand(_ => UpdateBook(), _ => SelectedBook != null);
            DeleteBookCommand = new RelayCommand(_ => DeleteBook(), _ => SelectedBook != null);

            AddMemberCommand = new RelayCommand(_ => AddMember(), _ => !string.IsNullOrWhiteSpace(MemberName));
            UpdateMemberCommand = new RelayCommand(_ => UpdateMember(), _ => SelectedMember != null);
            DeleteMemberCommand = new RelayCommand(_ => DeleteMember(), _ => SelectedMember != null);

            // Borrow: kitap + üye seçili olmalı
            AddLoanCommand = new RelayCommand(_ => BorrowLoan(), _ => LoanBook != null && LoanMember != null);

            UpdateLoanCommand = new RelayCommand(_ => UpdateLoan(), _ => SelectedLoanView != null);
            DeleteLoanCommand = new RelayCommand(_ => DeleteLoan(), _ => SelectedLoanView != null);

            // Return: sadece Borrowed satırda aktif
            ReturnLoanCommand = new RelayCommand(_ => ReturnLoan(),
                _ => SelectedLoanView != null && SelectedLoanView.IsReturned == false);

            RefreshAll();
        }

        private void UpdateCommandStates()
        {
            AddBookCommand.RaiseCanExecuteChanged();
            UpdateBookCommand.RaiseCanExecuteChanged();
            DeleteBookCommand.RaiseCanExecuteChanged();

            AddMemberCommand.RaiseCanExecuteChanged();
            UpdateMemberCommand.RaiseCanExecuteChanged();
            DeleteMemberCommand.RaiseCanExecuteChanged();

            AddLoanCommand.RaiseCanExecuteChanged();
            UpdateLoanCommand.RaiseCanExecuteChanged();
            DeleteLoanCommand.RaiseCanExecuteChanged();
            ReturnLoanCommand.RaiseCanExecuteChanged();
        }

        // Filters
        private bool BookFilter(object obj)
        {
            if (obj is not Book b) return false;
            if (string.IsNullOrWhiteSpace(BookSearch)) return true;
            var s = BookSearch.Trim().ToLower();
            return (b.Title ?? "").ToLower().Contains(s)
                || (b.Author ?? "").ToLower().Contains(s)
                || b.Year.ToString().Contains(s)
                || (b.ISBN ?? "").ToLower().Contains(s);
        }

        private bool MemberFilter(object obj)
        {
            if (obj is not Member m) return false;
            if (string.IsNullOrWhiteSpace(MemberSearch)) return true;
            var s = MemberSearch.Trim().ToLower();
            return (m.Name ?? "").ToLower().Contains(s)
                || (m.Email ?? "").ToLower().Contains(s)
                || (m.Phone ?? "").ToLower().Contains(s);
        }

        private bool LoanFilter(object obj)
        {
            if (obj is not LoanViewRow r) return false;
            if (string.IsNullOrWhiteSpace(LoanSearch)) return true;
            var s = LoanSearch.Trim().ToLower();
            return (r.Book ?? "").ToLower().Contains(s)
                || (r.Member ?? "").ToLower().Contains(s)
                || r.DueDate.ToString("dd.MM.yyyy").Contains(s)
                || r.Status.ToLower().Contains(s);
        }

        // Refresh
        private void RefreshAll()
        {
            Books.Clear();
            foreach (var b in _service.Books) Books.Add(b);

            Members.Clear();
            foreach (var m in _service.Members) Members.Add(m);

            Loans.Clear();
            foreach (var l in _service.Loans) Loans.Add(l);

            LoanViews.Clear();
            foreach (var row in BuildLoanViews()) LoanViews.Add(row);

            BooksView.Refresh();
            MembersView.Refresh();
            LoanViewsView.Refresh();

            UpdateCommandStates();
        }

        private ObservableCollection<LoanViewRow> BuildLoanViews()
        {
            var list = _service.Loans.Select(l =>
            {
                var b = _service.GetBookById(l.BookId);
                var m = _service.GetMemberById(l.MemberId);

                return new LoanViewRow
                {
                    Id = l.Id,
                    BookId = l.BookId,
                    MemberId = l.MemberId,
                    Book = b?.Title ?? "(deleted book)",
                    Member = m?.Name ?? "(deleted member)",
                    DueDate = l.DueDate,

                    IsReturned = l.IsReturned,
                    ReturnedDate = l.ReturnedDate
                };
            }).ToList();

            return new ObservableCollection<LoanViewRow>(list);
        }

        // CRUD Books
        private void AddBook()
        {
            _service.AddBook(new Book { Title = BookTitle, Author = BookAuthor, Year = BookYear, ISBN = BookISBN });
            ClearBookForm();
            RefreshAll();
        }

        private void UpdateBook()
        {
            if (SelectedBook == null) return;
            _service.UpdateBook(new Book { Id = SelectedBook.Id, Title = BookTitle, Author = BookAuthor, Year = BookYear, ISBN = BookISBN });
            RefreshAll();
        }

        private void DeleteBook()
        {
            if (SelectedBook == null) return;
            _service.DeleteBook(SelectedBook.Id);
            SelectedBook = null;
            ClearBookForm();
            RefreshAll();
        }

        private void ClearBookForm()
        {
            BookTitle = "";
            BookAuthor = "";
            BookYear = DateTime.Now.Year;
            BookISBN = "";
        }

        // CRUD Members
        private void AddMember()
        {
            _service.AddMember(new Member { Name = MemberName, Email = MemberEmail, Phone = MemberPhone });
            ClearMemberForm();
            RefreshAll();
        }

        private void UpdateMember()
        {
            if (SelectedMember == null) return;
            _service.UpdateMember(new Member { Id = SelectedMember.Id, Name = MemberName, Email = MemberEmail, Phone = MemberPhone });
            RefreshAll();
        }

        private void DeleteMember()
        {
            if (SelectedMember == null) return;
            _service.DeleteMember(SelectedMember.Id);
            SelectedMember = null;
            ClearMemberForm();
            RefreshAll();
        }

        private void ClearMemberForm()
        {
            MemberName = "";
            MemberEmail = "";
            MemberPhone = "";
        }

        // Loans (Borrow/Return)
        private void BorrowLoan()
        {
            LoanMessage = "";
            try
            {
                if (LoanBook == null || LoanMember == null) return;

                _service.AddLoan(new Loan
                {
                    BookId = LoanBook.Id,
                    MemberId = LoanMember.Id,
                    DueDate = DueDate
                });

                ClearLoanForm();
                RefreshAll();
            }
            catch (Exception ex)
            {
                LoanMessage = ex.Message;
            }
        }

        private void ReturnLoan()
        {
            LoanMessage = "";
            try
            {
                if (SelectedLoanView == null) return;

                _service.ReturnLoan(SelectedLoanView.Id);
                RefreshAll();
            }
            catch (Exception ex)
            {
                LoanMessage = ex.Message;
            }
        }

        private void UpdateLoan()
        {
            if (SelectedLoanView == null || LoanBook == null || LoanMember == null) return;

            _service.UpdateLoan(new Loan
            {
                Id = SelectedLoanView.Id,
                BookId = LoanBook.Id,
                MemberId = LoanMember.Id,
                DueDate = DueDate
            });

            RefreshAll();
        }

        private void DeleteLoan()
        {
            if (SelectedLoanView == null) return;
            _service.DeleteLoan(SelectedLoanView.Id);
            SelectedLoanView = null;
            ClearLoanForm();
            RefreshAll();
        }

        private void ClearLoanForm()
        {
            LoanBook = null;
            LoanMember = null;
            DueDate = DateTime.Today.AddDays(7);
        }
    }
}
