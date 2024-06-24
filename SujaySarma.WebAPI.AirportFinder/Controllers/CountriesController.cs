using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

using SujaySarma.WebAPI.AirportFinder.Internal;
using SujaySarma.WebAPI.AirportFinder.Internal.Data;

using System;
using System.Data;
using System.Linq;

namespace SujaySarma.WebAPI.AirportFinder.Controllers
{
    /// <summary>
    /// Search for and return information about countries
    /// </summary>
    [ApiController]
    public class CountriesController : ControllerBase
    {

        /// <summary>
        /// Get all countries
        /// </summary>
        /// <returns>An array of all countries and information about them.</returns>
        [HttpGet("api/countries")]
        public IActionResult GetAll()
            => Utils.GetJson(_ds_countries.Items);


        /// <summary>
        /// Get a country by the geographic numeric ID
        /// </summary>
        /// <param name="id">Numeric ID of the country</param>
        /// <returns>Array of matching countries</returns>
        [HttpGet("api/country/id/{id:int}")]
        public IActionResult GetById([FromRoute(Name = "id")] int id)
            => Utils.GetJson(_ds_countries.Items.Where(c => (c.Id == id)));

        /// <summary>
        /// Get a country by the ISO country code
        /// </summary>
        /// <param name="iso">ISO Country Code</param>
        /// <returns>Array of matching countries</returns>
        [HttpGet("api/country/iso/{iso}")]
        public IActionResult GetByISOCode([FromRoute(Name = "iso")] string iso)
        {
            Utils.AssertNotEmpty(iso);
            return Utils.GetJson(_ds_countries.Items.Where(c => (c.Code == iso)));
        }

        /// <summary>
        /// Get a country by it's name
        /// </summary>
        /// <param name="name">Name of the country</param>
        /// <returns>Array of matching countries</returns>
        [HttpGet("api/country/name/{name}")]
        public IActionResult GetByName([FromRoute(Name = "name")] string name)
        {
            Utils.AssertNotEmpty(name);
            return Utils.GetJson(_ds_countries.Items.Where(
                c => (
                    c.Name!.Contains(name, StringComparison.InvariantCultureIgnoreCase) 
                    || ((!string.IsNullOrWhiteSpace(c.Keywords)) && c.Keywords.Contains(name, StringComparison.InvariantCultureIgnoreCase))
                )
            ));
        }



        /// <inheritdoc />
        public CountriesController(IWebHostEnvironment webHostEnvironment)
        {
            _ds_countries = new DataSource<Country>(Constants.DataFileType.countries, webHostEnvironment.ContentRootPath);
        }

        private readonly DataSource<Country> _ds_countries;
    }
}
