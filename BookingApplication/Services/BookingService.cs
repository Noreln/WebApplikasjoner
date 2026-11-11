using BookingApplication.Models;
using BookingApplication.Repositories;


namespace BookingApplication.Services
{
public class BookingService: IBookingService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IBookingRepository _bookingRepository;
    
    public BookingService(IRoomRepository roomRepository, IBookingRepository bookingRepository)
        {
            _roomRepository=roomRepository;
            _bookingRepository=bookingRepository;
        } 
    private bool HasOverlap(IEnumerable<Booking>bookings,int roomId, DateTime date,TimeOnly startTime,TimeOnly endTime)
        {
    return bookings.Any(b=> b.RoomId == roomId && b.Date==date && b.StartTime<endTime && b.EndTime>startTime);      
        }

    public async Task<List<AvailableRoomViewModel>> SearchRooms(DateTime date,TimeOnly startTime,TimeOnly endTime,int people)
        {      
    var rooms= await _roomRepository.GetRoomsByMinCapacity(people);
    var bookings= await _bookingRepository.GetByDateAsync(date);
    return rooms
    .Where(r => !HasOverlap(bookings,r.Id,date,startTime,endTime))
    .Select(r => new AvailableRoomViewModel
    {
    Id=r.Id,
    Name=r.Name,
    Location=r.Location,
    Description=r.Description,
    Capacity=r.Capacity
    })
    .ToList();
        }
}
}