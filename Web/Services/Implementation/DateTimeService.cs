using System.Globalization;
using Blazored.LocalStorage;
using Web.Services.Interfaces;

namespace Web.Services.Implementation
{
    public class DateTimeService : IDateTimeService
    {
        private readonly ILocalStorageService _localStorage;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DateTimeService> _logger;

        private string _userTimeZone = "America/Santiago"; // Default para Chile
        private CultureInfo _userCulture = new CultureInfo("es-CL");
        private readonly HashSet<DateTime> _holidays = new();

        // Configuración de horario laboral
        private readonly TimeOnly _workDayStart = new(8, 0);
        private readonly TimeOnly _workDayEnd = new(18, 0);

        public DateTimeService(
            ILocalStorageService localStorage,
            IConfiguration configuration,
            ILogger<DateTimeService> logger)
        {
            _localStorage = localStorage;
            _configuration = configuration;
            _logger = logger;

            InitializeAsync().ConfigureAwait(false);
        }

        private async Task InitializeAsync()
        {
            try
            {
                // Cargar zona horaria del usuario desde localStorage
                var savedTimeZone = await _localStorage.GetItemAsStringAsync("userTimeZone");
                if (!string.IsNullOrEmpty(savedTimeZone))
                {
                    _userTimeZone = savedTimeZone;
                }

                // Cargar cultura del usuario
                var savedCulture = await _localStorage.GetItemAsStringAsync("userCulture");
                if (!string.IsNullOrEmpty(savedCulture))
                {
                    _userCulture = new CultureInfo(savedCulture);
                }

                // Cargar días feriados desde configuración
                LoadHolidays();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al inicializar DateTimeService");
            }
        }

        private void LoadHolidays()
        {
            // Cargar feriados de Chile para el año actual
            var currentYear = DateTime.Now.Year;

            // Feriados fijos
            _holidays.Add(new DateTime(currentYear, 1, 1));   // Año Nuevo
            _holidays.Add(new DateTime(currentYear, 5, 1));   // Día del Trabajo
            _holidays.Add(new DateTime(currentYear, 5, 21));  // Glorias Navales
            _holidays.Add(new DateTime(currentYear, 6, 20));  // Día de los Pueblos Indígenas
            _holidays.Add(new DateTime(currentYear, 7, 16));  // Virgen del Carmen
            _holidays.Add(new DateTime(currentYear, 8, 15));  // Asunción de la Virgen
            _holidays.Add(new DateTime(currentYear, 9, 18));  // Primera Junta Nacional
            _holidays.Add(new DateTime(currentYear, 9, 19));  // Glorias del Ejército
            _holidays.Add(new DateTime(currentYear, 10, 12)); // Encuentro de Dos Mundos
            _holidays.Add(new DateTime(currentYear, 10, 31)); // Día de las Iglesias Evangélicas
            _holidays.Add(new DateTime(currentYear, 11, 1));  // Día de Todos los Santos
            _holidays.Add(new DateTime(currentYear, 12, 8));  // Inmaculada Concepción
            _holidays.Add(new DateTime(currentYear, 12, 25)); // Navidad
        }

        // Propiedades de fecha y hora actual
        public DateTime Now => ConvertToUserTimeZone(DateTime.UtcNow);
        public DateTime UtcNow => DateTime.UtcNow;
        public DateTime Today => Now.Date;
        public DateOnly TodayDate => DateOnly.FromDateTime(Today);
        public TimeOnly CurrentTime => TimeOnly.FromDateTime(Now);

        // Conversiones de zona horaria
        public DateTime ConvertToUserTimeZone(DateTime utcDateTime)
        {
            try
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(_userTimeZone);
                return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZone);
            }
            catch
            {
                // Fallback a hora local si hay error
                return utcDateTime.ToLocalTime();
            }
        }

        public DateTime ConvertToUtc(DateTime localDateTime)
        {
            try
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(_userTimeZone);
                return TimeZoneInfo.ConvertTimeToUtc(localDateTime, timeZone);
            }
            catch
            {
                return localDateTime.ToUniversalTime();
            }
        }

        public DateTime ConvertBetweenTimeZones(DateTime dateTime, string fromTimeZone, string toTimeZone)
        {
            try
            {
                var fromTz = TimeZoneInfo.FindSystemTimeZoneById(fromTimeZone);
                var toTz = TimeZoneInfo.FindSystemTimeZoneById(toTimeZone);
                var utc = TimeZoneInfo.ConvertTimeToUtc(dateTime, fromTz);
                return TimeZoneInfo.ConvertTimeFromUtc(utc, toTz);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al convertir entre zonas horarias");
                return dateTime;
            }
        }

        // Información de zona horaria
        public string GetUserTimeZone() => _userTimeZone;

        public TimeSpan GetUserTimeZoneOffset()
        {
            try
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(_userTimeZone);
                return timeZone.GetUtcOffset(DateTime.UtcNow);
            }
            catch
            {
                return TimeSpan.Zero;
            }
        }

        public List<TimeZoneInfo> GetAvailableTimeZones()
        {
            return TimeZoneInfo.GetSystemTimeZones().ToList();
        }

        // Formateo de fechas
        public string FormatDate(DateTime dateTime)
        {
            return ConvertToUserTimeZone(dateTime).ToString("d", _userCulture);
        }

        public string FormatDateTime(DateTime dateTime)
        {
            return ConvertToUserTimeZone(dateTime).ToString("g", _userCulture);
        }

        public string FormatTime(DateTime dateTime)
        {
            return ConvertToUserTimeZone(dateTime).ToString("t", _userCulture);
        }

        public string FormatDateLong(DateTime dateTime)
        {
            return ConvertToUserTimeZone(dateTime).ToString("D", _userCulture);
        }

        public string FormatDateShort(DateTime dateTime)
        {
            return ConvertToUserTimeZone(dateTime).ToString("d", _userCulture);
        }

        public string FormatRelativeTime(DateTime dateTime)
        {
            var localDateTime = ConvertToUserTimeZone(dateTime);
            var now = Now;
            var diff = now - localDateTime;

            if (Math.Abs(diff.TotalSeconds) < 60)
                return "hace unos segundos";
            if (Math.Abs(diff.TotalMinutes) < 60)
                return $"hace {Math.Abs((int)diff.TotalMinutes)} minutos";
            if (Math.Abs(diff.TotalHours) < 24)
                return $"hace {Math.Abs((int)diff.TotalHours)} horas";
            if (Math.Abs(diff.TotalDays) < 30)
                return $"hace {Math.Abs((int)diff.TotalDays)} días";
            if (Math.Abs(diff.TotalDays) < 365)
                return $"hace {Math.Abs((int)(diff.TotalDays / 30))} meses";

            return $"hace {Math.Abs((int)(diff.TotalDays / 365))} años";
        }

        public string FormatDuration(TimeSpan duration)
        {
            if (duration.TotalDays >= 1)
                return $"{(int)duration.TotalDays}d {duration.Hours}h {duration.Minutes}m";
            if (duration.TotalHours >= 1)
                return $"{(int)duration.TotalHours}h {duration.Minutes}m";

            return $"{duration.Minutes}m";
        }

        public string FormatDateRange(DateTime startDate, DateTime endDate)
        {
            var start = ConvertToUserTimeZone(startDate);
            var end = ConvertToUserTimeZone(endDate);

            if (start.Date == end.Date)
                return $"{start:d} {start:t} - {end:t}";

            return $"{start:d} - {end:d}";
        }

        // Formateo personalizado
        public string Format(DateTime dateTime, string format)
        {
            return ConvertToUserTimeZone(dateTime).ToString(format, _userCulture);
        }

        public string FormatWithCulture(DateTime dateTime, string format, string cultureName)
        {
            var culture = new CultureInfo(cultureName);
            return ConvertToUserTimeZone(dateTime).ToString(format, culture);
        }

        // Cálculos de fechas
        public DateTime AddBusinessDays(DateTime date, int businessDays)
        {
            var result = date;
            var daysToAdd = Math.Abs(businessDays);
            var increment = businessDays > 0 ? 1 : -1;

            while (daysToAdd > 0)
            {
                result = result.AddDays(increment);
                if (IsBusinessDay(result))
                {
                    daysToAdd--;
                }
            }

            return result;
        }

        public int GetBusinessDaysBetween(DateTime startDate, DateTime endDate)
        {
            var start = startDate.Date;
            var end = endDate.Date;
            var days = 0;
            var current = start;

            while (current <= end)
            {
                if (IsBusinessDay(current))
                {
                    days++;
                }
                current = current.AddDays(1);
            }

            return days;
        }

        public bool IsBusinessDay(DateTime date)
        {
            return !IsWeekend(date) && !IsHoliday(date);
        }

        public bool IsWeekend(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }

        public bool IsHoliday(DateTime date)
        {
            return _holidays.Contains(date.Date);
        }

        public DateTime GetNextBusinessDay(DateTime date)
        {
            var next = date.AddDays(1);
            while (!IsBusinessDay(next))
            {
                next = next.AddDays(1);
            }
            return next;
        }

        public DateTime GetPreviousBusinessDay(DateTime date)
        {
            var previous = date.AddDays(-1);
            while (!IsBusinessDay(previous))
            {
                previous = previous.AddDays(-1);
            }
            return previous;
        }

        // Períodos y rangos
        public DateTime GetStartOfDay(DateTime dateTime) => dateTime.Date;

        public DateTime GetEndOfDay(DateTime dateTime) => dateTime.Date.AddDays(1).AddTicks(-1);

        public DateTime GetStartOfWeek(DateTime dateTime)
        {
            var diff = (7 + (dateTime.DayOfWeek - DayOfWeek.Monday)) % 7;
            return dateTime.AddDays(-diff).Date;
        }

        public DateTime GetEndOfWeek(DateTime dateTime)
        {
            return GetStartOfWeek(dateTime).AddDays(7).AddTicks(-1);
        }

        public DateTime GetStartOfMonth(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }

        public DateTime GetEndOfMonth(DateTime dateTime)
        {
            return GetStartOfMonth(dateTime).AddMonths(1).AddTicks(-1);
        }

        public DateTime GetStartOfQuarter(DateTime dateTime)
        {
            var quarter = (dateTime.Month - 1) / 3;
            return new DateTime(dateTime.Year, quarter * 3 + 1, 1);
        }

        public DateTime GetEndOfQuarter(DateTime dateTime)
        {
            return GetStartOfQuarter(dateTime).AddMonths(3).AddTicks(-1);
        }

        public DateTime GetStartOfYear(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, 1, 1);
        }

        public DateTime GetEndOfYear(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, 12, 31, 23, 59, 59, 999);
        }

        // Comparaciones
        public bool IsSameDay(DateTime date1, DateTime date2) => date1.Date == date2.Date;

        public bool IsSameWeek(DateTime date1, DateTime date2)
        {
            return GetStartOfWeek(date1) == GetStartOfWeek(date2);
        }

        public bool IsSameMonth(DateTime date1, DateTime date2)
        {
            return date1.Year == date2.Year && date1.Month == date2.Month;
        }

        public bool IsSameYear(DateTime date1, DateTime date2) => date1.Year == date2.Year;

        public bool IsPast(DateTime dateTime) => dateTime < Now;

        public bool IsFuture(DateTime dateTime) => dateTime > Now;

        public bool IsToday(DateTime dateTime) => IsSameDay(dateTime, Today);

        public bool IsTomorrow(DateTime dateTime) => IsSameDay(dateTime, Today.AddDays(1));

        public bool IsYesterday(DateTime dateTime) => IsSameDay(dateTime, Today.AddDays(-1));

        // Parsing
        public DateTime? ParseDate(string dateString)
        {
            if (DateTime.TryParse(dateString, _userCulture, DateTimeStyles.None, out var date))
            {
                return date;
            }
            return null;
        }

        public DateTime? ParseDateTime(string dateTimeString)
        {
            if (DateTime.TryParse(dateTimeString, _userCulture, DateTimeStyles.None, out var dateTime))
            {
                return dateTime;
            }
            return null;
        }

        public bool TryParseDate(string dateString, out DateTime date)
        {
            return DateTime.TryParse(dateString, _userCulture, DateTimeStyles.None, out date);
        }

        public bool TryParseDateTime(string dateTimeString, out DateTime dateTime)
        {
            return DateTime.TryParse(dateTimeString, _userCulture, DateTimeStyles.None, out dateTime);
        }

        // Configuración regional
        public string GetUserDateFormat() => _userCulture.DateTimeFormat.ShortDatePattern;

        public string GetUserTimeFormat() => _userCulture.DateTimeFormat.ShortTimePattern;

        public string GetUserCulture() => _userCulture.Name;

        public async void SetUserCulture(string cultureName)
        {
            try
            {
                _userCulture = new CultureInfo(cultureName);
                await _localStorage.SetItemAsStringAsync("userCulture", cultureName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al establecer cultura {Culture}", cultureName);
            }
        }

        // Utilidades para proyectos
        public int CalculateProjectDuration(DateTime startDate, DateTime endDate, bool excludeWeekends = true)
        {
            if (excludeWeekends)
            {
                return GetBusinessDaysBetween(startDate, endDate);
            }

            return (int)(endDate.Date - startDate.Date).TotalDays + 1;
        }

        public DateTime CalculateProjectEndDate(DateTime startDate, int durationInDays, bool excludeWeekends = true)
        {
            if (excludeWeekends)
            {
                return AddBusinessDays(startDate, durationInDays - 1);
            }

            return startDate.AddDays(durationInDays - 1);
        }

        public List<DateTime> GetProjectMilestones(DateTime startDate, DateTime endDate, int numberOfMilestones)
        {
            var milestones = new List<DateTime>();
            var totalDays = (endDate - startDate).TotalDays;
            var interval = totalDays / (numberOfMilestones + 1);

            for (int i = 1; i <= numberOfMilestones; i++)
            {
                var milestoneDate = startDate.AddDays(interval * i);
                // Ajustar al siguiente día hábil si es necesario
                if (!IsBusinessDay(milestoneDate))
                {
                    milestoneDate = GetNextBusinessDay(milestoneDate);
                }
                milestones.Add(milestoneDate);
            }

            return milestones;
        }

        // Horario laboral
        public bool IsWithinWorkingHours(DateTime dateTime)
        {
            if (!IsBusinessDay(dateTime))
                return false;

            var time = TimeOnly.FromDateTime(dateTime);
            return time >= _workDayStart && time <= _workDayEnd;
        }

        public DateTime GetNextWorkingHour(DateTime dateTime)
        {
            var result = dateTime;

            // Si es fuera del horario laboral del mismo día
            if (IsBusinessDay(result) && TimeOnly.FromDateTime(result) > _workDayEnd)
            {
                result = GetNextBusinessDay(result).Date.Add(_workDayStart.ToTimeSpan());
            }
            // Si es antes del horario laboral del mismo día
            else if (IsBusinessDay(result) && TimeOnly.FromDateTime(result) < _workDayStart)
            {
                result = result.Date.Add(_workDayStart.ToTimeSpan());
            }
            // Si no es día hábil
            else if (!IsBusinessDay(result))
            {
                result = GetNextBusinessDay(result).Date.Add(_workDayStart.ToTimeSpan());
            }

            return result;
        }

        public TimeSpan GetWorkingHoursBetween(DateTime start, DateTime end)
        {
            var totalHours = TimeSpan.Zero;
            var current = start;

            while (current < end)
            {
                if (IsWithinWorkingHours(current))
                {
                    var dayEnd = current.Date.Add(_workDayEnd.ToTimeSpan());
                    var periodEnd = end < dayEnd ? end : dayEnd;
                    totalHours += periodEnd - current;
                }

                current = GetNextWorkingHour(current.AddDays(1));
            }

            return totalHours;
        }
    }
}