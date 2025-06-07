using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class ProductQuantityController : Controller
    {
        // GET: Admin/ProductQuantity
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/ProductSize
        public ActionResult Index(int? page)
        {
            IEnumerable<ProductQuantity> items = db.ProductQuantities.OrderByDescending(x => x.id).ToList();
            var pageSize = 150;
            if (page == null)
            {
                page = 1;
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;
            return View(items);
        }
        public ActionResult Add()
        {

            ViewBag.Product = new SelectList(db.Products.ToList(), "id", "Title");
            ViewBag.Size = new SelectList(db.Sizes.ToList(), "id", "SizeName");
            ViewBag.Color = new SelectList(db.Colors.ToList(), "id", "ColorName");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(ProductQuantity model)
        {
            if (ModelState.IsValid)
            {
                var existingRecord = db.ProductQuantities.Any(p => p.ProductId == model.ProductId && p.SizeId == model.SizeId && p.ColorId == model.ColorId);
                if (existingRecord)
                {
                    TempData["ErrorMessage"] = "Sản phẩm với thông tin này đã tồn tại.";
                }
                else
                {
                    db.ProductQuantities.Add(model);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Sản phẩm được thêm thành công!";
                }
                return RedirectToAction("Index");
            }
            ViewBag.Product = new SelectList(db.Products.ToList(), "id", "Title");
            ViewBag.Size = new SelectList(db.Sizes.ToList(), "id", "SizeName");
            ViewBag.Color = new SelectList(db.Colors.ToList(), "id", "ColorName");

            return View(model);
        }
        public ActionResult Edit(int id)
        {
            ViewBag.Product = new SelectList(db.Products.ToList(), "id", "Title");
            ViewBag.Size = new SelectList(db.Sizes.ToList(), "id", "SizeName");
            ViewBag.Color = new SelectList(db.Colors.ToList(), "id", "ColorName");
            var item = db.ProductQuantities.Find(id);

            return View(item);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductQuantity model)
        {
            if (ModelState.IsValid)
            {
                var existingRecord = db.ProductQuantities
                   .Any(p => p.id != model.id && p.ProductId == model.ProductId && p.SizeId == model.SizeId && p.ColorId == model.ColorId);
                if (existingRecord)
                {
                    TempData["ErrorMessage"] = "Sản phẩm với thông tin màu sắc và kích thước này đã tồn tại.";
                }
                else
                {
                    db.ProductQuantities.Attach(model);
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Cập nhật thành công!";

                }
                return RedirectToAction("Index");
            }

            return View(model);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.ProductQuantities.Find(id);
            if (item != null)
            {
                db.ProductQuantities.Remove(item);
                db.SaveChanges();
                return Json(new { success = true, message = "Xóa thành công !" });
            }
            return Json(new { success = false, message = "Xóa thất bại !" });



        }
        public ActionResult DeleteAll(string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var items = ids.Split(',');
                if (items != null && items.Any())
                {
                    foreach (var item in items)
                    {
                        var obj = db.ProductQuantities.Find(Convert.ToInt32(item));
                        db.ProductQuantities.Remove(obj);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true, message = "Xóa thành công !" });
            }
            return Json(new { success = false, message = "Xóa thất bại !" });
        }
    }
}