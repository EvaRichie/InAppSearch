using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace InAppSearch.ViewModels
{
    // https://blogs.msdn.microsoft.com/adamdwilson/2016/03/30/searching-private-app-data-in-windows-10/
    // https://msdn.microsoft.com/en-us/library/windows/apps/dn575215.aspx
    // https://msdn.microsoft.com/en-us/library/windows/desktop/dd561977(v=vs.85).aspx

    public class MainPageViewModel : Bases.BinableBase
    {
        private string pageTitle;
        public string PageTitle
        {
            get { return pageTitle; }
            set { SetValue(ref pageTitle, value); }
        }

        private ObservableCollection<string> settingsCollection;
        public ObservableCollection<string> SettingsCollection
        {
            get { return settingsCollection; }
            set { settingsCollection = value; }
        }

        private string queryText;
        public string QueryText
        {
            get { return queryText; }
            set { SetValue(ref queryText, value); }
        }

        private string outputStr;
        public string OutputStr
        {
            get { return outputStr; }
            set { SetValue(ref outputStr, value); }
        }

        private const string IndexedFolderName = "Indexed";
        private const string ContenxtIndexerRevisionKey = "CIRevision";
        public ICommand CreateIndexedFolderCmd { get; private set; }
        public ICommand CreateContentIndexerCmd { get; private set; }

        public MainPageViewModel()
        {
            QueryText = string.Empty;
            PageTitle = "MainPage";
            settingsCollection = new ObservableCollection<string>();

            CreateIndexedFolderCmd = new Commands.RelayCommand(async () =>
            {
                await InitializeIndexedSearchAsync();
                await QueryTextAsync();
            });

            CreateContentIndexerCmd = new Commands.RelayCommand(async () =>
            {
                await CreateSampleIndexableContentAsync();
            });
        }

        private async Task InitializeIndexedSearchAsync()
        {

            var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var indexedFolder = await localFolder.CreateFolderAsync(IndexedFolderName, Windows.Storage.CreationCollisionOption.OpenIfExists);
            var queryOptions = new Windows.Storage.Search.QueryOptions(Windows.Storage.Search.CommonFileQuery.DefaultQuery, new string[] { ".appcontent-ms" });
            var settingsFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Settings");
            var settings = await settingsFolder.CreateFileQueryWithOptions(queryOptions).GetFilesAsync();
            foreach (var file in settings)
            {
                await file.CopyAsync(indexedFolder, file.Name, Windows.Storage.NameCollisionOption.ReplaceExisting);
            }
        }

        private async Task<Windows.Storage.Search.StorageFileQueryResult> CreateIndexedFolderQuery()
        {
            var indexedFolder = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFolderAsync(IndexedFolderName);
            var queryOptions = new Windows.Storage.Search.QueryOptions(Windows.Storage.Search.CommonFileQuery.OrderBySearchRank, new string[] { ".appcontent-ms" });
            queryOptions.Language = "en-us";
            queryOptions.IndexerOption = Windows.Storage.Search.IndexerOption.OnlyUseIndexer;
            var result = indexedFolder.CreateFileQueryWithOptions(queryOptions);
            return result;
        }

        public async void TextChangedHandler(object sender, object args)
        {
            await QueryTextAsync();
        }

        private async Task QueryTextAsync()
        {
            settingsCollection?.Clear();
            var indexedFolder = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFolderAsync(IndexedFolderName);
            var queryOptions = new Windows.Storage.Search.QueryOptions(Windows.Storage.Search.CommonFileQuery.DefaultQuery, new string[] { ".appcontent-ms" }) { IndexerOption = Windows.Storage.Search.IndexerOption.OnlyUseIndexer, ApplicationSearchFilter = QueryText };
            var queryFiles = await indexedFolder.CreateFileQueryWithOptions(queryOptions).GetFilesAsync();
            foreach (var file in queryFiles)
            {
                var fileProp = await file.Properties.RetrievePropertiesAsync(new string[] { Windows.Storage.SystemProperties.Comment, Windows.Storage.SystemProperties.Title, Windows.Storage.SystemProperties.Keywords });
                if (!string.IsNullOrEmpty(QueryText) || !string.IsNullOrWhiteSpace(QueryText))
                {
                    OutputStr += $"Query text : {QueryText}\n";
                    OutputStr += $"Title : {fileProp[Windows.Storage.SystemProperties.Title]}\n";
                    OutputStr += $"Comment : {fileProp[Windows.Storage.SystemProperties.Comment]}\n";
                    var test = (fileProp[Windows.Storage.SystemProperties.Keywords] as IEnumerable<string>);
                    foreach (var k in (string[])fileProp[Windows.Storage.SystemProperties.Keywords])
                        OutputStr += $"Keyword : {k}\n";
                    OutputStr += "=====\n";
                }
                settingsCollection?.Add($"{fileProp[Windows.Storage.SystemProperties.Comment]}");
            }
        }

        private async Task CreateSampleIndexableContentAsync()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (!localSettings.Values.ContainsKey(ContenxtIndexerRevisionKey))
                localSettings.Values.Add(ContenxtIndexerRevisionKey, (ulong)0);
            var indexer = Windows.Storage.Search.ContentIndexer.GetIndexer();
            var content = new Windows.Storage.Search.IndexableContent();
            for (var i = 0; i < 3; i++)
            {
                content.Id = $"CID_{i}";
                content.Properties[Windows.Storage.SystemProperties.ItemNameDisplay] = $"DisplayName_{i}";
                content.Properties[Windows.Storage.SystemProperties.Comment] = $"DisplayComment_{i}";
                content.Properties[Windows.Storage.SystemProperties.Title] = $"DisplayTitle_{i}";
                await indexer.AddAsync(content);
                localSettings.Values[ContenxtIndexerRevisionKey] = indexer.Revision;
            }

            var query = indexer.CreateQuery("*", new string[] { Windows.Storage.SystemProperties.Comment });
            var result = await query.GetAsync();
            foreach (var c in result)
                settingsCollection?.Add($"{c.Properties[Windows.Storage.SystemProperties.Comment]}");
        }
    }
}
