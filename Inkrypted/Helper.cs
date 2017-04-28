/*
Author : Eric Preisig
Version : 1.0.0
Date : 06.04.2017
*/

using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace Inkrypted
{
    public static class Helper
    {
        #region Public Methods

        /// <summary>
        /// Allow to add rang in a observablecollection #not thread safe
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="addedCollection"></param>
        public static void AddRang<T>(this ObservableCollection<T> collection, List<T> addedCollection)
        {
            foreach (var item in addedCollection)
                collection.Add(item);
        }

        /// <summary>
        /// Show a message
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="dialogStyle"></param>
        /// <returns></returns>
        public static Task<MessageDialogResult> ShowMessage(string title, string message, MessageDialogStyle dialogStyle)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var metroWindow = (Application.Current.MainWindow as MetroWindow);
                metroWindow.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Theme;
                return metroWindow.ShowMessageAsync(title, message, dialogStyle, metroWindow.MetroDialogOptions);
            });
            return null;
        }

        #endregion Public Methods
    }
}