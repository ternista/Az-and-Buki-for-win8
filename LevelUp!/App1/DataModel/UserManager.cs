﻿using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.Security.Cryptography;

namespace LevelUP
{
    public class UserManager
    {
        private bool IsInternetConnection()
        {
            var connectionProfile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
            return (connectionProfile != null);
        }

        UserManager()
        { 
        }

        public async static Task<int> AddUserAsync(User Newby)
        {

            Newby.Hash = ComputeMD5(Newby.Hash);
            var db = new SQLiteAsyncConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, "ABCdb.db"));

            var Result =await db.InsertAsync(Newby);
            if (Result > 0)
            {
                var u = db.QueryAsync<User>("SELECT * FROM User WHERE Name=?", Newby.Name);
                Newby.ID = u.Result[0].ID;
                if (Newby.Avatar != "ms-appx:///Assets/Userlogo.png")
                {

                    Newby.Avatar = String.Concat("Users/UL", Newby.ID.ToString(), ".png");

                    await db.UpdateAsync(Newby);
                }
                

                ApplicationData.Current.LocalSettings.Values["UserName"] = Newby.Name;
                ApplicationData.Current.LocalSettings.Values["UserLogo"] = Newby.Avatar;
                
                return Newby.ID;
            }
            else return -1;
        }

        public async static Task<bool> Authorize(string Name,string Pass)
        {
            var db = new SQLiteAsyncConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, "ABCdb.db"));

            var User = await db.QueryAsync<User>("SELECT * FROM User WHERE Name=?", Name);
            if (String.Compare( ComputeMD5(String.Concat(Name,Pass)), User[0].Hash)!=0)
                return false;
            
            ApplicationData.Current.LocalSettings.Values["UserName"]=Name;
            ApplicationData.Current.LocalSettings.Values["UserLogo"] = User[0].Avatar;

            return true;
            
        }



        public async static Task<bool> IsUniqueLoginAsync(String Login)
        {
            var db = new SQLiteAsyncConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, "ABCdb.db"));
            
            var UQuery = await db.QueryAsync<User>("SELECT * FROM User WHERE Name=?", Login);
            
            return UQuery.Count > 0 ? false : true;
        }

        private static string ComputeMD5(string str)
        {
            var alg = HashAlgorithmProvider.OpenAlgorithm("MD5");
            IBuffer buff = CryptographicBuffer.ConvertStringToBinary(str, BinaryStringEncoding.Utf8);
            var hashed = alg.HashData(buff);
            var res = CryptographicBuffer.EncodeToHexString(hashed);
            return res;
        }
    }
}
