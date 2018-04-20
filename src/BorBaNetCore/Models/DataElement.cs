namespace BorBaNetCore.Models
{
	public class DataElement
	{
		public string Controller { get; set; }
		public string Action { get; set; }
		public string Name { get; set; }
		public string Value { get; set; }
		public bool IsReadOnly { get; set; }
		public string Class { get; set; }
		public object Data { get; set; }
		public string Title { get; set; }

		public DataElement()
		{

		}

		public DataElement(string name, string value, bool isReadOnly, string @class = "", object data = null, string title = null)
		{
			this.Name = name;
			this.Value = value;
			this.IsReadOnly = isReadOnly;
			this.Class = @class;
			this.Data = data;
			this.Title = title;
		}

		public DataElement(string controller, string action, string name, string value, bool isReadOnly, string @class = "", object data = null, string title = null)
		{
			this.Controller = controller;
			this.Action = action;
			this.Name = name;
			this.Value = value;
			this.IsReadOnly = isReadOnly;
			this.Class = @class;
			this.Data = data;
			this.Title = title;
		}

		public DataElement(string title, bool isReadOnly = false)
		{
			IsReadOnly = isReadOnly;
			Title = title;
		}
	}
}
