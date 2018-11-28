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
    [Route("Blogs/{blogId}/Attachments")]
    public class BlogAttachmentsController : Controller
    {
        private readonly FunLandContext _context;
        public BlogAttachmentsController(FunLandContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index(Guid blogId)
        {
            if (blogId.Equals(Guid.Empty)) return BadRequest("invalid guid");
            var model = await _context.Blogs.Include(blog => blog.BlogAttachments).FirstOrDefaultAsync(blog => blog.BlogId.Equals(blogId));
            if (model == null) return NotFound();
            return Ok(Mapper.Map<BlogAttachmentView>(model.BlogAttachments));
        }
        
        [HttpGet("{attachmentId}")]
        public async Task<IActionResult> Get(Guid blogId, int attachmentId)
        {
            if (blogId.Equals(Guid.Empty)) return BadRequest("invalid guid");
            var blog = await _context.Blogs.Include(model => model.BlogAttachments).FirstOrDefaultAsync(model => model.BlogId.Equals(blogId));
            if (blog == null) return NotFound();
            var attachment = blog.BlogAttachments.FirstOrDefault(model => model.BlogAttachmentId == attachmentId);
            if (attachment == null) return NotFound();
            return Ok(Mapper.Map<BlogAttachmentView>(attachment));
        }

        [HttpPost]
        public async Task<IActionResult> Post(Guid blogId, [FromBody] BlogAttachmentView view)
        {
            if (blogId.Equals(Guid.Empty)) return BadRequest("invalid guid");
            var model = await _context.Blogs.Include(blog => blog.BlogAttachments).FirstOrDefaultAsync(blog => blog.BlogId.Equals(blogId));
            if (model == null) return NotFound();
            List<BlogAttachment> attachments;
            attachments = model.BlogAttachments ?? new List<BlogAttachment>();
            var attachment = new BlogAttachment
            {
                Path = view.Path
            };
            attachments.Add(attachment);
            await _context.SaveChangesAsync();
            return Created(Url.Action(nameof(Get)), attachment);
        }

        [HttpPatch("{attachmentId}")]
        public IActionResult Update(Guid blogId, int attachmentId)
        {
            return Ok();
        }

        [HttpDelete("{attachmentId}")]
        public IActionResult Delete(Guid blogId, int attachmentId)
        {
            return Ok();
        }
    }
}