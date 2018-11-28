using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FunLand.Data;
using FunLand.Data.Models;
using FunLand.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FunLand.Web.Controllers
{
    [Route("[controller]")]
    public class BlogsController : Controller
    {
        private readonly FunLandContext _context;
        public BlogsController(FunLandContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var models = await _context.Blogs.Include(x => x.BlogAttachments).Take(10).ToListAsync();
            return Ok(Mapper.Map<List<BlogView>>(models));
        }

        [HttpGet("{guid}")]
        public async Task<IActionResult> Get(Guid guid)
        {
            var model = await _context.Blogs.FirstOrDefaultAsync(x => x.BlogId.Equals(guid));
            return Ok(Mapper.Map<BlogView>(model));
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BlogView view)
        {
            var model = Mapper.Map<Blog>(view);
            var guid = Guid.NewGuid();
            model.BlogId = guid;
            _context.Add(model);
            await _context.SaveChangesAsync();
            return Created(Url.Action(nameof(Get), new { guid = guid }), Mapper.Map<BlogView>(model));
        }

        [HttpPatch("{guid}")]
        public IActionResult Update(Guid guid)
        {
            if (guid.Equals(Guid.Empty))
            {
                return BadRequest("invalid guid");
            }
            return Ok();
        }
        
        [HttpDelete("{guid}")]
        public IActionResult Delete(Guid guid)
        {
            if (guid.Equals(Guid.Empty))
            {
                return BadRequest("invalid guid");
            }
            return Ok();
        }
    }
}