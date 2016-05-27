﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShoppingList.Models.ShoppingListModels
{
    public class ShoppingListCreateViewModel
    {

        [Required]
        public int ListId { get; set; }

        [Required]
        [DisplayName("Name")]
        public string ListName { get; set; }
        [Required]
        [RegularExpression("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Color must be in hex format.")]
        public string Color { get; set; }

    }
}