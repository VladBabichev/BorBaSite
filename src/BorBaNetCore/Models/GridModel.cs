using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;

namespace BorBaNetCore.Models
{
	public class GridModel<T>
	{
		public IList<GridModelColumn<T>> Columns { get; set; }

		public IList<T> Data { get; set; }

		public string Class { get; set; }
	}

	public class GridModelColumn<T>
	{
		public GridModelColumn(string name, Func<T, IHtmlContent> itemTemplateFunc, Func<T, string> valueFunc, Func<string, IHtmlContent> editorTemplateFunc)
		{
			this.Name = name;
			this.ItemTemplateFunc = itemTemplateFunc;
			this.ValueFunc = valueFunc;
			this.EditorTemplateFunc = editorTemplateFunc;
		}

		public string HeaderText
		{
			set
			{
				this.HeaderTextFunc = () => new HtmlString(value);
			}
		}

		public Func<IHtmlContent> HeaderTextFunc { get; set; }

		public Func<T, IHtmlContent> ItemTemplateFunc { get; set; }

		public Func<T, string> ValueFunc { get; set; }

		public Func<string, IHtmlContent> EditorTemplateFunc { get; set; }

		public string Class { get; set; }

		public string Name { get; set; }
	}
}
