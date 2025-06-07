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
    public class SizeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Size
        public ActionResult Index()
        {
            var items = db.Sizes;
            return View(items);
        }
        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Size model)
        {
            if (ModelState.IsValid)
            {
                var existingRecord = db.Sizes.Any(p => p.SizeName == model.SizeName);
                if (existingRecord)
                {
                    TempData["ErrorMessage"] = "Thêm thất bại. Size này đã tồn tại!";
                }
                else
                {
                    db.Sizes.Add(model);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Size được thêm thành công!";
                }

                return RedirectToAction("Index");
            }
            return View();
        }
        public ActionResult Edit(int id)
        {
            var item = db.Sizes.Find(id);
            return View(item);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Size model)
        {
            if (ModelState.IsValid)
            {
                var existingRecord = db.Sizes.Any(p => p.SizeName == model.SizeName);
                if (existingRecord)
                {
                    TempData["ErrorMessage"] = "Sửa thất bại. Size này đã tồn tại!";
                }
                else
                {
                    db.Sizes.Attach(model);
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Size được sửa thành công!";
                }
                return RedirectToAction("Index");
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.Sizes.Find(id);
            if (item != null)
            {
                db.Sizes.Remove(item);
                db.SaveChanges();
                return Json(new { success = true, message = "Xóa thành công !" });
            }
            return Json(new { success = false, message = "Xóa thất bại!" });
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
                        var obj = db.Sizes.Find(Convert.ToInt32(item));
                        db.Sizes.Remove(obj);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true, message = "Xóa thành công !" });
            }
            return Json(new { success = false, message = "Xóa thất bại!" });
        }
    }
}