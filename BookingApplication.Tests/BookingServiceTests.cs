using BookingApplication.Models;
using BookingApplication.Repositories;
using BookingApplication.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace BookingApplication.Tests;

// Unit tests for BookingService (room search, overlap rule and capacity rule).
// The repositories are replaced with Moq mocks, so no database is used.
// 6 positive tests and 6 negative tests.
// The helper methods at the top build test data and set up what the mocks return. 
// Each [Fact] below then calls the service once and asserts the result.
public class BookingServiceTests
{




// Positive tests: 
// 1 - A valid booking in a free room is accepted with no errors.


// 2 - A booking that starts exactly when another one ends is accepted.


// 3 - A group exactly as large as the rooms capacity is accepted.


// 4 - Editing a booking does not make it clash with its own old time.


// 5 - With no bookings, the search returns every room that fits the group, with the right details. 


// 6 - A booking in one room does not remove a different room from the search results. 



// Negative tests

// 7


// 8


// 9


// 10


// 11


// 12

}