using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static ShoppingList.Data.IdentityModel;

namespace ShoppingList.Models
{
    public class ShoppingListItemsViewModel
    {
        [Required]
        public int ItemId { get; set; }
        public string Content { get; set; }
        public bool IsChecked { get; set; }
        public int ShoppingListId { get; set; }
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
        public DateTimeOffset CreatedUtc { get; set; }
        public DateTimeOffset? ModifiedUtc { get; set; }
        public string Color
        {
            get
            {
                using (var ctx = new ShoppingListDbContext())
                {
                    return ctx.Lists.Where(e => e.ListId == ShoppingListId).SingleOrDefault().Color;
                }
            }
        }
        public string ShoppingListName
        {
            get
            {
                using (var ctx = new ShoppingListDbContext())
                {
                    return ctx.Lists.Where(e => e.ListId == ShoppingListId).SingleOrDefault().ListName;
                }
            }
        }
    }
}