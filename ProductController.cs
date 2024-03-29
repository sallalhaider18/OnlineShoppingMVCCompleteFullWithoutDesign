﻿
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OnlineShopping.Models;
using PagedList;
using PagedList.Mvc;
namespace OnlineShopping.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private Entities db = new Entities();

        // GET: Product
        public ActionResult Index()
        {
            return View(db.Tbl_Product.ToList());
        }

        [HttpGet]
        public ActionResult Index(string search, int? i)
        {
            return View(db.Tbl_Product.Where(x => x.ProductName.Contains(search) || search == null).ToList().ToPagedList(i ?? 1, 3));
        }



        // GET: Product/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tbl_Product tbl_Product = db.Tbl_Product.Find(id);
            if (tbl_Product == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Product);
        }



        // GET: Product/Create
        [Authorize(Roles = "sallalhaider5@gmail.com")]
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "sallalhaider5@gmail.com")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tbl_Product tbl_Product)
        {
            //if (ModelState.IsValid)
            //{
            string fileName = Path.GetFileNameWithoutExtension(tbl_Product.ImageFile.FileName);
            string extension = Path.GetExtension(tbl_Product.ImageFile.FileName);
            fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
            tbl_Product.ProductImage = "~/Image/" + fileName;
            fileName = Path.Combine(Server.MapPath("~/Image/"), fileName);
            tbl_Product.ImageFile.SaveAs(fileName);
            using (Entities db = new Entities())
            {
                db.Tbl_Product.Add(tbl_Product);
                db.SaveChanges();

            }
            //Removes all the items from model state
            ModelState.Clear();
            return RedirectToAction("Index");


        }

        // GET: Product/Edit/5
        [Authorize(Roles = "sallalhaider5@gmail.com")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tbl_Product tbl_Product = db.Tbl_Product.Find(id);
            if (tbl_Product == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Product);
        }

        // POST: Product/Edit/5
        [Authorize(Roles = "sallalhaider5@gmail.com")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        //Bind here is used to provide information how model binding to parameters should occur
        public ActionResult Edit([Bind(Include = "ProductId,ProductName,CategoryId,IsActive,IsDelete,CreatedDate,ModifiedDate,Description,ProductImage,IsFeatured,Quantity")] Tbl_Product tbl_Product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_Product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbl_Product);
        }

        // GET: Product/Delete/5
        [Authorize(Roles = "sallalhaider5@gmail.com")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tbl_Product tbl_Product = db.Tbl_Product.Find(id);
            if (tbl_Product == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Product);
        }

        // POST: Product/Delete/5
        [Authorize(Roles = "sallalhaider5@gmail.com")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tbl_Product tbl_Product = db.Tbl_Product.Find(id);
            db.Tbl_Product.Remove(tbl_Product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [AllowAnonymous]
        public ActionResult AboutCompany()
        {
            return View();
        }
        public ActionResult AddToCart(int productId)
        {
            if (Session["cart"] == null)
            {
                List<Item> cart = new List<Item>();
                var product = db.Tbl_Product.Find(productId);
                cart.Add(new Item()
                {
                    Product = product
                    
                });
                Session["cart"] = cart;
            }
            else
            {
                List<Item> cart = (List<Item>)Session["cart"];
                var product = db.Tbl_Product.Find(productId);
                cart.Add(new Item()
                {
                    Product = product
                    
                });
                Session["cart"] = cart;
            }
            return Redirect("Index");
        }
        public ActionResult RemoveFromCart(int productId)
        {
            List<Item> cart = (List<Item>)Session["cart"];
            foreach (var item in cart)
            {
                if (item.Product.ProductId == productId)
                {
                    cart.Remove(item);
                    break;
                }
            }
            Session["cart"] = cart;
            return Redirect("Index");
        }
        public ActionResult Checkout()
        {
            return View();
        }

        // Release Unmanaged Resources
        // If disposing true then release both managed and unmanaged resources otherwise just release unmanaged resources
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
