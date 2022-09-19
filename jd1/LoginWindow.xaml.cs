using System;
using System.Linq;
using System.Windows;
using System.Data.SqlClient;
using System.Data;
namespace WpfApp1
{
    /// <summary>
    /// Logika interakcji dla klasy LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {

        public LoginWindow()
        {
            InitializeComponent();
        }

        public void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            MainW();

        }


        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow rw = new RegisterWindow();
            rw.Show();
            this.Close();
        }


        /// <summary>
        /// Funkcja zawierająca część logiki MainWindow i wywołująca MainWindow.
        /// Logowanie użytkownika.
        /// <param name="s">
        /// Połączenie z lokalną bazą danych LocalDB.
        /// </param>
        /// <param name="query">
        /// Query bazodanowe wybierające użytkownika na podstawie wpisanego Peselu oraz Hasła.
        /// </param>
        /// <param name="count">
        /// Zmienna ktora przechowuje informacje o tym czy query zostało wykonanie i na tej podstawie odbywa sie sprawdzanie czy mozna zalogowac uzytkownika.
        /// </param>
        /// <param name="UID">
        /// Method-based syntax query zwracające UserID zalogowanego użytkownika.
        /// </param>
        /// <param name="imie">
        /// Method-based syntax query zwracające Imie zalogowanego użytkownika.
        /// </param>
        /// <param name="nazwisko">
        /// Method-based syntax query zwracające Nazwisko zalogowanego użytkownika.
        /// </summary>
        public void MainW()
        {
            SqlConnection s = new SqlConnection("Data Source=DESKTOP-853AFB5;Initial Catalog=bank;Integrated Security=True");


            try
            {
                if (s.State == ConnectionState.Closed)
                {
                    s.Open();
                }


                string query = "SELECT COUNT(1) FROM Users WHERE Pesel ='" + txtPesel.Text + "' AND Haslo ='" + txtPassword.Password + "'";
                SqlCommand sc = new SqlCommand(query, s);
                sc.CommandType = CommandType.Text;
                int count = Convert.ToInt32(sc.ExecuteScalar());



                if (count == 1)
                {

                    string connectionString = @"Data Source=DESKTOP-853AFB5;Initial Catalog=bank;Integrated Security=True";
                    using (DatabaseContext db = new DatabaseContext(connectionString))
                    {
                        MainWindow mw = new MainWindow();
                        var UID = db.Users.Where(x => x.Pesel.Equals(txtPesel.Text)).Select(x => x.UserID).FirstOrDefault();

                        string name = db.Users.Where(x => x.Pesel.Equals(txtPesel.Text)).Select(x => x.Imie).FirstOrDefault();
                        string surname = db.Users.Where(x => x.Pesel.Equals(txtPesel.Text)).Select(x => x.Nazwisko).FirstOrDefault();

                        string ni = name + " " + surname;

                        mw.IN.Text = "Witaj " + db.Users.Where(x => x.Pesel.Equals(txtPesel.Text)).Select(x => x.Imie).FirstOrDefault();


                        mw.CardNum_TextBox.Text = db.Cards.Where(x => x.UserID.Equals(UID)).Select(x => x.NrKarty).FirstOrDefault().ToString();
                        mw.CVV_TextBox.Text = db.Cards.Where(x => x.UserID.Equals(UID)).Select(x => x.CVV).FirstOrDefault().ToString();
                        mw.ExpDate_TextBox.Text = db.Cards.Where(x => x.UserID.Equals(UID)).Select(x => x.DataWaznosci).FirstOrDefault().ToString().Substring(0, 10);
                        mw.CardOwner_TextBox.Text = ni;
                        mw.AccountBalance_TextBox.Text = $" {db.Users.Where(x => x.Pesel.Equals(txtPesel.Text)).Select(x => x.balance).First().ToString()}";
                        mw.Show();
                        this.Close();
                    }

                }
                else
                {
                    MessageBox.Show("Błędne login lub hasło.");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                s.Close();
            }
        }


    }
}

