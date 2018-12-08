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
            if (blogId.Equals(Guid.Empty)) return BadRequest("invalid blogId");
            var model = await _context.Blogs.Include(blog => blog.BlogAttachments).FirstOrDefaultAsync(blog => blog.BlogId.Equals(blogId));
            if (model == null) return NotFound();
            return Ok(Mapper.Map<BlogAttachmentView>(model.BlogAttachments));
        }
        
        [HttpGet]
        public async Task<IActionResult> Get(bool batch, Guid blogId, int[] attachmentIds)
        {
            bool invalid;
            IActionResult earlyResponse;
            List<BlogAttachment> blogAttachments;
            (invalid, _, blogAttachments, earlyResponse) = await Preflight(blogId, batch, attachmentIds.ToHashSet());
            if (invalid) return earlyResponse;
            
            return Ok(Mapper.Map<BlogAttachmentView>(blogAttachments));
        }

        [HttpPost]
        public async Task<IActionResult> Post(Guid blogId, [FromBody] BlogAttachmentView view)
        {
            if (blogId.Equals(Guid.Empty)) return BadRequest("invalid guid");
            var model = await _context.Blogs.Include(blog => blog.BlogAttachments).FirstOrDefaultAsync(blog => blog.BlogId.Equals(blogId));
            if (model == null) return NotFound();
            var attachments = model.BlogAttachments ?? new List<BlogAttachment>();
            var attachment = new BlogAttachment
            {
                Path = view.Path
            };
            attachments.Add(attachment);
            await _context.SaveChangesAsync();
            return Created(Url.Action(nameof(Get), new
            {
                blogId,
                attachmentId = attachment.BlogAttachmentId
            }), attachment);
        }

        [HttpPatch("{attachmentId}")]
        public IActionResult Update(Guid blogId, int attachmentId)
        {
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid blogId, bool batch, [FromBody] BlogAttachmentView[] blogAttachmentViews)
        {
            bool invalid;
            IActionResult earlyResponse;
            Blog blog;
            List<BlogAttachment> blogAttachments;
            (invalid, blog, blogAttachments, earlyResponse) = await Preflight(blogId, batch,
                blogAttachmentViews.Where(view => view.BlogAttachmentId != 0).Select(view => view.BlogAttachmentId)
                    .ToHashSet());
            if (invalid) return earlyResponse;
            
            foreach (var blogAttachmentToDelete in blogAttachments)
            {
                blog.BlogAttachments.Remove(blogAttachmentToDelete);
                _context.BlogAttachments.Remove(blogAttachmentToDelete);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        private async Task<(bool, Blog, List<BlogAttachment>, IActionResult)> Preflight(Guid blogId, bool batch, HashSet<int> blogAttachmentIdsToOperate)
        {
            if (!batch) return (true, null, null, BadRequest("batch need to be true"));
            if (blogId.Equals(Guid.Empty)) return (true, null, null, BadRequest("invalid blogId"));
            
            var blog = await _context.Blogs.Include(model => model.BlogAttachments)
                .FirstOrDefaultAsync(model => model.BlogId.Equals(blogId));
            if (blog == null) return (true, null, null, NotFound("blog not found"));
            if (!blogAttachmentIdsToOperate.Any()) return (true, blog, null, NoContent());

            var blogAttachmentsToOperate = blog.BlogAttachments
                .Where(model => blogAttachmentIdsToOperate.Contains(model.BlogAttachmentId)).ToList();

            if (blogAttachmentsToOperate.Count != blogAttachmentIdsToOperate.Count)
            {
                var blogAttachmentIdsNotFound = blogAttachmentIdsToOperate
                    .Except(blogAttachmentsToOperate.Select(model => model.BlogAttachmentId)).ToList();
                return (true, blog, null, NotFound(blogAttachmentIdsNotFound.Select(number => new BlogAttachmentView
                {
                    BlogAttachmentId = number
                })));
            }

            return (false, blog, blogAttachmentsToOperate, null);
        }
    }
}