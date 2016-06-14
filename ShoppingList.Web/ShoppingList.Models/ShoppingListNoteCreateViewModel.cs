using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static ShoppingList.Data.IdentityModel;

namespace ShoppingList.Models
{
    public class ShoppingListNoteCreateViewModel
    {
        [Key]
        public int NoteId { get; set; }
        public int ShoppingListItemId { get; set; }
        [Required]
        public string Body { get; set; }
        public DateTimeOffset CreatedUtc { get; set; }
        public string GetItemtContent(int id)
        {

            using (var ctx = new ShoppingListDbContext())
            {
                return ctx.Items.Where(e => e.ItemId == id).SingleOrDefault().Content;
            }
        }
    }
}