using Microsoft.AspNetCore.Mvc;
using SimpleSystem.Business;
using SimpleSystem.DataAccess.Entities;

namespace SimpleSystem.API.Controllers
{
    [ApiController]
    [Route("api/Users")]
    public class UsersController : ControllerBase
    {
        [HttpGet("All", Name = "GetAllUsers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<User>> GetAllUsers()
        {
            var users = UserBusiness.GetAllUsers();
            if (users == null || users.Count == 0)
            {
                return NotFound("No users found.");
            }
            return Ok(users);
        }

        [HttpGet("{id}", Name = "GetUserById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<User> GetUserById(int id)
        {
            if (id <= 0)
            {
                return BadRequest($"Invalid user ID: {id}");
            }

            var user = UserBusiness.Find(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            return Ok(user.ToEntity());
        }

        [HttpPost(Name = "AddUser")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<User> AddUser([FromBody] User newUser)
        {
            if (newUser == null || newUser.PersonId <= 0 || string.IsNullOrWhiteSpace(newUser.Username) || string.IsNullOrWhiteSpace(newUser.PasswordHash))
            {
                return BadRequest("Invalid user data.");
            }

            var userBusiness = new UserBusiness
            {
                PersonId = newUser.PersonId,
                Username = newUser.Username,
                PasswordHash = newUser.PasswordHash,
                IsActive = newUser.IsActive
            };

            if (userBusiness.Save())
            {
                newUser.UserId = userBusiness.UserId;
                newUser.CreatedDate = userBusiness.CreatedDate;
                return CreatedAtRoute("GetUserById", new { id = newUser.UserId }, newUser);
            }

            return BadRequest("Could not add the user. Ensure PersonId exists and Username is unique.");
        }

        [HttpPut("{id}", Name = "UpdateUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<User> UpdateUser(int id, [FromBody] User updatedUser)
        {
            if (id <= 0 || updatedUser == null || string.IsNullOrWhiteSpace(updatedUser.Username) || string.IsNullOrWhiteSpace(updatedUser.PasswordHash))
            {
                return BadRequest("Invalid user data.");
            }

            var userBusiness = UserBusiness.Find(id);
            if (userBusiness == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            userBusiness.Username = updatedUser.Username;
            userBusiness.PasswordHash = updatedUser.PasswordHash;
            userBusiness.IsActive = updatedUser.IsActive;

            if (userBusiness.Save())
            {
                return Ok(userBusiness.ToEntity());
            }

            return BadRequest("Could not update the user.");
        }

        [HttpDelete("{id}", Name = "DeleteUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteUser(int id)
        {
            if (id <= 0)
            {
                return BadRequest($"Invalid user ID: {id}");
            }

            if (UserBusiness.DeleteUser(id))
            {
                return Ok($"User with ID {id} has been deleted.");
            }

            return NotFound($"User with ID {id} not found or could not be deleted.");
        }
    }
}