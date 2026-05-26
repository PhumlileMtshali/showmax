using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Showmax.Server.Data;
using Showmax.Shared.DTOs;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;
using AppSubscription = Showmax.Shared.Models.Subscription;

namespace Showmax.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public PaymentsController(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
            StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];
        }

        // GET available plans
        [HttpGet("plans")]
        public IActionResult GetPlans()
        {
            var plans = new List<SubscriptionPlanDto>
            {
                new SubscriptionPlanDto
                {
                    Name = "Basic",
                    Price = 99,
                    Currency = "ZAR",
                    Description = "Essential streaming for one device",
                    Features = new List<string>
                    {
                        "Watch on 1 device at a time",
                        "HD available",
                        "Access to all movies and series",
                        "Cancel anytime"
                    }
                },
                new SubscriptionPlanDto
                {
                    Name = "Standard",
                    Price = 149,
                    Currency = "ZAR",
                    Description = "Great streaming for the family",
                    Features = new List<string>
                    {
                        "Watch on 2 devices at a time",
                        "Full HD available",
                        "Access to all movies and series",
                        "Download on 2 devices",
                        "Cancel anytime"
                    }
                },
                new SubscriptionPlanDto
                {
                    Name = "Premium",
                    Price = 199,
                    Currency = "ZAR",
                    Description = "Best experience for the whole family",
                    Features = new List<string>
                    {
                        "Watch on 4 devices at a time",
                        "Ultra HD + HDR available",
                        "Access to all movies and series",
                        "Download on 4 devices",
                        "Spatial audio",
                        "Cancel anytime"
                    }
                }
            };

            return Ok(plans);
        }

        // POST create checkout session
        [HttpPost("create-checkout-session")]
        [Authorize]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CreateCheckoutSessionDto dto)
        {
            var prices = new Dictionary<string, long>
            {
                { "Basic", 9900 },
                { "Standard", 14900 },
                { "Premium", 19900 }
            };

            if (!prices.ContainsKey(dto.PlanName))
                return BadRequest(new { Message = "Invalid plan selected." });

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = prices[dto.PlanName],
                            Currency = "zar",
                            Recurring = new SessionLineItemPriceDataRecurringOptions
                            {
                                Interval = "month"
                            },
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = $"Showmax {dto.PlanName}",
                                Description = $"Showmax {dto.PlanName} monthly subscription"
                            }
                        },
                        Quantity = 1
                    }
                },
                Mode = "subscription",
                SuccessUrl = dto.SuccessUrl,
                CancelUrl = dto.CancelUrl,
                Metadata = new Dictionary<string, string>
                {
                    { "userId", User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "" },
                    { "plan", dto.PlanName }
                }
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return Ok(new CheckoutSessionResponseDto
            {
                SessionId = session.Id,
                Url = session.Url
            });
        }

        // GET current user subscription
        [HttpGet("my-subscription")]
        [Authorize]
        public async Task<IActionResult> GetMySubscription()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var subscription = await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.UserId == userId && s.IsActive);

            if (subscription == null)
                return Ok(new UserSubscriptionDto { IsActive = false });

            return Ok(new UserSubscriptionDto
            {
                Plan = subscription.Plan,
                IsActive = subscription.IsActive,
                StartDate = subscription.StartDate,
                RenewalDate = subscription.RenewalDate
            });
        }

        // POST webhook from Stripe
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ParseEvent(json);

                if (stripeEvent.Type == "checkout.session.completed")
                {
                    var session = stripeEvent.Data.Object as Session;
                    if (session != null)
                    {
                        var userId = session.Metadata["userId"];
                        var plan = session.Metadata["plan"];

                        var existing = await _context.Subscriptions
                            .FirstOrDefaultAsync(s => s.UserId == userId);

                        if (existing != null)
                        {
                            existing.Plan = plan;
                            existing.IsActive = true;
                            existing.StartDate = DateTime.UtcNow;
                            existing.RenewalDate = DateTime.UtcNow.AddMonths(1);
                            existing.StripeSubscriptionId = session.SubscriptionId;
                        }
                        else
                        {
                            _context.Subscriptions.Add(new AppSubscription
                            {
                                UserId = userId,
                                Plan = plan,
                                IsActive = true,
                                StartDate = DateTime.UtcNow,
                                RenewalDate = DateTime.UtcNow.AddMonths(1),
                                StripeSubscriptionId = session.SubscriptionId
                            });
                        }

                        await _context.SaveChangesAsync();
                    }
                }

                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}