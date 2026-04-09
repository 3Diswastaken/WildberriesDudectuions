namespace WildberriesDeductions.Wildberries.Models
{
    public static class DataMapper
    {
        private static string ToShort(this string raw)
        {
            string[] words = raw.Split(' ');

            string result = "";

            foreach (string word in words)
            {
                if (string.IsNullOrWhiteSpace(word)) continue; // пропускаем пустые места, если случайно два пробела
                if (word == "г" || word == "Улица" || word == "Калуга") continue;

                char firstChar = word[0];

                if (firstChar >= '0' && firstChar <= '9')
                    result += word;
                else
                    result += char.ToUpper(firstChar);
            }

            return result;
        }
        public static DeductionGood ToDomain(this DefectGood raw, string pickpointName)
        {
            return new DeductionGood
            {
                Shk = raw.Shk,
                DeductionPrice = raw.Price,
                OnHoldDate = raw.OnHoldDate,
                PickpointName = pickpointName,
                ShortPickpointName = pickpointName.ToShort(),
                TicketId = raw.TicketId,
                Reason = raw.Reason
            };
        }

        public static DeductionGood ToDomain(this ReplaceGood raw, string pickpointName)
        {
            return new DeductionGood
            {
                Shk = raw.Shk,
                DeductionPrice = raw.Price,
                OnHoldDate = raw.OnHoldDate,
                PickpointName = pickpointName,
                ShortPickpointName = pickpointName.ToShort(),
                TicketId = raw.TicketId,
                Reason = raw.Reason
            };
        }

        public static DeductionGood ToDomain(this HandingGood raw, string pickpointName)
        {
            return new DeductionGood
            {
                Shk = raw.Shk,
                DeductionPrice = raw.Price,
                OnHoldDate = raw.OnHoldDate,
                PickpointName = pickpointName,
                ShortPickpointName = pickpointName.ToShort(),
                TicketId = raw.TicketId,
                Reason = raw.Reason
            };
        }

        public static Ticket ToDomain(this TicketDto raw, string pickpointName)
        {
            var rawLogs = raw.Logs;

            DateTimeOffset? onHoldDate = null;
            if (rawLogs?.Any() == true && rawLogs[^1].FullName.Trim() != "Руслан")
                onHoldDate = rawLogs[^1].CreateDate.Value.AddDays(2);


            return new Ticket
            {
                Id = raw.Id,
                CreateDate = raw.CreateDate,
                FinishDate = raw.FinishDate,
                OnHoldDate = onHoldDate,
                PickpointName = pickpointName,
                ShortPickpointName = pickpointName.ToShort(),
                Description = raw.Description,
                State = raw.State,
                Reason = raw.Reason,
                TypeName = raw.TypeName,
                Logs = rawLogs.Select(l => new TicketHistoryRecord
                {
                    CreateDate = l.CreateDate,
                    FullName = l.FullName,
                    Message = l.Message
                }).ToList()
            };
        }

        public static DeductionGood ToEnrich(this DeductionGood deduction, WbGoodSearchEntry raw)
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
