using System.Collections.Generic;
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
        [HttpGet("{id:int}")]
        public IActionResult GetById(int Id)
        {
            var celestialObject = _context.CelestialObjects.Find(Id);

            if (celestialObject == null)
                return NotFound();

            celestialObject.Satellites = new List<CelestialObject> { _context.CelestialObjects.Where(b => b.OrbitedObjectId == Id).FirstOrDefault() };

            return Ok(celestialObject);

        }
        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(b => b.Name == name);


            if (celestialObjects.Count() == 0)
                return NotFound();
            foreach (var item in celestialObjects)
            {
                item.Satellites = new List<CelestialObject> { _context.CelestialObjects.Where(b => b.OrbitedObjectId == item.Id).FirstOrDefault() };

            }

            return Ok(celestialObjects);

        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects;
            foreach (var item in celestialObjects)
            {
                item.Satellites = new List<CelestialObject> { item };
            }
            return Ok(celestialObjects);

        }
        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();

            var routeValues = new { id = celestialObject.Id };
            return CreatedAtRoute("GetById", routeValues, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] CelestialObject celestialObject)
        {
            var celObject = _context.CelestialObjects.Find(id);

            if (celObject == null)
                return NotFound();

            celObject.Name = celestialObject.Name;
            celObject.OrbitalPeriod = celestialObject.OrbitalPeriod;
            celObject.OrbitedObjectId = celestialObject.OrbitedObjectId;

            _context.CelestialObjects.Update(celObject);
            _context.SaveChanges();

            return NoContent();
        }
        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celObject = _context.CelestialObjects.Find(id);

            if (celObject == null)
                return NotFound();

            celObject.Name = name;

            _context.CelestialObjects.Update(celObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celObjects = _context.CelestialObjects.Where(f => f.Id == id || f.OrbitedObjectId == id).ToList();

            if (celObjects.Count() <= 0)
                return NotFound();

            _context.CelestialObjects.RemoveRange(celObjects);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
