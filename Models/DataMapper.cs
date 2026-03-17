using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WildberriesDeductions.Models
{
    public static class DataMapper
    {
        public static DeductionGood ToDomain(this DefectGood raw)
        {
            return new DeductionGood
            {
                Shk = raw.Shk,
                DeductionPrice = raw.Price,
                OnHoldDate = raw.OnHoldDate,
                Pickpoint = raw.Pickpoint,
                TicketId = raw.TicketId,
                Reason = raw.Reason
            };
        }

        public static DeductionGood ToDomain(this ReplaceGood raw)
        {
            return new DeductionGood
            {
                Shk = raw.Shk,
                DeductionPrice = raw.Price,
                OnHoldDate = raw.OnHoldDate,
                Pickpoint = raw.Pickpoint,
                TicketId = raw.TicketId,
                Reason = raw.Reason
            };
        }

        public static DeductionGood ToDomain(this HandingGood raw)
        {
            return new DeductionGood
            {
                Shk = raw.Shk,
                DeductionPrice = raw.Price,
                OnHoldDate = raw.OnHoldDate,
                Pickpoint = raw.Pickpoint,
                TicketId = raw.TicketId,
                Reason = raw.Reason
            };
        }

        public static DeductionGood ToEnrich(this WbGoodSearchEntry raw, DeductionGood deduction)
        {
            if (raw == null || deduction == null) return deduction;

            deduction.Details = new GoodDetails
            {
                Name = raw.Details?.Specs?.Name ?? "Неизвестно",
                Brand = raw.Details?.Specs?.Brand ?? "-",
                Size = raw.Details?.Specs?.Size ?? "-",
                Cell = raw.Details?.Cell ?? 0,
                FullPrice = raw.Details?.Price ?? 0,

                StatusHistory = raw.StatusHistory?.Select(h => new StatusHistoryRecord
                {
                    Date = h.Date,
                    StatusId = h.StatusId,
                    StatusName = h.StatusName ?? "Неизвестно",
                    EmployeeFullName = h.EmployeeFullName?.Trim() ?? "Неизвестно"
                }).ToList() ?? new(),

                DefectMark = raw.DefectMark?.Select(m => new DefectMarkData
                {
                    Date = m.Date,
                    Comment = m.Comment ?? "",
                    Photos = m.Photos?.ToList() ?? new()
                }).ToList() ?? new()
            };

            return deduction;
        }
    }
}
