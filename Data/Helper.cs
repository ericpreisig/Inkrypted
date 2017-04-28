/*
Author : Eric Preisig
Version : 1.0.0
Date : 06.04.2017
*/

using Data.Encryption;
using Data.Entity;
using Data.Model;
using Inkrypted.Properties;
using Inkrypted.ViewModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Data
{
    /// <summary>
    /// Anything related to the encryption
    /// </summary>
    public static class Helper
    {
        #region Public Methods

        /// <summary>
        /// Transform a path in a ExplorerElement usable by inkripted
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static ExplorerElement FileToExplorerFile(string filePath)
        {
            var isFolder = Directory.Exists(filePath);
            var explorerElement = new ExplorerElement(isFolder) { Name = Path.GetFileName(filePath), Path = filePath };
            if (isFolder)
            {
                DirectoryInfo dirInfos = new DirectoryInfo(filePath);
                explorerElement.Date = dirInfos.CreationTime;
            }
            else
            {
                FileInfo dirInfos = new FileInfo(filePath);
                explorerElement.Date = dirInfos.CreationTime;
            }
            return explorerElement;
        }

        /// <summary>
        /// Get a list of ExplorerElement from a folder path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<ExplorerElement> GetFilesFromFolder(string path, bool isEncrypted = false)
        {
            var fileList = new List<ExplorerElement>();
            var elementsList = new List<string>();
            elementsList.AddRange(Directory.GetDirectories(path));
            elementsList.AddRange(Directory.GetFiles(path));

            foreach (var file in elementsList)
            {
                var data = FileToExplorerFile(file);

                //if encrypted explorer, only show .ink files
                if (isEncrypted && data.IsFolder == false && Path.GetExtension(data.Name) != Config.EncryptedExtension) continue;

                //if not encrypted show only none ink files
                if (!isEncrypted && data.IsFolder == false && Path.GetExtension(data.Name) == Config.EncryptedExtension) continue;

                fileList.Add(data);
            }
            return fileList;
        }

        /// <summary>
        /// Look by the extension if file is encrypted
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsFileEncryped(string path) => Path.GetExtension(path) == Config.EncryptedExtension;

        /// <summary>
        /// Decrypte every element given (Explorer all folders recursive)
        /// </summary>
        /// <param name="element"></param>
        /// <param name="newPath"></param>
        /// <param name="soft">True to not delete the folder after the decryption</param>
        public static void ProcessDecryption(this ExplorerElement element, string newPath, bool soft = false)
        {
            if (element == null) return;
            //If it's a folder, make it recursive
            if (element.IsFolder)
            {
                element.ProcessDecryptionFolder(newPath, soft);
                return;
            }
            if (!IsFileEncryped(element.Name)) return;
            try
            {
                if (!File.Exists(element.Path)) return;
                Task.Run(() => InkryptedViewModel.Inkrypted.Progress = element.Name);
                string path = WithoutExtension(newPath);

                //Get content file
                string base64 = File.ReadAllText(element.Path);

                //decrypt the file
                var secret = Encrypt.DecryptString(base64);
                if (File.Exists(path)) return;

                //create new file at path
                File.WriteAllBytes(path, Convert.FromBase64String(secret));

                //delete old file
                File.Delete(element.Path);

                //remove tracking from db
                var repository = new Repository<FileTrack>();
                repository.Delete(repository.GetList().FirstOrDefault(a => Path.GetFullPath(a.Path) == Path.GetFullPath(element.Path)));
            }
            catch (Exception e)
            {
                Inkrypted.Helper.ShowMessage("Erreur", "Déchiffrement impossible", MessageDialogStyle.Affirmative);
            }
        }

        /// <summary>
        /// crypte every element given (Explorer all folders recursive)
        /// </summary>
        /// <param name="element"></param>
        /// <param name="newPath"></param>
        /// <param name="soft">True to not delete the folder after the cryption</param>
        public static void ProcessEncryption(this ExplorerElement element, string newPath, bool soft = false)
        {
            if (element == null) return;
            //If it's a folder, make it recursive

            if (element.IsFolder)
            {
                element.ProcessEncryptionFolder(newPath, soft);
                return;
            }
            if (IsFileEncryped(element.Name)) return;
            try
            {
                if (!File.Exists(element.Path)) return;
                Task.Run(() => InkryptedViewModel.Inkrypted.Progress = element.Name);
                string path = newPath + Config.EncryptedExtension;

                //Get content file
                string base64 = Convert.ToBase64String(File.ReadAllBytes(element.Path));
                var secret = Encrypt.EncryptString(base64);
                if (File.Exists(path)) return;

                //create new file
                File.WriteAllText(path, secret);

                //delete old one
                File.Delete(element.Path);

                //add tracking to db
                var repository = new Repository<FileTrack>();
                repository.Insert(new FileTrack { Path = path });
            }
            catch (Exception e)
            {
                Inkrypted.Helper.ShowMessage("Erreur", e.ToString(), MessageDialogStyle.Affirmative);
                throw;
            }
        }

        /// <summary>
        /// Remove the extension of file if it's the encrypted extension
        /// </summary>
        /// <param name="path"></param>
        public static string WithoutExtension(string path) => path.Replace(Config.EncryptedExtension, "");

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Decrypt a fodler
        /// </summary>
        /// <param name="element"></param>
        /// <param name="newPath"></param>
        /// <param name="soft">don't remove after</param>
        private static void ProcessDecryptionFolder(this ExplorerElement element, string newPath, bool soft = false)
        {
            if (element == null || !element.IsFolder) return;

            //if folder don't exist, create a new one
            if (!Directory.Exists(newPath)) Directory.CreateDirectory(newPath);

            foreach (var file in GetFilesFromFolder(element.Path, true))
                file.ProcessDecryption(newPath + "/" + file.Name, soft);

            //Wait until all files are done
            while (GetFilesFromFolder(element.Path, true).Any()) ;

            //if the folder is empty, and soft isn't true, delete empty folder
            if (!soft && !Directory.EnumerateFileSystemEntries(element.Path).Any())
                Directory.Delete(element.Path);
        }

        /// <summary>
        /// crypt a fodler
        /// </summary>
        /// <param name="element"></param>
        /// <param name="newPath"></param>
        /// <param name="soft">don't remove after</param>
        private static void ProcessEncryptionFolder(this ExplorerElement element, string newPath, bool soft = false)
        {
            if (element == null || !element.IsFolder) return;

            //if folder don't exist, create a new one
            if (!Directory.Exists(newPath)) Directory.CreateDirectory(newPath);

            foreach (var file in GetFilesFromFolder(element.Path))
                file.ProcessEncryption(newPath + "/" + file.Name, soft);

            if (!Directory.Exists(element.Path)) return;

            //Wait until all files are done
            while (GetFilesFromFolder(element.Path).Any()) ;

            //if the folder is empty, and soft isn't true, delete empty folder
            if (!soft && !Directory.EnumerateFileSystemEntries(element.Path).Any())
                Directory.Delete(element.Path);
        }

        #endregion Private Methods
    }
}