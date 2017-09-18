﻿using eShopServiceLibrary.Models;
using eShopServiceLibrary.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;

namespace eShopServiceLibrary
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CatalogService" in both code and config file together.
    public class CatalogService : ICatalogService
    {
        private eShopDatabaseEntities ents;

        public CatalogService()
        {
            ents = new eShopDatabaseEntities();
        }

        public CatalogService(eShopDatabaseEntities ents)
        {
            this.ents = ents;
        }

        public CatalogItem FindCatalogItem(int id)
        {
            return ents.CatalogItems.FirstOrDefault(x => x.Id == id);
        }
        public List<CatalogType> GetCatalogTypes()
        {
            return ents.CatalogTypes.ToList();
        }

        public List<CatalogBrand> GetCatalogBrands()
        {
            return ents.CatalogBrands.ToList();
        }

        public List<CatalogItem> GetCatalogItems()
        {
            return ents.CatalogItems.ToList();
        }

        public void CreateCatalogItem(CatalogItem catalogItem)
        {
            var maxId = ents.CatalogItems.Max(i => i.Id);
            catalogItem.Id = ++maxId;
            ents.CatalogItems.Add(catalogItem);
            ents.SaveChanges();
        }

        public void UpdateCatalogItem(CatalogItem catalogItem)
        {
            ents.Entry(catalogItem).State = EntityState.Modified;
            ents.SaveChanges();
        }

        public void RemoveCatalogItem(CatalogItem catalogItem)
        {
            ents.CatalogItems.Remove(catalogItem);
            ents.SaveChanges();
        }

        public void Dispose()
        {
            ents.Dispose();
        }

        public int GetAvailableStock(DateTime date, int catalogItemId)
        {
            CatalogItemsStock s = ents.CatalogItemsStocks.Where(x => x.CatalogItemId == catalogItemId).ToList().Where(y => y.Date.Date == date.Date).FirstOrDefault();
            if (s != null)
                return s.AvailableStock;
            else
                return -1;
        }

        public void CreateAvailableStock(CatalogItemsStock catalogItemsStock)
        {
            CatalogItemsStock s = ents.CatalogItemsStocks.Where(x => x.CatalogItemId == catalogItemsStock.CatalogItemId).ToList()
                    .Where(y => y.Date.Date == catalogItemsStock.Date.Date).FirstOrDefault();

            /* Overwrite the existing stock item for that date if we already have one for this item. Otherwise, make a new entry*/
            if(s != null)
            {
                s.AvailableStock = catalogItemsStock.AvailableStock;
                ents.Entry(s).State = EntityState.Modified;
                ents.SaveChanges();
            }
            else
            {
                var maxId = ents.CatalogItemsStocks.Max(i => i.StockId);
                catalogItemsStock.StockId = ++maxId;
                ents.CatalogItemsStocks.Add(catalogItemsStock);
                ents.SaveChanges();
            }
        }
    }
}
