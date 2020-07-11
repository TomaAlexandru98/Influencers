using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Influencers.BusinessLogic.ViewModels
{
    public class AuthorViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? Votes { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }
        public string ImageSource { get; set; }
    }
}
