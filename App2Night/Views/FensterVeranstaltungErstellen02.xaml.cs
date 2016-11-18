﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using App2Night.ModelsEnums.Model;
using App2Night.ModelsEnums.Enums;

// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace App2Night.Views
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class FensterVeranstaltungErstellen02 : Page
    {
        Party partyZuErstellen = new Party();

        public FensterVeranstaltungErstellen02()
        {
            //DateTimeOffset aktuellesJahr = DateTime.Today.AddYears(0);
            //DateTimeOffset aktuellesJahrPlusEins = DateTime.Today.AddYears(1);

            this.InitializeComponent();

            // MusicGenres in ComboBox anzeigen
            comboBoxErstellenMUSIKRICHTUNG.ItemsSource = Enum.GetValues(typeof(MusicGenre));

        }

        protected async void OnNavigatedTo(NavigationEventArgs e)
        {
            partyZuErstellen = e.Parameter as Party;
        }

        private void btnZurueck_wechselZuErstellen01(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(FensterVeranstaltungErstellen));
        }

        private void btnErstellen_wechselZuAnzeige(object sender, RoutedEventArgs e)
        {
            partyZuErstellen.MusicGenre = (MusicGenre)comboBoxErstellenMUSIKRICHTUNG.SelectedItem;
            // TODO: Maximale Zeichenzahl beachten
            partyZuErstellen.Description = textBoxErstellenWEITEREINFOS.Text;

            this.Frame.Navigate(typeof(FensterVeranstaltungAnzeigen));
        }

        

    }
}
