using GameStarBackend.Api.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Sprache;

namespace GameStarBackend.Api.Services
{
    public class ReviewsService
    {
        private readonly IMongoCollection<Review> _reviewsCollection;
        public ReviewsService(IOptions<GameInfoDatabaseSettings> gameInfoDatabaseSettings)
        {
            var mongoClient = new MongoClient(gameInfoDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(gameInfoDatabaseSettings.Value.DatabaseName);
            _reviewsCollection = mongoDatabase.GetCollection<Review>(gameInfoDatabaseSettings.Value.ReviewsCollectionName);
        }

        private List<Review> SortReviewsByDate(List<Review> reviews)
        {
            for(int i = 0; i < reviews.Count; ++i)
            {
                for(int j = 0; j < reviews.Count - 1 - i; ++j)
                {
                    if(new DateTimeOffset(reviews[j].WhenPosted).ToUnixTimeMilliseconds() < 
                        new DateTimeOffset(reviews[j + 1].WhenPosted).ToUnixTimeMilliseconds())
                    {
                        Review temp = reviews[j];
                        reviews[j] = reviews[j + 1];
                        reviews[j + 1] = temp;
                    }
                }
            }
            return reviews;
        }

        public async Task AddReview(Review review)
        {
            await _reviewsCollection.InsertOneAsync(review);
        }

        public async Task<List<Review>> GetReviews(string gameId)
        {
            List<Review> reviews = new();
            reviews = await _reviewsCollection.Find(x => x.GameId == gameId).ToListAsync();
        
            List<Review> sortedByDateReviews = SortReviewsByDate(reviews);
            return sortedByDateReviews;
        }

        public async Task<List<Review>> GetReviews()
        {
            var reviews = await _reviewsCollection.Find(_ => true).ToListAsync();
            var sortedReviews = SortReviewsByDate(reviews);
            return sortedReviews;
        }

        public async Task<List<Review>> DeleteReview(string Id)
        {
            await _reviewsCollection.DeleteOneAsync(x => x.Id == Id);
            return await GetReviews();
        }

        public async Task DeleteReviews(string[] Ids)
        {
            for(int i = 0; i < Ids.Length; i++)
            {
                await _reviewsCollection.DeleteOneAsync(x => x.Id == Ids[i]);
            }
        }
    }
}
