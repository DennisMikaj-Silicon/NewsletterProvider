using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class SubscribeEntity
{
	[Required]
	[Key]
	public string Email { get; set; } = null!;

	public bool IsSubscribed { get; set; } = false;
	public bool DailyNewsletter { get; set; }
	public bool AdvertisingUpdates { get; set; }
	public bool WeekinReview { get; set; }
	public bool EventUpdates { get; set; }
	public bool StartupsWeekly { get; set; }
	public bool Podcasts { get; set; }
}
