using GestorDeUsuarios.Application.Abstractions.UsesCases;
using GestorDeUsuarios.Application.Models.Requests;
using GestorDeUsuarios.Application.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace GestorDeUsuarios.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ICreateUserUseCase _createUserUseCase;
        private readonly IGetUserByIdUseCase _getUserByIdUseCase;
        private readonly IUpdateUserUseCase _updateUserUseCase;
        private readonly IDeleteUserUseCase _deleteUserUseCase;
        private readonly ISearchUsersUseCase _searchUsersUseCase;
        public UsersController(ICreateUserUseCase createUserUseCase, IGetUserByIdUseCase getUserByIdUseCase, IUpdateUserUseCase updateUserUseCase, IDeleteUserUseCase deleteUserUseCase, ISearchUsersUseCase searchUsersUseCase)
        {
            _createUserUseCase = createUserUseCase;
            _getUserByIdUseCase = getUserByIdUseCase;
            _updateUserUseCase = updateUserUseCase;
            _deleteUserUseCase = deleteUserUseCase;
            _searchUsersUseCase = searchUsersUseCase;
        }


        [HttpPost]
        public async Task<ActionResult<UserResponse>> CrearUsuario([FromBody] CreateUserRequest createUsuarioRequest)
        {
            if (createUsuarioRequest == null)
                return BadRequest("Se requiere el cuerpo de la solicitud");

            var resultado = await _createUserUseCase.ExecuteAsync(createUsuarioRequest);
            return StatusCode(201, resultado);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> GetUserById(int id)
        {
            var user = await _getUserByIdUseCase.ExecuteAsync(id);
            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _deleteUserUseCase.ExecuteAsync(id);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserResponse>> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            var usuarioActualizado = await _updateUserUseCase.ExecuteAsync(id, request);
            return Ok(usuarioActualizado);
        }

        [HttpGet] 
        public async Task<ActionResult<IEnumerable<UserResponse>>> SearchUsers(
            [FromQuery] string? name = null,
            [FromQuery] string? province = null,
            [FromQuery] string? city = null)
        {
            var request = new SearchUsersRequest(name, province, city);
            var users = await _searchUsersUseCase.ExecuteAsync(request);
            return Ok(users);
        }
    }
}