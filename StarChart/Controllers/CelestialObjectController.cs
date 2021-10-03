using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);
            
            if(celestialObject == null)
            {
                return NotFound();
            }

            foreach (var co in _context.CelestialObjects)
            {
                if (co.OrbitedObjectId == id)
                {
                    celestialObject.Satellites.Add(co);
                }
            }

            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(c => c.Name == name);

            if (!celestialObjects.Any())
            {
                return NotFound();
            }

            foreach (var co in _context.CelestialObjects)
            {
                var orbitedObject = celestialObjects.FirstOrDefault(c => c.Id == co.OrbitedObjectId);
                if (orbitedObject != null)
                {
                    orbitedObject.Satellites.Add(co);
                }
            }

            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects;

            foreach (var co in _context.CelestialObjects)
            {
                var orbitedObject = celestialObjects.FirstOrDefault(c => c.Id == co.OrbitedObjectId);
                if (orbitedObject != null)
                {
                    orbitedObject.Satellites.Add(co);
                }
            }

            return Ok(celestialObjects);
        }
    }
}
