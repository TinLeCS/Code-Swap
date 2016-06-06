﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShoppingList.Models
{
    public class ShoppingListNoteViewModel
    {
        [Key]
        public int NoteId { get; set; }
        public int ShoppingListItemId { get; set; }
        public string Body { get; set; }
    }
}