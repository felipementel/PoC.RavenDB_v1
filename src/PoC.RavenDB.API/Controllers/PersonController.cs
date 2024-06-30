using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents;

namespace PoC.RavenDB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly IDocumentStore _documentStore;

        public PersonController(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        [HttpGet]
        public IActionResult Get()
        {
            using (var session = _documentStore.OpenSession())
            {
                var people = session.Query<Person>().ToList();

                if (people.Count == 0)
                {
                    return NotFound();
                }

                return Ok(people);
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            using (var session = _documentStore.OpenSession())
            {
                var person = session.Load<Person>(id);

                if (person == null)
                {
                    return NotFound();
                }

                return Ok(person);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] Person person)
        {
            if (person == null)
            {
                return BadRequest();
            }

            if (string.IsNullOrEmpty(person.Name))
            {
                return BadRequest();
            }

            if (person.Age <= 0)
            {
                return BadRequest();
            }

            using (var session = _documentStore.OpenSession())
            {
                session.Store(person);
                session.SaveChanges();
            }

            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult Put(
            [FromHeader] string id,
            [FromBody] Person person)
        {
            using (var session = _documentStore.OpenSession())
            {
                var existedPerson = session.Load<Person>(id);

                if (existedPerson == null)
                {
                    return NotFound();
                }

                existedPerson.Name = person.Name;
                existedPerson.Age = person.Age;

                session.Store(person);
                session.SaveChanges();
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            using (var session = _documentStore.OpenSession())
            {
                var existedPerson = session.Load<Person>(id);

                if (existedPerson == null)
                {
                    return NotFound();
                }

                session.Delete(id);
                session.SaveChanges();
            }

            return NotFound();
        }
    }
}
