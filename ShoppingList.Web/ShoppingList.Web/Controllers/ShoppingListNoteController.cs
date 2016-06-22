using Microsoft.AspNet.Identity;
using ShoppingList.Models;
using ShoppingList.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ShoppingList.Data.IdentityModel;

namespace ShoppingList.Web.Controllers
{
    [Authorize]
    public class ShoppingListNoteController : ShoppingListItemController
    {
        private readonly Lazy<ShoppingListNoteService> _svc;

        public ShoppingListNoteController()
        {
            _svc =
                new Lazy<ShoppingListNoteService>(
                    () =>
                    {
                        var userId = Guid.Parse(User.Identity.GetUserId());
                        return new ShoppingListNoteService();
                    });
        }

        public ActionResult NoteIndex(int id)
        {
            var Notes = _svc.Value.GetNotes(id);
            using (var ctx = new ShoppingListDbContext())
            {
                ViewBag.ShoppingListId = ctx.Items.Where(e => e.ItemId == id).SingleOrDefault().ShoppingListId;
            };
            return View(Notes);
        }

        [HttpGet]
        public ActionResult CreateNote()
        {
                var vm = new ShoppingListNoteCreateViewModel();
                return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNote(ShoppingListNoteCreateViewModel vm, int id)
        {
            if (!ModelState.IsValid) return View(vm);

            if (!_svc.Value.CreateNote(vm, id))
            {
                ModelState.AddModelError("", " Unable to add note.");
                return View(vm);
            }
            return RedirectToAction("NoteIndex", new { id = Url.RequestContext.RouteData.Values["id"] });
        }

        public ActionResult DeleteAllNotes()
        {
            _svc.Value.DeleteAllNotes();
            return RedirectToAction("ItemIndex", "ShoppingListItem");
        }

        [HttpGet]
        [ActionName("DeleteNote")]
        public ActionResult DeleteGet(int id, int ShoppingListItemId)
        {
            try
            {
                var detail = _svc.Value.GetNoteById(id, ShoppingListItemId);

                return View(detail);
            }
            catch (ArgumentException e)
            {
                return RedirectToAction("NoteIndex", "ShoppingListNote", null);
            }
        }

        [HttpPost]
        [ActionName("DeleteNote")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(int id, int ShoppingListItemId)
        {
            _svc.Value.DeleteNote(id, ShoppingListItemId);
            return RedirectToAction("NoteIndex", new { id = Url.RequestContext.RouteData.Values["id"] });
        }
    }
}