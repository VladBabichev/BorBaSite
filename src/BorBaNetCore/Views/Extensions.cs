
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Newtonsoft.Json;

using System.Text;
using System.IO;
using Microsoft.AspNetCore.Html;

using Microsoft.AspNetCore.Mvc.ModelBinding;
using BorBaNetCore.Extensions;
using BorBaNetCore.WebExtension;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.AspNetCore.Mvc;
using BorBaNetCore.Models;
using Microsoft.AspNetCore.Http;

namespace BorBaNetCore.Views
{
	public static class Extensions
	{
		private static Dictionary<string, string> parentPageLinks = new Dictionary<string, string>();

		

		public static string ToString(this double? obj, string format)
		{
			if (obj.HasValue)
			{
				return obj.Value.ToString(format);
			}
			return string.Empty;
		}

		public static string ToString(this decimal? obj, string format)
		{
			if (obj.HasValue)
			{
				return obj.Value.ToString(format);
			}
			return string.Empty;
		}

		public static string ToString(this int? obj, string format)
		{
			if (obj.HasValue)
			{
				return obj.Value.ToString(format);
			}
			return string.Empty;
		}

		public static string ToJavascript(this bool value)
		{
			return value.ToString().ToLower();
		}

		private static Dictionary<string, string> _jobStatusMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
			{ "PartAllocated", "label-success" },
			{ "ExcessAllocation", "label-warning" },
		};

		private static IDictionary<string, object> GetHtmlAttributeDictionary(object htmlAttributes)
		{
			IDictionary<string, object> htmlAttributeDictionary = null;
			if (htmlAttributes != null)
			{
				htmlAttributeDictionary = htmlAttributes as IDictionary<string, object>;
				if (htmlAttributeDictionary == null)
				{
					htmlAttributeDictionary = Microsoft.AspNetCore.Mvc.ViewFeatures.HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
				}
				return htmlAttributeDictionary;
			}

			return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
		}

		public static IHtmlContent CreateNavigationLink(this IHtmlHelper html, string controller, string action, string text, string iconClass, object routeValues = null, string[] selectionControllers = null)
		{
			//< li class="@isActiveLink("Home","Index")">
			//    <a asp-controller="Home" asp-action="Index"><i class="fa fa-dashboard"></i> <span class="nav-label">Dashboard</span></a>
			//</li>
			var li = new TagBuilder(Constants.DOM.LI);
			var currController = html.ViewContext.RouteData.Values[Constants.Route.CONTROLLER].ToString();
			if (selectionControllers.IsNotEmpty() && currController.IsIn(selectionControllers) || currController == controller)
			{
				li.AddCssClass(Constants.Css.ACTIVE);
			}
			li.InnerHtml.SetContent(html.FALink(controller, action, iconClass, text, Constants.Css.NAV_LABEL, routeValues).ToString());
			return li;
		}

		/// <summary>
		/// Renders a link with Font Awesome icon tag as inner html
		/// </summary>
		/// <param name="html"></param>
		/// <param name="controller"></param>
		/// <param name="action"></param>
		/// <param name="text"></param>
		/// <param name="iconClass"></param>
		/// <returns></returns>
		public static TagBuilder FALink(this IHtmlHelper html, string controller, string action, string iconClass, Dictionary<string, object> routeValues = null, object htmlAttributes = null)
		{
			TagBuilder a = (TagBuilder)html.ActionLink(string.Empty, action, controller, routeValues, htmlAttributes);
			TagBuilder i = new TagBuilder(Constants.DOM.I);
			i.AddCssClass(Constants.Css.FONT_AWESOME);
			i.AddClass(iconClass);
			a.InnerHtml.Append(i.ToString());
			return a;
		}

		public static TagBuilder FALink(this IHtmlHelper html, string controller, string action, string iconClass, object routeValues = null, object htmlAttributes = null)
		{
			TagBuilder a = (TagBuilder)html.ActionLink(string.Empty, action, controller, routeValues, htmlAttributes);
			TagBuilder i = new TagBuilder(Constants.DOM.I);
			i.AddCssClass(Constants.Css.FONT_AWESOME);
			i.AddClass(iconClass);
			a.InnerHtml.Append(i.ToString());
			return a;
		}

		/// <summary>
		/// Renders a link with Font Awesome icon tag as inner html
		/// </summary>
		/// <param name="html"></param>
		/// <param name="controller"></param>
		/// <param name="action"></param>
		/// <param name="text"></param>
		/// <param name="iconClass"></param>
		/// <returns></returns>
		public static TagBuilder FALink(this IHtmlHelper html, string controller, string action, string iconClass, string text, string textClass, object routeValues = null, object htmlAttributes = null)
		{
			TagBuilder a = html.FALink(controller, action, iconClass, routeValues, htmlAttributes);
			TagBuilder span = new TagBuilder(Constants.DOM.SPAN);
			span.AddCssClass(textClass);
			span.InnerHtml.SetContent(text);
			a.InnerHtml.Append(span.ToString());
			return a;
		}

		public static TagBuilder GridDeleteLink(this IHtmlHelper html, string controller, string action, Dictionary<string, object> data, string objectTypeName, string objectName, object htmlAttr = null)
		{
			TagBuilder deleteLink = html.FALink(controller, action, "fa-trash text-danger", data, htmlAttr);
			deleteLink.Attributes["data-confirm-title"] = "Delete " + objectTypeName;
			deleteLink.Attributes["data-confirm-message"] = "Are you sure you want to delete {0} {1}".Frmt(objectName, objectTypeName);
			deleteLink.Attributes["data-grid-action"] = "delete";
			return deleteLink;
		}

		public static TagBuilder GriEditLink(this IHtmlHelper html, string controller, string action, Dictionary<string, object> data, string objectTypeName, bool editInGrid = false, string icoClass = "fa-pencil", object htmlAttr = null)
		{
			TagBuilder editLink = html.FALink(controller, action, icoClass + " text-primary", !editInGrid ? data : null, htmlAttr);
			editLink.Attributes["data-grid-action"] = "edit";
			return editLink;
		}

		public static TagBuilder GridEditDeleteLink(this IHtmlHelper html, string controller, string editAction, string deleteAction, Dictionary<string, object> data, string objectTypeName, string objectName, bool editInGrid = false, string deleteController = null, bool renderDeleteLink = true)
		{
			object htmlAttr = null;
			TagBuilder div = new TagBuilder(Constants.DOM.DIV);
			div.AddClass("actIconsContainer");

			if (editInGrid)
			{
				foreach (var key in data.Keys)
				{
					div.Attributes["data-" + key] = data[key].ToString();
				}
			}
			if (editAction.ToLower().Contains("edit"))
			{
				htmlAttr = new { title = "edit" };
			}
			div.InnerHtml.Append(html.GriEditLink(controller, editAction, data, objectTypeName, htmlAttr: htmlAttr).ToString());

			if (renderDeleteLink)
			{
				if (deleteAction.ToLower().Contains("delete"))
				{
					htmlAttr = new { title = "delete" };
				}
				div.InnerHtml.Append(html.GridDeleteLink(deleteController.Nz(controller), deleteAction, data, objectTypeName, objectName, htmlAttr).ToString());
			}
			return div;
		}


		public static IHtmlContent CreateTabLink(this IHtmlHelper html, string controller, string action, string text)
		{
			//< li class="active">
			//  <a href = "#employees" data-toggle="tab" aria-expanded="true">Employees</a>
			//</li>
			//< li class="@isActiveLink("Home","Index")">
			//    <a asp-controller="Home" asp-action="Index"><i class="fa fa-dashboard"></i> <span class="nav-label">Dashboard</span></a>
			//</li>
			var li = new TagBuilder(Constants.DOM.LI);
			if (html.ViewContext.RouteData.Values[Constants.Route.CONTROLLER].ToString() == controller
				&& html.ViewContext.RouteData.Values[Constants.Route.ACTION].ToString() == action)
			{
				li.AddCssClass(Constants.Css.ACTIVE);
			}
			TagBuilder a = (TagBuilder)html.ActionLink(string.Empty, action, controller, null, null);
			a.InnerHtml.Append(text);
			li.InnerHtml.SetContent(a.ToString());
			return li;
		}

		public static IHtmlContent BBChosenSelectFor<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TResult>> expression,
			IEnumerable<SelectListItem> options, string labeltext = null, string width = null, object htmlAttributes = null)
		{
			TagBuilder tag = htmlHelper.DropDownListFor(expression, options, labeltext, getDefaultAttributes(htmlHelper, expression)) as TagBuilder;
			tag.MergeAttributes(htmlAttributes);
			tag.AddCssClass("chosen-select");
			if (width != null)
			{
				tag.MergeAttribute("style", "width:" + width);
			}

			if (labeltext != null)
			{
				tag.Attributes[Constants.HtmlAttribute.PLACEHOLDER] = labeltext;
				tag.Attributes[Constants.HtmlAttribute.DATA_PLACEHOLDER] = labeltext;
			}

			return addValidationErrorMarker(htmlHelper, tag);
		}

		public static IHtmlContent ValidationErrorMarker(this IHtmlHelper html, string elName)
		{
			return new HtmlString(@"
				<div class='field-validation-valid error-container'>
					<i class='icon-error'></i>
					<i class='icon-success'></i>
					<label id='{0}-error' class='error' for='{0}' style='display: none;'></label>
				</div>".Frmt(elName));
		}

		private static IHtmlContent addValidationErrorMarker(this IHtmlHelper htmlHelper, TagBuilder tag)
		{
			var containerTag = new TagBuilder(Constants.DOM.DIV);
			containerTag.AddClass("validContainer");
			containerTag.InnerHtml.Append(tag.ToString());

			var validationKeys = new string[] { "number", "required", "min", "max" };

			//if (Constants.HtmlAttribute.VALIDATION_ATTRS.Any(a => tag.Attributes.ContainsKey(a)))
			{
				containerTag.InnerHtml.Append(ValidationErrorMarker(htmlHelper, tag.Attributes["id"]).ToString());
			}
			return containerTag;
		}
		private static IHtmlContent addValidationErrorMarker(this IHtmlHelper htmlHelper, IHtmlContent html, string elName)
		{
			var containerTag = new TagBuilder(Constants.DOM.DIV);
			containerTag.AddClass("validContainer");
			containerTag.InnerHtml.Append(html.ToString());

			var validationKeys = new string[] { "number", "required", "min", "max" };

			//if (Constants.HtmlAttribute.VALIDATION_ATTRS.Any(a => html.ToString().Contains(a)))
			{
				containerTag.InnerHtml.Append(ValidationErrorMarker(htmlHelper, elName).ToString());
			}
			return containerTag;
		}



		public static IHtmlContent DatePickerFor<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TResult>> expression,
			string extraClass = "",
			string extraAttr = "")
		{
			return DatePicker(htmlHelper, htmlHelper.IdFor(expression), htmlHelper.ValueFor(expression, "{0:dd/MM/yyyy}"), extraClass, extraAttr);
		}

		public static IHtmlContent DatePicker<TModel>(this IHtmlHelper<TModel> htmlHelper, string id, string value = "", string extraClass = "", string extraAttr = "")
		{
			var tg = new TagBuilder(Constants.DOM.DIV);
			tg.AddClass("input-group date");
			tg.Attributes.Add("id", id + "_dateContainer");
			var disabledAttr = "disabled";
			if (extraAttr.IsNotEmpty() && extraAttr.Contains(disabledAttr))
			{
				tg.Attributes.Add(disabledAttr, disabledAttr);
			}

			tg.InnerHtml.AppendHtml(@"
				<span class='input-group-addon'>
					<i class='fa fa-calendar'></i>
				</span>");
			tg.InnerHtml.Append(addValidationErrorMarker(htmlHelper, new HtmlString("<input type = 'text' id='{0}' name='{0}' asp-for='{0}' value='{1}' class='form-control {2}' {3} />".Frmt(id, value, extraClass, extraAttr)), id).ToString());

			return tg;
		}

		public static TagBuilder MergeAttributes(this TagBuilder tag, object attributes)
		{
			if (attributes == null)
				return tag;

			var attr = GetHtmlAttributeDictionary(attributes);
			tag.MergeAttributes(attr, true);
			if (attr.ContainsKey(Constants.HtmlAttribute.CLASS))
			{
				tag.AddCssClass(attr[Constants.HtmlAttribute.CLASS].ToString());
			}
			return tag;
		}

		public static IHtmlContent BBTextBoxFor<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression, object attributes = null, string format = null)
		{
			var textBox = (htmlHelper.TextBoxFor(expression, format, getDefaultAttributes(htmlHelper, expression)) as TagBuilder).MergeAttributes(attributes);
			return addValidationErrorMarker(htmlHelper, textBox);
		}

		public static IHtmlContent BBTextBox<TModel>(this IHtmlHelper<TModel> htmlHelper, string expression, object value, object attributes = null, bool isRequired = false, string format = null)
		{
			var textBox = (htmlHelper.TextBox(expression, value, format, getDefaultAttributes<TModel, string>(htmlHelper, isRequired)) as TagBuilder).MergeAttributes(attributes);
			return addValidationErrorMarker(htmlHelper, textBox);
		}

		public static IHtmlContent BBTextAreaFor<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression, object attributes = null)
		{
			return (htmlHelper.TextAreaFor(expression, getDefaultAttributes(htmlHelper, expression)) as TagBuilder).MergeAttributes(attributes);
		}

		private static IDictionary<string, object> getDefaultAttributes<TModel, TResult>(IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression)
		{
			ModelExplorer modelExplorer = ExpressionMetadataProvider.FromLambdaExpression(expression, htmlHelper.ViewData, htmlHelper.MetadataProvider);
			return getDefaultAttributes<TModel, TResult>(htmlHelper, modelExplorer.Metadata.IsRequired);
		}

		private static IDictionary<string, object> getDefaultAttributes<TModel, TResult>(IHtmlHelper<TModel> htmlHelper, bool isRequired)
		{
			IDictionary<string, object> defaultAttributes;
			if (isRequired)
			{
				defaultAttributes = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
				{
					{ Constants.HtmlAttribute.CLASS, Constants.Css.FORM_CONTROL },
					{ Constants.HtmlAttribute.PLACEHOLDER, " * " }
				};
			}
			else
			{
				defaultAttributes = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
				{
					{ Constants.HtmlAttribute.CLASS, Constants.Css.FORM_CONTROL }
				};
			}
			return defaultAttributes;
		}

		public static IHtmlContent Pager<TModel>(this IHtmlHelper<TModel> htmlHelper, int totalPages, int currentPage, string pageParam = "page")
		{
			if (totalPages <= 1)
				return null;

			IDictionary<string, object> routeValues = new Dictionary<string, object>(htmlHelper.ViewContext.RouteData.Values);
			string action = routeValues[Constants.Route.ACTION] as string;

			TagBuilder ul = new TagBuilder(Constants.DOM.UL);
			ul.AddCssClass(Constants.Css.PAGINATION);

			if (currentPage == 0)
			{
				ul.InnerHtml.Append(htmlHelper.PagerButton(Constants.ButtonText.PREVIOUS, action, null, Constants.Css.PREVIOUS, Constants.Css.DISABLED).ToString());
			}
			else
			{
				routeValues[pageParam] = currentPage - 1;
				ul.InnerHtml.Append(htmlHelper.PagerButton(Constants.ButtonText.PREVIOUS, action, routeValues, Constants.Css.PREVIOUS).ToString());
			}

			for (int i = 0; i < totalPages; i++)
			{
				routeValues[pageParam] = i;
				ul.InnerHtml.Append(htmlHelper.PagerButton((i + 1).ToString(), action, routeValues,
					(i == currentPage ? Constants.Css.ACTIVE : null)).ToString());
			}

			if (currentPage < totalPages - 1)
			{
				routeValues[pageParam] = currentPage + 1;
				ul.InnerHtml.Append(htmlHelper.PagerButton(Constants.ButtonText.NEXT, action, routeValues, Constants.Css.NEXT).ToString());
			}
			else
			{
				ul.InnerHtml.Append(htmlHelper.PagerButton(Constants.ButtonText.NEXT, action, routeValues, Constants.Css.NEXT, Constants.Css.DISABLED).ToString());
			}

			return ul;
		}

		public static IHtmlContent PagerButton(this IHtmlHelper htmlHelper, string text, string action, object routeValues, params string[] additionalClasses)
		{
			/*
            <li class="paginate_button previous disabled" tabindex="0">
                <a href="#">Previous</a>
            </li>
            */

			TagBuilder li = new TagBuilder(Constants.DOM.LI);
			li.AddCssClass(Constants.Css.PAGINATION_BUTTON);
			if (additionalClasses != null)
			{
				foreach (var item in additionalClasses)
				{
					li.AddCssClass(item);
				}
			}
			if (routeValues == null)
			{
				TagBuilder a = new TagBuilder(Constants.DOM.A);
				a.Attributes[Constants.HtmlAttribute.HREF] = Constants.HASH_LINK;
				a.InnerHtml.SetContent(text);
				li.InnerHtml.SetContent(a.ToString());
			}
			else
			{
				li.InnerHtml.SetContent(htmlHelper.ActionLink(text, action, routeValues).ToString());
			}

			return li;
		}

		public static IHtmlContent ChangeSummaryView(this IHtmlHelper htmlHelper, bool isSummary1View, bool? isSummary2View = null)
		{
			bool isSum2 = isSummary2View.Nz(false);
			var paramArr = new List<dynamic>();

			if (isSummary2View.HasValue)
			{
				paramArr.Add(
					new
					{
						IsSelected = isSum2,
						ItemName = "isSummary2View",
						Class = "fa-th-large",
						Title = "Month"
					});
			}

			paramArr.Add(
				new
				{
					IsSelected = isSummary1View && !isSum2,
					ItemName = "isSummary1View",
					Class = "fa-th-list",
					Title = "Week"
				});

			paramArr.Add(
				new
				{
					IsSelected = !isSummary1View && !isSum2,
					ItemName = "isTableView",
					Class = "fa-table",
					Title = "Detailed view"
				});

			return ChangeViewEx(htmlHelper, paramArr);
		}

		public static IHtmlContent ChangeView(this IHtmlHelper htmlHelper, bool isBlockView, bool? isMapView = null, string otherViewCss = "fa-table")
		{
			bool isMap = isMapView.Nz(false);
			var mapClass = "fa-map-marker";
			var paramArr = new List<dynamic>();
			if (isMapView.HasValue || otherViewCss == mapClass)
			{
				paramArr.Add(
					new
					{
						IsSelected = isMap,
						ItemName = "isMapView",
						Class = mapClass,
						Title = "Map"
					});
			}

			paramArr.Add(
				new {
					IsSelected = isBlockView && !isMap,
					ItemName = "isBlockView",
					Class = "fa-th-large",
					Title = "Block view"
				});

			paramArr.Add(
				new
				{
					IsSelected = !isBlockView && !isMap,
					ItemName = "isTableView",
					Class = "fa-table",
					Title = "Detailed view"
				});

			return ChangeViewEx(htmlHelper, paramArr);
		}

		public static IHtmlContent ChangeViewEx(this IHtmlHelper htmlHelper, List<dynamic> listIcons)
		{
			/*
            <div class="pull-right" id="change-view">
                <a class="btn btn-white btn-sm active" type="button"><i class="fa fa-table"></i></a>
                <a class="btn btn-white btn-sm" type="button"><i class="fa fa-th-large"></i></a>
            </div>
            */
			TagBuilder div = new TagBuilder(Constants.DOM.DIV);
			div.AddCssClass(Constants.Css.PULL_RIGHT);

			foreach (var iconItem in listIcons)
			{
				IDictionary<string, object> routeValues = new Dictionary<string, object>(htmlHelper.ViewContext.RouteData.Values);
				string action = routeValues[Constants.Route.ACTION] as string;

				string itemName = iconItem.ItemName;
				routeValues[itemName] = true;

				TagBuilder aTable = htmlHelper.ActionLink(string.Empty, action, routeValues) as TagBuilder;
				aTable.AddCssClass("btn btn-white btn-sm");
				aTable.Attributes.Add("title", iconItem.Title);
				aTable.Attributes.Add("data-itemName", iconItem.ItemName);
				if (iconItem.IsSelected)
				{
					aTable.AddCssClass(Constants.Css.ACTIVE);
				}
				TagBuilder iTable = new TagBuilder(Constants.DOM.I);
				iTable.AddCssClass(Constants.Css.FONT_AWESOME);
				iTable.AddCssClass(iconItem.Class);
				aTable.InnerHtml.SetContent(iTable.ToString());

				// Given space between two nodes
				div.InnerHtml.AppendHtmlLine(" ");
				div.InnerHtml.Append(aTable.ToString());
			}
			return div;
		}

		
		public static IHtmlContent Partial(this IHtmlHelper htmlHelper, string partialViewName, string fieldPrefix, object model, ViewDataDictionary viewData)
		{
			var newVD = new ViewDataDictionary(viewData);
			newVD.TemplateInfo.HtmlFieldPrefix = fieldPrefix;
			return htmlHelper.Partial(partialViewName, model, newVD);
		}

		public static List<SelectListItem> ToListItems<T>(this IHtmlHelper htmlHelper, params T[] excluded) where T : struct, IConvertible
		{
			Type enumType = typeof(T);
			if (!enumType.IsEnum)
			{
				throw new ArgumentException("T must be an enumerated type");
			}
			var options = new List<SelectListItem>();
			var values = Enum.GetValues(enumType) as T[];
			if (values != null)
			{
				if (excluded != null)
					values = values.Except(excluded).ToArray();
				foreach (T i in values)
				{
					options.Add(new SelectListItem()
					{
						Text = i.ToString(),
						Value = Convert.ToInt64(i).ToString()
					});
				}
			}
			return options;
		}

		public static List<SelectListItem> ToListItems<T>(this IHtmlHelper htmlHelper, IEnumerable<T> list, string selected, Func<T, string> getValue, Func<T, string> getText)
		{
			var result = new List<SelectListItem>();
			if (list != null)
			{
				foreach (T item in list)
				{
					string value = getValue(item);
					result.Add(new SelectListItem()
					{
						Text = getText(item),
						Value = value,
						//TODO: check as this may return true for select=12 and value=1
						Selected = selected.IsNotEmpty() && selected.Contains(value)
					});
				}
			}
			return result;
		}

		public static List<SelectListItem> ToIsActiveListItems(this IHtmlHelper htmlHelper, string selected)
		{
			return ToBoolListItems(htmlHelper, selected, new Dictionary<string, bool?>() { { "Active", true }, { "Inactive", false } });
		}

		public static List<SelectListItem> ToBoolListItems(this IHtmlHelper htmlHelper, string selected, Dictionary<string, bool?> checks)
		{
			var result = new List<SelectListItem>();
			foreach (var check in checks)
			{
				var val = check.Value.ToString();
				result.Add(new SelectListItem()
				{
					Text = check.Key,
					Value = val,
					Selected = selected == val
				});
			}
			return result;
		}

		#region Grid

		public static IHtmlContent Grid<T>(this IHtmlHelper htmlHelper, string actionUrl, GridModel<T> model)
		{
			TagBuilder formTag = new TagBuilder(Constants.DOM.FORM);
			formTag.MergeAttributes(new
			{
				method = "POST",
				action = actionUrl
			});
			TagBuilder tableTag = new TagBuilder(Constants.DOM.TABLE);
			tableTag.AddCssClass("table table-striped");
			tableTag.AddClass(model.Class);
			TagBuilder tHead = new TagBuilder("thead");
			TagBuilder headerRow = new TagBuilder(Constants.DOM.TR);
			foreach (GridModelColumn<T> col in model.Columns)
			{
				TagBuilder cell = new TagBuilder(Constants.DOM.TH);
				cell.AddClass(col.Class);
				cell.InnerHtml.SetContent(col.HeaderTextFunc().ToString());
				headerRow.InnerHtml.Append(cell.ToString());
			}
			tHead.InnerHtml.SetContent(headerRow.ToString());
			tableTag.InnerHtml.Append(tHead.ToString());

			TagBuilder tBody = new TagBuilder("tbody");
			foreach (T item in model.Data)
			{
				TagBuilder row = new TagBuilder(Constants.DOM.TR);
				foreach (GridModelColumn<T> col in model.Columns)
				{
					TagBuilder cell = new TagBuilder(Constants.DOM.TD);
					cell.Attributes["data-name"] = col.Name;
					if (col.ValueFunc != null)
					{
						cell.Attributes["data-value"] = col.ValueFunc(item);
					}
					cell.InnerHtml.SetContent(col.ItemTemplateFunc(item).ToString());
					row.InnerHtml.Append(cell.ToString());
				}
				tBody.InnerHtml.Append(row.ToString());
			}
			tableTag.InnerHtml.Append(tBody.ToString());
			formTag.InnerHtml.SetContent(tableTag.ToString());

			TagBuilder scriptTag = new TagBuilder(Constants.DOM.SCRIPT);
			scriptTag.Attributes.Add("type", "text/x-handlebars-template");
			TagBuilder editRow = new TagBuilder(Constants.DOM.TR);
			editRow.AddCssClass("edit-row");
			foreach (GridModelColumn<T> col in model.Columns)
			{
				TagBuilder cell = new TagBuilder(Constants.DOM.TD);
				cell.InnerHtml.SetContent(col.EditorTemplateFunc("{{" + col.Name + "}}").ToString());
				editRow.InnerHtml.Append(cell.ToString());
			}
			scriptTag.InnerHtml.SetContent(editRow.ToString());

			TagBuilder gridDiv = new TagBuilder(Constants.DOM.DIV);
			gridDiv.AddCssClass("grid-view table-responsive");
			gridDiv.InnerHtml.Append(formTag.ToString());
			gridDiv.InnerHtml.Append(scriptTag.ToString());
			return gridDiv;
		}

		#endregion

		public static string ImageUrl(this IUrlHelper url, int imageId)
		{
			return url.Action("Image", "Home", new { id = imageId });
		}

		//public static IHtmlContent DropZone(this IHtmlHelper html, IModelWithImage model, string dropZoneId)
		//{
		//	return html.Partial("_EditImages", new EditImages()
		//	{
		//		DropZoneId = dropZoneId,
		//		ImagesModel = model
		//	});
		//}

		public static IHtmlContent SubmitButton(this IHtmlHelper html, string name, string value, string innerText, string additionalClasses = null, Action<TagBuilder> setProperties = null)
		{
			TagBuilder result = new TagBuilder("button");
			result.Attributes[Constants.HtmlAttribute.NAME] = name;
			result.Attributes[Constants.HtmlAttribute.VALUE] = value;
			result.Attributes[Constants.HtmlAttribute.TYPE] = "submit";
			result.AddCssClass("btn");
			result.AddClass(additionalClasses);
			if (setProperties != null)
				setProperties(result);
			result.InnerHtml.SetHtmlContent(innerText);

			return result;
		}

		public static string GetRefUrl(this IHtmlHelper html)
		{
			HttpRequest request = html.ViewContext.HttpContext.Request;
			string returnUrl = request.Query[Constants.Route.RETURN_URL];
			var currUrl = "{0}|{1}".Frmt(html.ViewContext.ViewBag.CurrUserId as int?, html.CurrentUrl());
			if (returnUrl.IsEmpty() && parentPageLinks.IsNotEmpty() && parentPageLinks.ContainsKey(currUrl))
			{
				returnUrl = parentPageLinks[currUrl];
			}
			if (returnUrl.IsEmpty() || currUrl.EndsWith(returnUrl))
			{
				//Please note that the header spelling should be INCORRECT below:
				returnUrl = request.Headers["referer"];
				if (parentPageLinks.ContainsKey(currUrl))
				{
					parentPageLinks[currUrl] = returnUrl;
				}
				else {
					parentPageLinks.Add(currUrl, returnUrl);
				}
			}
			return returnUrl;
		}

		public static IHtmlContent Cancel(this IHtmlHelper html, string name = "submit", string value = "cancel", string innerText = "Cancel", string additionalClasses = null)
		{
			return html.SubmitButton(
				name, value, innerText, additionalClasses,
				btn =>
				{
					btn.AddCssClass("btn-white cancel left");
					btn.Attributes["data-return-url"] = GetRefUrl(html);
				}
			);
		}

		public static IHtmlContent CancelDialog(this IHtmlHelper html, string innerText = "Cancel", string additionalClasses = null)
		{
			return html.SubmitButton(null, null, innerText, additionalClasses,
				btn =>
				{
					btn.Attributes["data-dismiss"] = "modal";
					btn.Attributes[Constants.HtmlAttribute.TYPE] = "button";
					btn.AddCssClass("btn-white cancel left");
				}
			);
		}

		public static IHtmlContent Save(this IHtmlHelper html, string name = "submit", string value = "save", string innerText = "Save", string additionalClasses = null)
		{
			TagBuilder tg = new TagBuilder(Constants.DOM.SPAN);

			tg.AddClass("redirectContainer");
			tg.InnerHtml.Append(html.Hidden(Constants.Route.PARENT_PAGE_URL, GetRefUrl(html)).ToString());
			tg.InnerHtml.Append(html.SubmitButton(
				name, value, innerText, additionalClasses,
				btn => btn.AddCssClass("btn-primary save-btn")
			).ToString());

			return tg;
		}

		public static string CurrentUrl(this IHtmlHelper html)
		{
			HttpRequest request = html.ViewContext.HttpContext.Request;
			return request.Path + request.QueryString.ToUriComponent();
		}

		public static IHtmlContent RenderImages(this IUrlHelper html, int? mainImageId, Func<int[]> getImageIDs, Func<string[]> getImageDescriptions = null)
		{
			int[] imageIDs = getImageIDs();
			string[] imageDescr = getImageDescriptions != null ? getImageDescriptions() : null;
			int idsCount = imageIDs.SafeCount();
			var showGallery = idsCount > 1;

			if (mainImageId == null && idsCount > 0)
			{
				mainImageId = imageIDs[0];
			}
			string mainImgSrc = mainImageId.Nz() != 0
							? html.ImageUrl(mainImageId.Value)
							: Constants.Default.NO_IMAGE;

			var result = new StringBuilder(@"
                <div class='col-sm-4 " + (showGallery ? "lightBoxGallery" : "") + @"' style='margin-bottom:20px;'>
                <div class='text-center' id='images'>
                    <a href='" + mainImgSrc + @"' data-gallery=''>
                        <img src='" + mainImgSrc + @"' class='img-responsive m-t-xs'>
                    </a>
                    <div class='m-t-xs font-bold'></div>
                </div>");

			if (showGallery)
			{
				result.Append("<div id='thumbs' style='margin: 0 -2px;'>");

				for (int i = 0; i < idsCount; i++)
				{
					int imageId = imageIDs[i];
					string imageUrl = html.ImageUrl(imageId);
					string descr = imageDescr != null ? imageDescr[i] : "";

					if (imageId != mainImageId)
					{
						result.Append(@"
                            <a href='" + imageUrl + "' title='" + descr + @"' data-gallery=''>
                                <img src= '" + imageUrl + "' alt='" + descr + @"' class='img-responsive m-t-xs col-xs-3'>
                            </a>");
					}
				}
				result.Append("</div>");
			}
			result.Append("</div>");

			return new HtmlString(result.ToString());
		}

		public static string CheckedSelectorForId<TModel, TResult>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TResult>> expression, object value1 = null, object value2 = null)
		{
			return value1 != null && value2 != null
				? "#{0}[value={1}]:checked,#{0}[value={2}]:checked".Frmt(html.IdFor(expression), value1, value2)
				: value2 != null || value1 != null
				? "#{0}[value={1}]:checked".Frmt(html.IdFor(expression), value2 ?? value1)
				: "#{0}:checked".Frmt(html.IdFor(expression));
		}

		public static IHtmlContent StarMarker(this IHtmlHelper html, bool isShown, string title, string defClass = "fa-star-o")
		{
			return new HtmlString(isShown ? "<i class='fa " + defClass + "' title='" + title + "'></i>" : "");
		}

		public static IHtmlContent TickBoxFor<TModel>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool>> expression, object attributes = null, string extraClasses = null)
		{
			var tb = new TagBuilder(Constants.DOM.LABEL);
			tb.AddClass("checkbox-inline i-checks " + extraClasses);
			tb.InnerHtml.Append(addValidationErrorMarker(htmlHelper, htmlHelper.CheckBoxFor(expression, attributes), htmlHelper.IdFor(expression)).ToString());

			return tb;
		}

		public static IHtmlContent TickBox(this IHtmlHelper htmlHelper, string name, object value, bool isChecked, string extraClasses = null, string extraAttr = null)
		{
			TagBuilder tb = new TagBuilder(Constants.DOM.LABEL);
			tb.AddClass("checkbox-inline i-checks " + extraClasses);
			tb.InnerHtml.Append(addValidationErrorMarker(htmlHelper, new HtmlString(@"<input type = 'checkbox' value='{0}' name='{1}' id='{1}' {2} {3}/>".Frmt(value, name, isChecked ? "checked='checked'" : null, extraAttr)), name).ToString());

			return tb;
		}

		public static IHtmlContent HiddenEnumerableFor<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression, object[] values, object attributes = null)
		{
			string name = htmlHelper.IdFor(expression);
			TagBuilder divContainer = new TagBuilder(Constants.DOM.DIV);
			divContainer.AddClass("{0}Container".Frmt(name));
			if (values != null)
			{
				foreach (var value in values)
				{
					divContainer.InnerHtml.Append(htmlHelper.Hidden(name, value, attributes).ToString());
				}
			}
			return divContainer;
		}


		public static IHtmlContent ContractWizardLabelPlate(this IHtmlHelper htmlHelper, string name, string contentClass, string shortContent, Dictionary<string, string> contentDictionary, int stepTargetIndex, string href = "#")
		{
			var linkContent = "";
			if (href.IsNotEmpty())
			{
				linkContent = "<a href='{0}' data-step-target='{1}'>{2}</a>".Frmt(href, stepTargetIndex, shortContent);
			}
			else
			{
				linkContent = shortContent;
			}
			var content = "";
			foreach (var dictItem in contentDictionary)
			{
				if (dictItem.Value.IsNotEmpty())
				{
					content += "<b>{0}</b>:&nbsp;&nbsp;{1}<br/>".Frmt(dictItem.Key, dictItem.Value);
				}
				else
				{
					content += "{0}<br/>".Frmt(dictItem.Key);
				}
			}

			return new HtmlString(@"
			<div class='pre toggle'>
				<h3>
					{0} 
					<span class='font-normal'>{1}</span> 
					<a href='#' class='pull-right text-muted'><i class='fa fa-chevron-up'></i></a>
				</h3>
				<div class='toggle-content {2}'>
					{3}
				</div>
				<div class='clearfix'></div>
			</div>
			".Frmt(name, linkContent, contentClass, content));
		}

		public static IHtmlContent LabelPlateWithImage(this IHtmlHelper htmlHelper, string name, string contentClass, string shortContent, string content, int stepTargetIndex, string imageSrc, string href = "#")
		{
			var link = "";
			if (href.IsNotEmpty())
			{
				link = @"
				<div class='col-lg-8'>
					<a href='{0}' class='pull-right link-edit' data-step-target='{1}'>Change</a>
				</div>".Frmt(href, stepTargetIndex);
			}

			return new HtmlString(@"
			<div class='pre toggle'>
				<h3>{0} <span class='font-normal'>{1}</span> <a href='#' class='pull-right text-muted'><i class='fa fa-chevron-up'></i></a></h3>
				<div class='row {2}'>
					<div class='col-lg-3 col-md-3 col-sm-3 col-xs-3'>
						<a href = '{3}' >
							<img class='img-circle m-t-xs img-responsive' src='{3}' alt='image' style='max-width: 50px;'>
						</a>
					</div>
					<div class='col-lg-9'>
						{4}
					</div>
					{5}
					<div class='clearfix'></div>
				</div>
			</div>
			".Frmt(name, shortContent, contentClass, imageSrc, content, link));
		}

		public static IHtmlContent ListCheckBox(this IHtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> items, string title = "", bool readOnly = false, bool isToMirrorInputs = false)
		{
			return htmlHelper.Partial("_ListCheckBox", new DataElement()
			{
				Name = name,
				Data = items,
				Title = title,
				IsReadOnly = readOnly,
				Value = isToMirrorInputs.ToString()
			});
		}

		public static IHtmlContent TimePicker<TModel, TResult>(this IHtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression, bool isRequired = false)
		{
			TagBuilder divContainer = new TagBuilder(Constants.DOM.DIV);
			divContainer.AddClass("input-group bootstrap-timepicker");
			divContainer.InnerHtml.AppendHtml("<span class='input-group-addon'><i class='fa fa-clock-o'></i></span>");
			divContainer.InnerHtml.Append(BBTextBoxFor(helper, expression, new { required = isRequired, @class = "bs-timepicker form-control" }, "{0:t}").ToString());

			return divContainer;
		}

		public static IHtmlContent DatePicker<TModel, TResult>(this IHtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression, string extraClass = "", bool isRequired = false)
		{
			TagBuilder divContainer = new TagBuilder(Constants.DOM.DIV);
			divContainer.AddClass("input-group date");
			divContainer.InnerHtml.AppendHtml("<span class='input-group-addon'><i class='fa fa-calendar'></i></span>");
			divContainer.InnerHtml.Append(BBTextBoxFor(helper, expression, new { required = isRequired, @class = "form-control " + extraClass }, "{0:dd/MM/yyyy}").ToString());

			return divContainer;
		}

		public static IHtmlContent YesNoOptions(this IHtmlHelper helper, string elName, bool? value, string elPlaceholder)
		{
			return helper.DropDownList(elName,
				new List<SelectListItem>() {
					new SelectListItem() {
						Selected = value.Nz(),
						Text = "Yes",
						Value = "true"
					},
					new SelectListItem() {
						Selected = !value.Nz(),
						Text = "No",
						Value = "false"
					}
				}, elPlaceholder, new { data_placeholder = elPlaceholder, @class = "chosen-select", style = "width:100%" });
		}

		public static IHtmlContent ActiveOptions(this IHtmlHelper helper, string value)
		{
			var elPlaceholder = "Choose Is Active...";
			return helper.DropDownList("isActive", helper.ToIsActiveListItems(value), elPlaceholder, new { data_placeholder = elPlaceholder, @class = "chosen-select", style = "width:100%" });
		}


        public static IHtmlContent ListBox(this IHtmlHelper htmlHelper, string name, IEnumerable<DecoratedSelectListItem> items, string title = "", bool readOnly = false, bool isToMirrorInputs = false)
        {
            return htmlHelper.Partial("_ListBox", new DataElement()
            {
                Name = name,
                Data = items,
                Title = title,
                IsReadOnly = readOnly,
                Value = isToMirrorInputs.ToString()
            });
        }

        #region Validations

        public static IHtmlContent ValidatorRules<TModel>(this IHtmlHelper<TModel> htmlHelper)
		{
			Dictionary<string, object> rules = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
			foreach (ModelMetadata modelMetadata in htmlHelper.MetadataProvider.GetMetadataForProperties(typeof(TModel)))
			{
				Dictionary<string, object> propertyRule = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

				if (modelMetadata.IsRequired && modelMetadata.ModelType != typeof(bool))
				{
					propertyRule["required"] = true;
				}

				// Can add more such validators to the dictionary in future

				if (propertyRule.Count > 0)
				{
					rules[modelMetadata.PropertyName] = propertyRule;
				}
			}
			return new HtmlString(JsonConvert.SerializeObject(rules));
		}

		#endregion

	}
}
