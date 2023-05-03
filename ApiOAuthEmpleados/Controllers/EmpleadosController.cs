using ApiOAuthEmpleados.Models;
using ApiOAuthEmpleados.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace ApiOAuthEmpleados.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpleadosController : ControllerBase
    {
        private RepositoryEmpleados repo;

        public EmpleadosController(RepositoryEmpleados repo)
        {
            this.repo = repo;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<Empleado>>> GetEmpleados()
        {
            return await this.repo.GetEmpleadosAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Empleado>> FindEmpleado(int id)
        {
            return await this.repo.FindEmpleadoAsync(id);
        }


        [HttpGet]
        [Authorize]
        [Route("[action]")]
        public async Task<ActionResult<Empleado>> PerfilEmpleado()
        {
            //DEBEMOS BUSCAR EL CLAIM DEL EMPLEADO
            Claim claim = HttpContext.User.Claims
                .SingleOrDefault(x => x.Type == "UserData");
            string jsonEmpleado =
                claim.Value;
            Empleado empleado = JsonConvert.DeserializeObject<Empleado>
                (jsonEmpleado);
            return empleado;
        }

        [HttpGet]
        [Authorize]
        [Route("[action]")]

        public async Task<ActionResult<List<Empleado>>> CompisCurro()
        {

            string jsonEmpleado = HttpContext.User.Claims
                .SingleOrDefault(z => z.Type == "UserData").Value;

            Empleado empleado =
                JsonConvert.DeserializeObject<Empleado>(jsonEmpleado);
            List<Empleado> compis =
                await this.repo.GetCompisCurro(empleado.IdDepartamento);
            return compis;

        }
    }
}