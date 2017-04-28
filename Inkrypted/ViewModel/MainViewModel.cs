/*
Author : Eric Preisig
Version : 1.0.0
Date : 06.04.2017
*/

using Data;
using Data.Encryption;
using Data.Entity;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Inkrypted.Properties;
using Inkrypted.View;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace Inkrypted.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region Public Fields

        public MetroWindow metroWindow = (System.Windows.Application.Current.MainWindow as MetroWindow);

        #endregion Public Fields

        #region Private Fields

        private InkryptedViewModel _inkcryptedViewModel;
        private bool _isAddUserClosable = true;
        private bool _openFlyoutAbout = false;
        private bool _openFlyoutAddUser = false;
        private bool _openFlyoutHelp = false;
        private bool _openFlyoutSettings = false;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            OpenAddUserCommand = new RelayCommand(() => OpenFlyoutAddUser = true);
            OpenFlyoutSettingsCommand = new RelayCommand(() => OpenFlyoutSettings = true);
            OpenFlyoutAboutCommand = new RelayCommand(() => OpenFlyoutAbout = true);
            OpenFlyoutHelpCommand = new RelayCommand(() => OpenFlyoutHelp = true);
            //TODO look to all files
            ChangeEncryptKeyCommand = new RelayCommand(async () =>
            {
                //If the user confirms that he want to rewrite the key
                if (!(System.Windows.Forms.MessageBox.Show("Êtes vous sûr de vouloir modifier la clé de chiffrage ?\nSi vous ne savez pas ce que vous faites, cliquez sur NON !", "Changement de clé de chiffrage", MessageBoxButtons.YesNo) == DialogResult.Yes))
                    return;

                //get all paths
                var listFilePath = new Repository<FileTrack>().GetList();
                listFilePath = listFilePath.Where(a => File.Exists(a.Path)).ToList();

                //Decrypt all files
                foreach (var filePath in listFilePath)
                {
                    Data.Helper.ProcessDecryption(Data.Helper.FileToExplorerFile(filePath.Path), filePath.Path, true);
                }

                //Wait the user to change the key
                await SetEncryptKey();

                //Encrypt all files again with the new key
                foreach (var filePath in listFilePath)
                {
                    var pathWithoutExtension = Data.Helper.WithoutExtension(filePath.Path);
                    Data.Helper.ProcessEncryption(Data.Helper.FileToExplorerFile(pathWithoutExtension), pathWithoutExtension, true);
                }
                OpenFlyoutSettings = false;
            });
        }

        #endregion Public Constructors

        #region Public Properties

        public AddUserViewModel AddUserViewModel { get; set; } = new AddUserViewModel();
        public RelayCommand ChangeEncryptKeyCommand { get; set; }
        public RelayCommand ChangeFolderCommand { get; set; }

        public InkryptedViewModel InkcryptedViewModel
        {
            get { return _inkcryptedViewModel; }
            set
            {
                _inkcryptedViewModel = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand OpenAddUserCommand { get; set; }

        public bool OpenFlyoutAbout
        {
            get { return _openFlyoutAbout; }
            set
            {
                _openFlyoutAbout = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand OpenFlyoutAboutCommand { get; set; }

        public bool OpenFlyoutAddUser
        {
            get { return _openFlyoutAddUser; }
            set
            {
                if (_isAddUserClosable)
                    _openFlyoutAddUser = value;
                RaisePropertyChanged();
            }
        }

        public bool OpenFlyoutHelp
        {
            get { return _openFlyoutHelp; }
            set
            {
                _openFlyoutHelp = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand OpenFlyoutHelpCommand { get; set; }

        public bool OpenFlyoutSettings
        {
            get { return _openFlyoutSettings; }
            set
            {
                _openFlyoutSettings = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand OpenFlyoutSettingsCommand { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Show login message box
        /// </summary>
        public async void ShowLoginDialog()
        {
            //check if there is a last one user
            if (new Repository<User>().GetList().Count == 0)
                await SetFirstUser();

            var settings = new Repository<AppSettings>();

            //Check if there is there is a encrypt key
            if (settings.GetList().Count == 0 || string.IsNullOrEmpty(settings.GetList().FirstOrDefault().Key))
                await SetEncryptKey();

            while (true)
            {
                LoginDialogData result = await metroWindow.ShowLoginAsync("Login", "Connexion", new LoginDialogSettings
                {
                    ColorScheme = MetroDialogColorScheme.Accented,
                    NegativeButtonVisibility = Visibility.Visible,
                    NegativeButtonText = "Fermer",
                    InitialUsername = Settings.Default.LastUserName,
                    PasswordWatermark = "Mot de passe",
                    UsernameWatermark = "Login",
                });

                //if the user cancel
                if (result == null)
                {
                    System.Windows.Application.Current.Shutdown();
                    continue;
                }

                //Check login identity
                if (!Login.CheckUserLogin(result.Username, result.Password))
                {
                    var wrong = await metroWindow.ShowMessageAsync("Erreur", "Login inncorect", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings()
                    {
                        DefaultButtonFocus = MessageDialogResult.Affirmative,
                        NegativeButtonText = "Fermer"
                    });
                    if (wrong == MessageDialogResult.Negative)
                        System.Windows.Application.Current.Shutdown();

                    continue;
                }

                //Set the last name inputed for the next session
                Settings.Default.LastUserName = result.Username;
                Settings.Default.Save();

                //get the key from the database
                Encrypt.InitKey();

                //Load the two explorers
                InkcryptedViewModel = new InkryptedViewModel();
                break;
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Set encryption key
        /// </summary>
        /// <returns></returns>
        private async Task SetEncryptKey()
        {
            //check if there is no key
            while (true)
            {
                var resultKey = await metroWindow.ShowInputAsync("Création de la clé AES", "Vous êtes le premier utilisateur, veuillez créer une clé de chiffrement aléatoire", new MetroDialogSettings()
                {
                    ColorScheme = MetroDialogColorScheme.Accented,
                    NegativeButtonText = "Laisser le logiciel créer la clé ",
                    AffirmativeButtonText = "Créer la clé ",
                    SuppressDefaultResources = true,
                });

                //if user choose to let the progam make the key
                if (resultKey == null)
                {
                    Encrypt.SetNewKey(Encrypt.RandomKey(100));
                    break;
                }
                else
                {
                    if (resultKey.Length >= 5)
                    {
                        Encrypt.SetNewKey(resultKey);
                        break;
                    }
                    else
                        await metroWindow.ShowMessageAsync("Erreur", "La clé doit faire minimum 5 caractères", MessageDialogStyle.Affirmative);
                }
            }
        }

        /// <summary>
        /// Create an user
        /// </summary>
        /// <returns></returns>
        private async Task SetFirstUser()
        {
            OpenFlyoutAddUser = true;
            _isAddUserClosable = false;
            await Task.Run(() => { while (!AddUserView.UserAdded) { Thread.Sleep(10); } });

            _isAddUserClosable = true;
            OpenFlyoutAddUser = false;
        }

        #endregion Private Methods
    }
}