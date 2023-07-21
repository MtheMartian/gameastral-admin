using GameStarBackend.Api.Services;
using GameStarBackend.Api.Models;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using MongoDB.Driver;
using Microsoft.AspNetCore.Cors;

namespace GameStarBackend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewsService _reviewsService; 

        public async Task<Review> ReviewFromRequest()
        {
            Request.EnableBuffering();
            Request.Body.Position = 0;
            var streamReader = await new StreamReader(Request.Body).ReadToEndAsync();
            string newString = HttpUtility.UrlDecode(streamReader);
            var newReview = JsonSerializer.Deserialize<Review>(newString);
            return newReview!;
        }

        public ReviewsController(ReviewsService reviewsService) 
        {
            _reviewsService = reviewsService; 
        }

        [HttpGet]
        public async Task<List<Review>> GetReviews()
        {
            return await _reviewsService.GetReviews();
        }

        [EnableCors("_allowedOrigins")]
        [HttpGet("{gameId}")]
        public async Task<ReturnMessage> GetReviews(string gameId)
        {
            var reviews = await _reviewsService.GetReviews(gameId);
            ReturnMessage items= new ReturnMessage();
            if (reviews.Count > 0)
            {
                items.Data = reviews;
                items.Message = "Success";
            }
            else
            {
                items.Data = null;
                items.Message = "Failure";
            }
            return items;
        }

        [EnableCors("_allowedOrigins")]
        [HttpPost("[action]/{gameId}")]
        public async Task<ActionResult<ReturnMessage>> PostReview(string gameId)
        {
            var review = await ReviewFromRequest();

            ReturnMessage items = new ReturnMessage();

            if (!string.IsNullOrEmpty(review.GameReview) && review.GameReview != " " )
            {
                if (string.IsNullOrEmpty(review.DisplayName))
                {
                    review.DisplayName = "Anonymous";
                }
                review.GameId = gameId;
                await _reviewsService.AddReview(review);
                items.Data = await _reviewsService.GetReviews(gameId); ;
                items.Message = "Success";
            }
            else
            {
                items.Data = null;
                items.Message = "Failure";
            }
            return items;
        }

        [HttpDelete("[action]/{id}")]
        public async Task<ActionResult<ReturnMessage>> DeleteReview(string id)
        {
            ReturnMessage item = new ReturnMessage();
            item.Message = "Success";
            item.Data = await _reviewsService.DeleteReview(id);
            return item;
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteReviews()
        {
            string[] stringIds = new string[6];
            await _reviewsService.DeleteReviews(stringIds);
            return Ok();
        }
    }
}
