namespace Showmax.Shared.DTOs
{
    public class SubscriptionPlanDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Currency { get; set; } = "ZAR";
        public string Description { get; set; } = string.Empty;
        public List<string> Features { get; set; } = new();
    }

    public class CreateCheckoutSessionDto
    {
        public string PlanName { get; set; } = string.Empty;
        public string SuccessUrl { get; set; } = string.Empty;
        public string CancelUrl { get; set; } = string.Empty;
    }

    public class CheckoutSessionResponseDto
    {
        public string SessionId { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }

    public class UserSubscriptionDto
    {
        public string Plan { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? RenewalDate { get; set; }
    }
}