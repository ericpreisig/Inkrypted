/*
Author : Eric Preisig
Version : 1.0.0
Date : 06.04.2017
*/

using Inkrypted.ViewModel;
using System;

namespace Data.Model
{
    public class ExplorerElement
    {
        #region Public Constructors

        /// <summary>
        /// Get the good image (file or folder)
        /// </summary>
        /// <param name="isFolder"></param>
        public ExplorerElement(bool isFolder)
        {
            IsFolder = isFolder;
            Image = isFolder ? "/Resources/Images/folder.png" : "/Resources/Images/file.png";
        }

        #endregion Public Constructors

        #region Public Properties

        public DateTime Date { get; set; }
        public FileExplorerViewModel FromExplorer { get; set; }
        public string Image { get; }
        public bool IsFolder { get; }
        public string Name { get; set; }
        public string Path { get; set; }

        #endregion Public Properties
    }
}