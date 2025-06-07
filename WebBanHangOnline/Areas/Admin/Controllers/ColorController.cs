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
    public class ColorController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Size
        public ActionResult Index()
        {
            var items = db.Colors;
            return View(items);
        }
        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Color model)
        {
            if (ModelState.IsValid)
            {
                var existingRecord = db.Colors.Any(p => p.ColorName == model.ColorName);
                if (existingRecord)
                {
                    TempData["ErrorMessage"] = "Thêm thất bại. Màu này đã tồn tại!";
                }
                else
                {
                    db.Colors.Add(model);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Màu được thêm thành công!";
                }
                return RedirectToAction("Index");
            }
            return View();
        }
        public ActionResult Edit(int id)
        {
            var item = db.Colors.Find(id);
            return View(item);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Color model)
        {
            if (ModelState.IsValid)
            {
                var existingRecord = db.Colors.Any(p => p.ColorName == model.ColorName);
                if (existingRecord)
                {
                    TempData["ErrorMessage"] = "Sửa thất bại. Màu này đã tồn tại!";
                }
                else
                {
                    db.Colors.Attach(model);
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Màu được sửa thành công!";
                }
                return RedirectToAction("Index");
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.Colors.Find(id);
            if (item != null)
            {
                db.Colors.Remove(item);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
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
                        var obj = db.Colors.Find(Convert.ToInt32(item));
                        db.Colors.Remove(obj);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}