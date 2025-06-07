using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;
using System.Web.Mvc;
using System.Globalization;
using System.Data.Entity;


using PagedList;
namespace WebBanHangOnline.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin, Employee")]
    public class OrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Order
        public ActionResult Index(int? page,string searchtext)
        {
            var items = db.Orders.OrderByDescending(x => x.CreatedDate).AsQueryable();

            if (!string.IsNullOrEmpty(searchtext))
            {
                // Ánh xạ từ khóa thành giá trị số tương ứng
                int? status = null;
                switch (searchtext.ToLower())
                {
                    case "đã hủy":
                        status = 4; // Giả sử "Đã hủy" tương ứng với 4
                        break;
                    case "hoàn thành":
                        status = 3; // Giả sử "Hoàn thành" tương ứng với 3
                        break;
                    case "đã thanh toán":
                        status = 2; // Giả sử "Đã thanh toán" tương ứng với 2
                        break;
                    case "chưa thanh toán":
                        status = 1; // Giả sử "Chưa thanh toán" tương ứng với 1
                        break;
                }
                if (status.HasValue)
                {
                    items = items.Where(o => o.Status == status.Value);
                }
                else
                {
                    // Nếu không ánh xạ được thì có thể tìm kiếm theo tên khách hàng hoặc mã đơn hàng (tuỳ thuộc vào yêu cầu của bạn)
                    items = items.Where(o => o.CustomerName.Contains(searchtext) || o.Code.Contains(searchtext));
                }
            }
                if (page== null)
            {
                page = 1;
            }
            var pageNumber = page ?? 1;
            var pageSize = 10;
            ViewBag.PageSize = pageSize;
            ViewBag.Page = pageNumber;
            return View(items.ToPagedList(pageNumber,pageSize));
        }
        public ActionResult View(int id)
        {
            var item = db.Orders.Find(id);
            return View(item);
        }
        public ActionResult Partial_SanPham(int id)
        {
            var items = db.OrderDetails.Where(x => x.OrderId == id).ToList();
            return PartialView(items);
            
        }
        [HttpPost]
        public ActionResult UpdateTT(int id, int trangthai)
        {
            var item = db.Orders.Find(id);
            if(item != null)
            {
                db.Orders.Attach(item);
                item.Status = trangthai;
                db.Entry(item).Property(x => x.Status).IsModified = true;
                db.SaveChanges();
                return Json(new { message = "Cập nhật thành công", Success = true });
            }
            return Json(new { message = "Lỗi!!", Success = false });
        }
        [Authorize(Roles ="Admin")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.Orders.Find(id);
            if (item != null)
            {
                item.Status = 4;
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}