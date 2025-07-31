namespace Web.Services.Interfaces
{
    public interface IDateTimeService
    {
        // Fecha y hora actual
        DateTime Now { get; }
        DateTime UtcNow { get; }
        DateTime Today { get; }
        DateOnly TodayDate { get; }
        TimeOnly CurrentTime { get; }

        // Conversiones de zona horaria
        DateTime ConvertToUserTimeZone(DateTime utcDateTime);
        DateTime ConvertToUtc(DateTime localDateTime);
        DateTime ConvertBetweenTimeZones(DateTime dateTime, string fromTimeZone, string toTimeZone);

        // Información de zona horaria
        string GetUserTimeZone();
        TimeSpan GetUserTimeZoneOffset();
        List<TimeZoneInfo> GetAvailableTimeZones();

        // Formateo de fechas
        string FormatDate(DateTime dateTime);
        string FormatDateTime(DateTime dateTime);
        string FormatTime(DateTime dateTime);
        string FormatDateLong(DateTime dateTime);
        string FormatDateShort(DateTime dateTime);
        string FormatRelativeTime(DateTime dateTime);
        string FormatDuration(TimeSpan duration);
        string FormatDateRange(DateTime startDate, DateTime endDate);

        // Formateo personalizado
        string Format(DateTime dateTime, string format);
        string FormatWithCulture(DateTime dateTime, string format, string cultureName);

        // Cálculos de fechas
        DateTime AddBusinessDays(DateTime date, int businessDays);
        int GetBusinessDaysBetween(DateTime startDate, DateTime endDate);
        bool IsBusinessDay(DateTime date);
        bool IsWeekend(DateTime date);
        bool IsHoliday(DateTime date);
        DateTime GetNextBusinessDay(DateTime date);
        DateTime GetPreviousBusinessDay(DateTime date);

        // Períodos y rangos
        DateTime GetStartOfDay(DateTime dateTime);
        DateTime GetEndOfDay(DateTime dateTime);
        DateTime GetStartOfWeek(DateTime dateTime);
        DateTime GetEndOfWeek(DateTime dateTime);
        DateTime GetStartOfMonth(DateTime dateTime);
        DateTime GetEndOfMonth(DateTime dateTime);
        DateTime GetStartOfQuarter(DateTime dateTime);
        DateTime GetEndOfQuarter(DateTime dateTime);
        DateTime GetStartOfYear(DateTime dateTime);
        DateTime GetEndOfYear(DateTime dateTime);

        // Comparaciones
        bool IsSameDay(DateTime date1, DateTime date2);
        bool IsSameWeek(DateTime date1, DateTime date2);
        bool IsSameMonth(DateTime date1, DateTime date2);
        bool IsSameYear(DateTime date1, DateTime date2);
        bool IsPast(DateTime dateTime);
        bool IsFuture(DateTime dateTime);
        bool IsToday(DateTime dateTime);
        bool IsTomorrow(DateTime dateTime);
        bool IsYesterday(DateTime dateTime);

        // Parsing
        DateTime? ParseDate(string dateString);
        DateTime? ParseDateTime(string dateTimeString);
        bool TryParseDate(string dateString, out DateTime date);
        bool TryParseDateTime(string dateTimeString, out DateTime dateTime);

        // Configuración regional
        string GetUserDateFormat();
        string GetUserTimeFormat();
        string GetUserCulture();
        void SetUserCulture(string cultureName);

        // Utilidades para proyectos
        int CalculateProjectDuration(DateTime startDate, DateTime endDate, bool excludeWeekends = true);
        DateTime CalculateProjectEndDate(DateTime startDate, int durationInDays, bool excludeWeekends = true);
        List<DateTime> GetProjectMilestones(DateTime startDate, DateTime endDate, int numberOfMilestones);

        // Horario laboral
        bool IsWithinWorkingHours(DateTime dateTime);
        DateTime GetNextWorkingHour(DateTime dateTime);
        TimeSpan GetWorkingHoursBetween(DateTime start, DateTime end);
    }
}