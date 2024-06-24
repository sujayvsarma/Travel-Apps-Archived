using SujaySarma.Data.Files.TokenLimitedFiles;

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace SujaySarma.WebAPI.AirportFinder.Internal
{
    /// <summary>
    /// A base class implementing CSV data exposure
    /// </summary>
    internal class DataSource<T>
        where T : class, new()
    {

        /// <summary>
        /// DataTable containing data for this source
        /// </summary>
        public DataTable Data { get; private set; }

        /// <summary>
        /// Get items as a List[T]
        /// </summary>
        public List<T> Items 
        { 
            get
            {
                if (_items == default)
                {
                    _items = TokenLimitedFileReader.GetList<T>(Data, _applyFunction);
                }

                return _items;
            }
        }
        private List<T>? _items = default;       


        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="forType">This source is locked to the specified type</param>
        /// <param name="environmentContentRoot">ContentRoot path from WebHostEnvironment</param>
        /// <param name="applyFunction">An [Optional] transformation function to apply</param>
        public DataSource(Constants.DataFileType forType, string environmentContentRoot, Action<DataTable>? applyFunction = null)
        {
            string fileName = forType switch
            {
                Constants.DataFileType.airports => "airports",
                Constants.DataFileType.countries => "countries",
                Constants.DataFileType.regions => "regions",
                _ => throw new ArgumentException("Value of 'file' out of range", nameof(forType))
            };

            if (!OurAirportsDataFileRetriever.EnsureLatestFile(forType, environmentContentRoot))
            {
                throw new ApplicationException($"The latest file for type '{fileName}' could not be found.");
            }

            _applyFunction = applyFunction;

            string localPath = Path.Combine(environmentContentRoot, "_data", $"{fileName}.csv");
            Data = TokenLimitedFileReader.GetTable(localPath, encoding: System.Text.Encoding.UTF8);
        }

        private Action<DataTable>? _applyFunction;

        
    }
}
