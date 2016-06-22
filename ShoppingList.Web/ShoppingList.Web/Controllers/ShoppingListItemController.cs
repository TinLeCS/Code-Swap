using Microsoft.AspNet.Identity;
using ShoppingList.Models;
using ShoppingList.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoppingList.Web.Controllers
{
    [Authorize]
    public class ShoppingListItemController : ShoppingListController
    {
        private const int MB = 1024 * 1024;
        private readonly Lazy<ShoppingListItemService> _svc;

        private readonly Lazy<ShoppingListService> _svc2;

        public ShoppingListItemController()
        {
            _svc =
                new Lazy<ShoppingListItemService>(
                    () =>
                    {
                        var userId = Guid.Parse(User.Identity.GetUserId());
                        return new ShoppingListItemService();
                    });
        }

        [HttpGet]
        public ActionResult ItemIndex(string sortOrder, string currentFilter, int id)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ContentsSortOrder = String.IsNullOrEmpty(sortOrder) ? "ContentsDesc" : "";
            ViewBag.PrioritySortOrder = sortOrder == "Priority" ? "PriorityDesc" : "Priority";
            ViewBag.IsCheckedSortOrder = sortOrder == "IsChecked" ? "IsCheckedDesc" : "IsChecked";
            var ShoppingListItems = _svc.Value.GetItems(id);
            var Items = from items
                        in ShoppingListItems
                        select items;
            switch (sortOrder)
            {
                case "ContentsDesc":
                    ShoppingListItems = ShoppingListItems.OrderByDescending(s => s.Content);
                    break;
                case "Priority":
                    ShoppingListItems = ShoppingListItems.OrderBy(s => s.Priority);
                    break;
                case "PriorityDesc":
                    ShoppingListItems = ShoppingListItems.OrderByDescending(s => s.Priority);
                    break;
                case "IsChecked":
                    ShoppingListItems = ShoppingListItems.OrderBy(s => s.IsChecked);
                    break;
                case "IsCheckedDesc":
                    ShoppingListItems = ShoppingListItems.OrderByDescending(s => s.IsChecked);
                    break;
                default:
                    ShoppingListItems = ShoppingListItems.OrderBy(s => s.Content);
                    break;
            }
            return View("ItemIndex", ShoppingListItems);
        }

        [HttpGet]
        public ActionResult CreateItem()
        {
            try
            {
                var vm = new ShoppingListItemCreateViewModel();

                return View(vm);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("Argument Exception. Redirecting to Shopping List.");
                return RedirectToAction("Index", "ShoppingList", null);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateItem(ShoppingListItemCreateViewModel vm, int id)
        {
            if (!ModelState.IsValid) return View(vm);

            var file = Request.Files["File"];
            var allowedExtensions = new[] { ".jpg", ".png", ".jpeg" };
            var extension = Path.GetExtension(file.FileName);

            if (file.ContentLength > 0 && !allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("", "You are only able to upload a JPG, JPEG, PNG file.");
                return View(vm);
            }

            if (file.ContentLength > 4 * MB)
            {
                ModelState.AddModelError("", "The image size cannot exceed 4 MB.");
                return View(vm);
            }

            var result = _svc.Value.CreateItem(vm, id);

            if (result[0] != 1)
            {
                ModelState.AddModelError("", " Unable to add item.");
                return View(vm);
            }

            // Get the uploaded image from the Files collection
            if (file != null && file.ContentLength > 0)
            {
                // create an Item folder inside ~/Content folder
                Directory.CreateDirectory(Server.MapPath("~/Content/Item"));

                // store the image inside ~/Content/Item folder
                // a little trick to prevent file stored as the same name with a different type
                var path = GetDefaultPath(result[1]);

                var image = Image.FromStream(file.InputStream, true, true);

                image = ResizeImage(image, 200, 200);
                image.Save(path);
            }
            
            return RedirectToAction("ItemIndex", new { id = Url.RequestContext.RouteData.Values["id"] });
            
        }

        [HttpGet]
        public ActionResult EditItem(int id, int ShoppingListId)
        {
            var update = _svc.Value.GetItemById(id, ShoppingListId);
            var list =
                new ShoppingListItemEditViewModel
                {
                    ItemId = update.ItemId,
                    ShoppingListId = update.ShoppingListId,
                    Content = update.Content,
                    Priority = (ShoppingListItemEditViewModel.PriorityLevel)update.Priority
                };
            return View(list);
        }

        [HttpPost]
        public ActionResult EditItem(ShoppingListItemEditViewModel vm, int id)
        {
            if (!ModelState.IsValid) return View(vm);

            var file = Request.Files["File"];
            var allowedExtensions = new[] { ".jpg", ".png", ".jpeg" };
            var extension = Path.GetExtension(file.FileName);

            if (file.ContentLength > 0 && !allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("", "You are only able to upload a JPG, JPEG, PNG file.");
                return View(vm);
            }

            if (file.ContentLength > 4 * MB)
            {
                ModelState.AddModelError("", "The image size cannot exceed 4 MB.");
                return View(vm);
            }

            var result = _svc.Value.EditItem(vm);
            if (result[0] != 1)
            {
                ModelState.AddModelError("", "Unable to update note.");
                return View(vm);
            }

            // Get the uploaded image from the Files collection
            if (file != null && file.ContentLength > 0)
            {
                // create an Item folder inside ~/Content folder
                Directory.CreateDirectory(Server.MapPath("~/Content/Item"));

                // store the image inside ~/Content/Item folder
                // a little trick to prevent file stored as the same name with a different type
                var path = GetDefaultPath(result[1]);

                var image = Image.FromStream(file.InputStream, true, true);

                image = ResizeImage(image, 200, 200);
                image.Save(path);
            }

            return RedirectToAction("ItemIndex", new { id = vm.ShoppingListId });
        }

        public ActionResult DeleteAllItems()
        {
            _svc.Value.DeleteAllItems(Server.MapPath("~/Content/Item"));
            return RedirectToAction("Index", "ShoppingList");
        }

        [HttpGet]
        [ActionName("DeleteItem")]
        public ActionResult DeleteGet(int id, int ShoppingListId)
        {
            try
            {
                var detail = _svc.Value.GetItemById(id, ShoppingListId);

                return View(detail);
            }
            catch (ArgumentException e)
            {
                return RedirectToAction("ItemIndex", "ShoppingList", null);
            }
        }

        [HttpPost]
        [ActionName("DeleteItem")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(int id, int ShoppingListId)
        {
            _svc.Value.DeleteItem(id, ShoppingListId);
            DeleteFile(id);
            return RedirectToAction("ItemIndex/" + ShoppingListId);
        }

        public ActionResult DeleteChecked(int id, int[] CheckedIds)
        {
            if (CheckedIds != null && CheckedIds.Length > 0)
                _svc.Value.DeleteCheckedIds(CheckedIds, Server.MapPath("~/Content/Item"));
            return RedirectToAction("ItemIndex/" + id);
        }

        public string GetDefaultPath(int id)
        {
            return Path.Combine(Server.MapPath("~/Content/Item"), id + ".jpg");
        }

        public void DeleteFile(int id)
        {
            string path = GetDefaultPath(id);
            FileInfo file = new FileInfo(path);
            if (file.Exists)
            {
                file.Delete();
            }
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
