using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static ShoppingList.Data.IdentityModel;

namespace ShoppingList.Models
{
    public class ShoppingListItemCreateViewModel
    {
        [Key]
        public int ItemId { get; set; }
        public int ShoppingListId { get; set; }
        public string Content { get; set; }
        public enum PriorityLevel
        {
            [Display(Name = "It can wait")]
            ItCanWait = 0,
            [Display(Name = "Need it soon")]
            NeedItSoon = 1,
            [Display(Name = "Grab it now")]
            GrabItNow = 2
        }
        public PriorityLevel Priority { get; set; }

        public string GetShoppingListName(int id)
        {

            using (var ctx = new ShoppingListDbContext())
            {
                //var temp = ctx.Lists.Where(e => e.ListId == id).SingleOrDefault();
                //var temp1 = temp.ListName;
                //return "";
                return ctx.Lists.Where(e => e.ListId == id).SingleOrDefault().ListName;
            }
        }
    }
}