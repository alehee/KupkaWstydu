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
    public partial class Highscore : ContentPage
    {
        MySqlConnector mySqlConnector = new MySqlConnector();

        int userID = Convert.ToInt32(Application.Current.Properties["ID"]);

        public Highscore()
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

            B_Filter_BestArrears_Clicked(null, null);
        }

        /// PRZYCISK PRZEJŚCIA NA HOME
        async void B_Home_Clicked(object sender, EventArgs args)
        {
            App.Current.MainPage = new NavigationPage(new MainPage());
        }
        /// ==========

        /// PRZYCISK PRZEJŚCIA NA HIGHSCORE (aktualnie usunięcie bazy)
        async void B_Profile_Clicked(object sender, EventArgs args)
        {
            App.Current.MainPage = new NavigationPage(new Profile());
        }
        /// ==========

        /// PRZYCISK FILTROWANIA - OPCJA: NAJWIĘCEJ WYKONANYCH
        async void B_Filter_BestArrears_Clicked(object sender, EventArgs args)
        {
            B_Filter_BestArrears.BackgroundColor = Xamarin.Forms.Color.FromHex("#7e8181");
            B_Filter_WorstArrears.BackgroundColor = Xamarin.Forms.Color.FromHex("#b7b8b8");

            SL_HighscoreTable.Children.Clear();

            string[,] returnedArray = mySqlConnector.getHighscoreArray(1);

            refreshTable(returnedArray);
        }
        /// ==========

        /// PRZYCISK FILTROWANIA - OPCJA: NAJWIĘCEJ NIEWYKONANYCH
        async void B_Filter_WorstArrears_Clicked(object sender, EventArgs args)
        {
            B_Filter_BestArrears.BackgroundColor = Xamarin.Forms.Color.FromHex("#b7b8b8");
            B_Filter_WorstArrears.BackgroundColor = Xamarin.Forms.Color.FromHex("#7e8181");

            SL_HighscoreTable.Children.Clear();

            string[,] returnedArray = mySqlConnector.getHighscoreArray(2);

            refreshTable(returnedArray);
        }
        /// ==========
         
        async void refreshTable(string [,] scoreArray)
        {
            for (int i = 0; i < 100; i++)
            {
                if (!String.IsNullOrEmpty(scoreArray[i, 0]))
                {
                    string indexString = " #";
                    if (scoreArray[i, 0].Length < 2)
                        indexString += "000" + scoreArray[i, 0];
                    else if (scoreArray[i, 0].Length < 3)
                        indexString += "00" + scoreArray[i, 0];
                    else if (scoreArray[i, 0].Length < 4)
                        indexString += "0" + scoreArray[i, 0];
                    else
                        indexString += scoreArray[i, 0];

                    // DLA PIERWSZEGO INDEKSU INNY WIDOK
                    if (Convert.ToInt32(scoreArray[i, 0]) == userID)
                    {
                        SL_HighscoreTable.Children.Add(new Label
                        {
                            Text = scoreArray[i, 1] + indexString,
                            TextColor = Xamarin.Forms.Color.FromHex("#4d2600"),
                            FontAttributes = FontAttributes.Bold,
                            Margin = new Thickness(0, -5, 0, 0),
                            Padding = new Thickness(20, 0),
                            WidthRequest = 350,
                            HeightRequest = 30,
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalTextAlignment = TextAlignment.Center,
                            HorizontalTextAlignment = TextAlignment.Center,
                            BackgroundColor = Xamarin.Forms.Color.FromHex("#7e8181")
                        });
                    }
                    else if (i == 0)
                    {
                        SL_HighscoreTable.Children.Add(new Label
                        {
                            Text = scoreArray[i, 1] + indexString,
                            Margin = new Thickness(0, 0, 0, 0),
                            Padding = new Thickness(20, 0),
                            WidthRequest = 350,
                            HeightRequest = 30,
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalTextAlignment = TextAlignment.Center,
                            HorizontalTextAlignment = TextAlignment.Center,
                            BackgroundColor = Xamarin.Forms.Color.FromHex("#e6e6e6")
                        });
                    }
                    else if (i % 2 == 1)
                    {
                        SL_HighscoreTable.Children.Add(new Label
                        {
                            Text = scoreArray[i, 1] + indexString,
                            Margin = new Thickness(0, -5, 0, 0),
                            Padding = new Thickness(20, 0),
                            WidthRequest = 350,
                            HeightRequest = 30,
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalTextAlignment = TextAlignment.Center,
                            HorizontalTextAlignment = TextAlignment.Center,
                            BackgroundColor = Xamarin.Forms.Color.FromHex("#f2f2f2")
                        });
                    }
                    else
                    {
                        SL_HighscoreTable.Children.Add(new Label
                        {
                            Text = scoreArray[i, 1] + indexString,
                            Margin = new Thickness(0, -5, 0, 0),
                            Padding = new Thickness(20, 0),
                            WidthRequest = 350,
                            HeightRequest = 30,
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalTextAlignment = TextAlignment.Center,
                            HorizontalTextAlignment = TextAlignment.Center,
                            BackgroundColor = Xamarin.Forms.Color.FromHex("#e6e6e6")
                        });
                    }

                    SL_HighscoreTable.Children.Add(new Label
                    {
                        Text = (i + 1).ToString() + ".     " + scoreArray[i, 2],
                        TextColor = Xamarin.Forms.Color.FromHex("#4d2600"),
                        Margin = new Thickness(-290, -37, 0, 0),
                        WidthRequest = 50,
                        HeightRequest = 30,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        HorizontalTextAlignment = TextAlignment.Start
                    });
                }
            }
        }
    }
}