﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Основная страница" см. по адресу http://go.microsoft.com/fwlink/?LinkId=234237

namespace LevelUP
{
    /// <summary>
    /// Основная страница, которая обеспечивает характеристики, являющимися общими для большинства приложений.
    /// </summary>
    public sealed partial class UserSignOnPage : LevelUP.Common.LayoutAwarePage
    {
        String logofilePath;
        public UserSignOnPage()
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

            logofilePath = "ms-appx:///Assets/Userlogo.png";
        }

        /// <summary>
        /// Заполняет страницу содержимым, передаваемым в процессе навигации. Также предоставляется любое сохраненное состояние
        /// при повторном создании страницы из предыдущего сеанса.
        /// </summary>
        /// <param name="navigationParameter">Значение параметра, передаваемое
        /// <see cref="Frame.Navigate(Type, Object)"/> при первоначальном запросе этой страницы.
        /// </param>
        /// <param name="pageState">Словарь состояния, сохраненного данной страницей в ходе предыдущего
        /// сеанса. Это значение будет равно NULL при первом посещении страницы.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Сохраняет состояние, связанное с данной страницей, в случае приостановки приложения или
        /// удаления страницы из кэша навигации. Значения должны соответствовать требованиям сериализации
        /// <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">Пустой словарь, заполняемый сериализуемым состоянием.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        private async void btnOk_Click(object sender, RoutedEventArgs e)
        {
            var uniquelogin = await UserManager.IsUniqueLoginAsync(tbName.Text);
            if (uniquelogin)
            {
                var newUserID = await UserManager.AddUserAsync(new User()
                    {
                        Name = tbName.Text,
                        Avatar = logofilePath,
                        Hash = String.Concat(tbName.Text, PassBox.Key)
                    });

                if (newUserID > 0)
                {
                    if (logofilePath != "ms-appx:///Assets/Userlogo.png")
                    {
                        var file = await StorageFile.GetFileFromPathAsync(logofilePath);

                        await file.RenameAsync(String.Concat("UL", newUserID.ToString(), ".png"));
                    }

                    this.Frame.Navigate(typeof(MainMenu), "Autorized");
                    return;
                }

                var messageDialog = new MessageDialog("Что-то не получилось, попробуй в другой раз");

                messageDialog.Commands.Add(new UICommand("Close",
                    new UICommandInvokedHandler(CommandInvokedHandler)));

                messageDialog.DefaultCommandIndex = 0;

                await messageDialog.ShowAsync();
                return;
            }
            else
            {
                var messageDialog = new MessageDialog("Это имя занято другим пользователем, придумай другое");

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

        private async void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var OpenPicker = new FileOpenPicker();
            OpenPicker.FileTypeFilter.Add(".png");
            OpenPicker.FileTypeFilter.Add(".jpg");
            OpenPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            var file = await OpenPicker.PickSingleFileAsync();
            
            
            if (file != null)
            {
                var folder = StorageFolder.GetFolderFromPathAsync(Path.Combine(ApplicationData.Current.LocalFolder.Path,"Users")).GetResults();
                var logofile = await file.CopyAsync(folder, "UL.png");

                logofilePath = logofile.Path;

                this.imgUserLogo.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(logofile.Path, UriKind.Absolute));
                
            }

        }
    }
}
