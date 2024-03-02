using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dotnetapp.Models;
using Microsoft.AspNetCore.Authorization;
using dotnetapp.Services;

namespace dotnetapp.Controllers
{
    [Route("api/review")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly ReviewService _reviewService;

        public ReviewController(ReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Review>>> GetAllReviews()
        {
            var reviews = await _reviewService.GetAllReviews();
            return Ok(reviews);
        }

        [Authorize]
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviewsByUserId(long userId)
        {
            var reviews = await _reviewService.GetReviewsByUserId(userId);

            if (reviews == null || !reviews.Any())
            {
                return NotFound(new { message = "No reviews found for the user" });
            }

            return Ok(reviews);
        }

        [Authorize("Customer")]
        [HttpPost]
        public async Task<ActionResult> AddReview([FromBody] Review review)
        {
            try
            {
                review.DateCreated = DateTime.Now; 
                var success = await _reviewService.AddReview(review);

                if (success)
                {
                    return Ok(new { message = "Review added successfully" });
                }
                else
                {
                    return StatusCode(500, new { message = "Failed to add review" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
