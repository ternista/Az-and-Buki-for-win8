﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Шаблон элемента пользовательского элемента управления задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234236

namespace LevelUP
{
    public sealed partial class AutorizationControl : UserControl
    {
        MainMenu rootPage = MainMenu.Current;
        public AutorizationControl()
        {
            this.InitializeComponent();
            var passitemssource1 = new PasswordBoxImageSource("ms-appx:///Assets/CH1.png", "ms-appx:///Assets/CH2.png",
                "ms-appx:///Assets/CH3.png", "ms-appx:///Assets/CH4.png", "ms-appx:///Assets/CH5.png", "ms-appx:///Assets/CH6.png");
            var passitemssource2 = new PasswordBoxImageSource("ms-appx:///Assets/CH1.png", "ms-appx:///Assets/CH2.png",
                "ms-appx:///Assets/CH3.png", "ms-appx:///Assets/CH4.png", "ms-appx:///Assets/CH5.png", "ms-appx:///Assets/CH6.png");
            var passitemssource3 = new PasswordBoxImageSource("ms-appx:///Assets/CH1.png", "ms-appx:///Assets/CH2.png",
                "ms-appx:///Assets/CH3.png", "ms-appx:///Assets/CH4.png", "ms-appx:///Assets/CH5.png", "ms-appx:///Assets/CH6.png");
            var passitemssource4 = new PasswordBoxImageSource("ms-appx:///Assets/CH1.png", "ms-appx:///Assets/CH2.png",
                "ms-appx:///Assets/CH3.png", "ms-appx:///Assets/CH4.png", "ms-appx:///Assets/CH5.png", "ms-appx:///Assets/CH6.png");

            this.PassBox.ApplyTemplate();
            this.PassBox.SMItemSource1 = passitemssource1.Items;
            this.PassBox.SMItemSource2 = passitemssource2.Items;
            this.PassBox.SMItemSource3 = passitemssource3.Items;
            this.PassBox.SMItemSource4 = passitemssource4.Items;
        }


        private async void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            var result = await UserManager.Authorize(tbName.Text,PassBox.Key);
            if (result)
            {
                rootPage.UserLogIn();
                Popup p = this.Parent as Popup;
                p.IsOpen = false; // close the Popup
            }
            else
            {
                var messageDialog = new MessageDialog("Неверный логин или пароль, попробуй еще раз!");

                messageDialog.Commands.Add(new UICommand("Close",
                    new UICommandInvokedHandler(CommandInvokedHandler)));

                messageDialog.DefaultCommandIndex = 0;

                await messageDialog.ShowAsync();
                return;
            }

        }

        private void CommandInvokedHandler(IUICommand command)
        {
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            Popup p = this.Parent as Popup;
            p.IsOpen = false; // close the Popup
        }
    }
}