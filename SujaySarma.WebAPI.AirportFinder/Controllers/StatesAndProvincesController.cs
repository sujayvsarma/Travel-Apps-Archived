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
    /// Search for and return information about states/provinces (regions)
    /// </summary>
    [ApiController]
    public class StatesAndProvincesController : Controller
    {

        /// <summary>
        /// Get all regions
        /// </summary>
        /// <returns>An array of all regions and information about them.</returns>
        [HttpGet("api/regions")]
        public IActionResult GetAll()
            => Utils.GetJson(_ds_regions.Items);


        /// <summary>
        /// Get a region by the geographic numeric ID
        /// </summary>
        /// <param name="id">Numeric ID of the region</param>
        /// <returns>Array of matching regions</returns>
        [HttpGet("api/region/id/{id:int}")]
        public IActionResult GetById([FromRoute(Name = "id")] int id)
            => Utils.GetJson(_ds_regions.Items.Where(c => (c.Id == id)));

        /// <summary>
        /// Get a region by the ISO country code
        /// </summary>
        /// <param name="iso">ISO Region Code</param>
        /// <returns>Array of matching regions</returns>
        [HttpGet("api/region/iso/{iso}")]
        public IActionResult GetByISOCode([FromRoute(Name = "iso")] string iso)
        {
            Utils.AssertNotEmpty(iso);
            return Utils.GetJson(_ds_regions.Items.Where(c => (c.Code == iso)));
        }

        /// <summary>
        /// Get a region by it's name
        /// </summary>
        /// <param name="name">Name of the region</param>
        /// <returns>Array of matching regions</returns>
        [HttpGet("api/region/name/{name}")]
        public IActionResult GetByName([FromRoute(Name = "name")] string name)
        {
            Utils.AssertNotEmpty(name);
            return Utils.GetJson(_ds_regions.Items.Where(
                c => (
                    c.Name!.Contains(name, StringComparison.InvariantCultureIgnoreCase)
                    || ((!string.IsNullOrWhiteSpace(c.Keywords)) && c.Keywords.Contains(name, StringComparison.InvariantCultureIgnoreCase))
                )
            ));
        }

        /// <inheritdoc />
        public StatesAndProvincesController(IWebHostEnvironment webHostEnvironment)
        {
            _ds_regions = new DataSource<Region>(Constants.DataFileType.regions, webHostEnvironment.ContentRootPath);
        }

        private readonly DataSource<Region> _ds_regions;
    }
}
