﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoppingList.Controllers
{
    public class ShoppingListController : Controller
    {
        // GET: ShoppingList
        public ActionResult Index()
        {
            return View();
        }

        
    }
}