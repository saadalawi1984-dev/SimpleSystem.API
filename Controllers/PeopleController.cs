using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleSystem.Business;
using SimpleSystem.DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleSystem.API.Controllers
{
    [ApiController]
    [Route("api/People")]
    public class PeopleController : ControllerBase
    {
        // 1. جلب كل الأشخاص Async
        [HttpGet("All", Name = "GetAllPeople")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Person>>> GetAllPeople()
        {
            var people = await PersonBusiness.GetAllPeopleAsync();
            if (people == null || people.Count == 0)
            {
                return NotFound("No people found.");
            }
            return Ok(people);
        }

        // 2. جلب شخص حسب الرقم التعريفي Async
        [HttpGet("{id}", Name = "GetPersonById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Person>> GetPersonById(int id)
        {
            if (id <= 0)
            {
                return BadRequest($"Invalid person ID: {id}");
            }

            var person = await PersonBusiness.FindAsync(id);
            if (person == null)
            {
                return NotFound($"Person with ID {id} not found.");
            }

            return Ok(person.ToEntity());
        }

        // 3. إضافة شخص جديد Async
        [HttpPost(Name = "AddPerson")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Person>> AddPerson([FromBody] Person newPerson)
        {
            if (newPerson == null || string.IsNullOrWhiteSpace(newPerson.FirstName) || string.IsNullOrWhiteSpace(newPerson.LastName) || newPerson.CountryId <= 0)
            {
                return BadRequest("Invalid person data.");
            }

            var personBusiness = new PersonBusiness
            {
                FirstName = newPerson.FirstName,
                LastName = newPerson.LastName,
                DateOfBirth = newPerson.DateOfBirth,
                Phone = newPerson.Phone,
                Email = newPerson.Email,
                CountryId = newPerson.CountryId
            };

            if (await personBusiness.SaveAsync())
            {
                newPerson.PersonId = personBusiness.PersonId;
                return CreatedAtRoute("GetPersonById", new { id = newPerson.PersonId }, newPerson);
            }

            return BadRequest("Could not add the person.");
        }

        // 4. تعديل بيانات شخص Async
        [HttpPut("{id}", Name = "UpdatePerson")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Person>> UpdatePerson(int id, [FromBody] Person updatedPerson)
        {
            if (id <= 0 || updatedPerson == null || string.IsNullOrWhiteSpace(updatedPerson.FirstName) || string.IsNullOrWhiteSpace(updatedPerson.LastName))
            {
                return BadRequest("Invalid person data.");
            }

            var personBusiness = await PersonBusiness.FindAsync(id);
            if (personBusiness == null)
            {
                return NotFound($"Person with ID {id} not found.");
            }

            personBusiness.FirstName = updatedPerson.FirstName;
            personBusiness.LastName = updatedPerson.LastName;
            personBusiness.DateOfBirth = updatedPerson.DateOfBirth;
            personBusiness.Phone = updatedPerson.Phone;
            personBusiness.Email = updatedPerson.Email;
            personBusiness.CountryId = updatedPerson.CountryId;

            if (await personBusiness.SaveAsync())
            {
                return Ok(personBusiness.ToEntity());
            }

            return BadRequest("Could not update the person.");
        }

        // 5. حذف شخص Async
        [HttpDelete("{id}", Name = "DeletePerson")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeletePerson(int id)
        {
            if (id <= 0)
            {
                return BadRequest($"Invalid person ID: {id}");
            }

            if (await PersonBusiness.DeletePersonAsync(id))
            {
                return Ok($"Person with ID {id} has been deleted.");
            }

            return NotFound($"Person with ID {id} not found or could not be deleted.");
        }
    }
}
