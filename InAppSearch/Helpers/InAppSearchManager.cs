using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InAppSearch.Helpers
{
    public class InAppSearchManager : Bases.BinableBase
    {
        private string errorMessage;

        public string ErrorMessage
        {
            get { return errorMessage; }
            set { SetValue(ref errorMessage, value); }
        }

        private const string IndexedFolderName = "Indexed";
        private const string ContentIndexerRevisionKey = "ContentIndexerRevision";

        private async Task<Windows.Storage.IStorageFolder> CreateOrOpenIndexerFolderAsync()
        {
            try
            {
                var local = Windows.Storage.ApplicationData.Current.LocalFolder;
                var indexedFolder = await local.CreateFolderAsync(IndexedFolderName, Windows.Storage.CreationCollisionOption.OpenIfExists);
                return indexedFolder;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                ErrorMessage = ex.Message;
                return null;
            }
        }

        public Windows.Foundation.IAsyncAction InitializeIndexFolderAsync(string metadataFolderName)
        {
            return AsyncInitializeIndexFolder(metadataFolderName).AsAsyncAction();
        }

        private async Task AsyncInitializeIndexFolder(string metadataFolderName)
        {
            try
            {
                var indexedFolder = await CreateOrOpenIndexerFolderAsync();
                var metadataFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(metadataFolderName);
                var queryOptions = new Windows.Storage.Search.QueryOptions(Windows.Storage.Search.CommonFileQuery.DefaultQuery, new string[] { ".appcontent-ms" });
                var metadatas = await metadataFolder.CreateFileQueryWithOptions(queryOptions).GetFilesAsync();
                foreach (var file in metadatas)
                {
                    await file.CopyAsync(indexedFolder, file.Name, Windows.Storage.NameCollisionOption.ReplaceExisting);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                ErrorMessage = ex.Message;
            }
        }

        private void InitializeCacheRevision()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (!localSettings.Values.ContainsKey(ContentIndexerRevisionKey))
            {
                localSettings.Values.Add(ContentIndexerRevisionKey, (ulong)0);
            }
        }

        private void UpdateCacheRevision(ulong? revision)
        {
            if (revision == null)
                return;
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values[ContentIndexerRevisionKey] = revision;
        }

        public Windows.Foundation.IAsyncOperation<bool> PuchItemIntoContentIndexerAsync(Windows.Storage.Search.IIndexableContent item)
        {
            return AsyncPushItemIntoContentIndexer(item).AsAsyncOperation();
        }

        private async Task<bool> AsyncPushItemIntoContentIndexer(Windows.Storage.Search.IIndexableContent item)
        {
            InitializeCacheRevision();
            var indexer = Windows.Storage.Search.ContentIndexer.GetIndexer();
            await indexer.AddAsync(item);
            UpdateCacheRevision(indexer.Revision);
            return true;
        }

        public Windows.Foundation.IAsyncOperation<bool> PuchItemIntoContentIndexerAsync(Windows.Storage.Search.IIndexableContent item, Windows.Storage.Search.ContentIndexer indexer)
        {
            return AsyncPushItemIntoContentIndexer(item, indexer).AsAsyncOperation();
        }

        private async Task<bool> AsyncPushItemIntoContentIndexer(Windows.Storage.Search.IIndexableContent item, Windows.Storage.Search.ContentIndexer indexer)
        {
            InitializeCacheRevision();
            await indexer?.AddAsync(item);
            UpdateCacheRevision(indexer?.Revision);
            return true;
        }

        public Windows.Foundation.IAsyncOperation<IReadOnlyDictionary<string, object>> GetPropertiesAsync(string contentId, IEnumerable<string> properties)
        {
            return AsyncGetProperties(contentId, properties).AsAsyncOperation();
        }

        private async Task<IReadOnlyDictionary<string, object>> AsyncGetProperties(string contentId, IEnumerable<string> properties)
        {
            var indexer = Windows.Storage.Search.ContentIndexer.GetIndexer();
            var resultProperties = await indexer.RetrievePropertiesAsync(contentId, properties);
            return resultProperties;
        }

        public Windows.Foundation.IAsyncOperation<IReadOnlyDictionary<string, object>> GetPropertiesAsync(string contentId, IEnumerable<string> properties, Windows.Storage.Search.ContentIndexer indexer)
        {
            return AsyncGetProperties(contentId, properties, indexer).AsAsyncOperation();
        }

        private async Task<IReadOnlyDictionary<string, object>> AsyncGetProperties(string contentId, IEnumerable<string> properties, Windows.Storage.Search.ContentIndexer indexer)
        {
            var resultProperties = await indexer?.RetrievePropertiesAsync(contentId, properties);
            return resultProperties;
        }
    }
}
