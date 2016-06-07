﻿using ShoppingList.Data;
using ShoppingList.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static ShoppingList.Data.IdentityModel;
using static ShoppingList.Models.ShoppingListItemsViewModel;

namespace ShoppingList.Services
{
    public class ShoppingListItemService
    {
        public ShoppingListItemService()
        { }

        public IEnumerable<ShoppingListItemsViewModel> GetItems(int id)
        {
            using (var ctx = new ShoppingListDbContext())
            {
                return
                    ctx
                    .Items
                    .Where(e => e.ShoppingListId == id)
                    .Select(
                        e =>
                            new ShoppingListItemsViewModel
                            {
                                ItemId = e.ItemId,
                                ShoppingListId = e.ShoppingListId,
                                Content = e.Content,
                                IsChecked = e.IsChecked,
                                Priority = (ShoppingListItemsViewModel.PriorityLevel)e.Priority,
                                CreatedUtc = e.CreatedUtc,
                                ModifiedUtc = e.ModifiedUtc
                            })
                        .ToArray();
            }
        }

        public ShoppingListItemsViewModel GetItemById(int itemId, int listId)
        {
            ShoppingListItemEntity entity;
            using (var ctx = new ShoppingListDbContext())
            {
                entity =
                    ctx
                    .Items
                    .SingleOrDefault(e => e.ItemId == itemId && e.ShoppingListId == listId);
            }
            return
                new ShoppingListItemsViewModel
                {
                    ItemId = entity.ItemId,
                    ShoppingListId = entity.ShoppingListId,
                    Content = entity.Content,
                    IsChecked = entity.IsChecked,
                };
        }

        public bool CreateItem(ShoppingListItemCreateViewModel vm, int id)
        {
            using (var ctx = new ShoppingListDbContext())
            {
                var entity =
                    new ShoppingListItemEntity
                    {
                        ShoppingListId = id,
                        Content = vm.Content,
                        Priority = (ShoppingListItemEntity.PriorityLevel)vm.Priority,
                        CreatedUtc = DateTimeOffset.UtcNow,
                    };

                ctx.Items.Add(entity);

                return ctx.SaveChanges() == 1;
            }
        }

        public bool EditItem(ShoppingListItemEditViewModel vm)
        {
            using (var ctx = new ShoppingListDbContext())
            {
                var entity =
                    ctx
                    .Items
                    .SingleOrDefault(e => e.ItemId == vm.ItemId && e.ShoppingListId == vm.ShoppingListId);

                entity.ItemId = vm.ItemId;
                entity.ShoppingListId = vm.ShoppingListId;
                entity.Content = vm.Content;
                entity.IsChecked = vm.IsChecked;
                entity.Priority = (ShoppingListItemEntity.PriorityLevel)vm.Priority;
                entity.ModifiedUtc = DateTimeOffset.UtcNow;

                return ctx.SaveChanges() == 1;
            }
        }

        public bool DeleteItem(int? itemId, int? listId)
        {
            using (var ctx = new ShoppingListDbContext())
            {
                var entity =
                    ctx
                    .Items
                    .Single(e => e.ItemId == itemId && e.ShoppingListId == listId);
                ctx.Items.Remove(entity);
                return ctx.SaveChanges() == 1;
            }
        }

        public bool DeleteAllItems()
        {
            using (var ctx = new ShoppingListDbContext())
            {
                foreach (ShoppingListItemEntity item in ctx.Items)
                {
                    ctx.Items.Remove(item);
                }
                return ctx.SaveChanges() == 1;
            }
        }

        public bool DeleteCheckedIds(int[] CheckedIds)
        {
            using (var ctx = new ShoppingListDbContext())
            {
                foreach(var item in ctx.Items)
                {
                    foreach(var id in CheckedIds)
                    {
                        if (item.ItemId == id)
                            ctx.Items.Remove(item);
                    }
                }
                return ctx.SaveChanges() == 1;
            }
        }
    }
}