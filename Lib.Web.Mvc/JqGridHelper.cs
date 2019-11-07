using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Lib.Web.Mvc.JqGridFork.Constants;
using Lib.Web.Mvc.JqGridFork.Serialization;

namespace Lib.Web.Mvc.JqGridFork
{
	/// <summary>
	/// Helper class for generating jqGrid HMTL and JavaScript
	/// </summary>
	/// <typeparam name="TModel">Type of model for this grid</typeparam>
	public class JqGridHelper<TModel> : IJqGridHelper
	{
		#region Fields
		private JqGridNavigatorActionOptions _navigatorEditActionOptions;
		private JqGridNavigatorActionOptions _navigatorAddActionOptions;
		private JqGridNavigatorActionOptions _navigatorDeleteActionOptions;
		private JqGridNavigatorActionOptions _navigatorSearchActionOptions;
		private JqGridNavigatorActionOptions _navigatorViewActionOptions;
		private List<JqGridNavigatorControlOptions> _navigatorControlsOptions = new List<JqGridNavigatorControlOptions>();
		private bool _filterToolbar = false;
		private JqGridFilterToolbarOptions _filterToolbarOptions;
		private List<JqGridFilterGridRowModel> _filterGridModel;
		private JqGridFilterGridOptions _filterGridOptions;
		private IDictionary<string, object> _footerData = null;
		private bool _footerDataUseFormatters = true;
		private bool _groupHeadersUseColSpanStyle = false;
		private IEnumerable<JqGridGroupHeader> _groupHeaders = null;
		private bool _setFrozenColumns = false;
		private bool _asSubgrid = false;
		private object _subgridHelper = null;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the grid identifier.
		/// </summary>
		public string Id => Options.Id;

		private string GridSelector => _asSubgrid ? "'#' + subgridTableId" : $"'#{Id}'";

		/// <summary>
		/// Gets the grid pager identifier.
		/// </summary>
		public string PagerId => Options.Id + "Pager";

		private string TopPagerSelector => _asSubgrid ? "'#' + subgridTableId + '_toppager'" : $"'#{Id}_toppager'";

		private string PagerSelector => _asSubgrid ? "'#' + subgridPagerId" : $"'#{PagerId}'";

		/// <summary>
		/// Gets the filter grid (div) placeholder identifier.
		/// </summary>
		public string FilterGridId => Options.Id + "Search";

		private string FilterGridSelector => _asSubgrid ? "'#' + subgridFilterGridId" : $"'#{FilterGridId}'";

		/// <summary>
		/// Gets the grid and columns options
		/// </summary>
		public JqGridOptions Options { get; }

		/// <summary>
		/// Gets navigator options
		/// </summary>
		public JqGridNavigatorOptions NavigatorOptions { get; private set; }

		/// <summary>
		/// Gets navigator edit action options
		/// </summary>
		public JqGridNavigatorEditActionOptions NavigatorEditOptions => (JqGridNavigatorEditActionOptions)_navigatorEditActionOptions;

		/// <summary>
		/// Gets navigator add action options
		/// </summary>
		public JqGridNavigatorEditActionOptions NavigatorAddOptions => (JqGridNavigatorEditActionOptions)_navigatorAddActionOptions;

		/// <summary>
		/// Gets navigator delete action options
		/// </summary>
		public JqGridNavigatorDeleteActionOptions NavigatorDeleteOptions => (JqGridNavigatorDeleteActionOptions)_navigatorDeleteActionOptions;

		/// <summary>
		/// Gets navigator search action options
		/// </summary>
		public JqGridNavigatorSearchActionOptions NavigatorSearchOptions => (JqGridNavigatorSearchActionOptions)_navigatorSearchActionOptions;

		/// <summary>
		/// Gets navigator view action options
		/// </summary>
		public JqGridNavigatorViewActionOptions NavigatorViewOptions => (JqGridNavigatorViewActionOptions)_navigatorViewActionOptions;

		/// <summary>
		/// Gets inline navigator options
		/// </summary>
		public JqGridInlineNavigatorOptions InlineNavigatorOptions { get; private set; }

		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the JqGridHelper class.
		/// </summary>
		/// <param name="options">Options for the grid</param>
		/// <param name="subgridHelper">The subgrid helper for "Subgrid as Grid" scenario. If this option has value, the SugridModel, SubgridUrl and SubGridRowExpanded options are ignored. It will also add additional parameter 'id' to the request.</param>
		public JqGridHelper(JqGridOptions options, object subgridHelper = null)
		{
			Options = options;
			Options.SetModel(typeof(TModel));

			if (subgridHelper != null)
			{
				Type subgridHelperType = subgridHelper.GetType();
				if (!subgridHelperType.IsGenericType || subgridHelperType.GetGenericTypeDefinition() != typeof(JqGridHelper<>))
					throw new ArgumentException("The object must be of type JqGridHelper<T>.", nameof(subgridHelper));
				_subgridHelper = subgridHelper;
			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// Returns the HTML that is used to render the table placeholder for the grid. 
		/// </summary>
		/// <returns>The HTML that represents the table placeholder for jqGrid</returns>
		public MvcHtmlString GetTableHtml()
		{
			return MvcHtmlString.Create($"<table id='{Id}'></table>");
		}

		/// <summary>
		/// Returns the HTML that is used to render the pager (div) placeholder for the grid. 
		/// </summary>
		/// <returns>The HTML that represents the pager (div) placeholder for jqGrid</returns>
		public MvcHtmlString GetPagerHtml()
		{
			return MvcHtmlString.Create($"<div id='{PagerId}'></div>");
		}

		/// <summary>
		/// Returns the HTML that is used to render the filter grid (div) placeholder for the grid. 
		/// </summary>
		/// <returns>The HTML that represents the filter grid (div) placeholder for jqGrid</returns>
		public MvcHtmlString GetFilterGridHtml()
		{
			return MvcHtmlString.Create($"<div id='{FilterGridId}'></div>");
		}

		/// <summary>
		/// Returns the HTML that is used to render the table placeholder for the grid with pager placeholder below it and filter grid (if enabled) placeholder above it.
		/// </summary>
		/// <returns>The HTML that represents the table placeholder for jqGrid with pager placeholder below i</returns>
		public MvcHtmlString GetHtml()
		{
			if (_filterGridModel != null && Options.Pager)
				return MvcHtmlString.Create(GetFilterGridHtml().ToHtmlString() + GetTableHtml().ToHtmlString() + GetPagerHtml().ToHtmlString());
			if (_filterGridModel != null)
				return MvcHtmlString.Create(GetFilterGridHtml().ToHtmlString() + GetTableHtml().ToHtmlString());
			if (Options.Pager)
				return MvcHtmlString.Create(GetTableHtml().ToHtmlString() + GetPagerHtml().ToHtmlString());
			return GetTableHtml();
		}

		/// <summary>
		/// Return the JavaScript that is used to initialize jqGrid with given options.
		/// </summary>
		/// <returns>The JavaScript that initializes jqGrid with give options</returns>
		/// <exception cref="System.InvalidOperationException">
		/// <list type="bullet">
		/// <item><description>TreeGrid and data grouping are both enabled.</description></item>
		/// <item><description>Rows numbers and data grouping are both enabled.</description></item>
		/// <item><description>Dynamic scrolling and data grouping are both enabled.</description></item>
		/// <item><description>TreeGrid and GridView are both enabled.</description></item>
		/// <item><description>SubGrid and GridView are both enabled.</description></item>
		/// <item><description>AfterInsertRow event and GridView are both enabled.</description></item>
		/// </list> 
		/// </exception>
		public MvcHtmlString GetJavaScript()
		{
			ValidateLimitations();

			StringBuilder javaScriptBuilder = new StringBuilder();

			javaScriptBuilder.AppendFormat("$({0}).jqGrid({{", GridSelector).AppendLine();
			AppendColumnsNames(javaScriptBuilder);
			AppendColumnsModels(javaScriptBuilder);
			AppendOptions(javaScriptBuilder);
			javaScriptBuilder.Append("})");

			foreach (JqGridColumnModel columnModel in Options.ColumnsModels)
			{
				if (columnModel.LabelOptions != null)
					AppendColumnLabelOptions(columnModel.Name, columnModel.LabelOptions, javaScriptBuilder);
			}

			if (Options.FooterEnabled.GetValueOrDefault() && _footerData != null && _footerData.Any())
				AppendFooterData(javaScriptBuilder);

			if (NavigatorOptions != null)
				AppendNavigator(javaScriptBuilder);

			if (InlineNavigatorOptions != null)
				AppendInlineNavigator(javaScriptBuilder);

			if (NavigatorOptions != null || InlineNavigatorOptions != null)
				AppendNavigatorElements(javaScriptBuilder);

			if (_filterToolbar)
				AppendFilterToolbar(javaScriptBuilder);

			if (_groupHeaders != null && _groupHeaders.Any())
				AppendGroupHeaders(javaScriptBuilder);

			if (_setFrozenColumns)
				javaScriptBuilder.Append(".jqGrid('setFrozenColumns')");

			javaScriptBuilder.AppendLine(";");

			if (_filterGridModel != null)
				AppendFilterGrid(javaScriptBuilder);

			return MvcHtmlString.Create(javaScriptBuilder.ToString());
		}

		internal string GetSubGridRowExpanded()
		{
			_asSubgrid = true;

			StringBuilder subGridRowExpandedBuilder = new StringBuilder();
			subGridRowExpandedBuilder.AppendLine("function(subgridId, rowId) {");

			if (_filterGridModel != null)
			{
				subGridRowExpandedBuilder.AppendLine("var subgridFilterGridId = subgridId + '_s';");
				subGridRowExpandedBuilder.AppendLine("jQuery('#' + subgridId).append('<div id=\"' + subgridFilterGridId + '\"></div>');");
			}

			subGridRowExpandedBuilder.AppendLine("var subgridTableId = subgridId + '_t';");
			subGridRowExpandedBuilder.AppendLine("jQuery('#' + subgridId).append('<table id=\"' + subgridTableId + '\"></table>');");

			if (Options.Pager)
			{
				subGridRowExpandedBuilder.AppendLine("var subgridPagerId = subgridId + '_p';");
				subGridRowExpandedBuilder.AppendLine("jQuery('#' + subgridId).append('<div id=\"' + subgridPagerId + '\"></div>');");
			}

			subGridRowExpandedBuilder.AppendLine(GetJavaScript().ToString());

			subGridRowExpandedBuilder.Append("}");
			return subGridRowExpandedBuilder.ToString();
		}

		private void ValidateLimitations()
		{
			if (Options.TreeGridEnabled.GetValueOrDefault() && Options.GroupingEnabled.GetValueOrDefault())
				throw new InvalidOperationException("TreeGrid and data grouping can not be enabled at the same time.");

			if (Options.RowsNumbers.GetValueOrDefault() && Options.GroupingEnabled.GetValueOrDefault())
				throw new InvalidOperationException("Rows numbers and data grouping can not be enabled at the same time.");

			if (Options.DynamicScrollingMode != JqGridDynamicScrollingModes.Disabled && Options.GroupingEnabled.GetValueOrDefault())
				throw new InvalidOperationException("Dynamic scrolling and data grouping can not be enabled at the same time.");

			if (Options.TreeGridEnabled.GetValueOrDefault() && Options.GridView.GetValueOrDefault())
				throw new InvalidOperationException("TreeGrid and GridView can not be enabled at the same time.");

			if (Options.SubgridEnabled.GetValueOrDefault() && Options.GridView.GetValueOrDefault())
				throw new InvalidOperationException("SubGrid and GridView can not be enabled at the same time.");

			if (!string.IsNullOrWhiteSpace(Options.AfterInsertRow) && Options.GridView.GetValueOrDefault())
				throw new InvalidOperationException("AfterInsertRow event and GridView can not be enabled at the same time.");
		}

		private void AppendColumnsNames(StringBuilder javaScriptBuilder)
		{
			javaScriptBuilder.Append("colNames: [");

			foreach (string columnName in Options.ColumnsNames)
				javaScriptBuilder.AppendFormat("'{0}',", columnName);

			if (javaScriptBuilder[javaScriptBuilder.Length - 1] == ',')
				javaScriptBuilder.Remove(javaScriptBuilder.Length - 1, 1);

			javaScriptBuilder.AppendLine("],");
		}

		private void AppendColumnsModels(StringBuilder javaScriptBuilder)
		{
			javaScriptBuilder.AppendLine("colModel: [");

			int lastColumnModelIndex = Options.ColumnsModels.Count - 1;
			for (int columnModelIndex = 0; columnModelIndex < Options.ColumnsModels.Count; columnModelIndex++)
			{
				JqGridColumnModel columnModel = Options.ColumnsModels[columnModelIndex];
				javaScriptBuilder.Append("{ ");

				if (columnModel.Alignment != JqGridAlignments.Default)
					javaScriptBuilder.AppendFormat("align: '{0}', ", columnModel.Alignment.ToString().ToLower());

				if (columnModel.AutoSize.HasValue)
					javaScriptBuilder.AppendFormat("autosize: {0}, ", columnModel.AutoSize.Value.ToString().ToLower());

				if (columnModel.Index?.IsSetted ?? false)
					javaScriptBuilder.AppendFormat("index: '{0}', ", columnModel.Index.Value);

				if (columnModel.CellAttributes?.IsSetted ?? false)
					javaScriptBuilder.AppendFormat("cellattr: {0}, ", columnModel.CellAttributes.Value);

				if (columnModel.Classes?.IsSetted ?? false)
					javaScriptBuilder.AppendFormat("classes: '{0}', ", columnModel.Classes.Value);

				if (columnModel.DateFormat?.IsSetted ?? false)
					javaScriptBuilder.AppendFormat("datefmt: '{0}', ", columnModel.DateFormat.Value);

				if (columnModel.Editable.HasValue)
				{
					javaScriptBuilder.AppendFormat("editable: {0}, ", columnModel.Editable.Value.ToString().ToLower());
					if (columnModel.Editable.Value)
					{
						if (columnModel.EditType != JqGridColumnEditTypes.Default)
							javaScriptBuilder.AppendFormat("edittype: '{0}', ", columnModel.EditType.ToString().ToLower());
						AppendEditOptions(columnModel.EditOptions, javaScriptBuilder);
						AppendColumnRules("editrules", columnModel.EditRules, javaScriptBuilder);
						AppendFormOptions(columnModel.FormOptions, javaScriptBuilder);
					}
				}

				AppendExportOptions(columnModel.ExportOptions, javaScriptBuilder);

				if (columnModel.Fixed.HasValue)
					javaScriptBuilder.AppendFormat("fixed: {0}, ", columnModel.Fixed.ToString().ToLower());

				if (columnModel.Frozen.HasValue)
					javaScriptBuilder.AppendFormat("frozen: {0}, ", columnModel.Frozen.ToString().ToLower());

				if (columnModel.Formatter?.IsSetted ?? false)
				{
					javaScriptBuilder.AppendFormat("formatter: {0}, ", columnModel.Formatter.Value);
					AppendFormatterOptions(columnModel.Formatter.Value, columnModel.FormatterOptions, javaScriptBuilder);
					if (columnModel.UnFormatter?.IsSetted ?? false)
						javaScriptBuilder.AppendFormat("unformat: {0}, ", columnModel.UnFormatter.Value);
				}

				if (columnModel.InitialSortingOrder != JqGridSortingOrders.Default)
					javaScriptBuilder.AppendFormat("firstsortorder: '{0}', ", columnModel.InitialSortingOrder.ToString().ToLower());

				if (columnModel.Hidden.HasValue)
					javaScriptBuilder.AppendFormat("hidden: {0}, ", columnModel.Hidden.Value.ToString().ToLower());

				if (columnModel.HideInDialog.HasValue)
					javaScriptBuilder.AppendFormat("hidedlg: {0}, ", columnModel.HideInDialog.Value.ToString().ToLower());

				if (!string.IsNullOrWhiteSpace(columnModel.JsonMapping))
					javaScriptBuilder.AppendFormat("jsonmap: '{0}', ", columnModel.JsonMapping);

				if (columnModel.Key.HasValue)
					javaScriptBuilder.AppendFormat("key: {0}, ", columnModel.Key.Value.ToString().ToLower());

				if (columnModel.Resizable.HasValue)
					javaScriptBuilder.AppendFormat("resizable: {0}, ", columnModel.Resizable.Value.ToString().ToLower());

				if (columnModel.Searchable.HasValue)
				{
					javaScriptBuilder.AppendFormat("search: {0}, ", columnModel.Searchable.Value.ToString().ToLower());
					if (columnModel.Searchable.Value)
					{
						if (columnModel.SearchType != JqGridColumnSearchTypes.Default)
							javaScriptBuilder.AppendFormat("stype: '{0}', ", columnModel.SearchType.ToString().ToLower());
						AppendSearchOptions(columnModel.SearchOptions, javaScriptBuilder);
						AppendColumnRules("searchrules", columnModel.SearchRules, javaScriptBuilder);
					}
				}

				if (Options.GroupingEnabled.GetValueOrDefault())
				{
					if (columnModel.SummaryType.HasValue)
					{
						if (columnModel.SummaryType.Value != JqGridColumnSummaryTypes.Custom)
							javaScriptBuilder.AppendFormat("summaryType: '{0}', ", columnModel.SummaryType.Value.ToString().ToLower());
						else
							javaScriptBuilder.AppendFormat("summaryType: {0}, ", columnModel.SummaryFunction);
					}

					if (columnModel.SummaryTemplate?.IsSetted ?? false)
						javaScriptBuilder.AppendFormat("summaryTpl: '{0}', ", columnModel.SummaryTemplate.Value);
				}

				if (columnModel.Sortable.HasValue)
				{
					javaScriptBuilder.AppendFormat("sortable: {0}, ", columnModel.Sortable.Value.ToString().ToLower());
					if (columnModel.Sortable.Value && columnModel.SortType != JqGridColumnSortTypes.Default)
					{
						if (columnModel.SortType == JqGridColumnSortTypes.Function && (columnModel.SortFunction?.IsSetted ?? false))
							javaScriptBuilder.AppendFormat("sorttype: {0}, ", columnModel.SortFunction);
						else if (columnModel.SortType != JqGridColumnSortTypes.Function)
							javaScriptBuilder.AppendFormat("sorttype: '{0}', ", columnModel.SortType.ToString().ToLower());

					}
				}

				if (columnModel.Title.HasValue)
					javaScriptBuilder.AppendFormat("title: {0}, ", columnModel.Title.Value.ToString().ToLower());

				if (columnModel.Tooltip?.IsSetted ?? false)
					javaScriptBuilder.AppendFormat("tooltip: '{0}', ", columnModel.Tooltip.Value.ToNullString());

				if (columnModel.Width.HasValue)
					javaScriptBuilder.AppendFormat("width: {0}, ", columnModel.Width.Value);

				if (columnModel.Viewable.HasValue)
					javaScriptBuilder.AppendFormat("viewable: {0}, ", columnModel.Viewable.ToString().ToLower());

				if (!string.IsNullOrWhiteSpace(columnModel.XmlMapping))
					javaScriptBuilder.AppendFormat("xmlmap: '{0}', ", columnModel.XmlMapping);

				javaScriptBuilder.AppendFormat("name: '{0}' }}", columnModel.Name);

				if (lastColumnModelIndex == columnModelIndex)
					javaScriptBuilder.AppendLine();
				else
					javaScriptBuilder.AppendLine(",");
			}

			if (javaScriptBuilder[javaScriptBuilder.Length - 2] == ',')
				javaScriptBuilder.Remove(javaScriptBuilder.Length - 2, 1);

			javaScriptBuilder.AppendLine("],");
		}

		private void AppendEditOptions(JqGridColumnEditOptions editOptions, StringBuilder javaScriptBuilder)
		{
			if (editOptions != null)
			{
				JavaScriptSerializer serializer = new JavaScriptSerializer();
				javaScriptBuilder.Append("editoptions: { ");

				if (editOptions.IsCustomElementFunctionSetted)
					javaScriptBuilder.AppendFormat("custom_element: {0}, ", editOptions.CustomElementFunction);

				if (editOptions.IsCustomValueFunctionSetted)
					javaScriptBuilder.AppendFormat("custom_value: {0}, ", editOptions.CustomValueFunction);

				if (editOptions.InternalDataEvents != null)
				{
					if (editOptions.DataEvents == null)
						editOptions.DataEvents = new List<JqGridColumnDataEvent>();

					foreach (JqGridColumnDataEvent dataEvent in editOptions.InternalDataEvents)
						editOptions.DataEvents.Add(new JqGridColumnDataEvent(dataEvent.Type, string.Format(dataEvent.Function, Options.Id)));
				}
				AppendElementOptions(editOptions, serializer, javaScriptBuilder);

				if (editOptions.NullIfEmpty.HasValue)
					javaScriptBuilder.AppendFormat("NullIfEmpty: {0}, ", editOptions.NullIfEmpty.Value.ToString().ToLower());

				if (editOptions.HtmlAttributes != null && editOptions.HtmlAttributes.Any())
				{
					string htmlAttributesSerialized = serializer.Serialize(editOptions.HtmlAttributes);
					javaScriptBuilder.AppendFormat("{0}, ", htmlAttributesSerialized.Substring(1, htmlAttributesSerialized.Length - 2));
				}

				if (editOptions.IsPostDataScriptsSetted)
					javaScriptBuilder.AppendFormat("postData: {0}, ", editOptions.PostDataScript);
				else if (editOptions.PostData != null)
				{
					serializer = new JavaScriptSerializer();
					javaScriptBuilder.AppendFormat("postData: {0}, ", serializer.Serialize(editOptions.PostData));
				}

				if (javaScriptBuilder[javaScriptBuilder.Length - 2] == ',')
				{
					javaScriptBuilder.Remove(javaScriptBuilder.Length - 2, 2);
					javaScriptBuilder.Append(" }, ");
				}
				else
					javaScriptBuilder.Remove(javaScriptBuilder.Length - 15, 15);
			}
		}

		private void AppendElementOptions(JqGridColumnElementOptions elementOptions, JavaScriptSerializer serializer, StringBuilder javaScriptBuilder)
		{
			if (elementOptions.IsBuildSelectSetted)
				javaScriptBuilder.AppendFormat("buildSelect: {0}, ", elementOptions.BuildSelect);

			if (elementOptions.DataEvents != null && elementOptions.DataEvents.Any())
			{
				javaScriptBuilder.Append("dataEvents: [ ");
				foreach (JqGridColumnDataEvent dataEvent in elementOptions.DataEvents)
				{
					if (dataEvent.Data == null)
						javaScriptBuilder.AppendFormat("{{ type: '{0}', fn: {1} }}, ", dataEvent.Type, dataEvent.Function);
					else
						javaScriptBuilder.AppendFormat("{{ type: '{0}', data: {1}, fn: {2} }}, ", dataEvent.Type, serializer.Serialize(dataEvent.Data), dataEvent.Function);
				}
				javaScriptBuilder.Remove(javaScriptBuilder.Length - 2, 2);
				javaScriptBuilder.Append(" ], ");
			}

			if (elementOptions.JQueryUIWidgetDataInitRenderer != null)
				javaScriptBuilder.AppendFormat("dataInit: {0}, ", elementOptions.JQueryUIWidgetDataInitRenderer(Options.CompatibilityMode, GridSelector));
			else if (elementOptions.IsDataInitSetted)
				javaScriptBuilder.AppendFormat("dataInit: {0}, ", elementOptions.DataInit);

			if (elementOptions.IsDataUrlSetted)
				javaScriptBuilder.AppendFormat("dataUrl: '{0}', ", elementOptions.DataUrl);

			if (elementOptions.IsDefaultValueSetted)
				javaScriptBuilder.AppendFormat("defaultValue: '{0}', ", elementOptions.DefaultValue);

			if (elementOptions.IsValueSetted)
				javaScriptBuilder.AppendFormat("value: '{0}', ", elementOptions.Value);
			else if (elementOptions.ValueDictionary != null)
				javaScriptBuilder.AppendFormat("value: {0}, ", serializer.Serialize(elementOptions.ValueDictionary));
		}

		private static void AppendExportOptions(JqGridExportOptions exportOptions, StringBuilder javaScriptBuilder)
		{
			if (exportOptions == null) return;
			javaScriptBuilder.Append("exportoptions: { ");

			if (exportOptions.ExcelParser.HasValue)
				javaScriptBuilder.AppendFormat("excel_parser: {0}, ", exportOptions.ExcelParser.Value.ToString().ToLower());

			if (exportOptions.IsExcelFormatSetted)
				javaScriptBuilder.AppendFormat("excel_format: '{0}', ", exportOptions.ExcelFormat);

			if (exportOptions.IsReplaceFormatSetted)
				javaScriptBuilder.AppendFormat("replace_format: {0}, ", exportOptions.ReplaceFormat);

			if (javaScriptBuilder[javaScriptBuilder.Length - 2] == ',')
			{
				javaScriptBuilder.Remove(javaScriptBuilder.Length - 2, 2);
				javaScriptBuilder.Append(" }, ");
			}
			else
				javaScriptBuilder.Remove(javaScriptBuilder.Length - 17, 17);
		}

		private static void AppendColumnRules(string rulesName, JqGridColumnRules rules, StringBuilder javaScriptBuilder)
		{
			if (rules == null) return;
			javaScriptBuilder.AppendFormat("{0}: {{ ", rulesName);

			if (rules.Custom.HasValue)
			{
				javaScriptBuilder.AppendFormat("custom: {0}, ", rules.Custom.Value.ToString().ToLower());

				if (rules.IsCustomFunctionSetted)
					javaScriptBuilder.AppendFormat("custom_func: {0}, ", rules.CustomFunction);
			}

			if (rules.Date.HasValue)
				javaScriptBuilder.AppendFormat("date: {0}, ", rules.Date.Value.ToString().ToLower());

			if (rules.EditHidden.HasValue)
				javaScriptBuilder.AppendFormat("edithidden: {0}, ", rules.EditHidden.Value.ToString().ToLower());

			if (rules.Email.HasValue)
				javaScriptBuilder.AppendFormat("email: {0}, ", rules.Email.Value.ToString().ToLower());

			if (rules.Integer)
				javaScriptBuilder.Append("integer: true, ");

			if (rules.MaxValue.HasValue)
				javaScriptBuilder.AppendFormat("maxValue: {0}, ", rules.MaxValue.Value);

			if (rules.MinValue.HasValue)
				javaScriptBuilder.AppendFormat("minValue: {0}, ", rules.MinValue.Value);

			if (rules.Number)
				javaScriptBuilder.Append("number: true, ");

			if (rules.Required.HasValue)
				javaScriptBuilder.AppendFormat("required: {0}, ", rules.Required.Value.ToString().ToLower());

			if (rules.Time.HasValue)
				javaScriptBuilder.AppendFormat("time: {0}, ", rules.Time.Value.ToString().ToLower());

			if (rules.Url.HasValue)
				javaScriptBuilder.AppendFormat("url: {0}, ", rules.Url.Value.ToString().ToLower());

			if (javaScriptBuilder[javaScriptBuilder.Length - 2] == ',')
			{
				javaScriptBuilder.Remove(javaScriptBuilder.Length - 2, 2);
				javaScriptBuilder.Append(" }, ");
			}
			else
			{
				int rulesNameLength = rulesName.Length + 4;
				javaScriptBuilder.Remove(javaScriptBuilder.Length - rulesNameLength, rulesNameLength);
			}
		}

		private static void AppendFormOptions(JqGridColumnFormOptions formOptions, StringBuilder javaScriptBuilder)
		{
			if (formOptions == null) return;
			javaScriptBuilder.Append("formoptions: { ");

			if (formOptions.ColumnPosition.HasValue)
				javaScriptBuilder.AppendFormat("colpos: {0},", formOptions.ColumnPosition.Value);

			if (formOptions.IsElementPrefixSetted)
				javaScriptBuilder.AppendFormat("elmprefix: '{0}',", formOptions.ElementPrefix);

			if (formOptions.IsElementSuffixSetted)
				javaScriptBuilder.AppendFormat("elmsuffix: '{0}',", formOptions.ElementSuffix);

			if (formOptions.IsLabelSetted)
				javaScriptBuilder.AppendFormat("label: '{0}',", formOptions.Label);

			if (formOptions.RowPosition.HasValue)
				javaScriptBuilder.AppendFormat("rowpos: {0},", formOptions.RowPosition.Value);

			if (javaScriptBuilder[javaScriptBuilder.Length - 1] == ',')
			{
				javaScriptBuilder.Remove(javaScriptBuilder.Length - 1, 1);
				javaScriptBuilder.Append(" }, ");
			}
			else
				javaScriptBuilder.Remove(javaScriptBuilder.Length - 15, 15);
		}

		private static void AppendFormatterOptions(string formatter, JqGridColumnFormatterOptions formatterOptions, StringBuilder javaScriptBuilder)
		{
			if (formatterOptions == null || formatterOptions.IsDefault(formatter)) return;
			javaScriptBuilder.Append("formatoptions: { ");

			switch (formatter)
			{
				case JqGridColumnPredefinedFormatters.Integer:
					javaScriptBuilder.AppendFormat("{0}{1}", !formatterOptions.IsDefaultValueSetted ? string.Empty : $"defaultValue: '{formatterOptions.DefaultValue}', ", !formatterOptions.IsThousandsSeparatorSetted ? string.Empty : $"thousandsSeparator: '{formatterOptions.ThousandsSeparator}', ");
					break;
				case JqGridColumnPredefinedFormatters.Number:
					javaScriptBuilder.AppendFormat("{0}{1}{2}{3}", !formatterOptions.DecimalPlaces.HasValue ? string.Empty : $"decimalPlaces: {formatterOptions.DecimalPlaces.Value}, ", !formatterOptions.IsDecimalSeparatorSetted ? string.Empty : $"decimalSeparator: '{formatterOptions.DecimalSeparator}', ", !formatterOptions.IsDefaultValueSetted ? string.Empty : $"defaultValue: '{formatterOptions.DefaultValue}', ", !formatterOptions.IsThousandsSeparatorSetted ? string.Empty : $"thousandsSeparator: '{formatterOptions.ThousandsSeparator}', ");
					break;
				case JqGridColumnPredefinedFormatters.Currency:
					javaScriptBuilder.AppendFormat("{0}{1}{2}{3}{4}{5}", !formatterOptions.DecimalPlaces.HasValue ? string.Empty : $"decimalPlaces: {formatterOptions.DecimalPlaces.Value}, ", !formatterOptions.IsDecimalSeparatorSetted ? string.Empty : $"decimalSeparator: '{formatterOptions.DecimalSeparator}', ", !formatterOptions.IsDefaultValueSetted ? string.Empty : $"defaultValue: '{formatterOptions.DefaultValue}', ", !formatterOptions.IsPrefixSetted ? string.Empty : $"prefix: '{formatterOptions.Prefix}', ", !formatterOptions.IsSuffixSetted ? string.Empty : $"suffix: '{formatterOptions.Suffix}', ", !formatterOptions.IsThousandsSeparatorSetted ? string.Empty : $"thousandsSeparator: '{formatterOptions.ThousandsSeparator}', ");
					break;
				case JqGridColumnPredefinedFormatters.Date:
					javaScriptBuilder.AppendFormat("{0}{1}", !formatterOptions.IsSourceFormatSetted ? string.Empty : $"srcformat: '{formatterOptions.SourceFormat}', ", !formatterOptions.IsOutputFormatSetted ? string.Empty : $"newformat: '{formatterOptions.OutputFormat}', ");
					break;
				case JqGridColumnPredefinedFormatters.Link:
					javaScriptBuilder.AppendFormat("target: '{0}', ", formatterOptions.Target);
					break;
				case JqGridColumnPredefinedFormatters.ShowLink:
					javaScriptBuilder.AppendFormat("{0}{1}{2}{3}{4}", !formatterOptions.IsBaseLinkUrlSetted ? string.Empty : $"baseLinkUrl: '{formatterOptions.BaseLinkUrl}', ", !formatterOptions.IsShowActionSetted ? string.Empty : $"showAction: '{formatterOptions.ShowAction}', ", !formatterOptions.IsAddParamSetted ? string.Empty : $"addParam: '{formatterOptions.AddParam}', ", !formatterOptions.IsTargetSetted ? string.Empty : $"target: '{formatterOptions.Target}', ", !formatterOptions.IsIdNameSetted ? string.Empty : $"idName: '{formatterOptions.IdName}', ");
					break;
				case JqGridColumnPredefinedFormatters.CheckBox:
					javaScriptBuilder.Append("disabled: false, ");
					break;
				case JqGridColumnPredefinedFormatters.Actions:
					javaScriptBuilder.AppendFormat("{0}{1}{2}", formatterOptions.EditButton ? string.Empty : "editbutton: false, ", formatterOptions.DeleteButton ? string.Empty : "delbutton: false, ", !formatterOptions.UseFormEditing ? string.Empty : "editformbutton: true, ");

					if (formatterOptions.EditButton)
					{
						if (!formatterOptions.UseFormEditing && formatterOptions.InlineEditingOptions != null)
						{
							if (formatterOptions.InlineEditingOptions.Keys.HasValue)
								javaScriptBuilder.AppendFormat("keys: {0}, ", formatterOptions.InlineEditingOptions.Keys.Value.ToString().ToLower());

							if (formatterOptions.InlineEditingOptions.IsOnEditFunctionSetted)
								javaScriptBuilder.AppendFormat("onEdit: {0}, ", formatterOptions.InlineEditingOptions.OnEditFunction.ToNullString());

							if (formatterOptions.InlineEditingOptions.IsSuccessFunctionSetted)
								javaScriptBuilder.AppendFormat("onSuccess: {0}, ", formatterOptions.InlineEditingOptions.SuccessFunction.ToNullString());

							if (formatterOptions.InlineEditingOptions.IsUrlSetted)
								javaScriptBuilder.AppendFormat("url: '{0}', ", formatterOptions.InlineEditingOptions.Url.ToNullString());

							if (formatterOptions.InlineEditingOptions.IsExtraParamScriptSetted)
								javaScriptBuilder.AppendFormat("extraparam: {0}, ", formatterOptions.InlineEditingOptions.ExtraParamScript.ToNullString());
							else if (formatterOptions.InlineEditingOptions.IsExtraParamSetted)
							{
								JavaScriptSerializer serializer = new JavaScriptSerializer();
								javaScriptBuilder.AppendFormat("extraparam: {0}, ", serializer.Serialize(formatterOptions.InlineEditingOptions.ExtraParam));
							}

							if (formatterOptions.InlineEditingOptions.IsAfterSaveFunctionSetted)
								javaScriptBuilder.AppendFormat("afterSave: {0}, ", formatterOptions.InlineEditingOptions.AfterSaveFunction.ToNullString());

							if (formatterOptions.InlineEditingOptions.IsErrorFunctionSetted)
								javaScriptBuilder.AppendFormat("onError: {0}, ", formatterOptions.InlineEditingOptions.ErrorFunction.ToNullString());

							if (formatterOptions.InlineEditingOptions.IsAfterRestoreFunctionSetted)
								javaScriptBuilder.AppendFormat("afterRestore: {0}, ", formatterOptions.InlineEditingOptions.AfterRestoreFunction.ToNullString());

							if (formatterOptions.InlineEditingOptions.RestoreAfterError.HasValue)
								javaScriptBuilder.AppendFormat("restoreAfterError: {0}, ", formatterOptions.InlineEditingOptions.RestoreAfterError.Value.ToString().ToLower());

							if (formatterOptions.InlineEditingOptions.MethodType != JqGridMethodTypes.Default)
								javaScriptBuilder.AppendFormat("mtype: '{0}', ", formatterOptions.InlineEditingOptions.MethodType.ToString().ToUpper());
						}
						else if (formatterOptions.UseFormEditing && formatterOptions.FormEditingOptions != null)
							AppendNavigatorActionOptions("editOptions: ", formatterOptions.FormEditingOptions, javaScriptBuilder);
					}

					if (formatterOptions.DeleteButton && formatterOptions.DeleteOptions != null)
						AppendNavigatorActionOptions("delOptions: ", formatterOptions.DeleteOptions, javaScriptBuilder);
					break;
			}

			if (javaScriptBuilder[javaScriptBuilder.Length - 2] == ',')
			{
				javaScriptBuilder.Remove(javaScriptBuilder.Length - 2, 2);
				javaScriptBuilder.Append(" }, ");
			}
			else
				javaScriptBuilder.Remove(javaScriptBuilder.Length - 17, 17);
		}

		private void AppendSearchOptions(JqGridColumnSearchOptions searchOptions, StringBuilder javaScriptBuilder)
		{
			if (searchOptions == null) return;
			JavaScriptSerializer serializer = new JavaScriptSerializer();
			javaScriptBuilder.Append("searchoptions: { ");

			AppendElementOptions(searchOptions, serializer, javaScriptBuilder);

			if (searchOptions.HtmlAttributes != null && searchOptions.HtmlAttributes.Count > 0)
				javaScriptBuilder.AppendFormat("attr: {0}, ", serializer.Serialize(searchOptions.HtmlAttributes));

			if (searchOptions.ClearSearch.HasValue)
				javaScriptBuilder.AppendFormat("clearSearch: {0}, ", searchOptions.ClearSearch.Value.ToString().ToLower());

			if (searchOptions.SearchHidden.HasValue)
				javaScriptBuilder.AppendFormat("searchhidden: {0}, ", searchOptions.SearchHidden.Value.ToString().ToLower());

			AppendSearchOperators(searchOptions.SearchOperators, javaScriptBuilder);

			if (javaScriptBuilder[javaScriptBuilder.Length - 2] == ',')
			{
				javaScriptBuilder.Remove(javaScriptBuilder.Length - 2, 2);
				javaScriptBuilder.Append(" }, ");
			}
			else
				javaScriptBuilder.Remove(javaScriptBuilder.Length - 17, 17);
		}

		private static void AppendSearchOperators(JqGridSearchOperators? searchOperators, StringBuilder javaScriptBuilder)
		{
			if (searchOperators.HasValue && searchOperators.Value != JqGridSearchOperators.Default)
			{
				javaScriptBuilder.Append("sopt: [ ");
				foreach (JqGridSearchOperators searchOperator in Enum.GetValues(typeof(JqGridSearchOperators)))
				{
					if (searchOperator == JqGridSearchOperators.Default) continue;
					if (searchOperator != JqGridSearchOperators.EqualOrNotEqual && searchOperator != JqGridSearchOperators.NoTextOperators && searchOperator != JqGridSearchOperators.TextOperators && (searchOperators.Value & searchOperator) == searchOperator)
						javaScriptBuilder.AppendFormat("'{0}',", Enum.GetName(typeof(JqGridSearchOperators), searchOperator).ToLower());
				}
				javaScriptBuilder.Remove(javaScriptBuilder.Length - 1, 1);
				javaScriptBuilder.Append("], ");
			}
		}

		private void AppendOptions(StringBuilder javaScriptBuilder)
		{
			if (Options.IsAfterInsertRowSetted)
				javaScriptBuilder.AppendFormat("afterInsertRow: {0},", Options.AfterInsertRow.ToNullString()).AppendLine();

			if (Options.IsAfterEditCellSetted)
				javaScriptBuilder.AppendFormat("afterEditCell: {0},", Options.AfterEditCell.ToNullString()).AppendLine();

			if (Options.IsAfterRestoreCellSetted)
				javaScriptBuilder.AppendFormat("afterRestoreCell: {0},", Options.AfterRestoreCell.ToNullString()).AppendLine();

			if (Options.IsAfterSaveCellSetted)
				javaScriptBuilder.AppendFormat("afterSaveCell: {0},", Options.AfterSaveCell.ToNullString()).AppendLine();

			if (Options.IsAfterSumbitCellSetted)
				javaScriptBuilder.AppendFormat("afterSubmitCell: {0},", Options.AfterSubmitCell.ToNullString()).AppendLine();

			if (Options.IsAltClassSetted)
				javaScriptBuilder.AppendFormat("altclass: '{0}',", Options.AltClass.ToNullString()).AppendLine();

			if (Options.AltRows.HasValue)
				javaScriptBuilder.AppendFormat("altRows: {0},", Options.AltRows.Value.ToString().ToLower()).AppendLine();

			if (Options.AutoEncode.HasValue)
				javaScriptBuilder.AppendFormat("autoencode: {0},", Options.AutoEncode.Value.ToString().ToLower()).AppendLine();

			if (Options.AutoResizing.HasValue)
				javaScriptBuilder.AppendFormat("autoResizing: {0}, ", Options.AutoResizing.Value.ToString().ToLower()).AppendLine();

			if (Options.AutoWidth.HasValue)
				javaScriptBuilder.AppendFormat("autowidth: {0},", Options.AutoWidth.Value.ToString().ToLower()).AppendLine();

			if (Options.IsBeforeRequestSetted)
				javaScriptBuilder.AppendFormat("beforeRequest: {0},", Options.BeforeRequest.ToNullString()).AppendLine();

			if (Options.IsBeforeSelectRowSetted)
				javaScriptBuilder.AppendFormat("beforeSelectRow: {0},", Options.BeforeSelectRow.ToNullString()).AppendLine();

			if (Options.IsBeforeEditCellSetted)
				javaScriptBuilder.AppendFormat("beforeEditCell: {0},", Options.BeforeEditCell.ToNullString()).AppendLine();

			if (Options.IsBeforeSaveCellSetted)
				javaScriptBuilder.AppendFormat("beforeSaveCell: {0},", Options.BeforeSaveCell.ToNullString()).AppendLine();

			if (Options.IsBeforeSubmitCellSetted)
				javaScriptBuilder.AppendFormat("beforeSubmitCell: {0},", Options.BeforeSubmitCell.ToNullString()).AppendLine();

			if (Options.IsBeforeProcessingSetted)
				javaScriptBuilder.AppendFormat("beforeProcessing: {0},", Options.BeforeProcessing.ToNullString()).AppendLine();

			if (Options.CellLayout.HasValue)
				javaScriptBuilder.AppendFormat("cellLayout: {0},", Options.CellLayout.Value).AppendLine();

			if (Options.CellEditingEnabled.HasValue)
			{
				javaScriptBuilder.AppendFormat("cellEdit: {0},", Options.CellEditingEnabled.Value.ToString().ToLower()).AppendLine();
				if (Options.CellEditingEnabled.Value)
				{
					if (Options.CellEditingSubmitMode != JqGridCellEditingSubmitModes.Default)
						javaScriptBuilder.AppendFormat("cellsubmit: '{0}',", Options.CellEditingSubmitMode).AppendLine();

					if (Options.IsCellEditingUrlSetted)
						javaScriptBuilder.AppendFormat("cellurl: '{0}',", Options.CellEditingUrl.ToNullString()).AppendLine();
				}
			}

			if (Options.IsCaptionSetted)
				javaScriptBuilder.AppendFormat("caption: '{0}',", Options.Caption.ToNullString()).AppendLine();

			AppendCustomFilterDefinition(javaScriptBuilder);

			if (Options.HiddenEnabled.HasValue)
				javaScriptBuilder.AppendFormat("hidegrid: {0},", Options.HiddenEnabled.Value.ToString().ToLower()).AppendLine();

			if (Options.Hidden.HasValue)
				javaScriptBuilder.AppendFormat("hiddengrid: {0},", Options.Hidden.Value.ToString().ToLower()).AppendLine();

			if (Options.IsUsedDataString)
			{
				if (Options.IsDataStringSetted)
					javaScriptBuilder.AppendFormat("datastr: '{0}',", Options.DataString).AppendLine();
			}
			else if (_asSubgrid)
			{
				if (Options.IsUrlSetted)
					if (Options.Url.Contains("?"))
						javaScriptBuilder.AppendFormat("url: '{0}&id=' + encodeURIComponent(rowId),", Options.Url).AppendLine();
					else
						javaScriptBuilder.AppendFormat("url: '{0}?id=' + encodeURIComponent(rowId),", Options.Url).AppendLine();
			}
			else if (Options.IsUrlSetted)
				javaScriptBuilder.AppendFormat("url: '{0}',", Options.Url).AppendLine();

			if (Options.DataType != JqGridDataTypes.Default)
				javaScriptBuilder.AppendFormat("datatype: '{0}',", Options.DataType.ToString().ToLower()).AppendLine();

			if (Options.DeepEmpty.HasValue)
				javaScriptBuilder.AppendFormat("deepempty: {0},", Options.DeepEmpty.Value.ToString().ToLower()).AppendLine();

			if (Options.Direction != JqGridLanguageDirections.Default)
				javaScriptBuilder.AppendFormat("direction: '{0}',", Options.Direction.ToString().ToLower()).AppendLine();

			if (Options.DynamicScrollingMode == JqGridDynamicScrollingModes.Disabled)
				javaScriptBuilder.Append("scroll: false,").AppendLine();
			else if (Options.DynamicScrollingMode == JqGridDynamicScrollingModes.HoldAllRows)
				javaScriptBuilder.Append("scroll: true,").AppendLine();
			else if (Options.DynamicScrollingMode == JqGridDynamicScrollingModes.HoldVisibleRows)
			{
				javaScriptBuilder.Append("scroll: 10,").AppendLine();
				if (Options.DynamicScrollingTimeout.HasValue)
					javaScriptBuilder.AppendFormat("scrollTimeout: {0},", Options.DynamicScrollingTimeout.Value).AppendLine();
			}

			if (Options.IsEmptyRecordsSetted)
				javaScriptBuilder.AppendFormat("emptyrecords: '{0}',", Options.EmptyRecords.ToNullString()).AppendLine();

			if (Options.IsEditingUrlSetted)
				javaScriptBuilder.AppendFormat("editurl: '{0}',", Options.EditingUrl.ToNullString()).AppendLine();

			if (Options.EditNextRowCell.HasValue)
				javaScriptBuilder.AppendFormat("editNextRowCell: {0},", Options.EditNextRowCell.Value.ToString().ToLower());

			if (Options.IsErrorCellSetted)
				javaScriptBuilder.AppendFormat("errorCell: {0},", Options.ErrorCell.ToNullString()).AppendLine();

			if (Options.IsFormatCellSetted)
				javaScriptBuilder.AppendFormat("formatCell: {0},", Options.FormatCell.ToNullString()).AppendLine();

			if (Options.FooterEnabled.HasValue)
				javaScriptBuilder.AppendFormat("footerrow: {0},", Options.FooterEnabled.Value.ToString().ToLower()).AppendLine();

			if (Options.UserDataOnFooter.HasValue)
				javaScriptBuilder.AppendFormat("userDataOnFooter: {0},", Options.UserDataOnFooter.Value.ToString().ToLower()).AppendLine();

			if (Options.UserDataOnHeader.HasValue)
				javaScriptBuilder.AppendFormat("userDataOnHeader: {0},", Options.UserDataOnHeader.Value.ToString().ToLower()).AppendLine();

			if (Options.GridView.HasValue)
				javaScriptBuilder.AppendFormat("gridview: {0},", Options.GridView.Value.ToString().ToLower()).AppendLine();

			if (Options.GroupingEnabled.HasValue)
			{
				javaScriptBuilder.AppendFormat("grouping: {0},", Options.GroupingEnabled.Value.ToString().ToLower()).AppendLine();
				if (Options.GroupingEnabled.Value)
					AppendGroupingView(javaScriptBuilder);
			}

			if (Options.HeaderTitles.HasValue)
				javaScriptBuilder.AppendFormat("headertitles: {0},", Options.HeaderTitles.Value.ToString().ToLower()).AppendLine();

			if (Options.HeaderRow.HasValue)
				javaScriptBuilder.AppendFormat("headerrow: {0}, ", Options.HeaderRow.Value.ToString().ToLower()).AppendLine();

			if (Options.HoverRows.HasValue)
				javaScriptBuilder.AppendFormat("hoverrows: {0},", Options.HoverRows.Value.ToString().ToLower()).AppendLine();

			if (Options.IgnoreCase.HasValue)
				javaScriptBuilder.AppendFormat("ignoreCase: {0},", Options.IgnoreCase.Value.ToString().ToLower()).AppendLine();

			AppendJsonReader(javaScriptBuilder);

			if (Options.IsLoadBeforeSendSetted)
				javaScriptBuilder.AppendFormat("loadBeforeSend: {0},", Options.LoadBeforeSend.ToNullString()).AppendLine();

			if (Options.IsLoadCompleteSetted)
				javaScriptBuilder.AppendFormat("loadComplete: {0},", Options.LoadComplete.ToNullString()).AppendLine();

			if (Options.IsLoadErrorSetted)
				javaScriptBuilder.AppendFormat("loadError: {0},", Options.LoadError.ToNullString()).AppendLine();

			if (Options.LoadOnce.HasValue)
				javaScriptBuilder.AppendFormat("loadonce: {0},", Options.LoadOnce.Value.ToString().ToLower()).AppendLine();

			if (Options.MethodType != JqGridMethodTypes.Default)
				javaScriptBuilder.AppendFormat("mtype: '{0}',", Options.MethodType.ToString().ToUpper()).AppendLine();

			if (Options.MultiKey != JqGridMultiKeys.Default)
				if (Options.MultiKey == JqGridMultiKeys.Disable)
					javaScriptBuilder.Append("multikey: null,").AppendLine();
				else
					javaScriptBuilder.AppendFormat("multikey: '{0}Key',", Options.MultiKey.ToString().ToLower()).AppendLine();

			if (Options.MultiBoxOnly.HasValue)
				javaScriptBuilder.AppendFormat("multiboxonly: {0},", Options.MultiBoxOnly.Value.ToString().ToLower()).AppendLine();

			if (Options.MultiSelect.HasValue)
				javaScriptBuilder.AppendFormat("multiselect: {0},", Options.MultiSelect.Value.ToString().ToLower()).AppendLine();

			if (Options.MultiSelectWidth.HasValue)
				javaScriptBuilder.AppendFormat("multiselectWidth: {0},", Options.MultiSelectWidth.Value).AppendLine();

			if (Options.MultiSort.HasValue)
				javaScriptBuilder.AppendFormat("multiSort: {0},", Options.MultiSort.Value.ToString().ToLower()).AppendLine();

			if (Options.IsGridCompleteSetted)
				javaScriptBuilder.AppendFormat("gridComplete: {0},", Options.GridComplete.ToNullString()).AppendLine();

			if (Options.IsOnCellSelectSetted)
				javaScriptBuilder.AppendFormat("onCellSelect: {0},", Options.OnCellSelect.ToNullString()).AppendLine();

			if (Options.IsOnDoubleClickRowSetted)
				javaScriptBuilder.AppendFormat("ondblClickRow: {0},", Options.OnDoubleClickRow.ToNullString()).AppendLine();

			if (Options.IsOnHeaderClickSetted)
				javaScriptBuilder.AppendFormat("onHeaderClick: {0},", Options.OnHeaderClick.ToNullString()).AppendLine();

			if (Options.IsOnInitGridSetted)
				javaScriptBuilder.AppendFormat("onInitGrid: {0},", Options.OnInitGrid.ToNullString()).AppendLine();

			if (Options.IsOnPagingSetted)
				javaScriptBuilder.AppendFormat("onPaging: {0},", Options.OnPaging.ToNullString()).AppendLine();

			if (Options.IsOnRightClickRowSetted)
				javaScriptBuilder.AppendFormat("onRightClickRow: {0},", Options.OnRightClickRow.ToNullString()).AppendLine();

			if (Options.IsOnSelectAllSetted)
				javaScriptBuilder.AppendFormat("onSelectAll: {0},", Options.OnSelectAll.ToNullString()).AppendLine();

			if (Options.IsOnSelectCellSetted)
				javaScriptBuilder.AppendFormat("onSelectCell: {0},", Options.OnSelectCell.ToNullString()).AppendLine();

			if (Options.IsOnSelectRowSetted)
				javaScriptBuilder.AppendFormat("onSelectRow: {0},", Options.OnSelectRow.ToNullString()).AppendLine();

			if (Options.IsOnSortColSetted)
				javaScriptBuilder.AppendFormat("onSortCol: {0},", Options.OnSortCol.ToNullString()).AppendLine();

			if (Options.Pager)
			{
				javaScriptBuilder.AppendFormat("pager: {0},", PagerSelector).AppendLine();

				if (Options.PagerButtons.HasValue)
					javaScriptBuilder.AppendFormat("pgbuttons: {0},", Options.PagerButtons.Value.ToString().ToLower()).AppendLine();

				if (Options.PagerInput.HasValue)
					javaScriptBuilder.AppendFormat("pginput: {0},", Options.PagerInput.Value.ToString().ToLower()).AppendLine();

				if (Options.IsPagerTextSetted)
					javaScriptBuilder.AppendFormat("pgtext: {0},", Options.PagerText.ToNullString()).AppendLine();
			}

			AppendParametersNames(javaScriptBuilder);

			if (Options.IsPostDataScriptSetted)
				javaScriptBuilder.AppendFormat("postData: {0},", string.IsNullOrWhiteSpace(Options.PostDataScript) ? "{}" : Options.PostDataScript).AppendLine();
			else if (Options.IsPostDataSetted)
			{
				JavaScriptSerializer serializer = new JavaScriptSerializer();
				if (Options.PostData == null)
					javaScriptBuilder.Append("postData: {},");
				else
					javaScriptBuilder.AppendFormat("postData: {0},", serializer.Serialize(Options.PostData));
			}

			if (Options.PreserveSelection.HasValue)
				javaScriptBuilder.AppendFormat("preserveSelection: {0},", Options.PreserveSelection.Value.ToString().ToLower());

			if (Options.IsResizeStartSetted)
				javaScriptBuilder.AppendFormat("resizeStart: {0},", Options.ResizeStart.ToNullString()).AppendLine();

			if (Options.IsResizeStopSetted)
				javaScriptBuilder.AppendFormat("resizeStop: {0},", Options.ResizeStop.ToNullString()).AppendLine();

			if (Options.IsRowAttributesSetted)
				javaScriptBuilder.AppendFormat("rowattr: {0},", Options.RowAttributes.ToNullString()).AppendLine();

			if (Options.RowsList != null && Options.RowsList.Any())
			{
				javaScriptBuilder.Append("rowList: [");
				javaScriptBuilder.Append(string.Join(",", Options.RowsList));
				javaScriptBuilder.Append("],").AppendLine();
			}

			if (Options.RowsNumber.HasValue)
				javaScriptBuilder.AppendFormat("rowNum: {0},", Options.RowsNumber.Value).AppendLine();

			if (Options.RowsNumbers.HasValue)
				javaScriptBuilder.AppendFormat("rownumbers: {0},", Options.RowsNumbers.Value.ToString().ToLower()).AppendLine();

			if (Options.RowsNumbersWidth.HasValue)
				javaScriptBuilder.AppendFormat("rownumWidth: {0},", Options.RowsNumbersWidth.Value).AppendLine();

			if (Options.ColumnsRemaping != null && Options.ColumnsRemaping.Any())
			{
				javaScriptBuilder.Append("remapColumns: [");
				javaScriptBuilder.Append(string.Join(",", Options.ColumnsRemaping));
				javaScriptBuilder.Append("],").AppendLine();
			}

			if (Options.ShrinkToFit.HasValue)
				javaScriptBuilder.AppendFormat("shrinkToFit: {0},", Options.ShrinkToFit.Value.ToString().ToLower()).AppendLine();

			if (Options.ScrollOffset.HasValue)
				javaScriptBuilder.AppendFormat("scrollOffset: {0},", Options.ScrollOffset.Value).AppendLine();

			if (Options.IsSerializeCellDataSetted)
				javaScriptBuilder.AppendFormat("serializeCellData: {0},", Options.SerializeCellData.ToNullString()).AppendLine();

			if (Options.IsSerializeGridDataSetted)
				javaScriptBuilder.AppendFormat("serializeGridData: {0},", Options.SerializeGridData.ToNullString()).AppendLine();

			if (Options.IsSerializeSubGridDataSetted)
				javaScriptBuilder.AppendFormat("serializeSubGridData: {0},", Options.SerializeSubGridData.ToNullString()).AppendLine();

			if (Options.Sortable.HasValue)
				javaScriptBuilder.AppendFormat("sortable: {0},", Options.Sortable.Value.ToString().ToLower()).AppendLine();

			if (Options.IsSortingNameSetted)
				javaScriptBuilder.AppendFormat("sortname: '{0}',", Options.SortingName.ToNullString()).AppendLine();

			if (Options.SortingOrder != JqGridSortingOrders.Default)
				javaScriptBuilder.AppendFormat("sortorder: '{0}',", Options.SortingOrder.ToString().ToLower()).AppendLine();

			if (Options.StyleUI != JqGridStyleUIOptions.Default)
				javaScriptBuilder.AppendFormat("styleUI: '{0}',", Options.StyleUI.ToString()).AppendLine();

			if (Options.IconSet != JqGridBootstrap4IconSet.Default)
				javaScriptBuilder.AppendFormat("iconSet: '{0}',", Options.IconSet);

			if (Options.SubgridEnabled.HasValue)
			{
				javaScriptBuilder.AppendFormat("subGrid: {0},", Options.SubgridEnabled.Value.ToString().ToLower()).AppendLine();

				if (Options.SubgridColumnWidth.HasValue)
					javaScriptBuilder.AppendFormat("subGridWidth: {0},", Options.SubgridColumnWidth.Value).AppendLine();

				if (Options.IsSubGridBeforeExpandSetted)
					javaScriptBuilder.AppendFormat("subGridBeforeExpand: {0},", Options.SubGridBeforeExpand.ToNullString()).AppendLine();

				if (_subgridHelper != null)
				{
					Type subgridHelperType = _subgridHelper.GetType();
					javaScriptBuilder.AppendFormat("subGridRowExpanded: {0},", subgridHelperType.GetMethod("GetSubGridRowExpanded", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(_subgridHelper, null)).AppendLine();
				}
				else
				{
					AppendSubgridModel(javaScriptBuilder);

					if (Options.IsSubgridUrlSetted)
						javaScriptBuilder.AppendFormat("subGridUrl: '{0}',", Options.SubgridUrl.ToNullString()).AppendLine();


					if (Options.IsSubGridRowExpandedSetted)
						javaScriptBuilder.AppendFormat("subGridRowExpanded: {0},", Options.SubGridRowExpanded.ToNullString()).AppendLine();
				}

				if (Options.IsSubGridRowColapsedSetted)
					javaScriptBuilder.AppendFormat("subGridRowColapsed: {0},", Options.SubGridRowColapsed.ToNullString()).AppendLine();
			}

			if (Options.TopPager.HasValue)
				javaScriptBuilder.AppendFormat("toppager: {0},", Options.TopPager.Value.ToString().ToLower()).AppendLine();

			if (Options.TreeGridEnabled.HasValue)
			{
				javaScriptBuilder.AppendFormat("treeGrid: {0},", Options.TreeGridEnabled.Value.ToString().ToLower()).AppendLine();
				if (Options.TreeGridEnabled.Value)
				{

					if (Options.ExpandColumnClick.HasValue)
						javaScriptBuilder.AppendFormat("ExpandColClick: {0},", Options.ExpandColumnClick.Value.ToString().ToLower()).AppendLine();

					if (Options.IsExpandColumnSetted)
						javaScriptBuilder.AppendFormat("ExpandColumn: '{0}',", Options.ExpandColumn.ToNullString()).AppendLine();

					if (Options.TreeGridModel != JqGridTreeGridModels.Default)
						javaScriptBuilder.AppendFormat("treeGridModel: '{0}',", Options.TreeGridModel.ToString().ToLower()).AppendLine();

					if (Options.TreeRootLevel.HasValue)
						javaScriptBuilder.AppendFormat("tree_root_level: {0},", Options.TreeRootLevel.Value).AppendLine();

					if (Options.TreeGridBigData.HasValue)
						javaScriptBuilder.AppendFormat("treeGrid_bigData: {0},", Options.TreeGridBigData.Value.ToString().ToLower()).AppendLine();

					AppendTreeGridIcons(javaScriptBuilder);
					AppendTreeGridReader(javaScriptBuilder);
				}
			}

			if (Options.ViewRecords.HasValue)
				javaScriptBuilder.AppendFormat("viewrecords: {0},", Options.ViewRecords.Value.ToString().ToLower()).AppendLine();

			if (Options.Width.HasValue)
				javaScriptBuilder.AppendFormat("width: {0},", Options.Width.Value).AppendLine();

			if (Options.Height.HasValue)
				javaScriptBuilder.AppendFormat("height: {0}", Options.Height.Value).AppendLine();
			else
				javaScriptBuilder.AppendLine("height: '100%'");
		}

		private void AppendCustomFilterDefinition(StringBuilder javaScriptBuilder)
		{
			if (Options.CustomFilterDefinition == null || !Options.CustomFilterDefinition.Any()) return;
			javaScriptBuilder.Append("customFilterDef: { ");

			foreach (JqGridCustomFilterOperand operand in Options.CustomFilterDefinition)
			{
				javaScriptBuilder.AppendFormat("'{0}': {{ operand: '{1}', text: '{2}', action: {3} }}, ", 
					operand.Id,
					operand.Operand,
					operand.Text,
					operand.Action);
			}

			javaScriptBuilder.Remove(javaScriptBuilder.Length - 2, 2);
			javaScriptBuilder.AppendLine(" }, ");
		}

		private void AppendJsonReader(StringBuilder javaScriptBuilder)
		{
			if (Options.JsonReader == null || Options.JsonReader.IsGlobal) return;
			javaScriptBuilder.Append("jsonReader: { ");

			if (Options.JsonReader.Records != JqGridResponse.JsonReader.Records)
				javaScriptBuilder.AppendFormat("root: '{0}', ", Options.JsonReader.Records);

			if (Options.JsonReader.PageIndex != JqGridResponse.JsonReader.PageIndex)
				javaScriptBuilder.AppendFormat("page: '{0}', ", Options.JsonReader.PageIndex);

			if (Options.JsonReader.TotalPagesCount != JqGridResponse.JsonReader.TotalPagesCount)
				javaScriptBuilder.AppendFormat("total: '{0}', ", Options.JsonReader.TotalPagesCount);

			if (Options.JsonReader.TotalRecordsCount != JqGridResponse.JsonReader.TotalRecordsCount)
				javaScriptBuilder.AppendFormat("records: '{0}', ", Options.JsonReader.TotalRecordsCount);

			if (Options.JsonReader.RepeatItems != JqGridResponse.JsonReader.RepeatItems)
				javaScriptBuilder.AppendFormat("repeatitems: {0}, ", Options.JsonReader.RepeatItems.ToString().ToLower());

			if (Options.JsonReader.RecordValues != JqGridOptionsDefaults.ResponseRecordValues)
				javaScriptBuilder.AppendFormat("cell: '{0}', ", Options.JsonReader.RecordValues);

			if (Options.JsonReader.RecordId != JqGridResponse.JsonReader.RecordId)
				javaScriptBuilder.AppendFormat("id: '{0}', ", Options.JsonReader.RecordId);

			if (Options.JsonReader.UserData != JqGridResponse.JsonReader.UserData)
				javaScriptBuilder.AppendFormat("userdata: '{0}', ", Options.JsonReader.UserData);

			if (!Options.JsonReader.SubgridReader.IsGlobal)
			{
				javaScriptBuilder.Append("subgrid: { ");

				if (Options.JsonReader.SubgridReader.Records != JqGridResponse.JsonReader.SubgridReader.Records)
					javaScriptBuilder.AppendFormat("root: '{0}', ", Options.JsonReader.SubgridReader.Records);

				if (Options.JsonReader.SubgridReader.RepeatItems != JqGridResponse.JsonReader.SubgridReader.RepeatItems)
					javaScriptBuilder.AppendFormat("repeatitems: {0}, ", Options.JsonReader.SubgridReader.RepeatItems.ToString().ToLower());

				if (Options.JsonReader.SubgridReader.RecordValues != JqGridResponse.JsonReader.SubgridReader.RecordValues)
					javaScriptBuilder.AppendFormat("cell: '{0}', ", Options.JsonReader.SubgridReader.RecordValues);

				javaScriptBuilder.Remove(javaScriptBuilder.Length - 2, 2);
				javaScriptBuilder.Append(" }, ");
			}

			javaScriptBuilder.Remove(javaScriptBuilder.Length - 2, 2);
			javaScriptBuilder.Append(" }, ").AppendLine();
		}

		private void AppendParametersNames(StringBuilder javaScriptBuilder)
		{
			if (Options.ParametersNames == null || Options.ParametersNames.IsGlobal) return;
			javaScriptBuilder.Append("prmNames: { ");

			if (Options.ParametersNames.PageIndex != JqGridRequest.ParameterNames.PageIndex)
				javaScriptBuilder.AppendFormat("page: '{0}', ", Options.ParametersNames.PageIndex);

			if (Options.ParametersNames.RecordsCount != JqGridRequest.ParameterNames.RecordsCount)
				javaScriptBuilder.AppendFormat("rows: '{0}', ", Options.ParametersNames.RecordsCount);

			if (Options.ParametersNames.SortingName != JqGridRequest.ParameterNames.SortingName)
				javaScriptBuilder.AppendFormat("sort: '{0}', ", Options.ParametersNames.SortingName);

			if (Options.ParametersNames.SortingOrder != JqGridRequest.ParameterNames.SortingOrder)
				javaScriptBuilder.AppendFormat("order: '{0}', ", Options.ParametersNames.SortingOrder);

			if (Options.ParametersNames.Searching != JqGridRequest.ParameterNames.Searching)
				javaScriptBuilder.AppendFormat("search: '{0}', ", Options.ParametersNames.Searching);

			if (Options.ParametersNames.Id != JqGridRequest.ParameterNames.Id)
				javaScriptBuilder.AppendFormat("id: '{0}', ", Options.ParametersNames.Id);

			if (Options.ParametersNames.Operator != JqGridRequest.ParameterNames.Operator)
				javaScriptBuilder.AppendFormat("oper: '{0}', ", Options.ParametersNames.Operator);

			if (Options.ParametersNames.EditOperator != JqGridRequest.ParameterNames.EditOperator)
				javaScriptBuilder.AppendFormat("editoper: '{0}', ", Options.ParametersNames.EditOperator);

			if (Options.ParametersNames.AddOperator != JqGridRequest.ParameterNames.AddOperator)
				javaScriptBuilder.AppendFormat("addoper: '{0}', ", Options.ParametersNames.AddOperator);

			if (Options.ParametersNames.DeleteOperator != JqGridRequest.ParameterNames.DeleteOperator)
				javaScriptBuilder.AppendFormat("deloper: '{0}', ", Options.ParametersNames.DeleteOperator);

			if (Options.ParametersNames.SubgridId != JqGridRequest.ParameterNames.SubgridId)
				javaScriptBuilder.AppendFormat("subgridid: '{0}', ", Options.ParametersNames.SubgridId);

			if (Options.ParametersNames.PagesCount != JqGridRequest.ParameterNames.PagesCount)
				javaScriptBuilder.AppendFormat("npage: '{0}', ", Options.ParametersNames.PagesCount);

			if (Options.ParametersNames.TotalRows != JqGridRequest.ParameterNames.TotalRows)
				javaScriptBuilder.AppendFormat("totalrows: '{0}', ", Options.ParametersNames.TotalRows);

			javaScriptBuilder.Remove(javaScriptBuilder.Length - 2, 2);
			javaScriptBuilder.Append(" }, ").AppendLine();
		}

		private void AppendGroupingView(StringBuilder javaScriptBuilder)
		{
			if (Options.GroupingView == null) return;
			javaScriptBuilder.Append("groupingView: { ");

			if (Options.GroupingView.Fields != null && Options.GroupingView.Fields.Length > 0)
			{
				javaScriptBuilder.Append("groupField: [");
				foreach (string field in Options.GroupingView.Fields)
					javaScriptBuilder.AppendFormat("'{0}', ", field);
				javaScriptBuilder.Insert(javaScriptBuilder.Length - 2, ']');
			}

			if (Options.GroupingView.Orders != null && Options.GroupingView.Orders.Length > 0 && Options.GroupingView.Orders.Contains(JqGridSortingOrders.Desc))
			{
				javaScriptBuilder.Append("groupOrder: [");
				foreach (JqGridSortingOrders order in Options.GroupingView.Orders)
					javaScriptBuilder.AppendFormat("'{0}', ", order.ToString().ToLower());
				javaScriptBuilder.Insert(javaScriptBuilder.Length - 2, ']');
			}

			if (Options.GroupingView.Texts != null && Options.GroupingView.Texts.Length > 0)
			{
				javaScriptBuilder.Append("groupText: [");
				foreach (string text in Options.GroupingView.Texts)
					javaScriptBuilder.AppendFormat("'{0}', ", text);
				javaScriptBuilder.Insert(javaScriptBuilder.Length - 2, ']');
			}

			if (Options.GroupingView.Summary != null && Options.GroupingView.Summary.Length > 0 && Options.GroupingView.Summary.Contains(true))
			{
				javaScriptBuilder.Append("groupSummary: [");
				foreach (bool summary in Options.GroupingView.Summary)
					javaScriptBuilder.AppendFormat("{0}, ", summary.ToString().ToLower());
				javaScriptBuilder.Insert(javaScriptBuilder.Length - 2, ']');
			}

			if (Options.GroupingView.ColumnShow != null && Options.GroupingView.ColumnShow.Length > 0 && Options.GroupingView.ColumnShow.Contains(false))
			{
				javaScriptBuilder.Append("groupColumnShow: [");
				foreach (bool columnShow in Options.GroupingView.ColumnShow)
					javaScriptBuilder.AppendFormat("{0}, ", columnShow.ToString().ToLower());
				javaScriptBuilder.Insert(javaScriptBuilder.Length - 2, ']');
			}

			if (Options.GroupingView.IsInTheSameGroupCallbacks != null && Options.GroupingView.IsInTheSameGroupCallbacks.Length > 0)
			{
				javaScriptBuilder.Append("isInTheSameGroup: [");
				foreach (string isInTheSameGroupCallback in Options.GroupingView.IsInTheSameGroupCallbacks)
					javaScriptBuilder.AppendFormat("{0}, ", isInTheSameGroupCallback);
				javaScriptBuilder.Insert(javaScriptBuilder.Length - 2, ']');
			}

			if (Options.GroupingView.FormatDisplayFieldCallbacks != null && Options.GroupingView.FormatDisplayFieldCallbacks.Length > 0)
			{
				javaScriptBuilder.Append("formatDisplayField: [");
				foreach (string formatDisplayFieldCallback in Options.GroupingView.FormatDisplayFieldCallbacks)
					javaScriptBuilder.AppendFormat("{0}, ", formatDisplayFieldCallback);
				javaScriptBuilder.Insert(javaScriptBuilder.Length - 2, ']');
			}

			if (Options.GroupingView.SummaryOnHide)
				javaScriptBuilder.Append("showSummaryOnHide: true, ");

			if (Options.GroupingView.DataSorted)
				javaScriptBuilder.Append("groupDataSorted: true, ");

			if (Options.GroupingView.Collapse)
				javaScriptBuilder.Append("groupCollapse: true, ");

			if (Options.GroupingView.PlusIcon != JqGridOptionsDefaults.GroupingPlusIcon)
				javaScriptBuilder.AppendFormat("plusicon: '{0}', ", Options.GroupingView.PlusIcon);

			if (Options.GroupingView.MinusIcon != JqGridOptionsDefaults.GroupingMinusIcon)
				javaScriptBuilder.AppendFormat("minusicon: '{0}', ", Options.GroupingView.MinusIcon);

			if (javaScriptBuilder[javaScriptBuilder.Length - 2] == ',')
			{
				javaScriptBuilder.Remove(javaScriptBuilder.Length - 2, 2);
				javaScriptBuilder.Append(" }, ").AppendLine();
			}
			else
				javaScriptBuilder.Remove(javaScriptBuilder.Length - 16, 16);
		}

		private void AppendTreeGridIcons(StringBuilder javaScriptBuilder)
		{
			if (Options.TreeIcons == null) return;
			javaScriptBuilder.Append("treeIcons: { ");

			if (!string.IsNullOrWhiteSpace(Options.TreeIcons.PlusIcon))
				javaScriptBuilder.AppendFormat("plus: '{0}', ", Options.TreeIcons.PlusIcon);

			if (!string.IsNullOrWhiteSpace(Options.TreeIcons.MinusIcon))
				javaScriptBuilder.AppendFormat("minus: '{0}', ", Options.TreeIcons.MinusIcon);

			if (!string.IsNullOrWhiteSpace(Options.TreeIcons.LeafIcon))
				javaScriptBuilder.AppendFormat("leaf: '{0}', ", Options.TreeIcons.LeafIcon);

			if (javaScriptBuilder[javaScriptBuilder.Length - 2] == ',')
			{
				javaScriptBuilder.Remove(javaScriptBuilder.Length - 2, 2);
				javaScriptBuilder.Append(" },").AppendLine();
			}
			else
				javaScriptBuilder.Remove(javaScriptBuilder.Length - 13, 13);
		}

		private void AppendTreeGridReader(StringBuilder javaScriptBuilder)
		{
			if (Options.TreeReader == null) return;
			javaScriptBuilder.Append("treeReader: { ");

			if (!string.IsNullOrWhiteSpace(Options.TreeReader.LevelField))
				javaScriptBuilder.AppendFormat("level_field: '{0}', ", Options.TreeReader.LevelField);

			if (!string.IsNullOrWhiteSpace(Options.TreeReader.LeafField))
				javaScriptBuilder.AppendFormat("leaf_field: '{0}', ", Options.TreeReader.LeafField);

			if (!string.IsNullOrWhiteSpace(Options.TreeReader.ExpandedField))
				javaScriptBuilder.AppendFormat("expanded_field: '{0}', ", Options.TreeReader.ExpandedField);

			if (!string.IsNullOrWhiteSpace(Options.TreeReader.Loaded))
				javaScriptBuilder.AppendFormat("loaded: '{0}', ", Options.TreeReader.Loaded);

			if (!string.IsNullOrWhiteSpace(Options.TreeReader.IconField))
				javaScriptBuilder.AppendFormat("icon_field: '{0}', ", Options.TreeReader.IconField);

			if (Options.TreeReader is JqGridNestedTreeReader nestedTreeReader)
			{
				if (!string.IsNullOrWhiteSpace(nestedTreeReader.LeftField))
					javaScriptBuilder.AppendFormat("left_field: '{0}', ", nestedTreeReader.LeftField);

				if (!string.IsNullOrWhiteSpace(nestedTreeReader.RightField))
					javaScriptBuilder.AppendFormat("right_field: '{0}', ", nestedTreeReader.RightField);
			}

			if (Options.TreeReader is JqGridAdjacencyTreeReader adjacencyTreeReader)
			{
				if (!string.IsNullOrWhiteSpace(adjacencyTreeReader.ParentIdField))
					javaScriptBuilder.AppendFormat("parent_id_field: '{0}', ", adjacencyTreeReader.ParentIdField);
			}

			if (javaScriptBuilder[javaScriptBuilder.Length - 2] == ',')
			{
				javaScriptBuilder.Remove(javaScriptBuilder.Length - 2, 2);
				javaScriptBuilder.Append(" },").AppendLine();
			}
			else
				javaScriptBuilder.Remove(javaScriptBuilder.Length - 14, 14);
		}

		private void AppendSubgridModel(StringBuilder javaScriptBuilder)
		{
			if (Options.SubgridModel == null) return;
			javaScriptBuilder.AppendLine("subGridModel: [{ ");

			javaScriptBuilder.Append("name: [");
			foreach (string columnName in Options.SubgridModel.ColumnsNames)
				javaScriptBuilder.AppendFormat("'{0}',", columnName);
			if (Options.SubgridModel.ColumnsNames.Any())
				javaScriptBuilder.Remove(javaScriptBuilder.Length - 1, 1);
			javaScriptBuilder.AppendLine("],");

			javaScriptBuilder.Append("align: [");
			foreach (JqGridAlignments alignments in Options.SubgridModel.ColumnsAlignments)
			{
				JqGridAlignments align = alignments == JqGridAlignments.Default ? JqGridAlignments.Left : alignments;
				javaScriptBuilder.AppendFormat("'{0}',", align.ToString().ToLower());
			}

			if (Options.SubgridModel.ColumnsAlignments.Any())
				javaScriptBuilder.Remove(javaScriptBuilder.Length - 1, 1);
			javaScriptBuilder.AppendLine("],");

			javaScriptBuilder.Append("width: [");
			foreach (int width in Options.SubgridModel.ColumnsWidths)
				javaScriptBuilder.AppendFormat("{0},", width);
			if (Options.SubgridModel.ColumnsWidths.Any())
				javaScriptBuilder.Remove(javaScriptBuilder.Length - 1, 1);
			javaScriptBuilder.AppendLine("]");

			javaScriptBuilder.AppendLine(" }],");
		}

		private void AppendNavigator(StringBuilder javaScriptBuilder)
		{
			string navigatorPagerSelector = NavigatorOptions.Pager == JqGridNavigatorPagers.Top ? TopPagerSelector : PagerSelector;
			javaScriptBuilder.AppendFormat(".jqGrid('navGrid', {0},", navigatorPagerSelector).AppendLine();
			AppendNavigatorOptions(javaScriptBuilder);

			if (_navigatorEditActionOptions != null || _navigatorAddActionOptions != null || _navigatorDeleteActionOptions != null || _navigatorSearchActionOptions != null || _navigatorViewActionOptions != null)
			{
				javaScriptBuilder.AppendLine(",");
				AppendNavigatorActionOptions(string.Empty, _navigatorEditActionOptions, javaScriptBuilder);
				if (_navigatorAddActionOptions != null || _navigatorDeleteActionOptions != null || _navigatorSearchActionOptions != null || _navigatorViewActionOptions != null)
				{
					javaScriptBuilder.AppendLine(",");
					AppendNavigatorActionOptions(string.Empty, _navigatorAddActionOptions, javaScriptBuilder);
					if (_navigatorDeleteActionOptions != null || _navigatorSearchActionOptions != null || _navigatorViewActionOptions != null)
					{
						javaScriptBuilder.AppendLine(",");
						AppendNavigatorActionOptions(string.Empty, _navigatorDeleteActionOptions, javaScriptBuilder);
						if (_navigatorSearchActionOptions != null || _navigatorViewActionOptions != null)
						{
							javaScriptBuilder.AppendLine(",");
							AppendNavigatorActionOptions(string.Empty, _navigatorSearchActionOptions, javaScriptBuilder);
							if (_navigatorViewActionOptions != null)
							{
								javaScriptBuilder.AppendLine(",");
								AppendNavigatorActionOptions(string.Empty, _navigatorViewActionOptions, javaScriptBuilder);
							}
						}
					}
				}
			}

			javaScriptBuilder.Append(")");

			foreach (JqGridNavigatorControlOptions controlOptions in _navigatorControlsOptions.Where(o => !o.AddAfterInlineNavigator))
			{
				if (controlOptions is JqGridNavigatorButtonOptions buttonOptions)
				{
					AppendNavigatorButton(navigatorPagerSelector, buttonOptions, javaScriptBuilder);
					if (NavigatorOptions.Pager == JqGridNavigatorPagers.Standard && controlOptions.CloneToTop)
						AppendNavigatorButton(TopPagerSelector, buttonOptions, javaScriptBuilder);
				}
				else if (controlOptions is JqGridNavigatorSeparatorOptions separatorOptions)
				{
					AppendNavigatorSeparator(navigatorPagerSelector, separatorOptions, javaScriptBuilder);
					if (NavigatorOptions.Pager == JqGridNavigatorPagers.Standard && controlOptions.CloneToTop)
						AppendNavigatorSeparator(TopPagerSelector, separatorOptions, javaScriptBuilder);
				}
			}
		}

		private static void AppendBaseNavigatorOptions(JqGridNavigatorOptionsBase baseNavigatorOptions, StringBuilder javaScriptBuilder)
		{
			if (baseNavigatorOptions.Add.HasValue)
				javaScriptBuilder.AppendFormat("add: {0}, ", baseNavigatorOptions.Add.Value.ToString().ToLower());

			if (baseNavigatorOptions.IsAddIconSetted)
				javaScriptBuilder.AppendFormat("addicon: '{0}', ", baseNavigatorOptions.AddIcon.ToNullString());

			if (baseNavigatorOptions.IsAddTextSetted)
				javaScriptBuilder.AppendFormat("addtext: '{0}', ", baseNavigatorOptions.AddText.ToNullString());

			if (baseNavigatorOptions.IsAddToolTipSetted)
				javaScriptBuilder.AppendFormat("addtitle: '{0}', ", baseNavigatorOptions.AddToolTip.ToNullString());

			if (baseNavigatorOptions.Edit.HasValue)
				javaScriptBuilder.AppendFormat("edit: {0}, ", baseNavigatorOptions.Edit.Value.ToString().ToLower());

			if (baseNavigatorOptions.IsEditIconSetted)
				javaScriptBuilder.AppendFormat("editicon: '{0}', ", baseNavigatorOptions.EditIcon.ToNullString());

			if (baseNavigatorOptions.IsEditTextSetted)
				javaScriptBuilder.AppendFormat("edittext: '{0}', ", baseNavigatorOptions.EditText.ToNullString());

			if (baseNavigatorOptions.IsEditToolTipSetted)
				javaScriptBuilder.AppendFormat("edittitle: '{0}', ", baseNavigatorOptions.EditToolTip.ToNullString());

			if (baseNavigatorOptions.Position != JqGridAlignments.Default)
				javaScriptBuilder.AppendFormat("position: '{0}', ", baseNavigatorOptions.Position.ToString().ToLower());
		}

		private void AppendNavigatorOptions(StringBuilder javaScriptBuilder)
		{
			javaScriptBuilder.Append("{ ");

			AppendBaseNavigatorOptions(NavigatorOptions, javaScriptBuilder);

			if (NavigatorOptions.IsAlertCaptionSetted)
				javaScriptBuilder.AppendFormat("alertcap: '{0}', ", NavigatorOptions.AlertCaption.ToNullString());

			if (NavigatorOptions.IsAlertTextSetted)
				javaScriptBuilder.AppendFormat("alerttext: '{0}', ", NavigatorOptions.AlertText.ToNullString());

			if (NavigatorOptions.CloneToTop.HasValue)
				javaScriptBuilder.AppendFormat("cloneToTop: {0}, ", NavigatorOptions.CloneToTop.Value.ToString().ToLower());

			if (NavigatorOptions.CloseOnEscape.HasValue)
				javaScriptBuilder.AppendFormat("closeOnEscape: {0}, ", NavigatorOptions.CloseOnEscape.Value.ToString().ToLower());

			if (NavigatorOptions.Delete.HasValue)
				javaScriptBuilder.AppendFormat("del: {0}, ", NavigatorOptions.Delete.Value.ToString().ToLower());

			if (NavigatorOptions.IsDeleteIconSetted)
				javaScriptBuilder.AppendFormat("delicon: '{0}', ", NavigatorOptions.DeleteIcon.ToNullString());

			if (NavigatorOptions.IsDeleteTextSetted)
				javaScriptBuilder.AppendFormat("deltext: '{0}', ", NavigatorOptions.DeleteText.ToNullString());

			if (NavigatorOptions.IsDeleteToolTipSetted)
				javaScriptBuilder.AppendFormat("deltitle: '{0}', ", NavigatorOptions.DeleteToolTip.ToNullString());

			if (NavigatorOptions.Refresh.HasValue)
				javaScriptBuilder.AppendFormat("refresh: {0}, ", NavigatorOptions.Refresh.Value.ToString().ToLower());

			if (NavigatorOptions.IsRefreshIconSetted)
				javaScriptBuilder.AppendFormat("refreshicon: '{0}', ", NavigatorOptions.RefreshIcon.ToNullString());

			if (NavigatorOptions.IsRefreshTextSetted)
				javaScriptBuilder.AppendFormat("refreshtext: '{0}', ", NavigatorOptions.RefreshText.ToNullString());

			if (NavigatorOptions.IsRefreshToolTipSetted)
				javaScriptBuilder.AppendFormat("refreshtitle: '{0}', ", NavigatorOptions.RefreshToolTip.ToNullString());

			if (NavigatorOptions.RefreshMode != JqGridRefreshModes.Default)
				javaScriptBuilder.AppendFormat("refreshstate: '{0}', ", NavigatorOptions.RefreshMode.ToString().ToLower());

			if (NavigatorOptions.IsAfterRefreshSetted)
				javaScriptBuilder.AppendFormat("afterRefresh: {0}, ", NavigatorOptions.AfterRefresh.ToNullString());

			if (NavigatorOptions.IsBeforeRefreshSetted)
				javaScriptBuilder.AppendFormat("beforeRefresh: {0}, ", NavigatorOptions.BeforeRefresh.ToNullString());

			if (NavigatorOptions.Search.HasValue)
				javaScriptBuilder.AppendFormat("search: {0}, ", NavigatorOptions.Search.Value.ToString().ToLower());

			if (NavigatorOptions.IsSearchIconSetted)
				javaScriptBuilder.AppendFormat("searchicon: '{0}', ", NavigatorOptions.SearchIcon.ToNullString());

			if (NavigatorOptions.IsSearchTextSetted)
				javaScriptBuilder.AppendFormat("searchtext: '{0}', ", NavigatorOptions.SearchText.ToNullString());

			if (NavigatorOptions.IsSearchToolTipSetted)
				javaScriptBuilder.AppendFormat("searchtitle: '{0}', ", NavigatorOptions.SearchToolTip.ToNullString());

			if (NavigatorOptions.View.HasValue)
				javaScriptBuilder.AppendFormat("view: {0}, ", NavigatorOptions.View.Value.ToString().ToLower());

			if (NavigatorOptions.IsViewIconSetted)
				javaScriptBuilder.AppendFormat("viewicon: '{0}', ", NavigatorOptions.ViewIcon.ToNullString());

			if (NavigatorOptions.IsViewTextSetted)
				javaScriptBuilder.AppendFormat("viewtext: '{0}', ", NavigatorOptions.ViewText.ToNullString());

			if (NavigatorOptions.IsViewToolTipSetted)
				javaScriptBuilder.AppendFormat("viewtitle: '{0}', ", NavigatorOptions.ViewToolTip.ToNullString());

			if (NavigatorOptions.IsAddFunctionSetted)
				javaScriptBuilder.AppendFormat("addfunc: {0}, ", NavigatorOptions.AddFunction.ToNullString());

			if (NavigatorOptions.IsEditFunctionSetted)
				javaScriptBuilder.AppendFormat("editfunc: {0}, ", NavigatorOptions.EditFunction.ToNullString());

			if (NavigatorOptions.IsDeleteFunctionSetted)
				javaScriptBuilder.AppendFormat("delfunc: {0}, ", NavigatorOptions.DeleteFunction.ToNullString());

			if (javaScriptBuilder[javaScriptBuilder.Length - 2] == ',')
			{
				javaScriptBuilder.Remove(javaScriptBuilder.Length - 2, 2);
				javaScriptBuilder.Append(" }");
			}
			else
				javaScriptBuilder.Append("}");
		}

		private static void AppendNavigatorActionOptions(string optionsName, JqGridNavigatorActionOptions actionOptions, StringBuilder javaScriptBuilder)
		{
			javaScriptBuilder.AppendFormat("{0}{{ ", optionsName);

			if (actionOptions != null)
			{
				if (actionOptions.Left.HasValue)
					javaScriptBuilder.AppendFormat("left: {0}, ", actionOptions.Left.Value);

				if (actionOptions.Top.HasValue)
					javaScriptBuilder.AppendFormat("top: {0}, ", actionOptions.Top.Value);

				if (actionOptions.DataWidth.HasValue)
					javaScriptBuilder.AppendFormat("datawidth: {0}, ", actionOptions.DataWidth.Value);

				if (actionOptions.Height.HasValue)
					javaScriptBuilder.AppendFormat("height: {0}, ", actionOptions.Height.Value);

				if (actionOptions.DataHeight.HasValue)
					javaScriptBuilder.AppendFormat("dataheight: {0}, ", actionOptions.DataHeight.Value);

				if (actionOptions.Modal.HasValue)
					javaScriptBuilder.AppendFormat("modal: {0}, ", actionOptions.Modal.Value.ToString().ToLower());

				if (actionOptions.Dragable.HasValue)
					javaScriptBuilder.AppendFormat("drag: {0}, ", actionOptions.Dragable.Value.ToString().ToLower());

				if (actionOptions.Resizable.HasValue)
					javaScriptBuilder.AppendFormat("resize: {0}, ", actionOptions.Resizable.Value.ToString().ToLower());

				if (actionOptions.UseJqModal.HasValue)
					javaScriptBuilder.AppendFormat("jqModal: {0}, ", actionOptions.UseJqModal.Value.ToString().ToLower());

				if (actionOptions.CloseOnEscape.HasValue)
					javaScriptBuilder.AppendFormat("closeOnEscape: {0}, ", actionOptions.CloseOnEscape.Value.ToString().ToLower());

				if (actionOptions.Overlay.HasValue)
					javaScriptBuilder.AppendFormat("overlay: {0}, ", actionOptions.Overlay.Value);

				if (actionOptions.IsOnCloseSetted)
					javaScriptBuilder.AppendFormat("onClose: {0}, ", actionOptions.OnClose.ToNullString());

				JqGridNavigatorFormActionOptions formActionOptions = actionOptions as JqGridNavigatorFormActionOptions;
				if (formActionOptions != null)
				{
					AppendNavigatorFormActionOptions(formActionOptions, javaScriptBuilder);

					IJqGridNavigatorPageableFormActionOptions pageableFormActionOptions = formActionOptions as IJqGridNavigatorPageableFormActionOptions;
					if (pageableFormActionOptions != null)
						AppendNavigatorPageableFormActionOptions(pageableFormActionOptions, javaScriptBuilder);

					JqGridNavigatorModifyActionOptions modifyActionOptions = formActionOptions as JqGridNavigatorModifyActionOptions;
					if (modifyActionOptions != null)
					{
						AppendNavigatorModifyActionOptions(modifyActionOptions, javaScriptBuilder);
						JqGridNavigatorEditActionOptions editActionOptions = modifyActionOptions as JqGridNavigatorEditActionOptions;
						if (editActionOptions != null)
							AppendNavigatorEditActionOptions(editActionOptions, javaScriptBuilder);
						else
							AppendNavigatorDeleteActionOptions(modifyActionOptions as JqGridNavigatorDeleteActionOptions, javaScriptBuilder);
					}
					else
						AppendNavigatorViewActionOptions(formActionOptions as JqGridNavigatorViewActionOptions, javaScriptBuilder);
				}
				else
					AppendNavigatorSearchActionOptions(actionOptions as JqGridNavigatorSearchActionOptions, javaScriptBuilder);

			}

			if (javaScriptBuilder[javaScriptBuilder.Length - 2] == ',')
			{
				javaScriptBuilder.Remove(javaScriptBuilder.Length - 2, 2);
				if (!string.IsNullOrEmpty(optionsName))
					javaScriptBuilder.Append(" }, ");
				else
					javaScriptBuilder.Append(" }");
			}
			else if (!string.IsNullOrEmpty(optionsName))
			{
				int optionsNameLength = optionsName.Length + 2;
				javaScriptBuilder.Remove(javaScriptBuilder.Length - optionsNameLength, optionsNameLength);
			}
			else
				javaScriptBuilder.Append("}");
		}

		private static void AppendNavigatorFormActionOptions(JqGridNavigatorFormActionOptions formActionOptions, StringBuilder javaScriptBuilder)
		{
			if (formActionOptions.IsBeforeInitDataSetted)
				javaScriptBuilder.AppendFormat("beforeInitData: {0}, ", formActionOptions.BeforeInitData.ToNullString());

			if (formActionOptions.IsBeforShowFormSetted)
				javaScriptBuilder.AppendFormat("beforeShowForm: {0}, ", formActionOptions.BeforeShowForm.ToNullString());
		}

		private static void AppendNavigatorPageableFormActionOptions(IJqGridNavigatorPageableFormActionOptions pageableFormActionOptions, StringBuilder javaScriptBuilder)
		{
			if (pageableFormActionOptions.NavigationKeys != null && pageableFormActionOptions.NavigationKeys.Enabled.HasValue)
			{
				javaScriptBuilder.AppendFormat("navkeys: [{0}, ", pageableFormActionOptions.NavigationKeys.Enabled.Value.ToString().ToLower());
				if (pageableFormActionOptions.NavigationKeys.Enabled.Value && pageableFormActionOptions.NavigationKeys.RecordUp.HasValue)
				{
					javaScriptBuilder.AppendFormat("{0}, ", (int)pageableFormActionOptions.NavigationKeys.RecordUp.Value);
					if (pageableFormActionOptions.NavigationKeys.RecordDown.HasValue)
						javaScriptBuilder.AppendFormat("{0}, ", (int)pageableFormActionOptions.NavigationKeys.RecordDown.Value);
				}
				javaScriptBuilder.Insert(javaScriptBuilder.Length - 2, "]");
			}

			if (pageableFormActionOptions.ViewPagerButtons.HasValue)
				javaScriptBuilder.AppendFormat("viewPagerButtons: {0}, ", pageableFormActionOptions.ViewPagerButtons.Value.ToString().ToLower());

			if (pageableFormActionOptions.RecreateForm.HasValue)
				javaScriptBuilder.AppendFormat("recreateForm: {0}, ", pageableFormActionOptions.RecreateForm.Value.ToString().ToLower());
		}

		private static void AppendNavigatorModifyActionOptions(JqGridNavigatorModifyActionOptions modifyActionOptions, StringBuilder javaScriptBuilder)
		{
			if (modifyActionOptions.IsUrlSetted)
				javaScriptBuilder.AppendFormat("url: '{0}', ", modifyActionOptions.Url.ToNullString());

			if (modifyActionOptions.MethodType != JqGridMethodTypes.Default)
				javaScriptBuilder.AppendFormat("mtype: '{0}', ", modifyActionOptions.MethodType.ToString().ToUpper());

			if (modifyActionOptions.ReloadAfterSubmit.HasValue)
				javaScriptBuilder.AppendFormat("reloadAfterSubmit: {0}, ", modifyActionOptions.ReloadAfterSubmit.Value.ToString().ToLower());

			if (modifyActionOptions.IsAfterShowFormSetted)
				javaScriptBuilder.AppendFormat("afterShowForm: {0}, ", modifyActionOptions.AfterShowForm.ToNullString());

			if (modifyActionOptions.IsAfterSubmitSetted)
				javaScriptBuilder.AppendFormat("afterSubmit: {0}, ", modifyActionOptions.AfterSubmit.ToNullString());

			if (modifyActionOptions.IsBeforeSubmitSetted)
				javaScriptBuilder.AppendFormat("beforeSubmit: {0}, ", modifyActionOptions.BeforeSubmit.ToNullString());

			if (modifyActionOptions.IsOnClickSubmitSetted)
				javaScriptBuilder.AppendFormat("onclickSubmit: {0}, ", modifyActionOptions.OnClickSubmit.ToNullString());

			if (modifyActionOptions.IsErrorTextFormatSetted)
				javaScriptBuilder.AppendFormat("errorTextFormat: {0}, ", modifyActionOptions.ErrorTextFormat.ToNullString());
		}

		private static void AppendNavigatorEditActionOptions(JqGridNavigatorEditActionOptions editActionOptions, StringBuilder javaScriptBuilder)
		{
			if (editActionOptions.Width.HasValue)
				javaScriptBuilder.AppendFormat("width: {0}, ", editActionOptions.Width.Value);

			JavaScriptSerializer serializer = new JavaScriptSerializer();

			if (editActionOptions.IsExtraDataScriptSetted)
				javaScriptBuilder.AppendFormat("editData: {0},", editActionOptions.ExtraDataScript.ToNullString()).AppendLine();
			else if (editActionOptions.IsExtraDataSetted)
				javaScriptBuilder.AppendFormat("editData: {0}, ", serializer.Serialize(editActionOptions.ExtraData));

			if (editActionOptions.IsAjaxOptionSetted)
				javaScriptBuilder.AppendFormat("ajaxEditOptions: {0}, ", serializer.Serialize(editActionOptions.AjaxOptions));

			if (editActionOptions.IsSerializeDataSetted)
				javaScriptBuilder.AppendFormat("serializeEditData: {0}, ", editActionOptions.SerializeData.ToNullString());

			if (editActionOptions.AddedRowPosition != JqGridNewRowPositions.Default)
				javaScriptBuilder.AppendFormat("addedrow: '{0}', ", editActionOptions.AddedRowPosition.ToString().ToLower());

			if (editActionOptions.ClearAfterAdd.HasValue)
				javaScriptBuilder.AppendFormat("clearAfterAdd: {0}, ", editActionOptions.ClearAfterAdd.Value.ToString().ToLower());

			if (editActionOptions.CloseAfterAdd.HasValue)
				javaScriptBuilder.AppendFormat("closeAfterAdd: {0}, ", editActionOptions.CloseAfterAdd.Value.ToString().ToLower());

			if (editActionOptions.CloseAfterEdit.HasValue)
				javaScriptBuilder.AppendFormat("closeAfterEdit: {0}, ", editActionOptions.CloseAfterEdit.Value.ToString().ToLower());

			if (editActionOptions.CheckOnSubmit.HasValue)
				javaScriptBuilder.AppendFormat("checkOnSubmit: {0}, ", editActionOptions.CheckOnSubmit.Value.ToString().ToLower());

			if (editActionOptions.CheckOnUpdate.HasValue)
				javaScriptBuilder.AppendFormat("checkOnUpdate: {0}, ", editActionOptions.CheckOnUpdate.Value.ToString().ToLower());

			if (editActionOptions.IsTopInfoSetted)
				javaScriptBuilder.AppendFormat("topinfo: '{0}', ", editActionOptions.TopInfo.ToNullString());

			if (editActionOptions.IsBottomInfoSetted)
				javaScriptBuilder.AppendFormat("bottominfo: '{0}', ", editActionOptions.BottomInfo.ToNullString());

			if (editActionOptions.SaveKeyEnabled.HasValue)
			{
				javaScriptBuilder.AppendFormat("savekey: [{0}, ", editActionOptions.SaveKeyEnabled.Value.ToString().ToLower());
				if (editActionOptions.SaveKeyEnabled.Value && editActionOptions.SaveKey.HasValue)
					javaScriptBuilder.AppendFormat("{0}, ", (int)editActionOptions.SaveKey.Value);
				javaScriptBuilder.Insert(javaScriptBuilder.Length - 2, "]");
			}

			if (editActionOptions.SaveButtonIcon != null && editActionOptions.SaveButtonIcon.Enabled.HasValue)
			{
				javaScriptBuilder.AppendFormat("saveicon: [{0}, ", editActionOptions.SaveButtonIcon.Enabled.Value.ToString().ToLower());
				if (editActionOptions.SaveButtonIcon.Position != JqGridFormButtonIconPositions.Default)
				{
					javaScriptBuilder.AppendFormat("'{0}', ", editActionOptions.SaveButtonIcon.Position.ToString().ToLower());
					if (editActionOptions.SaveButtonIcon.IsClassSetted)
						javaScriptBuilder.AppendFormat("'{0}', ", editActionOptions.SaveButtonIcon.Class);
				}
				javaScriptBuilder.Insert(javaScriptBuilder.Length - 2, "]");
			}

			if (editActionOptions.CloseButtonIcon != null && editActionOptions.CloseButtonIcon.Enabled.HasValue)
			{
				javaScriptBuilder.AppendFormat("closeicon: [{0}, ", editActionOptions.CloseButtonIcon.Enabled.Value.ToString().ToLower());
				if (editActionOptions.CloseButtonIcon.Position != JqGridFormButtonIconPositions.Default)
				{
					javaScriptBuilder.AppendFormat("'{0}', ", editActionOptions.CloseButtonIcon.Position.ToString().ToLower());
					if (editActionOptions.CloseButtonIcon.IsClassSetted)
						javaScriptBuilder.AppendFormat("'{0}', ", editActionOptions.CloseButtonIcon.Class);
				}
				javaScriptBuilder.Insert(javaScriptBuilder.Length - 2, "]");
			}

			if (editActionOptions.IsAfterClickPgButtonsSetted)
				javaScriptBuilder.AppendFormat("afterclickPgButtons: {0}, ", editActionOptions.AfterClickPgButtons.ToNullString());

			if (editActionOptions.IsAfterCompleteSetted)
				javaScriptBuilder.AppendFormat("afterComplete: {0}, ", editActionOptions.AfterComplete.ToNullString());

			if (editActionOptions.IsBeforeCheckValuesSetted)
				javaScriptBuilder.AppendFormat("beforeCheckValues: {0}, ", editActionOptions.BeforeCheckValues.ToNullString());

			if (editActionOptions.IsOnClickPgButtonsSetted)
				javaScriptBuilder.AppendFormat("onclickPgButtons: {0}, ", editActionOptions.OnClickPgButtons.ToNullString());

			if (editActionOptions.IsOnInitializeFormSetted)
				javaScriptBuilder.AppendFormat("onInitializeForm: {0}, ", editActionOptions.OnInitializeForm.ToNullString());
		}

		private static void AppendNavigatorDeleteActionOptions(JqGridNavigatorDeleteActionOptions deleteActionOptions, StringBuilder javaScriptBuilder)
		{
			if (deleteActionOptions.Width.HasValue)
				javaScriptBuilder.AppendFormat("width: {0}, ", deleteActionOptions.Width.Value);

			JavaScriptSerializer serializer = new JavaScriptSerializer();

			if (deleteActionOptions.IsExtraDataScriptSetted)
				javaScriptBuilder.AppendFormat("delData: {0},", deleteActionOptions.ExtraDataScript.ToNullString()).AppendLine();
			else if (deleteActionOptions.IsExtraDataSetted)
				javaScriptBuilder.AppendFormat("delData: {0}, ", serializer.Serialize(deleteActionOptions.ExtraData));

			if (deleteActionOptions.IsAjaxOptionSetted)
				javaScriptBuilder.AppendFormat("ajaxDelOptions: {0}, ", serializer.Serialize(deleteActionOptions.AjaxOptions));

			if (deleteActionOptions.IsSerializeDataSetted)
				javaScriptBuilder.AppendFormat("serializeDelData: {0}, ", deleteActionOptions.SerializeData.ToNullString());

			if (deleteActionOptions.DeleteButtonIcon != null && deleteActionOptions.DeleteButtonIcon.Enabled.HasValue)
			{
				javaScriptBuilder.AppendFormat("delicon: [{0}, ", deleteActionOptions.DeleteButtonIcon.Enabled.Value.ToString().ToLower());
				if (deleteActionOptions.DeleteButtonIcon.Position != JqGridFormButtonIconPositions.Default)
				{
					javaScriptBuilder.AppendFormat("'{0}', ", deleteActionOptions.DeleteButtonIcon.Position.ToString().ToLower());
					if (deleteActionOptions.DeleteButtonIcon.IsClassSetted)
						javaScriptBuilder.AppendFormat("'{0}', ", deleteActionOptions.DeleteButtonIcon.Class);
				}
				javaScriptBuilder.Insert(javaScriptBuilder.Length - 2, "]");
			}

			if (deleteActionOptions.CancelButtonIcon != null && deleteActionOptions.CancelButtonIcon.Enabled.HasValue)
			{
				javaScriptBuilder.AppendFormat("cancelicon: [{0}, ", deleteActionOptions.CancelButtonIcon.Enabled.Value.ToString().ToLower());
				if (deleteActionOptions.CancelButtonIcon.Position != JqGridFormButtonIconPositions.Default)
				{
					javaScriptBuilder.AppendFormat("'{0}', ", deleteActionOptions.CancelButtonIcon.Position.ToString().ToLower());
					if (deleteActionOptions.CancelButtonIcon.IsClassSetted)
						javaScriptBuilder.AppendFormat("'{0}', ", deleteActionOptions.CancelButtonIcon.Class);
				}
				javaScriptBuilder.Insert(javaScriptBuilder.Length - 2, "]");
			}
		}

		private static void AppendNavigatorViewActionOptions(JqGridNavigatorViewActionOptions viewActionOptions, StringBuilder javaScriptBuilder)
		{
			if (viewActionOptions.Width.HasValue)
				javaScriptBuilder.AppendFormat("width: {0}, ", viewActionOptions.Width.Value);

			if (viewActionOptions.IsLabelsWidthSetted)
				javaScriptBuilder.AppendFormat("labelswidth: '{0}', ", viewActionOptions.LabelsWidth.ToNullString());

			if (viewActionOptions.CloseButtonIcon != null && viewActionOptions.CloseButtonIcon.Enabled.HasValue)
			{
				javaScriptBuilder.AppendFormat("closeicon: [{0}, ", viewActionOptions.CloseButtonIcon.Enabled.Value.ToString().ToLower());
				if (viewActionOptions.CloseButtonIcon.Position != JqGridFormButtonIconPositions.Default)
				{
					javaScriptBuilder.AppendFormat("'{0}', ", viewActionOptions.CloseButtonIcon.Position.ToString().ToLower());
					if (viewActionOptions.CloseButtonIcon.IsClassSetted)
						javaScriptBuilder.AppendFormat("'{0}', ", viewActionOptions.CloseButtonIcon.Class);
				}
				javaScriptBuilder.Insert(javaScriptBuilder.Length - 2, "]");
			}
		}

		private static void AppendNavigatorSearchActionOptions(JqGridNavigatorSearchActionOptions searchActionOptions, StringBuilder javaScriptBuilder)
		{
			if (searchActionOptions.Width.HasValue)
				javaScriptBuilder.AppendFormat("width: {0}, ", searchActionOptions.Width.Value);

			if (searchActionOptions.IsAfterRedrawSetted)
				javaScriptBuilder.AppendFormat("afterRedraw: {0}, ", searchActionOptions.AfterRedraw.ToNullString());

			if (searchActionOptions.IsAfterShowSearchSetted)
				javaScriptBuilder.AppendFormat("afterShowSearch: {0}, ", searchActionOptions.AfterShowSearch.ToNullString());

			if (searchActionOptions.IsBeforeShowSearchSetted)
				javaScriptBuilder.AppendFormat("beforeShowSearch: {0}, ", searchActionOptions.BeforeShowSearch.ToNullString());

			if (searchActionOptions.IsCaptionSetted)
				javaScriptBuilder.AppendFormat("caption: '{0}', ", searchActionOptions.Caption.ToNullString());

			if (searchActionOptions.CloseAfterSearch.HasValue)
				javaScriptBuilder.AppendFormat("closeAfterSearch: {0}, ", searchActionOptions.CloseAfterSearch.Value.ToString().ToLower());

			if (searchActionOptions.CloseAfterReset.HasValue)
				javaScriptBuilder.AppendFormat("closeAfterReset: {0}, ", searchActionOptions.CloseAfterReset.Value.ToString().ToLower());

			if (searchActionOptions.ErrorCheck.HasValue)
				javaScriptBuilder.AppendFormat("errorcheck: {0}, ", searchActionOptions.ErrorCheck.Value.ToString().ToLower());

			if (searchActionOptions.IsSearchTextSetted)
				javaScriptBuilder.AppendFormat("Find: '{0}', ", searchActionOptions.SearchText.ToNullString());

			if (searchActionOptions.AdvancedSearching.HasValue)
				javaScriptBuilder.AppendFormat("multipleSearch: {0}, ", searchActionOptions.AdvancedSearching.Value.ToString().ToLower());

			if (searchActionOptions.AdvancedSearchingWithGroups.HasValue)
				javaScriptBuilder.AppendFormat("multipleGroup: {0}, ", searchActionOptions.AdvancedSearchingWithGroups.Value.ToString().ToLower());

			if (searchActionOptions.CloneSearchRowOnAdd.HasValue)
				javaScriptBuilder.AppendFormat("cloneSearchRowOnAdd: {0}, ", searchActionOptions.CloneSearchRowOnAdd.Value.ToString().ToLower());

			if (searchActionOptions.IsOnInitiazeSearchSetted)
				javaScriptBuilder.AppendFormat("onInitializeSearch: {0}, ", searchActionOptions.OnInitializeSearch.ToNullString());

			if (searchActionOptions.IsOnResetSetted)
				javaScriptBuilder.AppendFormat("onReset: {0}, ", searchActionOptions.OnReset.ToNullString());

			if (searchActionOptions.IsOnSearchSetted)
				javaScriptBuilder.AppendFormat("onSearch: {0}, ", searchActionOptions.OnSearch.ToNullString());

			if (searchActionOptions.RecreateFilter.HasValue)
				javaScriptBuilder.AppendFormat("recreateFilter: {0}, ", searchActionOptions.RecreateFilter.Value.ToString().ToLower());

			if (searchActionOptions.IsResetTextSetted)
				javaScriptBuilder.AppendFormat("Reset: '{0}', ", searchActionOptions.ResetText.ToNullString());

			AppendSearchOperators(searchActionOptions.SearchOperators, javaScriptBuilder);

			if (searchActionOptions.ShowOnLoad.HasValue)
				javaScriptBuilder.AppendFormat("showOnLoad: {0}, ", searchActionOptions.ShowOnLoad.Value.ToString().ToLower());

			if (searchActionOptions.ShowQuery.HasValue)
				javaScriptBuilder.AppendFormat("showQuery: {0}, ", searchActionOptions.ShowQuery.Value.ToString().ToLower());

			if (searchActionOptions.IsLayerSetted)
				javaScriptBuilder.AppendFormat("layer: '{0}', ", searchActionOptions.Layer.ToNullString());

			if (searchActionOptions.Templates != null && searchActionOptions.Templates.Count > 0)
			{
				JavaScriptSerializer serializer = new JavaScriptSerializer();
				serializer.RegisterConverters(new JavaScriptConverter[] { new JqGridScriptConverter() });

				javaScriptBuilder.Append("tmplNames: [],");
				int templateNameIndex = javaScriptBuilder.Length - 2;
				javaScriptBuilder.Append("tmplFilters: [");
				foreach (KeyValuePair<string, JqGridRequestSearchingFilters> template in searchActionOptions.Templates)
				{
					javaScriptBuilder.Insert(templateNameIndex, "'" + template.Key + "', ");
					templateNameIndex += template.Key.Length + 4;
					javaScriptBuilder.Append(serializer.Serialize(template.Value) + ", ");
				}
				javaScriptBuilder.Remove(templateNameIndex - 2, 2);
				javaScriptBuilder.Remove(javaScriptBuilder.Length - 2, 2);
				javaScriptBuilder.Append("],");
			}
		}

		private void AppendNavigatorButton(string navigatorPagerSelector, JqGridNavigatorButtonOptions buttonOptions, StringBuilder javaScriptBuilder)
		{
			javaScriptBuilder.AppendFormat(".jqGrid('navButtonAdd', {0}, {{ ", navigatorPagerSelector);
			if (buttonOptions.Caption != JqGridNavigatorDefaults.ButtonCaption)
				javaScriptBuilder.AppendFormat("caption: '{0}', ", buttonOptions.Caption);

			if (buttonOptions.Icon != JqGridNavigatorDefaults.ButtonIcon)
				javaScriptBuilder.AppendFormat("buttonicon: '{0}', ", buttonOptions.Icon);

			if (!string.IsNullOrWhiteSpace(buttonOptions.OnClick))
				javaScriptBuilder.AppendFormat("onClickButton: {0}, ", buttonOptions.OnClick);

			if (buttonOptions.Position != JqGridNavigatorButtonPositions.Last)
				javaScriptBuilder.Append("position: 'first', ");

			if (!string.IsNullOrEmpty(buttonOptions.ToolTip))
				javaScriptBuilder.AppendFormat("title: '{0}', ", buttonOptions.ToolTip);

			if (buttonOptions.Cursor != JqGridNavigatorDefaults.ButtonCursor)
				javaScriptBuilder.AppendFormat("cursor: '{0}', ", buttonOptions.Cursor);

			if (!string.IsNullOrWhiteSpace(buttonOptions.Id))
				javaScriptBuilder.AppendFormat("id: '{0}', ", buttonOptions.Id);

			if (javaScriptBuilder[javaScriptBuilder.Length - 2] == ',')
			{
				javaScriptBuilder.Remove(javaScriptBuilder.Length - 2, 2);
				javaScriptBuilder.Append(" })");
			}
			else
			{
				javaScriptBuilder.Remove(javaScriptBuilder.Length - 4, 4);
				javaScriptBuilder.Append(")");
			}

			buttonOptions.ProcessedElement = true;
		}

		private void AppendNavigatorSeparator(string navigatorPagerSelector, JqGridNavigatorSeparatorOptions separatorOptions, StringBuilder javaScriptBuilder)
		{
			javaScriptBuilder.AppendFormat(".jqGrid('navSeparatorAdd', {0}, {{ ", navigatorPagerSelector);

			if (separatorOptions.Class != JqGridNavigatorDefaults.SeparatorClass)
				javaScriptBuilder.AppendFormat("sepclass: '{0}', ", separatorOptions.Class);

			if (!string.IsNullOrEmpty(separatorOptions.Content))
				javaScriptBuilder.AppendFormat("sepcontent: '{0}', ", separatorOptions.Content);

			if (javaScriptBuilder[javaScriptBuilder.Length - 2] == ',')
			{
				javaScriptBuilder.Remove(javaScriptBuilder.Length - 2, 2);
				javaScriptBuilder.Append(" })");
			}
			else
			{
				javaScriptBuilder.Remove(javaScriptBuilder.Length - 4, 4);
				javaScriptBuilder.Append(")");
			}

			separatorOptions.ProcessedElement = true;
		}

		private void AppendNavigatorElements(StringBuilder javaScriptBuilder)
		{
			string navigatorPagerSelector = NavigatorOptions.Pager == JqGridNavigatorPagers.Top ? TopPagerSelector : PagerSelector;
			foreach (JqGridNavigatorControlOptions controlOptions in _navigatorControlsOptions.Where(o => !o.ProcessedElement))
			{
				if (controlOptions is JqGridNavigatorButtonOptions buttonOptions)
				{
					AppendNavigatorButton(navigatorPagerSelector, buttonOptions, javaScriptBuilder);
					if (NavigatorOptions.Pager == JqGridNavigatorPagers.Standard && controlOptions.CloneToTop)
						AppendNavigatorButton(TopPagerSelector, buttonOptions, javaScriptBuilder);
				}
				else if (controlOptions is JqGridNavigatorSeparatorOptions separatorOptions)
				{
					AppendNavigatorSeparator(navigatorPagerSelector, separatorOptions, javaScriptBuilder);
					if (NavigatorOptions.Pager == JqGridNavigatorPagers.Standard && controlOptions.CloneToTop)
						AppendNavigatorSeparator(TopPagerSelector, separatorOptions, javaScriptBuilder);
				}
			}
		}

		private void AppendInlineNavigator(StringBuilder javaScriptBuilder)
		{
			string navigatorPagerSelector = NavigatorOptions.Pager == JqGridNavigatorPagers.Top ? TopPagerSelector : PagerSelector;

			javaScriptBuilder.AppendFormat(".jqGrid('inlineNav', {0}, ", navigatorPagerSelector).AppendLine();

			AppendInlineNavigatorOptions(javaScriptBuilder);

			javaScriptBuilder.Append(")");
		}

		private void AppendInlineNavigatorOptions(StringBuilder javaScriptBuilder)
		{
			javaScriptBuilder.Append("{ ");

			AppendBaseNavigatorOptions(InlineNavigatorOptions, javaScriptBuilder);

			if (InlineNavigatorOptions.Save.HasValue)
				javaScriptBuilder.AppendFormat("save: {0}, ", InlineNavigatorOptions.Save.Value.ToString().ToLower());

			if (InlineNavigatorOptions.IsSaveIconSetted)
				javaScriptBuilder.AppendFormat("saveicon: '{0}', ", InlineNavigatorOptions.SaveIcon.ToNullString());

			if (InlineNavigatorOptions.IsSaveTextSetted)
				javaScriptBuilder.AppendFormat("savetext: '{0}', ", InlineNavigatorOptions.SaveText.ToNullString());

			if (InlineNavigatorOptions.IsSaveToolTipSetted)
				javaScriptBuilder.AppendFormat("savetitle: '{0}', ", InlineNavigatorOptions.SaveToolTip.ToNullString());

			if (InlineNavigatorOptions.Cancel.HasValue)
				javaScriptBuilder.AppendFormat("cancel: {0}, ", InlineNavigatorOptions.Cancel.Value.ToString().ToLower());

			if (InlineNavigatorOptions.IsCancelIconSetted)
				javaScriptBuilder.AppendFormat("cancelicon: '{0}', ", InlineNavigatorOptions.CancelIcon.ToNullString());

			if (InlineNavigatorOptions.IsCancelTextSetted)
				javaScriptBuilder.AppendFormat("canceltext: '{0}', ", InlineNavigatorOptions.CancelText.ToNullString());

			if (InlineNavigatorOptions.IsCancelToolTipSetted)
				javaScriptBuilder.AppendFormat("canceltitle: '{0}', ", InlineNavigatorOptions.CancelToolTip.ToNullString());

			AppendInlineNavigatorAddActionOptions(InlineNavigatorOptions.AddActionOptions, javaScriptBuilder);
			AppendInlineNavigatorActionOptions("editParams", InlineNavigatorOptions.ActionOptions, javaScriptBuilder);

			if (javaScriptBuilder[javaScriptBuilder.Length - 2] == ',')
			{
				javaScriptBuilder.Remove(javaScriptBuilder.Length - 2, 2);
				javaScriptBuilder.Append(" }");
			}
			else
				javaScriptBuilder.Append("}");
		}

		private static void AppendInlineNavigatorActionOptions(string actionName, JqGridInlineNavigatorActionOptions actionOptions, StringBuilder javaScriptBuilder)
		{
			if (actionOptions != null)
			{
				javaScriptBuilder.AppendFormat("{0}: {{ ", actionName);

				if (actionOptions.Keys.HasValue)
					javaScriptBuilder.AppendFormat("keys: {0}, ", actionOptions.Keys.Value.ToString().ToLower());

				if (actionOptions.IsOnEditFunctionSetted)
					javaScriptBuilder.AppendFormat("oneditfunc: {0}, ", actionOptions.OnEditFunction.ToNullString());

				if (actionOptions.IsSuccessFunctionSetted)
					javaScriptBuilder.AppendFormat("successfunc: {0}, ", actionOptions.SuccessFunction.ToNullString());

				if (actionOptions.IsUrlSetted)
					javaScriptBuilder.AppendFormat("url: '{0}', ", actionOptions.Url.ToNullString());

				if (actionOptions.IsExtraParamScriptSetted)
					javaScriptBuilder.AppendFormat("extraparam: {0}, ", actionOptions.ExtraParamScript.ToNullString());
				else if (actionOptions.IsExtraParamSetted)
				{
					JavaScriptSerializer serializer = new JavaScriptSerializer();
					javaScriptBuilder.AppendFormat("extraparam: {0}, ", serializer.Serialize(actionOptions.ExtraParam));
				}

				if (actionOptions.IsAfterSaveFunctionSetted)
					javaScriptBuilder.AppendFormat("aftersavefunc: {0}, ", actionOptions.AfterSaveFunction.ToNullString());

				if (actionOptions.IsErrorFunctionSetted)
					javaScriptBuilder.AppendFormat("errorfunc: {0}, ", actionOptions.ErrorFunction.ToNullString());

				if (actionOptions.IsAfterRestoreFunctionSetted)
					javaScriptBuilder.AppendFormat("afterrestorefunc: {0}, ", actionOptions.AfterRestoreFunction.ToNullString());

				if (actionOptions.RestoreAfterError.HasValue)
					javaScriptBuilder.AppendFormat("restoreAfterError: {0}, ", actionOptions.RestoreAfterError.Value.ToString().ToLower());

				if (actionOptions.MethodType != JqGridMethodTypes.Default)
					javaScriptBuilder.AppendFormat("mtype: '{0}', ", actionOptions.MethodType.ToString().ToUpper());

				if (javaScriptBuilder[javaScriptBuilder.Length - 2] == ',')
				{
					javaScriptBuilder.Remove(javaScriptBuilder.Length - 2, 2);
					javaScriptBuilder.Append(" }, ");
				}
				else
				{
					int actionNameLength = actionName.Length + 4;
					javaScriptBuilder.Remove(javaScriptBuilder.Length - actionNameLength, actionNameLength);
				}
			}
		}

		private static void AppendInlineNavigatorAddActionOptions(JqGridInlineNavigatorAddActionOptions actionOptions, StringBuilder javaScriptBuilder)
		{
			if (actionOptions != null)
			{
				javaScriptBuilder.Append("addParams: { ");

				if (actionOptions.IsRowIdSetted)
					javaScriptBuilder.AppendFormat("rowID: '{0}', ", actionOptions.RowId.ToNullString());

				if (actionOptions.IsInitDataSetted)
				{
					JavaScriptSerializer serializer = new JavaScriptSerializer();
					javaScriptBuilder.AppendFormat("initdata: {0}, ", serializer.Serialize(actionOptions.InitData));
				}

				if (actionOptions.Position != JqGridNewRowPositions.Default)
					javaScriptBuilder.AppendFormat("position: '{0}', ", actionOptions.Position.ToString().ToLower());

				if (actionOptions.UseDefaultValues.HasValue)
					javaScriptBuilder.AppendFormat("useDefValues: {0}, ", actionOptions.UseDefaultValues.Value.ToString().ToLower());

				if (actionOptions.UseFormatter.HasValue)
					javaScriptBuilder.AppendFormat("useFormatter: {0}, ", actionOptions.UseFormatter.Value.ToString().ToLower());

				AppendInlineNavigatorActionOptions("addRowParams", actionOptions.Options, javaScriptBuilder);

				if (javaScriptBuilder[javaScriptBuilder.Length - 2] == ',')
				{
					javaScriptBuilder.Remove(javaScriptBuilder.Length - 2, 2);
					javaScriptBuilder.Append(" }, ");
				}
				else
				{
					javaScriptBuilder.Remove(javaScriptBuilder.Length - 13, 13);
				}
			}
		}

		private void AppendFilterToolbar(StringBuilder javaScriptBuilder)
		{
			javaScriptBuilder.Append(".jqGrid('filterToolbar'");

			if (_filterToolbarOptions != null)
			{
				javaScriptBuilder.Append(", { ");

				AppendFilterOptions(_filterToolbarOptions, javaScriptBuilder);

				if (_filterToolbarOptions.DefaultSearchOperator != JqGridSearchOperators.Bw)
					javaScriptBuilder.AppendFormat("defaultSearch: '{0}',", _filterToolbarOptions.DefaultSearchOperator.ToString().ToLower());

				if (_filterToolbarOptions.GroupingOperator != JqGridSearchGroupingOperators.And)
					javaScriptBuilder.Append("groupOp: 'OR',");

				if (!_filterToolbarOptions.SearchOnEnter)
					javaScriptBuilder.Append("searchOnEnter: false,");

				if (_filterToolbarOptions.SearchOperators)
					javaScriptBuilder.Append("searchOperators: true,");

				if (_filterToolbarOptions.OperandToolTip != JqGridFilterToolbarDefaults.OperandToolTip)
					javaScriptBuilder.AppendFormat("operandTitle: '{0}',", _filterToolbarOptions.OperandToolTip);

				if (_filterToolbarOptions.Operands != null && _filterToolbarOptions.Operands.Count > 0 && _filterToolbarOptions.Operands != JqGridFilterToolbarDefaults.Operands)
				{
					int javaScriptBuilderPosition = javaScriptBuilder.Length;

					foreach (KeyValuePair<JqGridSearchOperators, string> operand in _filterToolbarOptions.Operands)
					{
						string defaultShortText;
						if (JqGridFilterToolbarDefaults.Operands.TryGetValue(operand.Key, out defaultShortText) && operand.Value != defaultShortText)
							javaScriptBuilder.AppendFormat("'{0}':'{1}',", operand.Key.ToString().ToLower(), operand.Value);
					}

					if (javaScriptBuilderPosition != javaScriptBuilder.Length)
					{
						javaScriptBuilder.Insert(javaScriptBuilderPosition, "operands: {");
						javaScriptBuilder.Remove(javaScriptBuilder.Length - 1, 1);
						javaScriptBuilder.Append("},");
					}
				}

				if (_filterToolbarOptions.StringResult)
					javaScriptBuilder.Append("stringResult: true,");

				javaScriptBuilder.Remove(javaScriptBuilder.Length - 1, 1);
				javaScriptBuilder.Append(" }");
			}

			javaScriptBuilder.Append(")");
		}

		private void AppendFilterGrid(StringBuilder javaScriptBuilder)
		{
			javaScriptBuilder.AppendFormat("$({0}).jqGrid('filterGrid', {1}, {{", FilterGridSelector, GridSelector).AppendLine();
			javaScriptBuilder.AppendLine("gridModel: false, gridNames: false,");

			javaScriptBuilder.AppendLine("filterModel: [");

			int lastGridModelIndex = _filterGridModel.Count - 1;
			for (int gridModelIndex = 0; gridModelIndex < _filterGridModel.Count; gridModelIndex++)
			{
				JqGridFilterGridRowModel gridRowModel = _filterGridModel[gridModelIndex];
				javaScriptBuilder.AppendFormat("{{ label: '{0}', name: '{1}', ", gridRowModel.Label, gridRowModel.ColumnName);

				if (!string.IsNullOrWhiteSpace(gridRowModel.DefaultValue))
					javaScriptBuilder.AppendFormat("defval: '{0}', ", gridRowModel.DefaultValue);

				if (!string.IsNullOrWhiteSpace(gridRowModel.SelectUrl))
					javaScriptBuilder.AppendFormat("surl: '{0}', ", gridRowModel.SelectUrl);

				javaScriptBuilder.AppendFormat("stype: '{0}' }}", gridRowModel.SearchType.ToString().ToLower());

				if (lastGridModelIndex == gridModelIndex)
					javaScriptBuilder.AppendLine();
				else
					javaScriptBuilder.AppendLine(",");
			}

			javaScriptBuilder.Append("]");

			if (_filterGridOptions != null)
			{
				javaScriptBuilder.AppendLine(",");
				AppendFilterOptions(_filterGridOptions, javaScriptBuilder);

				if (!string.IsNullOrWhiteSpace(_filterGridOptions.ButtonsClass))
					javaScriptBuilder.AppendFormat("buttonclass: '{0}',", _filterGridOptions.ButtonsClass);

				if (_filterGridOptions.ClearEnabled.HasValue)
					javaScriptBuilder.AppendFormat("enableClear: {0},", _filterGridOptions.ClearEnabled.Value.ToString().ToLower());

				if (!string.IsNullOrWhiteSpace(_filterGridOptions.ClearText))
					javaScriptBuilder.AppendFormat("clearButton: '{0}',", _filterGridOptions.ClearText);

				if (!string.IsNullOrWhiteSpace(_filterGridOptions.FormClass))
					javaScriptBuilder.AppendFormat("formclass: '{0}',", _filterGridOptions.FormClass);

				if (_filterGridOptions.MarkSearched.HasValue)
					javaScriptBuilder.AppendFormat("marksearched: {0},", _filterGridOptions.MarkSearched.Value.ToString().ToLower());

				if (_filterGridOptions.SearchEnabled.HasValue)
					javaScriptBuilder.AppendFormat("enableSearch: {0},", _filterGridOptions.SearchEnabled.Value.ToString().ToLower());

				if (!string.IsNullOrWhiteSpace(_filterGridOptions.SearchText))
					javaScriptBuilder.AppendFormat("searchButton: '{0}',", _filterGridOptions.SearchText);

				if (!string.IsNullOrWhiteSpace(_filterGridOptions.TableClass))
					javaScriptBuilder.AppendFormat("tableclass: '{0}',", _filterGridOptions.TableClass);

				if (_filterGridOptions.Type.HasValue)
					javaScriptBuilder.AppendFormat("formtype: '{0}',", _filterGridOptions.Type.Value.ToString().ToLower());

				if (!string.IsNullOrWhiteSpace(_filterGridOptions.Url))
					javaScriptBuilder.AppendFormat("url: '{0}',", _filterGridOptions.Url);

				javaScriptBuilder.Remove(javaScriptBuilder.Length - 1, 1);
			}

			javaScriptBuilder.AppendLine("});");
		}

		private static void AppendFilterOptions(JqGridFilterOptions options, StringBuilder javaScriptBuilder)
		{
			if (options != null)
			{
				if (!string.IsNullOrWhiteSpace(options.AfterClear))
					javaScriptBuilder.AppendFormat("afterClear: {0},", options.AfterClear);

				if (!string.IsNullOrWhiteSpace(options.AfterSearch))
					javaScriptBuilder.AppendFormat("afterSearch: {0},", options.AfterSearch);

				if (options.AutoSearch.HasValue)
					javaScriptBuilder.AppendFormat("autosearch: {0},", options.AutoSearch.Value.ToString().ToLower());

				if (!string.IsNullOrWhiteSpace(options.BeforeClear))
					javaScriptBuilder.AppendFormat("beforeClear: {0},", options.BeforeClear);

				if (!string.IsNullOrWhiteSpace(options.BeforeSearch))
					javaScriptBuilder.AppendFormat("beforeSearch: {0},", options.BeforeSearch);
			}
		}

		private void AppendFooterData(StringBuilder javaScriptBuilder)
		{
			javaScriptBuilder.Append(".jqGrid('footerData', 'set', { ");

			foreach (KeyValuePair<string, object> footerValue in _footerData)
				javaScriptBuilder.AppendFormat(" {0}: '{1}', ", footerValue.Key, Convert.ToString(footerValue.Value));

			javaScriptBuilder.Remove(javaScriptBuilder.Length - 2, 2);
			javaScriptBuilder.AppendFormat(" }}, {0})", _footerDataUseFormatters.ToString().ToLower());
		}

		private void AppendGroupHeaders(StringBuilder javaScriptBuilder)
		{
			javaScriptBuilder.Append(".jqGrid('setGroupHeaders', { ");

			if (_groupHeadersUseColSpanStyle)
				javaScriptBuilder.Append("useColSpanStyle: true, ");

			javaScriptBuilder.Append("groupHeaders: [ ");
			foreach (JqGridGroupHeader groupHeader in _groupHeaders)
				javaScriptBuilder.AppendFormat("{{ startColumnName: '{0}', numberOfColumns: {1}, titleText: '{2}' }}, ", groupHeader.StartColumn, groupHeader.NumberOfColumns, groupHeader.Title);
			javaScriptBuilder.Remove(javaScriptBuilder.Length - 2, 2).Append(" ]");

			javaScriptBuilder.Append("})");
		}

		private static void AppendColumnLabelOptions(string columnName, JqGridColumnLabelOptions options, StringBuilder javaScriptBuilder)
		{
			if (options == null) return;
			JavaScriptSerializer serializer = new JavaScriptSerializer();
			javaScriptBuilder.AppendFormat(".jqGrid('setLabel', '{0}', ", columnName);
			javaScriptBuilder.AppendFormat("'{0}', ", string.IsNullOrWhiteSpace(options.Label) ? string.Empty : options.Label);

			if (string.IsNullOrWhiteSpace(options.Class))
			{
				if (options.CssStyles != null)
					javaScriptBuilder.AppendFormat("{0}, ", serializer.Serialize(options.CssStyles));
				else if (options.HtmlAttributes != null)
					javaScriptBuilder.Append("null, ");
			}
			else
				javaScriptBuilder.AppendFormat("'{0}', ", options.Class);

			if (options.HtmlAttributes != null)
				javaScriptBuilder.AppendFormat("{0}, ", serializer.Serialize(options.HtmlAttributes));

			javaScriptBuilder.Remove(javaScriptBuilder.Length - 2, 2);
			javaScriptBuilder.Append(" )");
		}

		private static IDictionary<string, object> AnonymousObjectToDictionary(object anonymousObject)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();

			if (anonymousObject != null)
			{
				foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(anonymousObject))
					dictionary.Add(property.Name, property.GetValue(anonymousObject));
			}

			return dictionary;
		}
		#endregion

		#region Fluent API
		/// <summary>
		/// Adds column with actions predefined formatter.
		/// </summary>
		/// <param name="name">Name for the column.</param>
		/// <param name="position">Position of the column.</param>
		/// <param name="width">Width of the column.</param>
		/// <param name="editButton">Value indicating if edit button is enabled.</param>
		/// <param name="deleteButton">Value indicating if delete button is enabled.</param>
		/// <param name="useFormEditing">Value indicating if form editing should be used instead of inline editing.</param>
		/// <param name="inlineEditingOptions">Options for inline editing (RestoreAfterError and MethodType options are ignored in this context).</param>
		/// <param name="formEditingOptions">Options for form editing.</param>
		/// <param name="deleteOptions">Options for deleting.</param>
		/// <returns>JqGridHelper instance.</returns>
		/// <remarks>It is adviced to set JqGridResponse.JsonReader.RepeatItems to false if you want to use this method, otherwise jqGrid will not skip this column while mapping data.</remarks>
		public JqGridHelper<TModel> AddActionsColumn(string name, int position = 0, int width = 60, bool editButton = true, bool deleteButton = true, bool useFormEditing = false, JqGridInlineNavigatorActionOptions inlineEditingOptions = null, JqGridNavigatorEditActionOptions formEditingOptions = null, JqGridNavigatorDeleteActionOptions deleteOptions = null)
		{
			JqGridColumnModel actionsColumnModel = new JqGridColumnModel(name);
			actionsColumnModel.Width = width;
			actionsColumnModel.Resizable = false;
			actionsColumnModel.Searchable = false;
			actionsColumnModel.Sortable = false;
			actionsColumnModel.Viewable = false;
			actionsColumnModel.Formatter = new SettedString(true, JqGridColumnPredefinedFormatters.Actions);
			actionsColumnModel.FormatterOptions = new JqGridColumnFormatterOptions()
			{
				EditButton = editButton,
				DeleteButton = deleteButton,
				UseFormEditing = useFormEditing,
				InlineEditingOptions = inlineEditingOptions,
				FormEditingOptions = formEditingOptions,
				DeleteOptions = deleteOptions
			};

			if (position <= 0)
			{
				Options.ColumnsNames.Insert(0, string.Empty);
				Options.ColumnsModels.Insert(0, actionsColumnModel);
			}
			else if (position >= Options.ColumnsModels.Count)
			{
				Options.ColumnsNames.Add(string.Empty);
				Options.ColumnsModels.Add(actionsColumnModel);
			}
			else
			{
				Options.ColumnsNames.Insert(position, string.Empty);
				Options.ColumnsModels.Insert(position, actionsColumnModel);
			}

			return this;
		}

		/// <summary>
		/// Enables Navigator for this JqGridHelper instance.
		/// </summary>
		/// <param name="options">The options for the Navigator.</param>
		/// <param name="editActionOptions">The options for the edit action.</param>
		/// <param name="addActionOptions">The options for the add action.</param>
		/// <param name="deleteActionOptions">The options for the delete action.</param>
		/// <param name="searchActionOptions">The options for the search action.</param>
		/// <param name="viewActionOptions">The options for the view action.</param>
		/// <returns>JqGridHelper instance with enabled Navigator.</returns>
		/// <exception cref="System.ArgumentNullException">The <paramref name="options"/> parameter is null.</exception>
		public JqGridHelper<TModel> Navigator(JqGridNavigatorOptions options, JqGridNavigatorEditActionOptions editActionOptions = null, JqGridNavigatorEditActionOptions addActionOptions = null, JqGridNavigatorDeleteActionOptions deleteActionOptions = null, JqGridNavigatorSearchActionOptions searchActionOptions = null, JqGridNavigatorViewActionOptions viewActionOptions = null)
		{
			NavigatorOptions = options ?? throw new ArgumentNullException(nameof(options));
			_navigatorEditActionOptions = editActionOptions;
			_navigatorAddActionOptions = addActionOptions;
			_navigatorDeleteActionOptions = deleteActionOptions;
			_navigatorSearchActionOptions = searchActionOptions;
			_navigatorViewActionOptions = viewActionOptions;

			return this;
		}

		/// <summary>
		/// Enables Inline Navigator for this JqGridHelper instance.
		/// </summary>
		/// <param name="options">The options for the Inline Navigator.</param>
		/// <returns>JqGridHelper instance with enabled Inline Navigator.</returns>
		/// <exception cref="System.ArgumentNullException">The <paramref name="options"/> parameter is null.</exception>
		/// <exception cref="System.InvalidOperationException">Navigator method has not been called.</exception>
		public JqGridHelper<TModel> InlineNavigator(JqGridInlineNavigatorOptions options)
		{
			if (NavigatorOptions == null)
				throw new InvalidOperationException("In order to call InlineNavigator method you must call Navigator method first.");
			InlineNavigatorOptions = options ?? throw new ArgumentNullException(nameof(options));

			return this;
		}

		/// <summary>
		/// Adds button to this JqGridHelper instance Navigator.
		/// </summary>
		/// <param name="caption">The caption for the button.</param>
		/// <param name="icon">The icon (form UI theme images) for the button. If this is set to "none" only text will appear.</param>
		/// <param name="onClick">The function for button click action.</param>
		/// <param name="position">The position where the button will be added.</param>
		/// <param name="toolTip">The tooltip for the button.</param>
		/// <param name="cursor">The value which determines the cursor when user mouseover the button.</param>
		/// <param name="cloneToTop">The value which defines if the button added to the bottom pager should be coppied to the top pager.</param>
		/// <param name="addAfterInlineNavigator">The value which defines if the button added after Inline Navigator.</param>
		/// <returns>JqGridHelper instance.</returns>
		/// <exception cref="System.InvalidOperationException">Navigator method has not been called.</exception>
		public JqGridHelper<TModel> AddNavigatorButton(string caption = JqGridNavigatorDefaults.ButtonCaption, string icon = JqGridNavigatorDefaults.ButtonIcon, string onClick = null, JqGridNavigatorButtonPositions position = JqGridNavigatorButtonPositions.Last, string toolTip = "", string cursor = JqGridNavigatorDefaults.ButtonCursor, bool cloneToTop = false, bool addAfterInlineNavigator = false)
		{
			if (NavigatorOptions == null)
				throw new InvalidOperationException("In order to call AddNavigatorButton method you must call Navigator method first.");

			_navigatorControlsOptions.Add(new JqGridNavigatorButtonOptions()
			{
				CloneToTop = cloneToTop,
				Caption = caption,
				Icon = icon,
				OnClick = onClick,
				Position = position,
				ToolTip = toolTip,
				Cursor = cursor,
				AddAfterInlineNavigator = addAfterInlineNavigator
			});

			return this;
		}

		/// <summary>
		/// Adds button to this JqGridHelper instance Navigator.
		/// </summary>
		/// <param name="options">The options for the button.</param>
		/// <returns>JqGridHelper instance.</returns>
		/// <exception cref="System.ArgumentNullException">The <paramref name="options"/> parameter is null.</exception>
		/// <exception cref="System.InvalidOperationException">Navigator method has not been called.</exception>
		public JqGridHelper<TModel> AddNavigatorButton(JqGridNavigatorButtonOptions options)
		{
			if (options == null)
				throw new ArgumentNullException(nameof(options));

			if (NavigatorOptions == null)
				throw new InvalidOperationException("In order to call AddNavigatorButton method you must call Navigator method first.");

			_navigatorControlsOptions.Add(options);

			return this;
		}

		/// <summary>
		/// Adds separator to this JqGridHelper instance Navigator.
		/// </summary>
		/// <param name="class">The class for the separator.</param>
		/// <param name="content">The content for the separator.</param>
		/// <param name="cloneToTop">The value which defines if the separator added to the bottom pager should be coppied to the top pager.</param>
		/// <param name="addAfterInlineNavigator">The value which defines if the separator added after InlineNavigator.</param>
		/// <returns>JqGridHelper instance.</returns>
		/// <exception cref="System.InvalidOperationException">Navigator method has not been called.</exception>
		public JqGridHelper<TModel> AddNavigatorSeparator(string @class = JqGridNavigatorDefaults.SeparatorClass, string content = "", bool cloneToTop = false, bool addAfterInlineNavigator = false)
		{
			if (NavigatorOptions == null)
				throw new InvalidOperationException("In order to call AddNavigatorSeparator method you must call Navigator method first.");

			_navigatorControlsOptions.Add(new JqGridNavigatorSeparatorOptions()
			{
				CloneToTop = cloneToTop,
				Class = @class,
				Content = content,
				AddAfterInlineNavigator = addAfterInlineNavigator
			});

			return this;
		}

		/// <summary>
		/// Adds separator to this JqGridHelper instance Navigator.
		/// </summary>
		/// <param name="options">The options for the separator.</param>
		/// <returns>JqGridHelper instance.</returns>
		/// <exception cref="System.InvalidOperationException">Navigator method has not been called.</exception>
		public JqGridHelper<TModel> AddNavigatorSeparator(JqGridNavigatorSeparatorOptions options)
		{
			if (options == null)
				throw new ArgumentNullException(nameof(options));

			if (NavigatorOptions == null)
				throw new InvalidOperationException("In order to call AddNavigatorSeparator method you must call Navigator method first.");

			_navigatorControlsOptions.Add(options);

			return this;
		}

		/// <summary>
		/// Enables filter toolbar for this JqGridHelper instance.
		/// </summary>
		/// <param name="options">The options for the filter toolbar.</param>
		/// <returns>JqGridHelper instance with enabled filter toolbar.</returns>
		public JqGridHelper<TModel> FilterToolbar(JqGridFilterToolbarOptions options = null)
		{
			_filterToolbar = true;
			_filterToolbarOptions = options;

			return this;
		}

		/// <summary>
		/// Enables filter grid for this JqGridHelper instance.
		/// </summary>
		/// <param name="model">The model for the filter grid (if null, the model will be build based on ColumnsModels and ColumnsNames).</param>
		/// <param name="options">The options for the filter grid.</param>
		/// <returns>JqGridHelper instance with enabled filter grid.</returns>
		/// <exception cref="System.InvalidOperationException">The filter grid model has not been provided and the automatic generation is not possible because the count of items in ColumnsModel  is different from ColumnsNames.</exception>
		public JqGridHelper<TModel> FilterGrid(List<JqGridFilterGridRowModel> model = null, JqGridFilterGridOptions options = null)
		{
			if (model == null)
			{
				if (Options.ColumnsModels.Count != Options.ColumnsNames.Count)
					throw new InvalidOperationException("Can't build filter grid model because ColumnsModels.Count is not equal to ColumnsNames.Count");

				_filterGridModel = new List<JqGridFilterGridRowModel>();
				for (int i = 0; i < Options.ColumnsModels.Count; i++)
				{
					if (Options.ColumnsModels[i].Searchable.GetValueOrDefault())
					{
						_filterGridModel.Add(new JqGridFilterGridRowModel(Options.ColumnsModels[i].Name, Options.ColumnsNames[i])
						{
							DefaultValue = Options.ColumnsModels[i].SearchOptions != null ? Options.ColumnsModels[i].SearchOptions.DefaultValue : string.Empty,
							SearchType = Options.ColumnsModels[i].SearchType,
							SelectUrl = Options.ColumnsModels[i].SearchOptions != null ? Options.ColumnsModels[i].SearchOptions.DataUrl : string.Empty
						});
					}
				}
			}
			else
				_filterGridModel = model;
			_filterGridOptions = options;

			return this;
		}

		/// <summary>
		/// Sets data on footer of this JqGridHelper instance (requires footerEnabled = true).
		/// </summary>
		/// <param name="data">The object with values for the footer. Should have names which are the same as names from ColumnsModels.</param>
		/// <param name="useFormatters">The value indicating if the formatters from columns should be used for footer.</param>
		/// <returns>JqGridHelper instance with footer data.</returns>
		public JqGridHelper<TModel> SetFooterData(object data, bool useFormatters = true)
		{
			return SetFooterData(AnonymousObjectToDictionary(data), useFormatters);
		}

		/// <summary>
		/// Sets data on footer of this JqGridHelper instance (requires footerEnabled = true).
		/// </summary>
		/// <param name="data">The dictionary with values for the footer. Should have keys which are the same as names from ColumnsModels.</param>
		/// <param name="useFormatters">The value indicating if the formatters from columns should be used for footer.</param>
		/// <returns>JqGridHelper instance with footer data.</returns>
		public JqGridHelper<TModel> SetFooterData(IDictionary<string, object> data, bool useFormatters = true)
		{
			_footerData = data;
			_footerDataUseFormatters = useFormatters;
			return this;
		}

		/// <summary>
		/// Sets grouping headers for this JqGridHelper instance.
		/// </summary>
		/// <param name="groupHeaders">Settings for grouping headers.</param>
		/// <param name="useColSpanStyle">The value which determines if the non grouping header cell should have cell above it (false), or the column should be treated as one combining boot (true).</param>
		/// <returns>JqGridHelper instance with grouping header.</returns>
		/// <exception cref="System.InvalidOperationException">Columns sorting (reordering) is enabled. </exception>
		public JqGridHelper<TModel> SetGroupHeaders(IEnumerable<JqGridGroupHeader> groupHeaders, bool useColSpanStyle = false)
		{
			if (Options.Sortable.GetValueOrDefault())
				throw new InvalidOperationException("Header grouping can not be set-up when columns sorting (reordering) is enabled.");

			_groupHeaders = groupHeaders;
			_groupHeadersUseColSpanStyle = useColSpanStyle;
			return this;
		}

		/// <summary>
		/// Sets frozen columns for this JqGridHelper instance.
		/// </summary>
		/// <returns>JqGridHelper instance with frozen columns.</returns>
		/// <exception cref="System.InvalidOperationException">
		/// <list type="bullet">
		/// <item><description>TreeGrid is enabled.</description></item>
		/// <item><description>SubGrid is enabled.</description></item>
		/// <item><description>Columns sorting (reordering) is enabled.</description></item>
		/// <item><description>Dynamic scrolling is enabled.</description></item>
		/// <item><description>Data grouping is enabled.</description></item>
		/// </list> 
		/// </exception>
		public JqGridHelper<TModel> SetFrozenColumns()
		{
			if (Options.TreeGridEnabled.GetValueOrDefault())
				throw new InvalidOperationException("Frozen columns can not be set-up when TreeGrid is enabled.");

			if (Options.SubgridEnabled.GetValueOrDefault())
				throw new InvalidOperationException("Frozen columns can not be set-up when SubGrid is enabled.");

			if (Options.Sortable.GetValueOrDefault())
				throw new InvalidOperationException("Frozen columns can not be set-up when columns sorting (reordering) is enabled.");

			if (Options.DynamicScrollingMode != JqGridDynamicScrollingModes.Disabled)
				throw new InvalidOperationException("Frozen columns can not be set-up when dynamic scrolling is enabled.");

			if (Options.GroupingEnabled.GetValueOrDefault())
				throw new InvalidOperationException("Frozen columns can not be set-up when data grouping is enabled.");

			_setFrozenColumns = true;
			return this;
		}

		/// <summary>
		/// Sets some column options with dynamic data, that we can't set in attributes
		/// </summary>
		/// <param name="column">Name of column</param>
		/// <param name="model">Model with data to set</param>
		/// <returns>This JqGridHelper</returns>
		/// <exception cref="ArgumentNullException">If column and/or model not defined</exception>
		/// <exception cref="KeyNotFoundException">If column doesn't exist in model</exception>
		public JqGridHelper<TModel> ExtendColumnOptions(string column, JqGridExtendColumnModel model)
		{
			if (string.IsNullOrWhiteSpace(column))
				throw new ArgumentNullException(nameof(column));
			if (Options.ColumnsModels.All(m => m.Name != column))
				throw new KeyNotFoundException("This column doesn't exist in model.");
			if (model == null)
				throw new ArgumentNullException(nameof(model));

			JqGridColumnModel columnModel = Options.ColumnsModels.Single(c => c.Name == column);

			if (model.IsEditOptionsPostDataSetted)
				columnModel.EditOptions.PostData = model.EditOptionsPostData;

			if (model.IsEditOptionsHtmlAttributesSetted)
				columnModel.EditOptions.HtmlAttributes = model.EditOptionsHtmlAttributes;

			if (model.IsEditOptionsValueDictionarySetted)
				columnModel.EditOptions.ValueDictionary = model.EditOptionsValueDictionary;

			if (model.IsLabelOptionsHtmlAttributesSetted)
				columnModel.LabelOptions.HtmlAttributes = model.LabelOptionsHtmlAttributes;

			if (model.IsLabelOptionsCssStylesSetted)
				columnModel.LabelOptions.CssStyles = model.LabelOptionsCssStyles;

			if (model.IsSearchOptionsHtmlAttributesSetted)
				columnModel.SearchOptions.HtmlAttributes = model.SearchOptionsHtmlAttributes;

			if (model.IsSearchOptionsValueDictionarySetted)
				columnModel.SearchOptions.ValueDictionary = model.SearchOptionsValueDictionary;

			return this;
		}
		#endregion
	}
}

