using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//TODO: Finalizar a implementação do TipoFotoController
namespace ImobAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TipoFotoController : ControllerBase
    {
        private readonly Context.ImobContext _context;
        public TipoFotoController(Context.ImobContext context)
        {
            _context = context;
        }
    }
}
