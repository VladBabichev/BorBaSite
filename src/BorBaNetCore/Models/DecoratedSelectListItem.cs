using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;


namespace BorBaNetCore.Models
{
	public class DecoratedSelectListItem : SelectListItem
	{
		public DecoratedSelectListItem()
			: base()
		{
		}

		public DecoratedSelectListItem(string value, string text, bool selected, string renderClass, params string[] extraData)
			: base()
		{
			Value = value;
			Text = text;
			Selected = selected;
			RenderClass = renderClass;
			ExtraData = extraData;
		}

		public static DecoratedSelectListItem GetFilterTriggerItems(string triggerElemendId, string dependantElementId, int triggerId, IEnumerable<int> dependantIDs)
		{
			return new DecoratedSelectListItem()
			{
				Value = triggerElemendId,
				Text = dependantElementId,
				ExtraData = new string[] { triggerId.ToString(), "," + string.Join(",", dependantIDs) + "," }
			};
		}

		public bool IsSelectedDefault { get; set; }

		public string RenderClass { get; set; }

		public string[] ExtraData { get; set; }
	}
}
