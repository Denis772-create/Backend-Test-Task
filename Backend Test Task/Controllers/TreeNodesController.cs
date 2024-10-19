using Backend_Test_Task.Exceptions;
using Backend_Test_Task.Infrastructure;
using Backend_Test_Task.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend_Test_Task.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TreeNodesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TreeNodesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TreeNode>> GetNode(int id)
        {
            var node = await _context.TreeNodes.FindAsync(id);
            if (node == null)
            {
                return NotFound();
            }

            return Ok(node);
        }

        [HttpPost]
        public async Task<ActionResult<TreeNode>> CreateNode(TreeNode node)
        {
            _context.TreeNodes.Add(node);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetNode), new { id = node.Id }, node);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNode(int id)
        {
            var node = await _context.TreeNodes
                .Include(x => x.Children)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (node is null)
            {
                return NotFound();
            }

            if (node.Children.Any())
            {
                throw new SecureException("You have to delete all children nodes first");
            }

            _context.TreeNodes.Remove(node);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

}
