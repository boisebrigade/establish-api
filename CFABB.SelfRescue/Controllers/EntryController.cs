using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFABB.SelfRescue.Data;
using CFABB.SelfRescue.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CFABB.SelfRescue.Controllers {
    [Produces("application/json")]
    [Route("api/Entry")]
    [AllowAnonymous]
    public class EntryController : Controller {

        #region CRUD
        [HttpGet]
        [ProducesResponseType(typeof(Models.Entry[]), 200)]
        public async Task<IActionResult> GetAllEntries([FromServices] ApplicationDbContext db) {
            return Ok(await db.Entries.ToListAsync());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Models.Entry), 200)]
        public async Task<IActionResult> GetById(int id, [FromServices] ApplicationDbContext db) {
            var entry = await db.Entries.SingleOrDefaultAsync(i => i.Id == id);
            if (entry == null) {
                return NotFound("Entry not found");
            }
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(typeof(Models.Entry), 201)]
        public async Task<IActionResult> AddNewEntry([FromBody] Models.Entry entry, [FromServices] ApplicationDbContext db) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            //db.Entries.Add(entry);
            //await db.SaveChangesAsync();
            return Created("", null);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateEntry([FromBody] Models.Entry entry, [FromServices] ApplicationDbContext db) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            var existingEntry = await db.Entries.SingleOrDefaultAsync(i => i.Id == entry.Id);
            if (existingEntry == null) {
                return NotFound("Entry not found");
            }


            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEntry(int id, [FromServices] ApplicationDbContext db) {
            var entry = db.Entries.SingleOrDefaultAsync(i => i.Id == id);
            if (entry == null) {
                return NotFound("Entry not found");
            }
            db.Remove(entry);
            await db.SaveChangesAsync();
            return NoContent();
        }
        #endregion

        #region Categories
        [SwaggerOperation(Tags = new[] { "Entry Categories" })]
        [HttpGet("{id}/categories")]
        [ProducesResponseType(typeof(Models.Category[]), 200)]
        public async Task<IActionResult> GetEntryCategories(int id, [FromServices] ApplicationDbContext db) {
            var entry = await db.Entries
                .Include(e => e.Categories)
                .ThenInclude(ec => ec.Category)
                .SingleOrDefaultAsync(i => i.Id == id);
            if (entry == null) {
                return NotFound();
            }
            var categories = entry.Categories
                .Select(i => new Models.Category() { Description = i.Category.CategoryDescription, Name = i.Category.CategoryName })
                .ToList();
            return Ok(categories);
        }

        [SwaggerOperation(Tags = new[] {"Entry Categories"})]
        [HttpPost("{id}/category")]
        [ProducesResponseType(typeof(Models.Category[]), 204)]
        public async Task<IActionResult> AddCategoryToEntry(int id, [FromBody] Models.Category category, [FromServices] ApplicationDbContext db) {
            var entry = await db.Entries.SingleOrDefaultAsync(i => i.Id == id);
            if (entry == null) {
                return NotFound("Entry not found");
            }
            var dbCat = await db.Categories.SingleOrDefaultAsync(i => i.CategoryName.Equals(category.Name, StringComparison.InvariantCultureIgnoreCase));
            if (dbCat == null) {
                dbCat = new Data.Category() { CategoryDescription = category.Description, CategoryName = category.Name };
                db.Categories.Add(dbCat);
                // if the category is new, there is no way it can exist in the entrycategory table, defer saving here til after EC is added.
            }
            var ec = await db.EntryCategories.SingleOrDefaultAsync(e => e.CategoryId == dbCat.Id && e.EntryId == entry.Id);
            if (ec == null) {
                ec = new EntryCategory() { Category = dbCat, Entry = entry };
                db.EntryCategories.AddRange(ec);
                await db.SaveChangesAsync();
            }

            return Created("", dbCat);
        }

        [SwaggerOperation(Tags = new[] { "Entry Categories" })]
        [HttpDelete("{id}/category/{categoryId}")]
        public async Task<IActionResult> RemoveCategoryFromEntry(int id, int categoryId, [FromServices] ApplicationDbContext db) {
            var entry = await db.Entries.SingleOrDefaultAsync(i => i.Id == id);
            if (entry == null) {
                return NotFound("Entry not found");
            }
            var ec = await db.EntryCategories.SingleOrDefaultAsync(i => i.EntryId == id && i.CategoryId == categoryId);
            if (ec == null) {
                // it's not part of the db. we're good
                return NoContent();
            }
            db.EntryCategories.Remove(ec);
            await db.SaveChangesAsync();
            return NoContent();
        }
        #endregion

        #region Tags
        [SwaggerOperation(Tags = new[] { "Entry Tags" })]
        [HttpGet("{id}/tags")]
        [ProducesResponseType(typeof(Models.Tag[]), 200)]
        public async Task<IActionResult> GetTagsForEntry(int id, [FromServices] ApplicationDbContext db) {
            var entry = await db.Entries
                .Include(i => i.Tags)
                .ThenInclude(et => et.Tag)
                .SingleOrDefaultAsync(i => i.Id == id);
            if (entry == null) {
                return NotFound("Entry not found");
            }

            var tags = entry.Tags
                .Select(i => new Models.Tag() { Name = i.Tag.Name })
                .ToList();
            return Ok(tags);
        }

        [SwaggerOperation(Tags = new[] { "Entry Tags" })]
        [HttpPost("{id}/tag")]
        [ProducesResponseType(typeof(Models.Tag[]), 204)]
        public async Task<IActionResult> AddTagToEntry(int id, [FromBody] Models.Tag tag, [FromServices] ApplicationDbContext db) {
            var entry = await db.Entries.SingleOrDefaultAsync(i => i.Id == id);
            if (entry == null) {
                return NotFound("Entry not found");
            }
            var dbTag = await db.Tags.SingleOrDefaultAsync(i => i.Name.Equals(tag.Name, StringComparison.InvariantCultureIgnoreCase));
            if (dbTag == null) {
                dbTag = new Data.Tag() { Name = tag.Name };
                db.Tags.Add(dbTag);
            }
            var ec = await db.EntryTags.SingleOrDefaultAsync(e => e.TagId == dbTag.Id && e.EntryId == entry.Id);
            if (ec == null) {
                ec = new EntryTag() { Tag = dbTag, Entry = entry };
                db.EntryTags.AddRange(ec);
                await db.SaveChangesAsync();
            }

            return Created("", dbTag);
        }

        [SwaggerOperation(Tags = new[] { "Entry Tags" })]
        [HttpDelete("{id}/tag/{tagId}")]
        public async Task<IActionResult> RemoveTagFromEntry(int id, int tagId, [FromServices] ApplicationDbContext db) {
            var entry = await db.Entries.SingleOrDefaultAsync(i => i.Id == id);
            if (entry == null) {
                return NotFound("Entry not found");
            }
            var ec = await db.EntryTags.SingleOrDefaultAsync(i => i.EntryId == id && i.TagId == tagId);
            if (ec == null) {
                // it's not part of the db. we're good
                return NoContent();
            }
            db.EntryTags.Remove(ec);
            await db.SaveChangesAsync();
            return NoContent();
        }
        #endregion
    }
}