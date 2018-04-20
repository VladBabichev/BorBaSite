using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BorBaNetCore
{
	public static class Constants
	{
        public const int DEFAULT_PAGE_SIZE = 20;
        public const char COMMA_SUBSTITUTE = (char)1;
        public const char PIPE_SUBSTITUTE = (char)2;
        public static class ButtonText
		{
			public const string
				PREVIOUS = "Previous",
				NEXT = "Next";
		}

		public static class Css
		{
			public const string
				PREVIOUS = "previous",
				NEXT = "next",
				DISABLED = "disabled",
				ACTIVE = "active",
				PAGINATION = "pagination",
				FORM_CONTROL = "form-control",
				NAV_LABEL = "nav-label",
				PULL_RIGHT = "pull-right",
				FONT_AWESOME = "fa",
				PAGINATION_BUTTON = "paginate_button",
				TEXT_DANGER = "text-danger",
				ERROR_CONTAINER = "error-container";
		}

		public static class DOM
		{
			public const string
				UL = "ul",
				LI = "li",
				LABEL = "label",
				A = "a",
				I = "i",
				IFRAME = "iframe",
				INPUT = "input",
				SPAN = "span",
				DIV = "div",
				FORM = "form",
				TABLE = "table",
				TR = "tr",
				TD = "td",
				TH = "th",
				SCRIPT = "script";
		}

		public class HtmlAttribute
		{
			public const string
				HREF = "href",
				NAME = "name",
				TYPE = "type",
				VALUE = "value",
				CLASS = "class",
				CHECKED = "checked",
				PLACEHOLDER = "placeholder",
				DATA_PLACEHOLDER = "data-placeholder";

			// all validation attributes which are used
			public static readonly string[] VALIDATION_ATTRS = { "number", "required", "min", "max", "data-val-checkEmail" };
		}

		public static class Cookie
		{
			public const string
				REMEMBER_ME = "CustomAuth";
		}

		public static class Session
		{
			public const string
				CURRENT_USER = "CurrentUser";
		}

		public static class Data
		{
			public const string
				TITLE = "Title",
				HEADING = "Heading",
				BREADCRUMB_TITLE = "BreadCrumbTitle",
				HEADER_ACTION = "HeaderAction",
				NOTIFICATION = "Notifications",
				NAV_PREFERENCE = "NavPreference",
				ALL_CLIENTS = "AllClients",
				SHOW_NOTIFICATION = "SHOW_NOTIFICATION";
		}

		public static class Route
		{
			public const string
				PARENT_PAGE_URL = "parentPageUrl",
				RETURN_URL = "returnUrl",
				ACTION = "action",
				CONTROLLER = "controller";
		}

		public static class Default
		{
			public const string
				PROFILE_IMAGE = "~/images/profile.jpg",
				PROFILE_SMALL_IMAGE = "~/images/profile_small.jpg",
				NO_IMAGE = "/images/no-image.png",
				NO_SMALL_IMAGE = "/images/no-image-small.png",
				TIME_FORMAT = "h:mm tt",
				DATE_FORMAT = "dd/MM/yyyy",
				START_TIME = "8:00 AM",
				END_TIME = "5:00 PM";
				
			
			public static readonly TimeSpan
				START_OF_DAY = new TimeSpan(0, 0, 0),
				END_OF_DAY = new TimeSpan(23, 59, 59),
				START_TIME_TS = new TimeSpan(8, 0, 0),
				END_TIME_TS = new TimeSpan(17, 0, 0);

			public static int DEFAULT_MAX_SCHEDULING_PERIOD = 365; // days
			public static int DEFAULT_MIN_JOB_INTERVAL = 15; //minutes
			public static int DASHBOARD_TOP_JOBS = 5; //minutes
		}

		public const string LOGIN_PAGE = "/Account/Login";

		public const string HASH_LINK = "#";

		public enum ExtraInfoDataTypes
		{
			ListOptions = 0,
			BooleanOptions,
			Notes
		}
	}
}
