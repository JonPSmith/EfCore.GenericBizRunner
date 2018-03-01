// // Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// // Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.EfClasses
{
    public class Book
    {
        public const int PromotionalTextLength = 200;

        private HashSet<Review> _reviews;
        private HashSet<BookAuthor> _authorsLink;

        public int BookId { get; private set; }          
        public string Title { get; private set; }        
        public string Description { get; private set; }  
        public DateTime PublishedOn { get; private set; }
        public string Publisher { get; private set; }
        public decimal OrgPrice { get; private set; } 
        public decimal ActualPrice { get; private set; } 
        [MaxLength(PromotionalTextLength)]
        public string PromotionalText { get; private set; }  
        public string ImageUrl { get; private set; }     

        //-----------------------------------------------
        //relationships

        public IEnumerable<Review> Reviews => _reviews?.ToList();
        public IEnumerable<BookAuthor> AuthorsLink => _authorsLink?.ToList();

        //-----------------------------------------------
        //ctors

        private Book() { } 

        public Book(string title, string description, 
            DateTime publishedOn, string publisher, 
            decimal price, string imageUrl,
            ICollection<Author> authors)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException(nameof(title)); 

            Title = title;
            Description = description;
            PublishedOn = publishedOn;
            Publisher = publisher;
            ActualPrice = price;
            OrgPrice = price;
            ImageUrl = imageUrl;
            _reviews = new HashSet<Review>();       //We add an empty list on create. I allows reviews to be added when building test data

            if (authors == null || authors.Count < 1)
                throw new ArgumentException("You must have at least one Author for a book", nameof(authors));
            byte order = 0;
            _authorsLink = new HashSet<BookAuthor>(authors.Select(a => new BookAuthor(this, a, order++)));
        }

        public void ChangePubDate(DateTime newDate)
        {
            PublishedOn = newDate;
        }

        public void AddReview(int numStars, string comment, string voterName, DbContext context = null) 
        {
            if (_reviews != null)    
            {
                _reviews.Add(new Review(numStars, comment, voterName));   
            }
            else if (context == null)
            {
                throw new ArgumentNullException("You must provide a context if the Reviews collection isn't valid.");
            }
            else if (context.Entry(this).IsKeySet)  
            {
                context.Add(new Review(numStars, comment, voterName, BookId));
            }
            else                                     
            {                                        
                throw new InvalidOperationException( 
                    "Could not add a new review.");  
            }
        }

        public void RemoveReview(Review review)                          
        {
            if (_reviews == null)
                throw new NullReferenceException("You must use .Include(p => p.Reviews) before calling this method.");

            _reviews.Remove(review); 
        }

        public string AddPromotion(decimal newPrice, string promotionalText)                  
        {
            if (promotionalText == null) 
                return "You must provide some text to go with the promotion";

            ActualPrice = newPrice;  
            PromotionalText = promotionalText; 
            return null; 
        }

        public void RemovePromotion() 
        {
            ActualPrice = OrgPrice; 
            PromotionalText = null; 
        }
    }

}