using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;
namespace WebBanHangOnline.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ThongKeDoanhThuController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/ThongKeDoanhThu
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult ThongKeDoanhThu(string fromDate, string toDate)
        {
            if ((!string.IsNullOrEmpty(fromDate) && fromDate.Length != 10) ||
          (!string.IsNullOrEmpty(toDate) && toDate.Length != 10))
            {
                return Json(new { Error = "Vui lòng nhập đầy đủ ngày tháng theo định dạng ngày/tháng/năm" }, JsonRequestBehavior.AllowGet);
            }
            var query = from order in db.Orders
                        join orderdetail in db.OrderDetails on order.id equals orderdetail.OrderId
                        join product in db.Products on orderdetail.ProductId equals product.id
                        select new
                        {
                            CreatedDate = order.CreatedDate,
                            Quantity = orderdetail.Quantity,
                            Price = orderdetail.Price,
                            OriginalPrice = product.OriginalPrice
                        };
            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime startDate;
                if (!DateTime.TryParseExact(fromDate, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out startDate))
                {
                    return Json(new { Error = "Ngày bắt đầu không hợp lệ. Vui lòng nhập theo định dạng dd/MM/yyyy" }, JsonRequestBehavior.AllowGet);
                }
                query = query.Where(x => x.CreatedDate >= startDate);
            }
            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime endDate;
                if (!DateTime.TryParseExact(toDate, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out endDate))
                {
                    return Json(new { Error = "Ngày kết thúc không hợp lệ. Vui lòng nhập theo định dạng dd/MM/yyyy" }, JsonRequestBehavior.AllowGet);
                }
                query = query.Where(x => x.CreatedDate <= endDate);
            }
            var result = query.GroupBy(x => DbFunctions.TruncateTime(x.CreatedDate)).Select(x => new
            {
                Date = x.Key.Value,
                TotalBuy = x.Sum(y => y.Quantity * y.OriginalPrice),
                TotalSell = x.Sum(y => y.Quantity * y.Price)
            }).Select(x => new
            {
                Date = x.Date,
                DoanhThu = x.TotalSell,
                LoiNhuan = x.TotalSell - x.TotalBuy
            });
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);

        }
    }
}