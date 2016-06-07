﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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
    }
}