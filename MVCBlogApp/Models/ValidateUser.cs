using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCBlogApp.Models
{
    public class ValidateUser
    {

        public bool IsUserValid()
        {

            if (HttpContext.Current.Session["UserID"] != null)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

    }
}