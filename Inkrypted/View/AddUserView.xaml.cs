/*
Author : Eric Preisig
Version : 1.0.0
Date : 06.04.2017
*/

using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using System.Windows.Controls;

namespace Inkrypted.View
{
    /// <summary>
    /// Interaction logic for AddUser.xaml
    /// </summary>
    public partial class AddUserView : UserControl
    {
        #region Public Fields

        public static bool UserAdded;

        #endregion Public Fields

        #region Public Constructors

        public AddUserView()
        {
            InitializeComponent();
        }

        #endregion Public Constructors

        #region Private Methods

        /// <summary>
        /// Sign in the user (Breaking the Mvvm pattern for security purpose)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonAddUser_OnClick(object sender, RoutedEventArgs e)
        {
            if (Login.Text == "" || Password.Password == "")
            {
                Helper.ShowMessage("Erreur", "Veuillez remplir tout les champs", MessageDialogStyle.Affirmative);
                return;
            }
            if (Password.Password == Password2.Password)
            {
                if (!Data.Login.IsExistingLogin(Login.Text))
                {
                    UserAdded = true;
                    Data.Login.SignIn(Login.Text, Password.Password);
                    Helper.ShowMessage("Confirmation", "L'utilisateur à bien été ajouté", MessageDialogStyle.Affirmative);
                }
                else
                    Helper.ShowMessage("Erreur", "Un utilisateur avec le même login existe déjà !", MessageDialogStyle.Affirmative);
            }
            else
                Helper.ShowMessage("Erreur", "Les mot de passe ne correspondent pas !", MessageDialogStyle.Affirmative);
        }

        #endregion Private Methods
    }
}