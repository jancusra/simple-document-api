namespace App.Persistence
{
    public partial interface IDataProviderManager
    {
        /// <summary>
        /// Property to get actual data provider
        /// </summary>
        IDataProvider DataProvider { get; }
    }
}
