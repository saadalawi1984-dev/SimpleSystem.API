using Microsoft.AspNetCore.Mvc;
using SimpleSystem.Business;
using SimpleSystem.DataAccess.Entities;

namespace SimpleSystem.API.Controllers
{
    [ApiController]
    [Route("api/Countries")]
    public class CountriesController : ControllerBase
    {
        [HttpGet("All", Name = "GetAllCountries")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<Country>> GetAllCountries()
        {
            var countries = CountryBusiness.GetAllCountries();
            if (countries == null || countries.Count == 0)
            {
                return NotFound("No countries found.");
            }
            return Ok(countries);
        }

        [HttpGet("{id}", Name = "GetCountryById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Country> GetCountryById(int id)
        {
            if (id <= 0)
            {
                return BadRequest($"Invalid country ID: {id}");
            }

            var country = CountryBusiness.Find(id);
            if (country == null)
            {
                return NotFound($"Country with ID {id} not found.");
            }

            return Ok(country.ToEntity());
        }

        [HttpPost(Name = "AddCountry")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Country> AddCountry([FromBody] Country newCountry)
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

            if (countryBusiness.Save())
            {
                newCountry.CountryId = countryBusiness.CountryId;
                return CreatedAtRoute("GetCountryById", new { id = newCountry.CountryId }, newCountry);
            }

            return BadRequest("Could not add the country.");
        }

        [HttpPut("{id}", Name = "UpdateCountry")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Country> UpdateCountry(int id, [FromBody] Country updatedCountry)
        {
            if (id <= 0 || updatedCountry == null || string.IsNullOrWhiteSpace(updatedCountry.CountryName) || string.IsNullOrWhiteSpace(updatedCountry.CountryCode))
            {
                return BadRequest("Invalid country data.");
            }

            var countryBusiness = CountryBusiness.Find(id);
            if (countryBusiness == null)
            {
                return NotFound($"Country with ID {id} not found.");
            }

            countryBusiness.CountryName = updatedCountry.CountryName;
            countryBusiness.CountryCode = updatedCountry.CountryCode;

            if (countryBusiness.Save())
            {
                return Ok(countryBusiness.ToEntity());
            }

            return BadRequest("Could not update the country.");
        }

        [HttpDelete("{id}", Name = "DeleteCountry")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteCountry(int id)
        {
            if (id <= 0)
            {
                return BadRequest($"Invalid country ID: {id}");
            }

            if (CountryBusiness.DeleteCountry(id))
            {
                return Ok($"Country with ID {id} has been deleted.");
            }

            return NotFound($"Country with ID {id} not found or could not be deleted.");
        }
    }
}