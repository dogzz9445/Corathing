using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Wpf;
using Microsoft.CodeAnalysis.CSharp;
using Wpf.Ui.Appearance;
using Corathing.Contracts.Services;
using Microsoft.Extensions.DependencyInjection;
using ApplicationTheme = Wpf.Ui.Appearance.ApplicationTheme;

namespace Corathing.Widgets.Basics.Widgets.Monacos;

public class MonacoController
{
    private const string EditorContainerSelector = "#root";

    private const string EditorObject = "wpfUiMonacoEditor";

    private readonly WebView2 _webView;
    private readonly IApplicationService _applicationService;

    public string LightUiThemeName = "wpf-ui-app-light-theme";
    public string DarkUiThemeName = "wpf-ui-app-dark-theme";
    public string LightMonacoTheme = "vs";
    public string DarkMonacoTheme = "vs-dark";
    public string LightBackgroundColor = "FFFFFF00";
    public string DarkBackgroundColor = "1F1F1F00";

    public MonacoController(IApplicationService applicationService, WebView2 webView)
    {
        _applicationService = applicationService;
        _webView = webView;
    }

    public async Task CreateAsync()
    {
        _ = await _webView.ExecuteScriptAsync(
            $$"""
            const {{EditorObject}} = monaco.editor.create(document.querySelector('{{EditorContainerSelector}}'));
            window.onresize = () => {{{EditorObject}}.layout();}
            """
        );
    }

    public async Task DefineThemeAsync()
    {
        _ = await _webView.ExecuteScriptAsync(
            $$$"""
            monaco.editor.defineTheme('{{{LightUiThemeName}}}', {
                base: '{{{LightMonacoTheme}}}',
                inherit: true,
                rules: [{ background: '{{{LightBackgroundColor}}}' }],
                colors: {'editor.background': '#{{{LightBackgroundColor}}}','minimap.background': '#{{{LightBackgroundColor}}}',}});

            monaco.editor.defineTheme('{{{DarkUiThemeName}}}', {
                base: '{{{DarkMonacoTheme}}}',
                inherit: true,
                rules: [{ background: '{{{DarkBackgroundColor}}}' }],
                colors: {'editor.background': '#{{{DarkBackgroundColor}}}','minimap.background': '#{{{DarkBackgroundColor}}}',}});
            """
        );
    }

    public async Task SetThemeAsync(ApplicationTheme appApplicationTheme)
    {
        var uiThemeName = appApplicationTheme == ApplicationTheme.Light ? LightMonacoTheme : DarkMonacoTheme;

        _ = await _webView.ExecuteScriptAsync(
            $$$"""
            monaco.editor.setTheme('{{{uiThemeName}}}');
            """
        );
    }

    public async Task SetLanguageAsync(MonacoLanguage monacoLanguage)
    {
        var languageId =
            monacoLanguage == MonacoLanguage.ObjectiveC ? "objective-c" : monacoLanguage.ToString().ToLower();

        await _webView.ExecuteScriptAsync(
            "monaco.editor.setModelLanguage(" + EditorObject + $".getModel(), \"{languageId}\");"
        );
    }

    public async Task SetContentAsync(string contents)
    {
        var literalContents = SymbolDisplay.FormatLiteral(contents, false);

        await _webView.ExecuteScriptAsync(EditorObject + $".setValue(\"{literalContents}\");");
    }

    public void DispatchScript(string script)
    {
        if (_webView == null)
        {
            return;
        }

        var task = _applicationService.DispatchAsync(async () => await _webView!.ExecuteScriptAsync(script));

    }
}
