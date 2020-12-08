using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KW_XamarinForms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Profile : ContentPage
    {
        MySqlConnector mySqlConnector = new MySqlConnector();

        int userID = Convert.ToInt32(Application.Current.Properties["ID"]);

        public Profile()
        {
            InitializeComponent();

            // USTAW JAKO AKTUALNY PAGE
            NavigationPage.SetHasNavigationBar(this, false);

            // POKAŻ AKTUALNĄ WERSJĘ
            L_Version.Text = "Ver. " + MainPage.VERSION;

            // NADAJ FUNKCJĘ DO EDYCJI
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) =>
            {
                L_EditProfile_Clicked();
            };
            tapGestureRecognizer.NumberOfTapsRequired = 1;
            L_EditProfile.GestureRecognizers.Add(tapGestureRecognizer);

            // PONOWNA INICJALIZACJA PAGE'a
            reinitializeProfile();
        }

        /// PRZYCISK PRZEJŚCIA NA HOME
        async void B_Home_Clicked(object sender, EventArgs args)
        {
            App.Current.MainPage = new NavigationPage(new MainPage());
        }
        /// ==========

        /// PRZYCISK PRZEJŚCIA NA HIGHSCORE (aktualnie usunięcie bazy)
        async void B_Highscore_Clicked(object sender, EventArgs args)
        {
            App.Current.MainPage = new NavigationPage(new Highscore());
        }
        /// ==========

        /// PRZY KLIKNIĘCIU "E D Y T U J" USTAWIA NOWY PSEUDONIM
        async void L_EditProfile_Clicked()
        {
            string[] terms = new string[] { "Ambitny", "Odważny", "Beztroski", "Okrutny", "Kreatywny", "Oszczędny", "Zabawny", "Złośliwy", "Romantyczny", "Mały", "Nieśmiały", "Wesoły", "Agresywny", "Umięśniony", "Ogromny" };
            string[] animals = new string[] { "Pies", "Kot", "Lis", "Kurczak", "Kaczor", "Koń", "Dzik", "Indyk", "Królik", "Jeż", "Słoń", "Struś", "Jeleń" };

            string term = await DisplayActionSheet("Wybierz określenie pseudonimu", "Anuluj", "", terms);
            if (term != "Anuluj" && term.Length > 0)
            {
                string animal = await DisplayActionSheet("Wybierz zwierzę pseudonimu", "Anuluj", "", animals);
                if (animal != "Anuluj" && animal.Length > 0)
                {
                    mySqlConnector.modifyUserData(userID, term + " " + animal);
                }
            }
        }
        /// ========== 

        /// OTWARCIE STRONY O AUTORZE
        async void B_CreatorInfo_Clicked(object sender, EventArgs args)
        {
            App.Current.MainPage = new NavigationPage(new Author());
        }
        /// ==========

        /// PODSTAWOWA FUNKCJA STRONY PROFILE
        async void reinitializeProfile()
        {
            string[] userData = mySqlConnector.getUserData(userID);
            if(userData != null)
            {
                // PSEUDONIM
                L_Pseudonym.Text = userData[0];

                // ID PROFILU
                string userIdString = "#";
                if (userID > 1000)
                    userIdString += userID.ToString();
                else if(userID > 100)
                    userIdString += "0" + userID.ToString();
                else if(userID > 10)
                    userIdString += "00" + userID.ToString();
                else
                    userIdString += "000" + userID.ToString();
                L_Id.Text = userIdString;

                // ZALEGŁOŚCI WYKONANE
                int arrearsCompleted = Convert.ToInt32(userData[2]);
                L_StatCompleted.Text = arrearsCompleted.ToString();

                // ZALEGŁOŚCI DO WYKONANIA
                int arrearsTodo = Convert.ToInt32(userData[3]);
                L_StatTodo.Text = arrearsTodo.ToString();

                // SUMA ZALEGŁOŚCI
                int arrearsSum = arrearsCompleted + arrearsTodo;
                L_StatAll.Text = arrearsSum.ToString();
            }
        }
        /// ========== 
        
    }
}