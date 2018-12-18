﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Model;
using InternalApi.DataAccess;
using InternalApi.DataManagement.IDataManagement;

namespace InternalApi.DataManagement
{
    internal class ElementDM: IElementDM
    {
        private readonly ElementDa _elementDa;

        internal ElementDM()
        {
            _elementDa = new ElementDa();
        }

        public List<Element> GetInvoiceElements(int id)
        {
            using (var db = new DatabaseContext())
            {
                return _elementDa.GetInvoiceElements(db, id).GroupBy(x =>
                        new
                        {
                            x.Item.SerNumber,
                            x.Item.Price,
                            x.Item.Product_ID
                        })
                    .Select(g => new
                    {
                        item = g.Select(c => c).FirstOrDefault(),
                        count = g.Count()
                    })
                    .Select(x => { x.item.Item.Quantity = x.count; return x.item; })
                    .ToList();
            }
        }
    } 
}