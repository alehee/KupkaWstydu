using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace KW_XamarinForms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Author : ContentPage
    {
        public Author()
        {
            InitializeComponent();

            // USTAW JAKO AKTUALNY PAGE
            NavigationPage.SetHasNavigationBar(this, false);

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += async (s, e) => {
                await Launcher.OpenAsync(new Uri("https://github.com/alehee"));
            };
            L_Github.GestureRecognizers.Add(tapGestureRecognizer);
            tapGestureRecognizer = new TapGestureRecognizer();

            tapGestureRecognizer.Tapped += async (s, e) => {
                await Launcher.OpenAsync(new Uri("https://www.flaticon.com/authors/smashicons"));
            };
            L_Smashicons.GestureRecognizers.Add(tapGestureRecognizer);
            tapGestureRecognizer = new TapGestureRecognizer();

            tapGestureRecognizer.Tapped += async (s, e) => {
                await Launcher.OpenAsync(new Uri("https://www.flaticon.com"));
            };
            L_Flaticon.GestureRecognizers.Add(tapGestureRecognizer);
        }

        /// POWRÓT NA PROFIL
        async void ReturnButton_Clicked(object sender, EventArgs args)
        {
            App.Current.MainPage = new NavigationPage(new Profile());
        }
        /// ==========
    }
}