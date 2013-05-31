﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Storage;
using Windows.Storage.Streams;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.MobileServices;

namespace levelupspace.DataModel
{
    class AzureDBProvider
    {

        #region MobileService connection
        public static MobileServiceClient MobileService = new MobileServiceClient(
        "https://levelupbackend.azure-mobile.net/",
        "hGDmdRlTFfjhgwkOzAAICUBJliMLzn13"
        );
        #endregion

        #region MobileService Table providers

        //private static MobileServiceCollectionView<Alphabet> AlphabetItems;
        private static IMobileServiceTable<Alphabet> AlphabetTable =
            MobileService.GetTable<Alphabet>();

        //private static MobileServiceCollectionView<AlphabetLocalization> AlphabetLocalizationtItems;
        private static IMobileServiceTable<AlphabetLocalization> AlphabetLocalizationTable =
            MobileService.GetTable<AlphabetLocalization>();

        //private static MobileServiceCollectionView<User> UserItems;
        private static IMobileServiceTable<User> UserTable =
            MobileService.GetTable<User>();

        //private static MobileServiceCollectionView<UserAward> UserAwardItems;
        private static IMobileServiceTable<UserAward> UserAwardTable =
            MobileService.GetTable<UserAward>();

        //private static MobileServiceCollectionView<Award> AwardItems;
        private static IMobileServiceTable<Award> AwardTable =
            MobileService.GetTable<Award>();

        //private static MobileServiceCollectionView<AwardLocalization> AwardLocalizationItems;
        private static IMobileServiceTable<AwardLocalization> AwardLocalizationTable =
            MobileService.GetTable<AwardLocalization>();

        //AlphabetLocalization

        #endregion

        #region Users data providing methods

        public async static void AddNewUser(User user)
        {
            //// This code inserts a new TodoItem into the database. When the operation completes
            //// and Mobile Services has assigned an Id, the item is added to the CollectionView
            await UserTable .InsertAsync(user);
        }

        public async static Task<bool> CheckThisLogin(string Name)
        {
            UserTable = MobileService.GetTable<User>();
            List<User> list = await UserTable.Where(user => user.Name == Name).ToListAsync();
            return list.Count > 0;
        }


        #endregion

        #region Packages data providing methods

        public async static Task<List<Alphabet>> GetAllPackages(User user)
        {
            //// This code inserts a new TodoItem into the database. When the operation completes
            //// and Mobile Services has assigned an Id, the item is added to the CollectionView
            AlphabetTable = MobileService.GetTable<Alphabet>();
            return await AlphabetTable.ToListAsync();
        }

        public async static Task<List<AlphabetLocalization>> GetPackageLocalization(Alphabet alphabet, String LocalizationId)
        {
            AlphabetLocalizationTable = MobileService.GetTable<AlphabetLocalization>();
            return await AlphabetLocalizationTable.Where(local => local.AlphabetID == alphabet.ID && local.LanguageID == LocalizationId).ToListAsync();
        }

        #endregion

        #region Awards data providing methods

        public async static void AddAwardToUser(User user, Award award)
        {
            //// This code inserts a new TodoItem into the database. When the operation completes
            //// and Mobile Services has assigned an Id, the item is added to the CollectionView
            UserAward newAward = new UserAward() { AwardID = award.ID, UserID = user.ID };
            await UserAwardTable.InsertAsync(newAward);
        }

        public async static Task<List<UserAward>> GetAwardsOfThisUser(User user)
        {
            UserAwardTable = MobileService.GetTable<UserAward>();
            return await UserAwardTable.Where(u => u.UserID == user.ID).ToListAsync();
        }

        public async static Task<List<Award>> GetAllAwards(User user)
        {
            AwardTable = MobileService.GetTable<Award>();
            return await AwardTable.ToListAsync();
        }

        public async static Task<List<AwardLocalization>> GetAwardLocalization(Award award, String LocalizationId)
        {
            AwardLocalizationTable = MobileService.GetTable<AwardLocalization>();
            return await AwardLocalizationTable.Where(local => local.AwardId == award.ID && local.LanguageID == LocalizationId ).ToListAsync();
        }

        #endregion

    }

    class AzureStorageProvider
    {
        private static CloudStorageAccount storageAccount = 
            CloudStorageAccount.Parse("DefaultEndpointsProtocol=http;AccountName=[levelupstorage];AccountKey=[k9OkEg5CQHVD415z+s8xD/zx4lCKyWdBgWrDxqUnCsVbohmxgYVUUs8q4ZknpJdpgOEikk0damf2/lTSksSTZg==");

        public static CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

        public static async void UploadAvatarToStorage(StorageFile file, String username)
        {

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("userpic");

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(username);

            // Create or overwrite the "myblob" blob with contents from a local file.

            var stream = await file.OpenAsync(FileAccessMode.Read);

            await blockBlob.UploadFromStreamAsync(stream.GetInputStreamAt(0));
            
        }

        public static async void DownloadPackageFromStorage(StorageFile file, String packageName)
        {

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("userpic");

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(packageName);
            // Create or overwrite the "myblob" blob with contents from a local file.
            using (var fileStream = await file.OpenStreamForWriteAsync())
            {
                await blockBlob.DownloadToStreamAsync(fileStream.AsOutputStream());
            }

        }

        public static async void DownloadAvatarFromStorage(StorageFile file, String userID)
        {

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("userpic");

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("UL" + userID);
            // Create or overwrite the "myblob" blob with contents from a local file.
            using (var fileStream = await file.OpenStreamForWriteAsync())
            {
                await blockBlob.DownloadToStreamAsync(fileStream.AsOutputStream());
            }

        }

    }

}