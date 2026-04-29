using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyShop.CORE.Dtos.Review;
using MyShop.CORE.Interfaces;

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
            if (!result.IsSuccess)
            {
                return StatusCode(int.Parse(result.Error.Code), result.Error.Message);
            }
            return Ok(result.Data);
        }
        [HttpGet("GetReviewsByProductId")]
        public async Task<IActionResult> GetReviewsByProductId(Guid productId)
        {
            var result = await _reviewService.GetReviewsByProductId(productId);
            if (!result.IsSuccess)
            {
                return StatusCode(int.Parse(result.Error.Code), result.Error.Message);
            }
            return Ok(result.Data);
        }
    }
}
