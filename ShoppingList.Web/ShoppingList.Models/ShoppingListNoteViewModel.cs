using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static ShoppingList.Data.IdentityModel;

namespace ShoppingList.Models
{
    public class ShoppingListNoteViewModel
    {
        [Key]
        public int NoteId { get; set; }
        public int ShoppingListItemId { get; set; }
        public string Body { get; set; }

        public string ItemContent
        {
            get
            {
                using (var ctx = new ShoppingListDbContext())
                {
                    return ctx.Items.Where(e => e.ItemId == ShoppingListItemId).SingleOrDefault().Content;
                }
            }
        }
    }
}