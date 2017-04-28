/*
Author : Eric Preisig
Version : 1.0.0
Date : 06.04.2017
*/
/*
Author : Eric Preisig
Version : 1.0.0
Date : 20.03.2017
*/

using Data.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Inkrypted.ViewModel
{
    /// <summary>
    /// File explorer
    /// </summary>
    public class FileExplorerViewModel : ViewModelBase
    {
        #region Public Fields

        public static FileExplorerViewModel Hovered;

        #endregion Public Fields

        #region Private Fields

        private readonly bool _isEncrypted;
        private string _actualPath;
        private string _rootPath;
        private ExplorerElement _selectedItem;

        #endregion Private Fields

        #region Public Constructors

        public FileExplorerViewModel(string path, bool isEncrypted = false)
        {
            _isEncrypted = isEncrypted;
            _rootPath = ActualPath = path;
            OnDoubleClickCommand = new RelayCommand<ExplorerElement>(OnDoubleClick);
            OnBackCommand = new RelayCommand(OnBack);
            SetHoveredContextCommand = new RelayCommand(() => setHovered(this));
            SetNotHoveredContextCommand = new RelayCommand(() => setHovered(null));
            ChangePathCommand = new RelayCommand(() => NewPath(ActualPath));

            ChangeFolder(path);
        }

        /// <summary>
        /// Change the root folder
        /// </summary>
        /// <param name="newRoot"></param>
        public void SetNewRoot(string newRoot)
        {
            _rootPath = newRoot;
            NewPath(newRoot);
        }

        #endregion Public Constructors

        #region Public Properties

        public string ActualPath
        {
            get { return _actualPath; }
            set
            {
                _actualPath = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand ChangePathCommand { get; set; }
        public ObservableCollection<ExplorerElement> ExplorerElements { get; set; } = new ObservableCollection<ExplorerElement>();
        public RelayCommand OnBackCommand { get; set; }
        public RelayCommand<ExplorerElement> OnDoubleClickCommand { get; set; }

        public ExplorerElement SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                if (!InkryptedViewModel.IsDragging)
                    InkryptedViewModel.Inkrypted.Drag(new List<ExplorerElement> { _selectedItem });
                RaisePropertyChanged();
            }
        }

        public RelayCommand<object> SelectItemsCommand { get; set; }
        public RelayCommand SetHoveredContextCommand { get; set; }
        public RelayCommand SetNotHoveredContextCommand { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Change the path of the current folder
        /// </summary>
        /// <param name="path"></param>
        public void NewPath(string path)
        {
            if (!Directory.Exists(path))
            {
                Helper.ShowMessage("Erreur", "Dossier introuvable", MessageDialogStyle.Affirmative);
                return;
            }

            //Clean the view
            ExplorerElements.Clear();

            ActualPath = path;
            ChangeFolder(path);
        }

        /// <summary>
        /// Go on previous folder
        /// </summary>
        public void OnBack()
        {
            var parentFolder = Directory.GetParent(ActualPath);
            if (parentFolder != null)
                NewPath(parentFolder.FullName);
        }

        /// <summary>
        /// Open a folder
        /// </summary>
        /// <param name="element"></param>
        public void OnDoubleClick(ExplorerElement element)
        {
            if (element.IsFolder)
                NewPath(element.Path);
        }

        /// <summary>
        /// When the datagrid is hovered
        /// </summary>
        /// <param name="value"></param>
        public void setHovered(FileExplorerViewModel value)
        {
            Hovered = value;
        }

        /// <summary>
        /// Refresh the view
        /// </summary>
        public void Update()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ExplorerElements.Clear();
                ChangeFolder(ActualPath);
            });
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Load a folder
        /// </summary>
        /// <param name="folderPath"></param>
        private void ChangeFolder(string folderPath)
        {
            try
            {
                var elementList = Data.Helper.GetFilesFromFolder(folderPath, _isEncrypted);

                foreach (var element in elementList)
                    element.FromExplorer = this;

                ExplorerElements.AddRang(elementList);
            }
            catch (Exception e)
            {
                Helper.ShowMessage("Erreur", e.ToString(), MessageDialogStyle.Affirmative);
            }
        }

        #endregion Private Methods
    }
}