/*
Author : Eric Preisig
Version : 1.0.0
Date : 06.04.2017
*/

using Inkrypted.ViewModel;
using System.Windows;

namespace Inkrypted
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Public Constructors

        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion Public Constructors

        #region Private Methods

        /// <summary>
        /// Launch the login popup whenever the window is ready
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = new MainViewModel();
            ((MainViewModel)DataContext).ShowLoginDialog();
        }

        #endregion Private Methods
    }
}