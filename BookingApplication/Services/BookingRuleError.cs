namespace BookingApplication.Services;

///<summary> One broken booking rule: which form field it belongs to and the message to show. </summary>

public record BookingRuleError(string Field, string Message);