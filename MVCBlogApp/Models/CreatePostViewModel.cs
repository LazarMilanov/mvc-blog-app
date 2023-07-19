using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCBlogApp.Models
{
    public class CreatePostViewModel
    {
        [Required]
        public string Title { get; set; }

        [AllowHtml]
        [Required]
        public string Content { get; set; }
    }
}