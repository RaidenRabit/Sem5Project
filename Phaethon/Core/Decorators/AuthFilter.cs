﻿using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Core.Model;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Core.Decorators
{
    public class AuthFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            try
            {
                base.OnActionExecuting(actionContext);
                string requestUrl = actionContext.Request.RequestUri.ToString();
                
                if (requestUrl.Contains("Login/Login"))
                    return;

                var userToken = actionContext.Request.Headers.GetValues("UserToken").FirstOrDefault().ToString();
                if (!UtilityMethods.EvaluateToken(userToken))
                    throw new Exception("Not logged in");

                string id = userToken.Remove(userToken.Length - 64);
                actionContext.Request.Headers.Remove("UserToken");
                actionContext.Request.Headers.Add("UserToken", id);
            }
            catch (Exception)
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden);
            }
        }
    }

    public abstract class UtilityMethods
    { 
        public static bool EvaluateToken(string token)
        {
            try
            {
                int id = Int32.Parse(token.Remove(token.Length - 64));
                Login login = GetLogin(id);
                string shaComposed = id + ComputeSha256Hash(Encipher(login.Username + login.Salt, id));
                if(shaComposed.Equals(token))
                    return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private static char Cipher(char ch, int key)
        {
            if (!char.IsLetter(ch))
            {

                return ch;
            }

            char d = char.IsUpper(ch) ? 'A' : 'a';
            return (char)((((ch + key) - d) % 26) + d);
        }


        public static string Encipher(string input, int key)
        {
            string output = string.Empty;

            foreach (char ch in input)
                output += Cipher(ch, key);

            return output;
        }

        private static Login GetLogin(int id)
        {
            using (var db = new DatabaseContext())
            {
                return db.Login.SingleOrDefault(x => x.ID == id);
            }
        }
        
    }
}
