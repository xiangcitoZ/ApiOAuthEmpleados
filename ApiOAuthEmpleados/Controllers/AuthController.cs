using ApiOAuthEmpleados.Helpers;
using ApiOAuthEmpleados.Models;
using ApiOAuthEmpleados.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiOAuthEmpleados.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private RepositoryEmpleados repo;
        private HelperOAuthToken helper;

        public AuthController(RepositoryEmpleados repo,
            HelperOAuthToken helper)
        {
            this.repo = repo;
            this.helper = helper;

        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> Login(LoginModel model)
        {
            Empleado empleado = 
                await this.repo.ExisteEmpeladoAsync
                (model.UserName, int.Parse(model.Password));   
            
            if (empleado == null)
            {
                return Unauthorized();
            }
            else
            {
                SigningCredentials credentials =
                    new SigningCredentials(this.helper.GetKeyToken()
                    , SecurityAlgorithms.HmacSha256);

                string jsonEmpleado = 
                    JsonConvert.SerializeObject(empleado);
                Claim[] informacion = new[]
                {
                    new Claim("UserData", jsonEmpleado)
                };


                JwtSecurityToken token =
                    new JwtSecurityToken(
                        claims: informacion,
                        issuer: this.helper.Issuer,
                        audience: this.helper.Audience,
                        signingCredentials: credentials,
                        expires: DateTime.UtcNow.AddMinutes(30),
                        notBefore: DateTime.UtcNow
                        );
                return Ok(new
                {
                    response =
                    new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
        }


    }
}
