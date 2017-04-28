/*
Author : Eric Preisig
Version : 1.0.0
Date : 06.04.2017
*/

using Data;
using Data.Model;
using GalaSoft.MvvmLight;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Inkrypted.ViewModel
{
    /// <summary>
    /// The software core, contain the two explorers
    /// </summary>
    public class InkryptedViewModel : ViewModelBase
    {
        #region Public Fields

        public static InkryptedViewModel Inkrypted;
        public static bool IsDragging;

        #endregion Public Fields

        #region Private Fields

        private FileExplorerViewModel _encryptedExplorer;

        private bool _isProgressRing;

        private FileExplorerViewModel _normalExplorer;

        private string _progress;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Make the two explorers
        /// </summary>
        public InkryptedViewModel()
        {
            Inkrypted = this;
            NormalExplorer = new FileExplorerViewModel(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
            EncryptedExplorer = new FileExplorerViewModel(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), true);
        }

        #endregion Public Constructors

        #region Public Properties

        public FileExplorerViewModel EncryptedExplorer
        {
            get { return _encryptedExplorer; }
            set
            {
                _encryptedExplorer = value;
                RaisePropertyChanged();
            }
        }

        public bool IsProgressRing
        {
            get { return _isProgressRing; }
            set
            {
                _isProgressRing = value;
                RaisePropertyChanged();
            }
        }

        public FileExplorerViewModel NormalExplorer
        {
            get { return _normalExplorer; }
            set
            {
                _normalExplorer = value;
                RaisePropertyChanged();
            }
        }

        public string Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                RaisePropertyChanged();
            }
        }

        #endregion Public Properties

        #region Public Methods

        private ExplorerElement _elementToDrop;

        /// <summary>
        /// Handle the drag
        /// </summary>
        /// <param name="element"></param>
        public void Drag(List<ExplorerElement> elements)
        {
            Task.Run(() =>
            {
                _elementToDrop = elements.FirstOrDefault();

                //wait that it's actually a true dragging
                Thread.Sleep(150);

                IsDragging = true;

                //if it was a double click, stop it there
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (Mouse.LeftButton == MouseButtonState.Released)
                        IsDragging = false;
                });

                //Check if still Dragging
                while (IsDragging)
                {
                    Thread.Sleep(4);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Mouse.SetCursor(Cursors.Hand);
                        if (Mouse.LeftButton != MouseButtonState.Released) return;

                        IsDragging = false;

                        Mouse.SetCursor(Cursors.AppStarting);
                        IsProgressRing = true;
                        Task.Run(() =>
                        {
                            //if the drop is on the same windows than the file come from
                            if (_elementToDrop != null && _elementToDrop.FromExplorer != FileExplorerViewModel.Hovered)
                                Drop(_elementToDrop);
                            IsProgressRing = false;
                            _elementToDrop.FromExplorer.SelectedItem = null;
                        });
                    });
                }
            });
        }

        /// <summary>
        /// Drop an element on the hovered datagrid
        /// </summary>
        public void Drop(ExplorerElement element)
        {
            if (FileExplorerViewModel.Hovered == null || element == null) return;

            //check if the element isn't his own child
            if (element.IsFolder && Path.GetFullPath(FileExplorerViewModel.Hovered.ActualPath)
                    .StartsWith(Path.GetFullPath(element.Path)))
            {
                Helper.ShowMessage("Erreur", "Ce dossier ne peut pas être son propre enfant", MessageDialogStyle.Affirmative);
                return;
            }

            //Get the new path
            var newPath = FileExplorerViewModel.Hovered.ActualPath + "/" + element.Name;

            try
            {
                //If file dropped in encrypted window
                if (FileExplorerViewModel.Hovered == EncryptedExplorer)
                {
                    element.ProcessEncryption(newPath);
                }
                else if (FileExplorerViewModel.Hovered == NormalExplorer)
                {
                    element.ProcessDecryption(newPath);
                }
            }
            catch (Exception e)
            {
                Helper.ShowMessage("Erreur", e.ToString(), MessageDialogStyle.Affirmative);
                throw;
            }

            //Refresh the explorers
            NormalExplorer.Update();
            EncryptedExplorer.Update();
        }

        #endregion Public Methods
    }
}