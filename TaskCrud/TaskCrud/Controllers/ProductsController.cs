using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskCrud.DAL;
using TaskCrud.Models;
using System.Text.Json.Serialization;
using Google.Cloud.Translation.V2;

namespace TaskCrud.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            this._context = context;
        }
        // GET: api/Product
        [HttpGet]
        [Route("GetAll")]
        public async Task<ActionResult<IEnumerable>> GetAllProducts()
        {
          
            return await _context.Products.ToListAsync();
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public async Task<ActionResult> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) 
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPut]
        [Route("PutProduct/{id}")]
        public async Task<ActionResult> PutProducts(int? id, Product product)
        {
           

            if (id == null) return NotFound();

            Product productDb = await _context.Products.FindAsync(id);
            if (productDb == null) return NotFound();

            Translate tr = new Translate();

            product.DescriptionEng = product.Description;
            tr.Translates(product.DescriptionEng);
            product.DescriptionEng = tr.result;

            productDb.DescriptionEng = product.DescriptionEng;
            productDb.Name = product.Name;
            productDb.Description = product.Description;
            productDb.Price = product.Price;
            await _context.SaveChangesAsync();
            return Ok(product);

        }

        [HttpPost]
        [Route("PostProduct")]
        public async Task<ActionResult> PostProducts(Product product)
        {
            Translate tr = new Translate();
            product.DescriptionEng = product.Description;
            tr.Translates(product.DescriptionEng);
            product.DescriptionEng = tr.result;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProducts", new { id = product.Id }, product);
           
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<ActionResult> DeleteProducts(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(product);
        }
        

    }
}
