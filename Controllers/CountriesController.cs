using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleSystem.Business;
using SimpleSystem.DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleSystem.API.Controllers
{
    [ApiController]
    [Route("api/Countries")]
    public class CountriesController : ControllerBase
    {
        // 1. جلب كل الدول Async
        [HttpGet("All", Name = "GetAllCountries")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Country>>> GetAllCountries()
        {
            var countries = await CountryBusiness.GetAllCountriesAsync();
            if (countries == null || countries.Count == 0)
            {
                return NotFound("No countries found.");
            }
            return Ok(countries);
        }

        // 2. جلب دولة حسب الرقم التعريف Async
        [HttpGet("{id}", Name = "GetCountryById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Country>> GetCountryById(int id)
        {
            if (id <= 0)
            {
                return BadRequest($"Invalid country ID: {id}");
            }

            var country = await CountryBusiness.FindAsync(id);
            if (country == null)
            {
                return NotFound($"Country with ID {id} not found.");
            }

            return Ok(country.ToEntity());
        }

        // 3. إضافة دولة جديدة Async
        [HttpPost(Name = "AddCountry")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Country>> AddCountry([FromBody] Country newCountry)
        {
            if (newCountry == null || string.IsNullOrWhiteSpace(newCountry.CountryName) || string.IsNullOrWhiteSpace(newCountry.CountryCode))
            {
                return BadRequest("Invalid country data.");
            }

            var countryBusiness = new CountryBusiness
            {
                CountryName = newCountry.CountryName,
                CountryCode = newCountry.CountryCode
            };

            if (await countryBusiness.SaveAsync())
            {
                newCountry.CountryId = countryBusiness.CountryId;
                return CreatedAtRoute("GetCountryById", new { id = newCountry.CountryId }, newCountry);
            }

            return BadRequest("Could not add the country.");
        }

        // 4. تعديل بيانات دولة Async
        [HttpPut("{id}", Name = "UpdateCountry")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Country>> UpdateCountry(int id, [FromBody] Country updatedCountry)
        {
            if (id <= 0 || updatedCountry == null || string.IsNullOrWhiteSpace(updatedCountry.CountryName) || string.IsNullOrWhiteSpace(updatedCountry.CountryCode))
            {
                return BadRequest("Invalid country data.");
            }

            var countryBusiness = await CountryBusiness.FindAsync(id);
            if (countryBusiness == null)
            {
                return NotFound($"Country with ID {id} not found.");
            }

            countryBusiness.CountryName = updatedCountry.CountryName;
            countryBusiness.CountryCode = updatedCountry.CountryCode;

            if (await countryBusiness.SaveAsync())
            {
                return Ok(countryBusiness.ToEntity());
            }

            return BadRequest("Could not update the country.");
        }

        // 5. حذف دولة Async
        [HttpDelete("{id}", Name = "DeleteCountry")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteCountry(int id)
        {
            if (id <= 0)
            {
                return BadRequest($"Invalid country ID: {id}");
            }

            if (await CountryBusiness.DeleteCountryAsync(id))
            {
                return Ok($"Country with ID {id} has been deleted.");
            }

            return NotFound($"Country with ID {id} not found or could not be deleted.");
        }
    }
}
