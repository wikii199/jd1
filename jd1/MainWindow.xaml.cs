using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;


namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();

        }
        SqlConnection ca;
        SqlCommand cma;
        SqlDataReader da;
        bool minusplus = false;
        // adding data to credit card and amount of the account to main window
        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = @"Data Source=DESKTOP-853AFB5;Initial Catalog=bank;Integrated Security=True";
            using (DatabaseContext db = new DatabaseContext(connectionString))
            {

                var UIDq = db.Cards.Where(x => x.NrKarty.Equals(CardNum_TextBox.Text)).Select(x => x.UserID).FirstOrDefault();
                var balance1 = db.Users.Where(x => x.UserID.Equals(UIDq)).Select(x => x.balance).FirstOrDefault();

                if (AccountNumber_Button.Text.Length == 0 || Amount_button.Text.Length == 0 || dpDate.Text.Length == 0 || Amount_button.Text.Any(char.IsLetter) || !Amount_button.Text.All(char.IsDigit))
                {
                    MessageBox.Show("Wprowadz poprawdne dane do przelewu.");
                }
                else if (decimal.Parse(Amount_button.Text) <= 0)
                {
                    MessageBox.Show("Kwota przelewu powinna być większa od 0");
                }
                else if (decimal.Parse(Amount_button.Text) > balance1)
                {
                    MessageBox.Show("Zbyt niskie saldo");
                }
                else
                {
                    db.Add(new Przelew { DataPrzelewu = dpDate.DisplayDate, Kwota = long.Parse(Amount_button.Text), UserID = UIDq, NaKonto = AccountNumber_Button.Text });
                    db.SaveChanges();

                    MessageBox.Show("Pomyślnie wykonano transfer");

                    User user1 = db.Users.Where(x => x.UserID.Equals(UIDq)).First();

                    var transfer = decimal.Parse(Amount_button.Text);

                    user1.balance = user1.balance - transfer;
                    db.SaveChanges();
                    this.UpdateLayout();


                    try
                    {
                        User user2 = db.Users.Where(x => x.UserID.Equals(int.Parse(AccountNumber_Button.Text))).FirstOrDefault();
                        if (!user2.Equals(null))
                        {
                            user2.balance = user2.balance + transfer;
                            minusplus = true;
                            db.SaveChanges();
                        }
                    }
                    catch (System.NullReferenceException)
                    {
                        MessageBox.Show("Bledny numer konta");
                    }

                }
            }
        }
        // Binding data to list box
        private void BindComboBox_Load(object sender, EventArgs e)
        {
            ca = new SqlConnection(@"Data Source=DESKTOP-853AFB5;Initial Catalog=bank;Integrated Security=True");
            ca.Open();

            BindData();
        }
        public void BindData()
        {
            cma = new SqlCommand("select  Kwota  from Przelewy", ca);
            da = cma.ExecuteReader();
            while (da.Read())
            {
                if (minusplus == true)
                {
                    Transaction_ListBox.Items.Add("+" + da[0].ToString());
                }
                else
                    Transaction_ListBox.Items.Add("-" + da[0].ToString());
            }
            da.Close();
        }
        // logout button
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow lw = new LoginWindow();
            lw.Show();
            this.Close();

        }
        // refreshing button for listbox

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            Transaction_ListBox.Items.Clear();
            BindComboBox_Load(sender, e);

        }
    }


}





