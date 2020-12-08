using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using SQLite;
using SQLitePCL;
using System.IO;
using Plugin.Toast;

namespace KW_XamarinForms
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public const string VERSION = "1.0.1";

        MySqlConnector mySqlConnector = new MySqlConnector();
        List<Category> categories = new List<Category>();

        int userID = 0;

        public MainPage()
        {
            InitializeComponent();

            // SPRAWDZENIE ID
            checkIn();

            reinitializeCategories();

            NavigationPage.SetHasNavigationBar(this, false);

            // OTWIERA MOŻLIWOŚĆ USUWANIA KATEGORII
            App.Database.DeleteCategoryAsync(0);

            //App.Database.TerminateDatabase();
        }

        /// PRZYCISK DODANIA KATEGORII LUB ZALEGŁOŚCI
        async void B_AddTask_Clicked(object sender, EventArgs args)
        {
            try
            {
                string option;
                if (categories.Count > 0)
                    option = await DisplayActionSheet("Co chcesz dodać?", "Anuluj", "", "Kategorię", "Zaległość");
                else
                    option = await DisplayActionSheet("Co chcesz dodać?", "Anuluj", "", "Kategorię");

                if (option == "Kategorię")
                {
                    string category = await DisplayActionSheet("Tworzenie kategorii", "Anuluj", "", "Filmy", "Seriale", "Gry", "Książki", "Różne");
                    if (category != "Anuluj")
                    {
                        string newName = await DisplayPromptAsync("Tworzenie kategorii", "Podaj nazwę kategorii", "OK", "Anuluj", null, -1, null, "");
                        if (newName != "ANULUJ" && newName.Length > 0)
                        {
                            bool isGood = true;
                            short icoId = 5;
                            switch (category)
                            {
                                case "Filmy":
                                    icoId = 1;
                                    break;
                                case "Seriale":
                                    icoId = 2;
                                    break;
                                case "Gry":
                                    icoId = 3;
                                    break;
                                case "Książki":
                                    icoId = 4;
                                    break;
                                default:
                                    icoId = 5;
                                    break;
                            }

                            foreach (Category x in categories)
                            {
                                if (x.name == newName)
                                {
                                    isGood = false;
                                }
                            }

                            if (isGood == false)
                            {
                                // WYŚWIETL BŁĄD DODANIA
                                await DisplayAlert("Błąd dodania kategorii", "Taka kategoria już istnieje!", "OK");
                            }
                            else
                            {
                                // DODAJ DO BAZY DANYCH
                                await App.Database.SaveCategoriesAsync(new DT_Category
                                {
                                    title = newName,
                                    iconId = icoId
                                });
                            }
                        }
                    }
                }
                else if (option == "Zaległość")
                {
                    List<string> categoriesStrings = new List<string>();
                    string[] categoriesArray = new string[categories.Count];
                    for (int i = 0; i < categories.Count; i++)
                    {
                        categoriesArray[i] = categories[i].name;
                    }

                    string category = await DisplayActionSheet("Tworzenie zaległości", "Anuluj", "", categoriesArray);
                    if (category != "Anuluj" && category.Length > 0)
                    {
                        string newName = await DisplayPromptAsync("Tworzenie zaległości", "Podaj nazwę zaległości", "OK", "Anuluj", null, -1, null, "");
                        if (newName != "ANULUJ" && newName.Length > 0)
                        {
                            short catId;
                            short catPos;
                            Category cat = null;

                            foreach (Category x in categories)
                            {
                                if (x.name == category)
                                {
                                    cat = x;
                                }
                            }

                            catId = cat.id;
                            catPos = Convert.ToInt16(cat.arrears.Count + 1);

                            // DODAJ DO BAZY DANYCH
                            await App.Database.SaveArrearAsync(new DT_Arrear
                            {
                                categoryId = catId,
                                categoryPosition = catPos,
                                status = 0,
                                name = newName
                            });

                            // WYŚLIJ DO BAZY MYSQL
                            mySqlConnector.modifyNotCompletedArrears(userID, 1);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                CrossToastPopUp.Current.ShowToastError("Wystąpił błąd");
            }
            reinitializeCategories();
        }
        /// ==========
        
        /// PRZYCISK DODANIA KATEGORII LUB ZALEGŁOŚCI
        async void B_DelCategory_Clicked(object sender, EventArgs args)
        {
            try
            {
                List<string> categoriesStrings = new List<string>();
                string[] categoriesArray = new string[categories.Count];
                for (int i = 0; i < categories.Count; i++)
                {
                    categoriesArray[i] = categories[i].name;
                }

                if (categories.Count > 0)
                {
                    string category = await DisplayActionSheet("Usuwanie kategorii", "Anuluj", "", categoriesArray);
                    if (category != "Anuluj" && category.Length > 0)
                    {
                        bool verification = await DisplayAlert("Usuwanie kategorii", "Czy napewno chcesz usunąć kategorię " + category + "?", "TAK", "NIE");
                        if (verification)
                        {
                            short catId;
                            Category cat = null;

                            foreach (Category x in categories)
                            {
                                if (x.name == category)
                                {
                                    cat = x;
                                }
                            }

                            foreach (Arrear x in cat.arrears)
                            {
                                short arrearId = x.id;
                                await App.Database.DeleteArrearsAsync(arrearId);

                                // JEŚLI NIE ZROBIONE TO WYŚLIJ DO BAZY MYSQL
                                if (x.status==0)
                                    mySqlConnector.modifyNotCompletedArrears(userID, 0);
                            }

                            catId = cat.id;

                            // USUŃ Z BAZY DANYCH KATEGORIĘ I JEJ ZALEGŁOŚCI
                            await App.Database.DeleteCategoryAsync(catId);
                        }
                        else
                        {
                            CrossToastPopUp.Current.ShowToastError("Wybrano nie");
                        }
                    }
                }
                else
                {
                    CrossToastPopUp.Current.ShowToastMessage("Brak kategorii do usunięcia!");
                }
            }
            catch (Exception e)
            {
                CrossToastPopUp.Current.ShowToastError("Wystąpił "+e.ToString());
            }
            reinitializeCategories();
        }
        /// ==========

        /// PRZYCISK WYKONANIA LUB "ODKONANIA" ZALEGŁOŚCI
        async void I_ComArrear_Clicked(short arrearId, short arrearStatus)
        {
            if(arrearStatus == 0)
            {
                await App.Database.UpdateArrearAsync(arrearId, 1);
                if (userID > 0)
                {
                    mySqlConnector.modifyCompletedArrears(userID, 1);
                    mySqlConnector.modifyNotCompletedArrears(userID, 0);
                }
            }
            else
            {
                await App.Database.UpdateArrearAsync(arrearId, 0);
                if (userID > 0)
                {
                    mySqlConnector.modifyCompletedArrears(userID, 0);
                    mySqlConnector.modifyNotCompletedArrears(userID, 1);
                }
            }
                
            reinitializeCategories();
        }
        /// ========== 

        /// PRZYCISK USUNIĘCIA ZALEGŁOŚCI
        async void I_DelArrear_Clicked(short arrearId, string arrearName)
        {
            bool verification = await DisplayAlert("Usuwanie zaległości", "Czy napewno chcesz usunąć zaległość " + arrearName + "?", "TAK", "NIE");
            if (verification)
            {
                App.Database.DeleteArrearsAsync(arrearId);

                // WYŚLIJ DO BAZY MYSQL
                mySqlConnector.modifyNotCompletedArrears(userID, 0);

                reinitializeCategories();
            }
        }
        /// ========== 

        /// PRZY KLIKNIĘCIU KATEGORII MINIMALIZUJE LUB MAKSYMALIZUJE ZADANIA
        async void L_CatToggle_Clicked(short categoryId, bool alreadyHidden)
        {
            if (alreadyHidden)
            {
                App.Database.HideArrearsInCategoryAsync(categoryId, false);
            }
            else
            {
                App.Database.HideArrearsInCategoryAsync(categoryId, true);
            }

            reinitializeCategories();
        }
        /// ========== 

        /// PRZYCISK PRZEJŚCIA NA HIGHSCORE (aktualnie usunięcie bazy)
        async void B_Highscore_Clicked(object sender, EventArgs args)
        {
            App.Current.MainPage = new NavigationPage(new Highscore());
        }
        /// ==========
        
        /// PRZYCISK PRZEJŚCIA NA PROFIL
        async void B_Profile_Clicked(object sender, EventArgs args)
        {
            App.Current.MainPage = new NavigationPage(new Profile());
        }
        /// ==========

        /// ODŚWIEŻENIE LISTY KATEGORII I ZALEGŁOŚCI
        async void reinitializeCategories()
        {
            /// WRZUCENIE KATEGORII I ZALEGŁOŚCI DO KLAS
            categories.Clear();
            List<DT_Category> dt_CategoryList = await App.Database.GetCategoriesAsync();
            List<DT_Arrear> dT_ArrearList = await App.Database.GetArrearsAsync();
            short catId = -1;

            for(int i=0; i<dt_CategoryList.Count; i++)
            {
                categories.Add(new Category(Convert.ToInt16(dt_CategoryList[i].id), Convert.ToInt16(dt_CategoryList[i].iconId), dt_CategoryList[i].title, dt_CategoryList[i].hidden));
                catId = Convert.ToInt16(dt_CategoryList[i].id);
                for(int j=0; j<dT_ArrearList.Count; j++)
                {
                    if(dT_ArrearList[j].categoryId == catId)
                    {
                        categories[i].arrears.Add(new Arrear(Convert.ToInt16(dT_ArrearList[j].id), dT_ArrearList[j].categoryId, dT_ArrearList[j].categoryPosition, dT_ArrearList[j].status, dT_ArrearList[j].name));
                    }
                }
                categories[i].arrears = categories[i].arrears.OrderBy(x => x.categoryPosition).ToList();
            }
            categories = categories.OrderBy(x => x.id).ToList();
            /// ==========

            /// WYŚWIETLENIE KATEGORII I ZALEGŁOŚCI
            SL_CategoriesList.Children.Clear();
            SL_CategoriesList.Children.Add(new Button{
                Text = "DODAJ ZALEGŁOŚĆ",
                Margin = new Thickness(0,15,0,0),
                Padding = new Thickness(0, 0, 0, 0),
                HeightRequest = 35,
                WidthRequest = 250,
                HorizontalOptions = LayoutOptions.Center,
                CornerRadius = 20
            });
            SL_CategoriesList.Children.Add(new Button
            {
                Text = "",
                Margin = new Thickness(300, -42, 0, 0),
                Padding = new Thickness(0, 0, 0, 0),
                ImageSource = "thrash20.png",
                HeightRequest = 35,
                WidthRequest = 35,
                HorizontalOptions = LayoutOptions.Center,
                CornerRadius = 20
            });
            
            foreach (Button x in SL_CategoriesList.Children)
            {
                if(x.Text == "")
                {
                    x.Clicked += B_DelCategory_Clicked;
                }
                else if(x.Text == "DODAJ ZALEGŁOŚĆ")
                {
                    x.Clicked += B_AddTask_Clicked;
                }
            }

            foreach (Category x in categories)
            {
                // ILOŚĆ ZADAŃ WYKONANYCH W KATEGORII / ILOŚĆ ZADAŃ W KATEGORII
                int categoryArrears = x.arrears.Count;
                int categoryArrearsCompleted = 0;
                foreach (Arrear a in x.arrears)
                {
                    if (a.status == 1)
                        categoryArrearsCompleted++;
                }

                // KATEGORIA
                SL_CategoriesList.Children.Add(new Label
                {
                    Text = x.name,
                    BackgroundColor = Xamarin.Forms.Color.FromHex("#cccccc"),
                    Margin = new Thickness(20, 10, 20, 20),
                    VerticalTextAlignment = TextAlignment.Center,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Center,
                    FontAttributes = FontAttributes.Bold,
                    FontFamily = "Roboto",
                    WidthRequest = 400,
                    HeightRequest = 35,
                    Padding = new Thickness(50, 0, 30, 0)
                });

                if(categoryArrears == categoryArrearsCompleted)
                {
                    SL_CategoriesList.Children[SL_CategoriesList.Children.Count - 1].BackgroundColor = Xamarin.Forms.Color.FromHex("#77d496");
                }

                // LICZNIK ZALEGŁOŚCI DLA KATEGORII
                if(x.hidden == false || categoryArrears == 0)
                {
                    SL_CategoriesList.Children.Add(new Label
                    {
                        Text = categoryArrearsCompleted.ToString() + "/" + categoryArrears.ToString(),
                        Margin = new Thickness(280, -60, 0, 18),
                        VerticalTextAlignment = TextAlignment.Center,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center,
                        HorizontalTextAlignment = TextAlignment.Center,
                        FontFamily = "Roboto",
                        WidthRequest = 40,
                        HeightRequest = 35
                    });
                }
                else
                {
                    SL_CategoriesList.Children.Add(new Label
                    {
                        Text = categoryArrearsCompleted.ToString() + "/" + categoryArrears.ToString(),
                        Margin = new Thickness(280, -60, 0, 18),
                        VerticalTextAlignment = TextAlignment.Center,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center,
                        HorizontalTextAlignment = TextAlignment.Center,
                        FontFamily = "Roboto",
                        WidthRequest = 40,
                        HeightRequest = 35,
                        FontAttributes = FontAttributes.Bold
                    });
                }

                string iconString;
                switch (x.iconId)
                {
                    case 1:
                        iconString = "film";
                        break;
                    case 2:
                        iconString = "series";
                        break;
                    case 3:
                        iconString = "games";
                        break;
                    case 4:
                        iconString = "book";
                        break;
                    default:
                        iconString = "other";
                        break;
                }

                // IKONA
                SL_CategoriesList.Children.Add(new Image
                {
                    Source = iconString + ".png",
                    Margin = new Thickness(-280, -56, 0, 0)
                });

                // ZMIENNA DO NADAWANIA FUNKCJI KLIKANIA
                var tapGestureRecognizer = new TapGestureRecognizer();

                // JEŚLI KATEGORIA MA JAKIEKOLWIEK ZADANIA DAJ MOŻLIWOŚĆ UKRYWANIA JEJ
                if (categoryArrears > 0)
                {
                    // NADANIE KATEGORII FUNKCJI UKRYWANIA ZALEGŁOŚCI
                    tapGestureRecognizer = new TapGestureRecognizer();
                    tapGestureRecognizer.Tapped += (s, e) =>
                    {
                        L_CatToggle_Clicked(x.id, x.hidden);
                    };
                    tapGestureRecognizer.NumberOfTapsRequired = 1;
                    SL_CategoriesList.Children[SL_CategoriesList.Children.Count - 1].GestureRecognizers.Add(tapGestureRecognizer);
                }

                // JEŚLI ZALEGŁOŚCI KATEGORII NIE SĄ UKRYTE
                if(x.hidden == false)
                {
                    int arrearsIn = 0;
                    foreach (Arrear arrear in x.arrears)
                    {
                        // PIERWSZA ZALEGŁOŚĆ
                        if (arrearsIn == 0)
                        {
                            if (arrear.status == 0)
                            {
                                SL_CategoriesList.Children.Add(new Label
                                {
                                    Text = arrear.name,
                                    BackgroundColor = Xamarin.Forms.Color.FromHex("#e6e6e6"),
                                    Margin = new Thickness(20, 0, 20, 0),
                                    VerticalTextAlignment = TextAlignment.Center,
                                    VerticalOptions = LayoutOptions.Center,
                                    HorizontalOptions = LayoutOptions.Center,
                                    HorizontalTextAlignment = TextAlignment.Center,
                                    FontAttributes = FontAttributes.Bold,
                                    FontFamily = "Roboto",
                                    WidthRequest = 400,
                                    HeightRequest = 35,
                                    Padding = new Thickness(20, 0, 80, 0)
                                });
                            }
                            else
                            {
                                SL_CategoriesList.Children.Add(new Label
                                {
                                    Text = arrear.name,
                                    BackgroundColor = Xamarin.Forms.Color.FromHex("#99ffbb"),
                                    Margin = new Thickness(20, 0, 20, 0),
                                    VerticalTextAlignment = TextAlignment.Center,
                                    VerticalOptions = LayoutOptions.Center,
                                    HorizontalOptions = LayoutOptions.Center,
                                    HorizontalTextAlignment = TextAlignment.Center,
                                    FontAttributes = FontAttributes.Bold,
                                    FontFamily = "Roboto",
                                    WidthRequest = 400,
                                    HeightRequest = 35,
                                    Padding = new Thickness(20, 0, 80, 0)
                                });
                            }
                        }
                        else
                        {
                            if (arrear.status == 0)
                            {
                                SL_CategoriesList.Children.Add(new Label
                                {
                                    Text = arrear.name,
                                    BackgroundColor = Xamarin.Forms.Color.FromHex("#e6e6e6"),
                                    Margin = new Thickness(20, 0, 20, 0),
                                    VerticalTextAlignment = TextAlignment.Center,
                                    VerticalOptions = LayoutOptions.Center,
                                    HorizontalOptions = LayoutOptions.Center,
                                    HorizontalTextAlignment = TextAlignment.Center,
                                    FontAttributes = FontAttributes.Bold,
                                    FontFamily = "Roboto",
                                    WidthRequest = 400,
                                    HeightRequest = 35,
                                    Padding = new Thickness(20, 0, 80, 0)
                                });
                            }
                            else
                            {
                                SL_CategoriesList.Children.Add(new Label
                                {
                                    Text = arrear.name,
                                    BackgroundColor = Xamarin.Forms.Color.FromHex("#99ffbb"),
                                    Margin = new Thickness(20, 0, 20, 0),
                                    VerticalTextAlignment = TextAlignment.Center,
                                    VerticalOptions = LayoutOptions.Center,
                                    HorizontalOptions = LayoutOptions.Center,
                                    HorizontalTextAlignment = TextAlignment.Center,
                                    FontAttributes = FontAttributes.Bold,
                                    FontFamily = "Roboto",
                                    WidthRequest = 400,
                                    HeightRequest = 35,
                                    Padding = new Thickness(20, 0, 80, 0)
                                });
                            }
                        }

                        if (arrear.status == 0)
                        {
                            // PRZYCISK COMPLETE
                            SL_CategoriesList.Children.Add(new Image
                            {
                                Source = "ygreen25.png",
                                Margin = new Thickness(300, -36, 20, 0)
                            });

                            // NADANIE PRZYCISKOWI FUNKCJI COMPLETE
                            tapGestureRecognizer = new TapGestureRecognizer();
                            tapGestureRecognizer.Tapped += (s, e) =>
                            {
                                I_ComArrear_Clicked(arrear.id, 0);
                            };
                            tapGestureRecognizer.NumberOfTapsRequired = 1;
                            SL_CategoriesList.Children[SL_CategoriesList.Children.Count - 1].GestureRecognizers.Add(tapGestureRecognizer);

                            // PRZYCISK DELETE
                            SL_CategoriesList.Children.Add(new Image
                            {
                                Source = "x25.png",
                                Margin = new Thickness(260, -31, 60, 0)
                            });

                            // NADANIE PRZYCISKOWI FUNKCJI DELETE
                            tapGestureRecognizer = new TapGestureRecognizer();
                            tapGestureRecognizer.Tapped += (s, e) =>
                            {
                                I_DelArrear_Clicked(arrear.id, arrear.name);
                            };
                            tapGestureRecognizer.NumberOfTapsRequired = 1;
                            SL_CategoriesList.Children[SL_CategoriesList.Children.Count - 1].GestureRecognizers.Add(tapGestureRecognizer);
                        }
                        else
                        {
                            // PRZYCISK COMPLETE
                            SL_CategoriesList.Children.Add(new Image
                            {
                                Source = "yblue25.png",
                                Margin = new Thickness(290, -36, 20, 0)
                            });

                            // NADANIE PRZYCISKOWI FUNKCJI COMPLETE
                            tapGestureRecognizer = new TapGestureRecognizer();
                            tapGestureRecognizer.Tapped += (s, e) =>
                            {
                                I_ComArrear_Clicked(arrear.id, 1);
                            };
                            tapGestureRecognizer.NumberOfTapsRequired = 1;
                            SL_CategoriesList.Children[SL_CategoriesList.Children.Count - 1].GestureRecognizers.Add(tapGestureRecognizer);
                        }

                        arrearsIn++;
                    }
                }
            }
            /// ==========
        }
        /// ==========

        /// SPRAWDZENIE CZY UŻYTKOWNIK ISTNIEJE W BAZIE
        async void checkIn()
        {
            // Jeśli nie ma takiej opcji to utwórz
            if(!Application.Current.Properties.ContainsKey("ID"))
            {
                Application.Current.Properties.Add("ID", 0);
            }

            // Jeśli użytkownika nie ma jeszcze w bazie to utwórz mu rekord
            if(Convert.ToInt32(Application.Current.Properties["ID"]) == 0)
            {
                // Utwórz rekord w bazie danych
                int idBuffer = mySqlConnector.createUser();
                if (idBuffer == 0)
                {
                    CrossToastPopUp.Current.ShowToastError("Wystąpił błąd podczas tworzenia użytkownika w bazie!");
                    userID = 0;
                }
                else
                {
                    Application.Current.Properties["ID"] = idBuffer;
                    Application.Current.SavePropertiesAsync();
                    userID = idBuffer;
                }
            }

            if (Application.Current.Properties.ContainsKey("ID"))
            {
                userID = Convert.ToInt32(Application.Current.Properties["ID"]);
            }
        }
        /// ==========
    }
}
