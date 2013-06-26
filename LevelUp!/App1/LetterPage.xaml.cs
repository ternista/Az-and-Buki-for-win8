﻿using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;

// Документацию по шаблону элемента "Основная страница" см. по адресу http://go.microsoft.com/fwlink/?LinkId=234237

namespace levelupspace
{

    /// <summary>
    /// Основная страница, которая обеспечивает характеристики, являющимися общими для большинства приложений.
    /// </summary>
    public sealed partial class LetterPage : levelupspace.Common.LayoutAwarePage
    {
        GridView gvWords = null;

        public LetterPage()
        {
            this.InitializeComponent();
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
            if (pageState != null && pageState.ContainsKey("SelectedLetterItem"))
            {
                navigationParameter = pageState["SelectedLetterItem"];
            }

            LetterItem letter = ContentManager.GetLetterItem((String)navigationParameter, DBconnectionPath.Local);
            if (letter != null)
            {
                this.DefaultViewModel["Letters"] = letter.Alphabet.LetterItems;
                this.DefaultViewModel["Words"] = letter.WordItems;
                this.fwLetters.SelectedItem = letter;
            }
        }

        /// <summary>
        /// Сохраняет состояние, связанное с данной страницей, в случае приостановки приложения или
        /// удаления страницы из кэша навигации. Значения должны соответствовать требованиям сериализации
        /// <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">Пустой словарь, заполняемый сериализуемым состоянием.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            var selectedItem = (LetterItem)this.fwLetters.SelectedItem;
            pageState["SelectedLetterItem"] = selectedItem.UniqueId;
        }

        private void fwLetters_SelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            LetterItem letter = ContentManager.GetLetterItem(((LetterItem)this.fwLetters.SelectedItem).UniqueId, DBconnectionPath.Local);
            if (letter != null)
            {

                this.DefaultViewModel["Words"] = letter.WordItems;
            }
        }

        private async void btnPlayLetter_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = (LetterItem)this.fwLetters.SelectedItem;
            MediaElement snd = new MediaElement();
            StorageFile file = await StorageFile.GetFileFromPathAsync(selectedItem.Sound);
            var stream = await file.OpenAsync(FileAccessMode.Read);
            snd.SetSource(stream, file.ContentType);
            snd.Play();
        }

        private async void btnPlayWord_Click(object sender, RoutedEventArgs e)
        {

            var selectedItem = ContentManager.GetWordItem(((Button)sender).Tag as String);
            if (selectedItem.Sound != "none" && selectedItem.Sound != "")
            {
                MediaElement snd = new MediaElement();
                StorageFile file = await StorageFile.GetFileFromPathAsync(selectedItem.Sound);
                var stream = await file.OpenAsync(FileAccessMode.Read);
                snd.SetSource(stream, file.ContentType);
                snd.Play();
            }
        }

        private void gvWords_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            gvWords = sender as GridView;
        }

        private void pageRoot_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs();
            if (e.Key == Windows.System.VirtualKey.Escape) this.GoBack(this, args);
        }

     

    }
}
