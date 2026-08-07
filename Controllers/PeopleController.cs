using Microsoft.AspNetCore.Mvc;
using SimpleSystem.Business;
using SimpleSystem.DataAccess.Entities;

namespace SimpleSystem.API.Controllers
{
    [ApiController]
    [Route("api/People")]
    public class PeopleController : ControllerBase
    {
        [HttpGet("All", Name = "GetAllPeople")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<Person>> GetAllPeople()
        {
            var people = PersonBusiness.GetAllPeople();
            if (people == null || people.Count == 0)
            {
                return NotFound("No people found.");
            }
            return Ok(people);
        }

        [HttpGet("{id}", Name = "GetPersonById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Person> GetPersonById(int id)
        {
            if (id <= 0)
            {
                return BadRequest($"Invalid person ID: {id}");
            }

            var person = PersonBusiness.Find(id);
            if (person == null)
            {
                return NotFound($"Person with ID {id} not found.");
            }

            return Ok(person.ToEntity());
        }

        [HttpPost(Name = "AddPerson")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Person> AddPerson([FromBody] Person newPerson)
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

            if (personBusiness.Save())
            {
                newPerson.PersonId = personBusiness.PersonId;
                return CreatedAtRoute("GetPersonById", new { id = newPerson.PersonId }, newPerson);
            }

            return BadRequest("Could not add the person.");
        }

        [HttpPut("{id}", Name = "UpdatePerson")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Person> UpdatePerson(int id, [FromBody] Person updatedPerson)
        {
            if (id <= 0 || updatedPerson == null || string.IsNullOrWhiteSpace(updatedPerson.FirstName) || string.IsNullOrWhiteSpace(updatedPerson.LastName))
            {
                return BadRequest("Invalid person data.");
            }

            var personBusiness = PersonBusiness.Find(id);
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

            if (personBusiness.Save())
            {
                return Ok(personBusiness.ToEntity());
            }

            return BadRequest("Could not update the person.");
        }

        [HttpDelete("{id}", Name = "DeletePerson")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeletePerson(int id)
        {
            if (id <= 0)
            {
                return BadRequest($"Invalid person ID: {id}");
            }

            if (PersonBusiness.DeletePerson(id))
            {
                return Ok($"Person with ID {id} has been deleted.");
            }

            return NotFound($"Person with ID {id} not found or could not be deleted.");
        }
    }
}