﻿using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using App2Night.ModelsEnums.Model;
using App2Night.ModelsEnums.Enums;
using App2Night.Logik;
using Windows.UI.Popups;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI;
using App2Night.Ressources;
using Windows.UI.Xaml.Media.Imaging;

namespace App2Night.Views
{
    /// <summary>
    /// Diese Seite zeigt alle Daten zur einer gewählten Party an. 
    /// Hier kann man Teilnehmen/Voten und der Ersteller der Party kann diese auch bearbeiten. 
    /// </summary>
    public sealed partial class FensterVeranstaltungAnzeigen : Page
    {
        public Party uebergebeneParty = new Party();

        public FensterVeranstaltungAnzeigen()
        {
            this.InitializeComponent();
            ProgRingAnzeigen.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Anzeigen der Daten der Party.
        /// Anpassen der Ansicht abhängig vom Nutzer, Stand der Vormerkung und Teilnahme und der Position der Party.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            uebergebeneParty = e.Parameter as Party;

            // Auslesen und Anzeigen aller Daten der übergebenen Party
            DateTime partyDatumUhrzeit = uebergebeneParty.PartyDate;
            DateTime partyDatum = partyDatumUhrzeit.Date;
            TimeSpan partyUhrzeit = partyDatumUhrzeit.TimeOfDay;
            
            textBlVeranstAnzeigenNAME.Text = uebergebeneParty.PartyName;
            textBoxAnzeigenDATUM.Text = partyDatum.ToString("dd/MM/yyyy");
            textBoxAnzeigenUHRZEIT.Text = partyDatumUhrzeit.ToString("HH:mm");
            textBoxAnzeigenORT.Text = uebergebeneParty.Location.CityName;
            textBoxAnzeigenMUSIKRICHTUNG.Text = uebergebeneParty.MusicGenre.ToString();
            textBoxAnzeigenWeitereINFOS.Text = uebergebeneParty.Description;
            textBoxAnzahlUPVOTES.Text = uebergebeneParty.GeneralUpVoting.ToString();
            textBoxAnzahlDOWNVOTES.Text = uebergebeneParty.GeneralDownVoting.ToString();
            
            // Farbliche Hervorhebung der Upvotes
            if (uebergebeneParty.GeneralUpVoting != 0)
            {
                textBoxAnzahlUPVOTES.Foreground = new SolidColorBrush(Colors.Green);
            }
            else
            {
                textBoxAnzahlUPVOTES.Foreground = new SolidColorBrush(Colors.Black);
            }

            // Farbliche Hervorhebung der Downvotes
            if (uebergebeneParty.GeneralUpVoting != 0)
            {
                textBoxAnzahlUPVOTES.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                textBoxAnzahlUPVOTES.Foreground = new SolidColorBrush(Colors.Black);
            }

            // Falls der aktuelle Nutzer der Ersteller der Party ist, wird ihm der Button um zur Bearbeitung zu wechseln angezeigt.
            if (uebergebeneParty.HostedByUser == true)
            {
                appBarButtonBearbeiten.Visibility = Visibility.Visible;
                appBarButtonLoeschen.Visibility = Visibility.Visible;
                textBlVeranstAnzeigenNAME.Width = 245;
            }
            else
            {
                appBarButtonBearbeiten.Visibility = Visibility.Collapsed;
                appBarButtonLoeschen.Visibility = Visibility.Collapsed;
                textBlVeranstAnzeigenNAME.Width = 303;
            }

            // Einstellen der Anzeige abhängig vom Status der Vormerkung
            if (uebergebeneParty.UserCommitmentState != EventCommitmentState.Noted)
            {
                AppBarButtonVormerken.Icon = new SymbolIcon(Symbol.OutlineStar);
                AppBarButtonVormerken.Label = "Vormerken";
            }
            else
            {
                AppBarButtonVormerken.Icon = new SymbolIcon(Symbol.Favorite);
                AppBarButtonVormerken.Label = "Nicht vormerken";
            }

            // Anzeigen der Teilnahme/Absage abhängig von Zu-/Absage
            if (uebergebeneParty.UserCommitmentState != EventCommitmentState.Accepted)
            {
                // Teilnehmen kann man, wenn der aktuelle Status zur Party Rejected oder Noted ist
                appBarButtonTeilnehmen.Icon = new SymbolIcon(Symbol.Audio); ;
                appBarButtonTeilnehmen.Label = "Teilnehmen";

            }
            else
            {
                appBarButtonTeilnehmen.Icon = new SymbolIcon(Symbol.Undo); ;
                appBarButtonTeilnehmen.Label = "Absagen";
            }

            // Party auf Karte anzeigen
            MapPartyAnzeigen(uebergebeneParty);
        }

        /// <summary>
        /// Zeigt die Party mittig auf der Karte an.
        /// </summary>
        /// <param name="uebergebeneParty"></param>
        private void MapPartyAnzeigen(Party uebergebeneParty)
        {
            // Festlegen der Position
            BasicGeoposition partyPosition = new BasicGeoposition() { Latitude = uebergebeneParty.Location.Latitude, Longitude = uebergebeneParty.Location.Longitude };
            Geopoint partyZentrum = new Geopoint(partyPosition);

            // Festlegen des Mittelpunkts
            mapControlKarte.Center = partyZentrum;
            mapControlKarte.ZoomLevel = 15;
            mapControlKarte.LandmarksVisible = true;

            // Icon für Standort Party
            MapIcon partyIcon = new MapIcon();
            // TODO: Icon in Map
            //partyIcon.Title = uebergebeneParty.PartyName;
            //var logo = new BitmapIcon();
            //logo.UriSource = new Uri("../Assets/Square150x150Logo.scale-400.png", UriKind.Relative);
            //partyIcon.Image = RandomAccessStreamReference.CreateFromUri(logo.UriSource);

            mapControlKarte.MapElements.Add(partyIcon);
        }

        /// <summary>
        /// Einfacher Wechsel zur Hauptansicht.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Zurueck_wechselZuHauptansicht(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(FensterHauptansicht));
        }

        /// <summary>
        /// Hier wird die Party für den Nutzer vorgemerkt/nicht mehr vorgemerkt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Vormerken_wechselZuHauptansicht(object sender, RoutedEventArgs e)
        {
            CommitmentParty commitment = new CommitmentParty();
            bool notiert = false;

            // Sperren der Oberfläche
            this.IsEnabled = false;
            ProgRingAnzeigen.Visibility = Visibility.Visible;

            // Hier wird der Status der Vormerkung notiert
            if (uebergebeneParty.UserCommitmentState != EventCommitmentState.Noted)
            {
                commitment.Teilnahme = EventCommitmentState.Noted;

                notiert = true;
            }
            else
            {
                commitment.Teilnahme = EventCommitmentState.Rejected;

                notiert = false;
            }

            // Vormerkung ans BackEnd schicken
            bool antwort = await BackEndComPartyLogik.PutPartyCommitmentState(uebergebeneParty, commitment);

            // Entsperrung der Oberfläche
            ProgRingAnzeigen.Visibility = Visibility.Collapsed;
            this.IsEnabled = true;


            if (antwort == true)
            {
                // Abhängig davon, ob der Nutzer die Party vormerken will oder nicht, wird ihm eine entsprechende Nachricht ausgegeben.
                if (notiert == true)
                {
                    var message = new MessageDialog(Meldungen.Anzeige.ErfolgVormerken, "Erfolg!");
                    await message.ShowAsync();
                    uebergebeneParty.UserCommitmentState = EventCommitmentState.Noted; 
                }
                else
                {
                    var message = new MessageDialog(Meldungen.Anzeige.ErfolgVergessen, "Erfolg!");
                    await message.ShowAsync();
                    uebergebeneParty.UserCommitmentState = EventCommitmentState.Rejected;
                }
            }
            else
            {
                var message = new MessageDialog(Meldungen.Anzeige.FehlerVormerkenVergessen, "Fehler!");
                await message.ShowAsync();
            }

            // Zum Schluss wechselt man zur Hauptansicht
            this.Frame.Navigate(typeof(FensterHauptansicht));
        }

        /// <summary>
        /// Einfacher Wechsel zum Bearbeiten der Party. Die zu bearbeitende Party wird mitgegeben. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bearbeiten_wechselZuErstellen(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(FensterErstellen), uebergebeneParty);
        }

        /// <summary>
        /// Hier wird die Teilnahme/Absage für die Party vorgenommen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Teilnehmen_CommitmentStateSetzen(object sender, RoutedEventArgs e)
        {
            CommitmentParty teilnehmen = new CommitmentParty();
            bool zusagen = false;

            // Hier wird der Status der Teilnahme notiert
            if (uebergebeneParty.UserCommitmentState != EventCommitmentState.Accepted) 
            {          
                teilnehmen.Teilnahme = EventCommitmentState.Accepted;

                zusagen = true;
            }
            else
            {
                teilnehmen.Teilnahme = EventCommitmentState.Rejected;

                zusagen = false;
            }

            // Sperren der Oberfläche
            this.IsEnabled = false;
            ProgRingAnzeigen.Visibility = Visibility.Visible;

            // Teilnahme/Absage ans BackEnd schicken
            bool teilnahme = await BackEndComPartyLogik.PutPartyCommitmentState(uebergebeneParty, teilnehmen);

            if (teilnahme == true)
            {
                // Abhängig davon, ob der Nutzer teilnehmen oder absagen will, wird ihm eine entsprechende Nachricht ausgegeben.
                if (zusagen == true)
                {
                    var message = new MessageDialog(Meldungen.Anzeige.ErfolgTeilnahme, "Viel Spaß!");
                    await message.ShowAsync();
                    uebergebeneParty.UserCommitmentState = EventCommitmentState.Accepted;
                    this.Frame.Navigate(typeof(FensterHauptansicht));
                }
                else
                {
                    var message = new MessageDialog(Meldungen.Anzeige.ErfolgAbsage, "Schade!");
                    await message.ShowAsync();
                    uebergebeneParty.UserCommitmentState = EventCommitmentState.Rejected;
                }
            }
            else
            {
                var message = new MessageDialog(Meldungen.Anzeige.FehlerTeilnahmeAbsage, "Fehler!");
                await message.ShowAsync();

            }

            // Entsperren der Oberfläche
            ProgRingAnzeigen.Visibility = Visibility.Collapsed;
            this.IsEnabled = true;

            // Wechsel zur Hauptansicht
            this.Frame.Navigate(typeof(FensterHauptansicht));
        }

        /// <summary>
        /// Gibt dem Nutzer die Möglichkeit seine erstelle Party zu löschen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Loeschen_PartyLoeschen(object sender, RoutedEventArgs e)
        {
            // Teile übernommen von http://stackoverflow.com/questions/35392306/does-the-messagedialog-class-for-uwp-apps-support-three-buttons-on-mobile  
            const int OK = 1;
            const int ABBR = 2;

            // Dialog, der abfragt, ob der Nutzer die Party wirklich löschen will
            var message = new MessageDialog(Meldungen.Anzeige.AbsicherungLoeschen, "Achtung!");
            message.Commands.Add(new UICommand { Label = "Ja", Id = OK });
            message.Commands.Add(new UICommand { Label = "Abbrechen", Id = ABBR });
            var reaktion = await message.ShowAsync();

            var id = (int)(reaktion?.Id ?? ABBR);

            // Falls der Nutzer bestätigt, dass er die Party löschen will
            if (id == 1)
            {
                this.IsEnabled = false;
                ProgRingAnzeigen.Visibility = Visibility.Visible;

                bool erfolg = await BackEndComPartyLogik.DeletePartyByID(uebergebeneParty);

                ProgRingAnzeigen.Visibility = Visibility.Collapsed;
                this.IsEnabled = true;

                if (erfolg == true)
                {
                    message = new MessageDialog(Meldungen.Anzeige.ErfolgLoeschen, "Erfolg!");
                    await message.ShowAsync();
                }
                else
                {
                    message = new MessageDialog(Meldungen.Anzeige.MisserfolgLoeschen, "Fehler!");
                    await message.ShowAsync();
                }
            }
            else
            {
                message = new MessageDialog(Meldungen.Anzeige.AbbrechenLoeschen, "Abbrechen");
                await message.ShowAsync();
            }

        }
    }
}
