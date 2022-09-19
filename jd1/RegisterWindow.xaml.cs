using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.SqlClient;

namespace WpfApp1
{
    /// <summary>
    /// Logika interakcji dla klasy RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();

        }


        // checking if every registration data is correct 
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = @"Data Source=DESKTOP-853AFB5;Initial Catalog=bank;Integrated Security=True";
            using (DatabaseContext db = new DatabaseContext(connectionString))
            {
                bool isCorrect = false;
                bool isCorrectPassword = false;


                if (txtPesel.Text.Length != 11 || txtNrT.Text.Length != 9 || txtKod.Text.Length != 6 || txtImie.Text.Length == 0 || txtNazwisko.Text.Length == 0 || txtMiasto.Text.Length == 0 || txtAdres.Text.Length == 0)
                {
                    MessageBox.Show("Sprawdz wpisane przez Ciebie dane.");
                }

                else if (!txtPesel.Text.All(char.IsDigit))
                {
                    MessageBox.Show("Pesel może zawierać tylko cyfry.");
                }
                else
                {
                    isCorrect = true;
                }

                if (txtHaslo.Password.Length < 8 || !txtHaslo.Password.Any(char.IsUpper) || !txtHaslo.Password.Any(char.IsLower) || !txtHaslo.Password.Any(char.IsDigit) || !txtHaslo.Password.Any(char.IsPunctuation))
                {
                    MessageBox.Show("Hasło musi mieć co najmniej 8 znaków, jedną dużą literę, jedną małą literę, jedną cyfrę i jeden znak specjalny.");
                }
                else if (txtHaslo.Password != txtHaslo2.Password)
                {
                    MessageBox.Show("Hasła nie są identyczne.");
                }
                else
                {
                    isCorrectPassword = true;
                }





                LoginWindow lw = new LoginWindow();
                // registration of new user

                if (isCorrect == true && isCorrectPassword == true)
                {
                    db.Add(new User { Imie = txtImie.Text, Nazwisko = txtNazwisko.Text, Haslo = txtHaslo.Password, Pesel = txtPesel.Text, NrTel = txtNrT.Text, balance = 0 });
                    db.SaveChanges();

                    db.Add(new xAdres { UserID = db.Users.Where(x => x.Pesel == txtPesel.Text).FirstOrDefault().UserID, Adres = txtAdres.Text, KodPocztowy = txtKod.Text, Miasto = txtMiasto.Text });
                    db.SaveChanges();

                    db.Add(new Karta { UserID = db.Users.Where(x => x.Pesel == txtPesel.Text).FirstOrDefault().UserID, CVV = RandomDigits(3), NrKarty = RandomDigits(16), DataWaznosci = DateTime.Parse(DateTime.Now.AddYears(5).ToString().Substring(0, 10)) });
                    db.SaveChanges();
                    MessageBox.Show("Pomyslnie zarejestrowano, proszę się zalogować.");


                    lw.Show();
                    this.Close();



                }

            }
        }
        // random number generation method for credit card 
        public string RandomDigits(int length)
        {
            var random = new Random();
            string bf = string.Empty;
            for (int i = 0; i < length; i++)
                bf = String.Concat(bf, random.Next(10).ToString());
            return bf;
        }
    }
}
