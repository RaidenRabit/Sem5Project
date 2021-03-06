﻿using System;
using System.Security.Cryptography;
using System.Text;
using Core;
using Core.Model;
using InternalApi.DataAccess;
using InternalApi.DataManagement.IDataManagement;

namespace InternalApi.DataManagement
{
    public class LoginDM : ILoginDM
    {
        private readonly LoginDa _LoginDa;

        internal LoginDM()
        {
            _LoginDa = new LoginDa();
        }

        public bool CreateOrUpdate(Login login)
        {
            try
            {
                using (var db = new DatabaseContext())
                {
                    login.Salt = GenerateSalt();
                    login.Password = Convert.ToBase64String(ComputeHMAC_SHA256(Encoding.UTF8.GetBytes(login.Password), login.Salt));
                    _LoginDa.CreateOrUpdate(db, login);
                    return true;
                }
            }

            catch
            {
                return false;
            }
        }

        public bool Delete(int id)
        {
            using (var db = new DatabaseContext())
            {
                Login login = _LoginDa.GetLogin(db, id);
                return _LoginDa.Delete(db, login);
            }
        }

        public Login GetLogin(int id)
        {
            using (var db = new DatabaseContext())
            {
                return _LoginDa.GetLogin(db, id);
            }
        }

        public int Login(string username, string password)
        {
            using (var db = new DatabaseContext())
            {
                Login login = _LoginDa.Login(db, username);
                if (login != null && password != null)
                {
                    if (login.Password.Equals(Convert.ToBase64String(ComputeHMAC_SHA256(Encoding.UTF8.GetBytes(password),
                        login.Salt))))
                    {
                        return login.ID;
                    }
                }
            }
            
            return 0;
        }

        #region Encryption
        private byte[] GenerateSalt()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var randomNumber = new byte[32];
                rng.GetBytes(randomNumber);
                return randomNumber;
            }
        }

        private byte[] ComputeHMAC_SHA256(byte[] data, byte[] salt)
        {
            using (var hmac = new HMACSHA256(salt))
            {
                return hmac.ComputeHash(data);
            }
        }
        #endregion
    }
}