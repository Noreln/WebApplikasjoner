namespace BookingApplication.Models;

/// <summary> Data for My Bookings page: the search name and the matching bookings </summary>
public class MyBookingsViewModel
{
public string? Name { get; set; }
public List<BookingListItemViewModel> Bookings { get; set; } = [];
public string? ErrorMessage { get; set; }
public bool HasSearched => !string.IsNullOrWhiteSpace(Name);
}

