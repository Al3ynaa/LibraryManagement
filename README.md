#  Library Management System (WPF – MVVM)

##  Project Overview

This project is a **Library Management System** developed using **C# and WPF**, following the **MVVM (Model–View–ViewModel)** architectural pattern.

The application models a simple but realistic library workflow and allows managing:

- Books  
- Members  
- Loans (Borrow / Return)


##  Features

### Login
- Simple login screen
- Basic validation for username and password
- Entry point to the main application

---

###  Books Management
- Add new books  
- Update existing books  
- Delete books  
- Search books by:
  - Title
  - Author
  - Year
  - ISBN

---

###  Members Management
- Add new members  
- Update member information  
- Delete members  
- Search members by:
  - Name
  - Email
  - Phone

---

###  Loans (Borrow / Return)
- Borrow a book for a member  
- A book can be borrowed by **only one member at a time**  
- Return borrowed books  
- View loan status:
  - Borrowed
  - Returned
- Display:
  - Due date
  - Returned date
- Search loans by:
  - Book name
  - Member name
  - Date

---

##  Business Rules

- A book cannot be borrowed by more than one member at the same time  
- Returned books become available again  
- Loan status is automatically updated  
- When a book or member is deleted, related loans are also removed  

These rules reflect real-world library behavior.

---

##  Project Structure

###  Models
Represents the core data objects of the system.

- **Book**
  - Id, Title, Author, Year, ISBN

- **Member**
  - Id, Name, Email, Phone

- **Loan**
  - BookId, MemberId  
  - DueDate  
  - IsReturned  
  - ReturnedDate  

- **LoanViewRow**
  - UI-friendly representation of loans  
  - Includes book name, member name, and loan status  

---

###  Services
Handles application data and business logic.

- **LibraryDataService**
  - Manages in-memory collections of Books, Members, and Loans  
  - Contains all CRUD operations  
  - Implements borrow / return rules  
  - Acts as a simple data layer  

All data is kept **in memory** to keep the project focused on design principles.

---

###  ViewModels
Implements MVVM logic and connects Views with Models.

- **BaseViewModel**
  - Implements `INotifyPropertyChanged`

- **LoginViewModel**
  - Handles login logic and validation

- **MainViewModel**
  - Manages application state  
  - Handles commands for:
    - Books
    - Members
    - Loans  
  - Implements search filters  
  - Controls borrow / return behavior  

- **RelayCommand**
  - Custom `ICommand` implementation  
  - Used for MVVM command binding  

---

###  Views
Defines the user interface.

- **LoginWindow.xaml**
  - Login screen

- **MainWindow.xaml**
  - Main application window  
  - Tab-based layout:
    - Books
    - Members
    - Loans

---

###  Themes
- **Theme.xaml**
  - Centralized styles and colors  
  - Provides a clean and consistent UI appearance  

---



