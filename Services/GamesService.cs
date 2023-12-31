﻿using MongoDB.Driver;
using GameStarBackend.Api.Models;
using Microsoft.Extensions.Options;
using GameStarBackend.Api.Services;
using MongoDB.Bson.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using AlgoPrac;
using GameStarBackend.DSA;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace GameStarBackend.Api.Services
{
    public class GamesService
    {
        private readonly IMongoCollection<Game> _gamesCollection;
        private ResourceType image;

        public bool CompareDate(string date, string typeDate, int min, int max)
        {
            var titleDate = DateTime.Parse(date).Date;
            var currentDate = DateTime.Now.Date;
            int year = titleDate.Year - currentDate.Year;

            switch (typeDate.ToLower())
            {
                case "year":
                    if (year >= min && year <= max)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case "month":
                    int currentMonth = currentDate.Month;
                    int titleMonth = titleDate.Month;
                    if (titleMonth >= (currentMonth + min) && titleMonth <= (currentMonth + max) && year == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case "day":
                    int currentDay = currentDate.Day;
                    int titleDay = titleDate.Day;
                    if (titleDay >= (currentDay + min) && titleDay <= (currentDay + max) && year == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                default: return false;
            }
        }

        public GamesService(
            IOptions<GameInfoDatabaseSettings> gameInfoDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                gameInfoDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                gameInfoDatabaseSettings.Value.DatabaseName);

            _gamesCollection = mongoDatabase.GetCollection<Game>(
                gameInfoDatabaseSettings.Value.GamesCollectionName);
        }

        public async Task<List<Game>> GetAsync()
        {
            var games = await _gamesCollection.Find(_ => true).ToListAsync();
            BinarySearchTree<Game> searchTree = new BinarySearchTree<Game>();
            foreach (var game in games)
            {
                long unixDate = DateTimeOffset.Parse(game.ReleaseDate).ToUnixTimeMilliseconds();
                searchTree.Insert(unixDate, game);
            }

            var returnedNodes = searchTree.Traverse();

            List<Game> results = new List<Game>();
            foreach (var node in returnedNodes)
            {
                results.Add(node.Item);
            }

            return results;
        }

        public async Task<Game?> GetAsync(string id) =>
            await _gamesCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Game newGame) =>
            await _gamesCollection.InsertOneAsync(newGame);

        public async Task UpdateAsync(string id, Game updateGame) =>
            await _gamesCollection.ReplaceOneAsync(x => x.Id == id, updateGame);

        public async Task RemoveAsync(string id) =>
            await _gamesCollection.DeleteOneAsync(x => x.Id == id);

        public async Task<List<Game>> CarouselTitles()
        {
            List<Game> games = new();
            var randomNum = new Random();
            var titles = await _gamesCollection.Find(_ => true).ToListAsync();
            for (int i = 0; i < titles.Count; i++)
            {
                if (CompareDate(titles[i].ReleaseDate, "month", 1, 3))
                {
                    games.Add(titles[i]);
                }
            }

            if (games.Count <= 3)
            {
                while (games.Count < 5)
                {
                    var randomTitle = randomNum.Next(titles.Count);
                    if (!games.Contains(titles[randomTitle]))
                    {
                        games.Add(titles[randomTitle]);
                    }
                }
            }
            return games;
        }

        public async Task<List<Game>> GetTitles(string entry)
        {
            var list = await this.GetAsync();

            if (entry != "{Empty}")
            {
                Dictionary<string, Game> gameFinder = new Dictionary<string, Game>();
                TrieTree trieTree = new TrieTree();
                List<Game> games = new List<Game>();

                foreach (Game game in list)
                {
                    gameFinder.Add(game.GameTitle, game);
                    trieTree.Insert(game.GameTitle);
                }

                var foundTitles = trieTree.AutoComplete(entry);
                foreach (var title in foundTitles)
                {
                    gameFinder.TryGetValue(title, out Game? game);

                    if (game != null)
                    {
                        games.Add(game);
                    }
                }

                return games;
            }

            return list;
        }

        public async Task<List<Game>> SimilarTitles(string gameId)
        {
            List<Game> similarTitles = new();
            Game currentTitle = await _gamesCollection.Find(x => x.Id == gameId).FirstOrDefaultAsync();
            List<Game> allTitles = await _gamesCollection.Find(x => x.Id != gameId).ToListAsync();

            for (int i = 0; i < currentTitle.Tags.Length; i++)
            {
                for (int j = 0; j < allTitles.Count; j++)
                {
                    if (allTitles[j].Tags.Contains(currentTitle.Tags[i]) && !similarTitles.Contains(allTitles[j]))
                    {
                        similarTitles.Add(allTitles[j]);
                    }
                }
            }

            return similarTitles;
        }

        public async Task<List<string>> GetGenres(string genre)
        {
            List<string> genres = new();
            TrieTree trie = new TrieTree();
            List<Game> games = await this.GetAsync();

            for (int i = 0; i < games.Count; i++)
            {
                for (int j = 0; j < games[i].Tags.Length; j++)
                {
                    if (!genres.Contains(games[i].Tags[j]))
                    {
                        genres.Add(games[i].Tags[j]);
                    }
                }
            }

            if (genre == "{Empty}")
            {
                return genres;
            }

            foreach (string str in genres)
            {
                trie.Insert(str);
            }

            return trie.AutoComplete(genre);          
        }
    }
}
