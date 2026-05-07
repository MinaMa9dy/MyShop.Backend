using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyShop.Application.DTOs.Review;
using MyShop.Application.Interfaces;

using System.Security.Claims;

namespace MyShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        
        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
            

        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddReview(AddReviewDto review)
        {
            
            var result = await _reviewService.AddReview(review);
            return StatusCode(result.Status, result);
        }
        [HttpGet("Product/{ProductId}")]
        public async Task<IActionResult> GetReviewsByProduct(Guid productId)
        {
            var result = await _reviewService.GetReviewsByProductId(productId);
            return StatusCode(result.Status, result);
        }
    }
}
