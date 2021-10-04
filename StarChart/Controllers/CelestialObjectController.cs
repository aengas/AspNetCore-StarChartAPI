using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

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

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var existingObject = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);

            if (existingObject == null)
            {
                return NotFound();
            }

            existingObject.Name = celestialObject.Name;
            existingObject.OrbitalPeriod = celestialObject.OrbitalPeriod;
            existingObject.OrbitedObjectId = celestialObject.OrbitedObjectId;

            _context.Update(existingObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var existingObject = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);

            if (existingObject == null)
            {
                return NotFound();
            }

            existingObject.Name = name;

            _context.Update(existingObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var objectsToBeDeleted = _context.CelestialObjects.Where(c => c.Id == id || c.OrbitedObjectId == id);

            if (!objectsToBeDeleted.Any())
            {
                return NotFound();
            }
            
            _context.CelestialObjects.RemoveRange(objectsToBeDeleted);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
