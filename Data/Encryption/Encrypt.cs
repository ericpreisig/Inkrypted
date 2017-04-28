/*
Author : Eric Preisig
Version : 1.0.0
Date : 06.04.2017
*/

using CsharpAes;
using Data.Entity;
using Inkrypted.Properties;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace Data.Encryption
{
    /// <summary>
    /// Everything about the encryption
    /// </summary>
    public static class Encrypt
    {
        #region Private Fields

        private static string _key;

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Decrypte a string
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string DecryptString(string text)
        {
            try
            {
                return new PasswordAes(_key).Decrypt(text);
            }
            catch (Exception e)
            {
                MessageBox.Show("La clé de déchiffrement est différente de la clé avec laquelle ce ficher a été chiffré");
            }
            return null;
        }

        /// <summary>
        /// Encrypt the string
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string EncryptString(string text)
        {
            try
            {
                return new PasswordAes(_key).Encrypt(text);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            return null;
        }

        /// <summary>
        /// int the encryption key
        /// </summary>
        public static void InitKey()
        {
            var encryptedKey = new Repository<AppSettings>().GetList().FirstOrDefault().Key;
            _key = new PasswordAes(Config.KeyOfTheCryptKey).Decrypt(encryptedKey);
        }

        /// <summary>
        /// Take a random key
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string RandomKey(int size)
        {
            Random rand = new Random();
            string Alphabet = "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            char[] chars = new char[size];
            for (int i = 0; i < size; i++)
                chars[i] = Alphabet[rand.Next(Alphabet.Length)];
            return new string(chars);
        }

        /// <summary>
        /// Set the new encryption key
        /// </summary>
        /// <param name="newKey"></param>
        public static void SetNewKey(string newKey)
        {
            var keyRepository = new Repository<AppSettings>();
            if (keyRepository.GetList().Count == 0)
            {
                keyRepository.Insert(new AppSettings { Key = new PasswordAes(Config.KeyOfTheCryptKey).Encrypt(newKey) });
            }
            else
            {
                var updatedKey = keyRepository.GetList().FirstOrDefault();
                updatedKey.Key = new PasswordAes(Config.KeyOfTheCryptKey).Encrypt(newKey);
                keyRepository.Update(updatedKey);
            }
            _key = newKey;
        }

        #endregion Public Methods
    }
}