using System;
using Persistance;
using System.Collections.Generic;
using BL;

namespace ConsolePL
{
    class UserInterFace{
        private string greeting = "+--------------------------+\n|        Book Store        |\n+--------------------------+\n";
        private string ending =  "+--------------------------+\n";
        private UserBl UserBl;
        private BookBl BookBl;
        private CustomerBl CustomerBl;
        private InvoiceBl InvoiceBl;
        private User user;
        private Invoice invoice = new Invoice();
        private List<Book> books = new List<Book>();
        private List<Customer> customers = new List<Customer>();
        private List<Invoice> invoices = new List<Invoice>();
        public UserInterFace(){
            BookBl = new BookBl();
            UserBl = new UserBl();
            CustomerBl = new CustomerBl();
            InvoiceBl = new InvoiceBl();
        }
        private int Menu(string greeting,string[] elements,string ending){
            int choice = -1,i;
            string input;
            Console.Write(greeting);
            for(i = 0; i < elements.Length; i++){
                Console.WriteLine("{0}. {1}",i+1,elements[i]);
            }
            Console.Write(ending);
            Console.Write("Choice : ");
            input = Console.ReadLine();
            if(!int.TryParse(input,out choice)) choice = -1;
            while(choice < 1 || choice > i){
                Console.Write("Invalid!choice again : ");
                input = Console.ReadLine();
                if(!int.TryParse(input,out choice)) choice = -1;
            }
            return choice;
        }
        public void MainUI() {
            string[] MainMenu = {"Login","Display All Book","Exit"};
            int option = 0;
            bool stop = false;
            while(!stop){
                // Console.Clear();
                option = Menu(greeting,MainMenu,ending);
                switch(option){
                    case 1:
                        Login();
                        break;
                    case 2:
                        DisplayAllBook(books);
                        Console.ReadLine();
                        break;
                    case 3:
                        stop = true;
                        Console.WriteLine("See you later!!");
                        break;
                }
            }
        }
        public void Login(){
            string userName;
            string password;
            bool stop = false;
            while(!stop){
                Console.Clear ();
                Console.WriteLine("+--------------------------+");
                Console.WriteLine("|          Login           |");
                Console.WriteLine("+--------------------------+");
                Console.Write("User Name: ");
                userName = Console.ReadLine();
                Console.Write("Password: ");
                password = GetPassword();
                Console.WriteLine();
                user = new User(){UserName = userName,UserPassword = password};
                int login = UserBl.Login(user);
                if(login <= 0){
                    Console.WriteLine("Can't Login");
                    Console.WriteLine("Do you want try again?");
                    Console.WriteLine("Press any key to input again...");
                    Console.Write("Or Press N or n to exit Login : ");
                    string choice = Console.ReadLine();
                    if(choice == "N" || choice == "n"){
                        stop = true;
                    }
                }else{
                    SuccessLogin();
                }
            }
        }
        static string GetPassword()
        {
            var pass = string.Empty;
            ConsoleKey key;
            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    Console.Write("\b \b");
                    pass = pass[0..^1];
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    pass += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);
            return pass;
        }

        public void SuccessLogin(){
            string[] MainMenu = {"Books","Customers","Invoices","Log Out"};
            int option = 0;
            bool stop = false;
            while(!stop){
                Console.Clear();
                option = Menu(greeting,MainMenu,ending);
                switch(option){
                    case 1:
                        Search();
                        break;
                    case 2:
                        Customers();
                        break;
                    case 3:
                        Invoice();
                        break;
                    case 4:
                        stop = true;
                        break;
                    
                }
            }
        }
        public void Invoice(){
            string[] MainMenu = {"Create Invoice","Invoices","Out Invoice"};
            string greetingInvoice = "+--------------------------+\n|          Invoice         |\n+--------------------------+\n";
            int option = 0;
            bool stop = false;
            while(!stop){
                Console.Clear();
                option = Menu(greetingInvoice,MainMenu,ending);
                switch(option){
                    case 1:
                        CreateInvoice(invoice);
                        break;
                    case 2:
                        DisplayInvoiceByID(SearchInvoiceByID(DisplayAllInvoice(invoices)));
                        Console.ReadLine();
                        break;
                    case 3:
                        stop = true;
                        break;
                }
            }
        }
        public void Payment(List<Invoice> listInvoices){
                Console.WriteLine("Choice id : ");
                int choiceId = int.Parse(Console.ReadLine());
                while(choiceId < 1){
                    Console.WriteLine("Choice again");
                    choiceId = int.Parse(Console.ReadLine());
                }
                invoice = InvoiceBl.GetInvoiceId(choiceId);
                if(invoice == null){
                    Console.WriteLine("not exists");
                    Console.WriteLine("Press any key to continue");
                    Console.ReadKey();
                }else{
                    DisplayInvoiceByID(invoice);
                }
                Console.WriteLine("Do you want to pay?? Y/N");
                string y = Console.ReadLine();
                do{
                    if(y == "Y" || y == "y"){
                        bool paySuccess = InvoiceBl.PayMent(choiceId);
                        if(paySuccess){
                            Console.WriteLine("PayMent Success");
                            Console.WriteLine("Press any key to continue");
                            Console.ReadKey();
                            break;
                        }else{
                            Console.WriteLine("PayMent Fail");
                            Console.WriteLine("Press any key to continue");
                            Console.ReadKey();
                            break;
                        }
                    }else if(y != "n" || y != "N"){
                        break;
                    }else{
                        Console.Write("Invalid Input Choice again: ");
                        y = Console.ReadLine();
                    }
                }while(y != "n" || y != "N");
            
        }
        public List<Invoice> DisplayAllInvoice(List<Invoice> invoiceList){
            string status;
            invoiceList = InvoiceBl.GetInvoices();
            if(invoiceList.Count < 1){
                Console.WriteLine("Don't Found any invoices");
            }else{
                Console.WriteLine("+----------------------------------------------------------------------+");
                Console.WriteLine("|                        Display Invoice                               |");
                Console.WriteLine("+----------------------------------------------------------------------+");
                Console.WriteLine("| ID | Date Time                          | Status     |CustomerName   |");
                Console.WriteLine("+----------------------------------------------------------------------+");
                for(int i = 0; i < invoiceList.Count; i++){
                    status = invoiceList[i].Status == InvoiceStatus.CREATE_NEW_INVOICE ? "Paid" : "UnPaid";
                    Console.WriteLine($"|{invoiceList[i].InvoiceId,2}  |{invoiceList[i].InvoiceDate,-36}|{status,-12}|{invoiceList[i].InvoiceCustomer.CustomerName,-15}|");
                    Console.WriteLine("+----------------------------------------------------------------------+");
                }
            }
            return invoiceList;
        }
        public Invoice SearchInvoiceByID(List<Invoice> invoiceList){
            bool stop = false;
            int choice =  -1;
            while(!stop){
                Console.WriteLine("+--------------------------+");
                Console.WriteLine("|       Search By ID       |");
                Console.WriteLine("+--------------------------+");
                Console.Write("Input ID : ");
                string input = Console.ReadLine();
                if(!int.TryParse(input,out choice)) choice = -1;
                if(choice == 0){
                    stop = true;
                    break;
                }
                while(choice < 1 || choice > invoiceList.Count){
                    Console.Write("Invalid!choice again : ");
                    input = Console.ReadLine();
                    if(!int.TryParse(input,out choice)) choice = -1;
                    if(choice == 0){
                        stop = true;
                        break;
                    }
                }
                stop = true;
            }
            return InvoiceBl.GetInvoiceId(choice);
        }

        public void DisplayInvoiceByID(Invoice i){
            decimal? total = 0;
            string status;
            status = i.Status == InvoiceStatus.CREATE_NEW_INVOICE ? "Paid" : "UnPaid";
            Console.WriteLine("------Invoice Detail------\n");
            Console.WriteLine("Invoice ID : {0}",i.InvoiceId);
            Console.WriteLine("Date       : {0}",i.InvoiceDate);
            Console.WriteLine("Status     : {0}",status);
            for(int j = 0; j < i.BooksList.Count; j++){
                Console.WriteLine("Book name : {0}",i.BooksList[j].BookName);
                Console.WriteLine("Unit Price : {0}",i.BooksList[j].BookPrice);
                Console.WriteLine("Quantity : {0}",i.BooksList[j].Amount);
                Console.WriteLine("Amount : {0}",i.BooksList[j].Amount*i.BooksList[j].BookPrice);
                total += i.BooksList[j].Amount*i.BooksList[j].BookPrice;
            }
            Console.WriteLine("Total : {0}",total);
        }
        
        public void CreateInvoice(Invoice invoice){
            bool stop = false;
            invoice = new Invoice();
            DisplayAllCustomer(customers);
            invoice.InvoiceCustomer = SearchCustomersByName(customers);
            while(!stop){
                DisplayAllBook(books);
                invoice.BooksList.Add(SearchByName(books));
                Console.Write("Press any key except \"N\" to add more !");
                string input = Console.ReadLine();
                if(input == "n" || input == "N"){
                    stop = true;
                }else{
                    stop = false;
                }
            }
            for(int i = 0;i < invoice.BooksList.Count;i++){
                Console.Write("input amount of {0} :",invoice.BooksList[i].BookName);
                int amount = int.Parse(Console.ReadLine());
                invoice.BooksList[i].Amount = amount;
            }
            Console.WriteLine("Create Invoice: " + (InvoiceBl.CreateInvoice(invoice) ? "completed!" : "not complete!"));
            Console.ReadLine();
        }

        public List<Book> DisplayAllBook(List<Book> bookList){
            string status;
            bookList = BookBl.GetAll();
            Console.Clear();
            Console.WriteLine("+----------------------------------------------------------------------------------------+");
            Console.WriteLine("|                                   Display Books                                        |");
            Console.WriteLine("+----------------------------------------------------------------------------------------+");
            Console.WriteLine("| ID | Name Book                          | Status          | Amount       | Price       |");
            Console.WriteLine("+----------------------------------------------------------------------------------------+");
            for(int i = 0; i < bookList.Count; i++){
                status = bookList[i].Status == BookStatus.ACTIVE ? "Active" : "UnActive";
                Console.WriteLine($"|{bookList[i].BookId,2}  |{bookList[i].BookName,-36}|{status,-17}|{bookList[i].Amount,-14}|{bookList[i].BookPrice,-9} VND|");
                Console.WriteLine("+----------------------------------------------------------------------------------------+");
            }
            return bookList;
        }

        public void Search(){
            string[] MainMenu = {"Search Book By ID","Search By Name","Out Search"};
            string greetingSearch = "+--------------------------+\n|        Search Books      |\n+--------------------------+\n";
            int option = 0;
            bool stop = false;
            while(!stop){
                Console.Clear();
                option = Menu(greetingSearch,MainMenu,ending);
                switch(option){
                    case 1:
                        DisplayAllBook(books);
                        Console.WriteLine(SearchByID(DisplayAllBook(books)));
                        Console.ReadLine();
                        break;
                    case 2:
                        DisplayAllBook(books);
                        Console.WriteLine(SearchByName(books));
                        Console.ReadLine();
                        break;
                    case 3:
                        stop = true;
                        break;
                }
            }
        }

        public Book SearchByID(List<Book> bookList){
            bool stop = false;
            int choice =  -1;
            while(!stop){
                Console.WriteLine("+--------------------------+");
                Console.WriteLine("|       Search By ID       |");
                Console.WriteLine("+--------------------------+");
                Console.Write("Input ID : ");
                string input = Console.ReadLine();
                if(!int.TryParse(input,out choice)) choice = -1;
                if(choice == 0){
                    stop = true;
                    break;
                }
                while(choice < 1 || choice > bookList.Count){
                    Console.Write("Invalid!choice again : ");
                    input = Console.ReadLine();
                    if(!int.TryParse(input,out choice)) choice = -1;
                    if(choice == 0){
                        stop = true;
                        break;
                    }
                }
                return BookBl.GetBookById(choice);
            }
            return BookBl.GetBookById(choice);
        }

        public Book SearchByName(List<Book> bookList){
            bookList = BookBl.GetAll();
            bool stop = false;
            int choice = -1;
            while(!stop){
                Console.WriteLine("+--------------------------+");
                Console.WriteLine("|      Search By Name      |");
                Console.WriteLine("+--------------------------+");
                Console.Write("Input Name : ");
                string input = Console.ReadLine();
                if(input == "0"){
                    stop = true;
                    break;
                }
                List<Book> reBooks = null;
                reBooks = BookBl.GetByName(input);
                if(reBooks.Count < 1){
                    Console.WriteLine("not found result!");
                }else{
                    Console.WriteLine("+----------------------------------------------------------------------------------------+");
                    Console.WriteLine("|                                   Display Books                                        |");
                    Console.WriteLine("+----------------------------------------------------------------------------------------+");
                    Console.WriteLine("| ID | Name Book                          | Status          | Amount       | Price       |");
                    Console.WriteLine("+----------------------------------------------------------------------------------------+");
                    for(int i = 0; i < reBooks.Count; i++){
                        Console.WriteLine($"|{reBooks[i].BookId,2}  |{reBooks[i].BookName,-36}|{reBooks[i].Status,-17}|{reBooks[i].Amount,-14}|{reBooks[i].BookPrice,-9} VND|");
                        Console.WriteLine("+----------------------------------------------------------------------------------------+");
                    }
                    Console.Write("choice ID to see detail: ");
                    input = Console.ReadLine();
                    if(!int.TryParse(input,out choice)) choice = -1;
                    if(choice == 0){
                        stop = true;
                        break;
                    }
                    while(choice < 1 || choice > bookList.Count){
                        Console.Write("Invalid!choice again : ");
                        input = Console.ReadLine();
                        int.TryParse(input,out choice);
                        if(choice == 0){
                            stop = true;
                            break;
                        }
                    }
                }
                stop = true;
            }
            return BookBl.GetBookById(choice);
        }
        
        public void Customers(){
            string[] mainMenu = {"Search Customer By ID","Search By Name","Dislay All Customer","Out Customers"};
            string greetingCustomer = "+--------------------------+\n|         Customers        |\n+--------------------------+\n";
            int option = 0;
            bool stop = false;
            while(!stop){
                Console.Clear();
                option = Menu(greetingCustomer,mainMenu,ending);
                switch(option){
                    case 1:
                        DisplayAllCustomer(customers);
                        Console.WriteLine(SearchCustomerByID(DisplayAllCustomer(customers)));
                        Console.ReadLine();
                        break;
                    case 2:
                        DisplayAllCustomer(customers);
                        Console.WriteLine(SearchCustomersByName(customers));
                        Console.ReadLine();
                        break;
                    case 3:
                        DisplayAllCustomer(customers);
                        Console.ReadLine();
                        break;
                    case 4:
                        stop = true;
                        break;
                }
            }
        }

        public Customer SearchCustomerByID(List<Customer> customerList){
            bool stop = false;
            int choice =  -1;
            while(!stop){
                Console.WriteLine("+--------------------------+");
                Console.WriteLine("|       Search By ID       |");
                Console.WriteLine("+--------------------------+");
                Console.Write("Input ID : ");
                string input = Console.ReadLine();
                if(!int.TryParse(input,out choice)) choice = -1;
                if(choice == 0){
                    stop = true;
                    break;
                }
                while(choice < 1 || choice > customerList.Count){
                    Console.Write("Invalid!choice again : ");
                    input = Console.ReadLine();
                    if(!int.TryParse(input,out choice)) choice = -1;
                    if(choice == 0){
                        stop = true;
                        break;
                    }   
                }
                return CustomerBl.GetById(choice);
            }
            return CustomerBl.GetById(choice);
        }

        public List<Customer> DisplayAllCustomer(List<Customer> customerList){
            customerList = CustomerBl.GetAll();
            Console.WriteLine("+------------------------------------------------------------------------------+");
            Console.WriteLine("|                                Display Customers                             |");
            Console.WriteLine("+------------------------------------------------------------------------------+");
            Console.WriteLine("| ID | Name Customer                          | Address         | Phone        |");
            Console.WriteLine("+------------------------------------------------------------------------------+");
            for(int i = 0; i < customerList.Count; i++){
                Console.WriteLine($"|{customerList[i].CustomerId,2}  |{customerList[i].CustomerName,-40}|{customerList[i].CustomerAddress,-17}|{customerList[i].CustomerPhone,-14}|");
                Console.WriteLine("+------------------------------------------------------------------------------+");
            }
            return customerList;
        }

        public Customer SearchCustomersByName(List<Customer> customerList){
            customerList = CustomerBl.GetAll();
            List<Customer> reCustomers = new List<Customer>();
            bool stop = false;
            int choice = -1;
            while(!stop){
                Console.WriteLine("+--------------------------+");
                Console.WriteLine("|      Search By Name      |");
                Console.WriteLine("+--------------------------+");
                Console.Write("Input Name : ");
                string input = Console.ReadLine();
                if(input == "0"){
                    stop = true;
                    break;
                }
                reCustomers = CustomerBl.GetByName(input);
                if(reCustomers.Count < 1){
                    Console.WriteLine("Not found result");
                }else{
                    Console.WriteLine("+------------------------------------------------------------------------------+");
                    Console.WriteLine("|                               Display Customers                              |");
                    Console.WriteLine("+------------------------------------------------------------------------------+");
                    Console.WriteLine("| ID | Name Customer                          | Address         | Phone        |");
                    Console.WriteLine("+------------------------------------------------------------------------------+");
                    for(int i = 0; i < reCustomers.Count; i++){
                        Console.WriteLine($"|{reCustomers[i].CustomerId,2}  |{reCustomers[i].CustomerName,-40}|{reCustomers[i].CustomerAddress,-17}|{reCustomers[i].CustomerPhone,-14}|");
                        Console.WriteLine("+------------------------------------------------------------------------------+");
                    }
                    Console.Write("choice ID to see detail: ");
                    input = Console.ReadLine();
                    if(!int.TryParse(input,out choice)) choice = -1;
                    if(choice == 0){
                        stop = true;
                        break;
                    }
                    while(choice < 1 || choice > customerList.Count){
                        Console.Write("Invalid!choice again : ");
                        input = Console.ReadLine();
                        int.TryParse(input,out choice);
                        if(choice == 0){
                            stop = true;
                            break;
                        }
                    }
                }
                stop = true;
            }
            return CustomerBl.GetById(choice);
        }
    }
}