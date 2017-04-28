/*
Author : Eric Preisig
Version : 1.0.0
Date : 06.04.2017
*/

using Inkrypted.ViewModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Inkrypted.View
{
    /// <summary>
    /// Interaction logic for ExplorerView.xaml
    /// </summary>
    public partial class FileExplorerView
    {
        #region Public Constructors

        public FileExplorerView()
        {
            InitializeComponent();
        }

        #endregion Public Constructors

        #region Private Methods

        /// <summary>
        /// When a drag event is fired
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileExplorer_OnDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effects = DragDropEffects.Copy;
        }

        /// <summary>
        /// Get dropend element
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileExplorer_OnDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files == null) return;
            InkryptedViewModel.Inkrypted.IsProgressRing = true;
            Task.Run(() =>
            {
                foreach (var file in files) InkryptedViewModel.Inkrypted.Drop(Data.Helper.FileToExplorerFile(file));
                InkryptedViewModel.Inkrypted.IsProgressRing = false;
            });
        }

        #endregion Private Methods

        #region Private Fields

        private int _indexSelectedItem;

        #endregion Private Fields

        private void FileExplorer_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if not dragging, set the index to the selected item
            if (!InkryptedViewModel.IsDragging) _indexSelectedItem = FileExplorer.SelectedIndex;

            //if dragging,block the selcted item to be changed
            if (InkryptedViewModel.IsDragging) FileExplorer.SelectedIndex = _indexSelectedItem;
        }
    }
}