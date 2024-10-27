using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Guppi.Core.Configurations;
using Guppi.Core.Exceptions;
using Guppi.Core.Interfaces.Services;
using Guppi.Core.Providers;
using Microsoft.Playwright;

namespace Guppi.Core.Services;

internal class BillService : IBillService
{
    readonly BillConfiguration _configuration = Configuration.Load<BillConfiguration>("billing");
    WorkbookProvider _workbook;

    readonly static string DOWNLOAD_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "bills");
    readonly static string[] HEADERS = ["Account", "Date", "Amount"];
    const string WORKSHEET = "Bills";

    private bool Configured => _configuration.Configured;

    public async Task DownloadAllBills()
    {
        EnsureConfigured();
        CreateWorkbook("Bills.xlsx");

        await DownloadAlectraBillsInternal();
        await DownloadEnbridgeBillsInternal();

        _workbook.Save();
    }

    public async Task DownloadAlectraBills()
    {
        EnsureConfigured();
        CreateWorkbook("Alectra.xlsx");

        await DownloadAlectraBillsInternal();

        _workbook.Save();
    }

    public async Task DownloadEnbridgeBills()
    {
        EnsureConfigured();
        CreateWorkbook("Enbridge.xlsx");

        await DownloadEnbridgeBillsInternal();

        _workbook.Save();
    }

    private void EnsureConfigured()
    {
        if (!Configured)
        {
            throw new UnconfiguredException("Please configure the Billing provider");
        }
    }

    private void CreateWorkbook(string filename)
    {
        string path = Path.Combine(DOWNLOAD_PATH, filename);
        _workbook = new WorkbookProvider(path, WORKSHEET, HEADERS);
    }

    private async Task DownloadAlectraBillsInternal()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        // Login to Alectra
        await page.GotoAsync("https://myalectra.alectrautilities.com/portal/#/login");
        await page.GetByLabel("Please enter Username").FillAsync(_configuration.AlectraUsername);
        await page.GetByLabel("Please enter your password.").FillAsync(_configuration.AlectraPassword);
        await page.GetByLabel("Click Here to Sign In").ClickAsync();

        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await DownloadAlectraBillsForAccount(page, "8501783878");
        await DownloadAlectraBillsForAccount(page, "7030931444");
        await DownloadAlectraBillsForAccount(page, "9676981145");
        await DownloadAlectraBillsForAccount(page, "7076520332");

        await Task.Delay(5000);

        await browser.CloseAsync();
    }

    private async Task DownloadEnbridgeBillsInternal()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        // Enbridge login page
        await page.GotoAsync("https://myaccount.enbridgegas.com/");

        // Accept cookies
        await page.GetByTestId("uc-accept-all-button").ClickAsync();

        // Login to Enbridge
        await page.GetByLabel("Email address:").FillAsync(_configuration.EnbridgeUsername);
        await page.GetByLabel("Password:").FillAsync(_configuration.EnbridgePassword);
        await page.GetByRole(AriaRole.Button, new() { Name = "Sign in", Exact = true }).ClickAsync();

        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await DownloadEnbridgeBillsForAccount(page, "201 VICTORIA AVE N UNIT 1");
        await DownloadEnbridgeBillsForAccount(page, "201 VICTORIA AVE N UNIT 2");

        await Task.Delay(5000);

        await browser.CloseAsync();
    }

    private async Task DownloadAlectraBillsForAccount(IPage page, string account)
    {
        // Navigate to Billing History
        await page.GotoAsync("https://myalectra.alectrautilities.com/portal/#/billinghistory");

        // Wait for the bills to load
        await page.WaitForSelectorAsync("tr.billing-history-row");

        // Click the account selector dropdown
        var dropdown = await page.QuerySelectorAsync("button#accountSelect");
        await dropdown.ClickAsync();

        // Click the account in the dropdown
        var menu = await page.QuerySelectorAsync($"li[value=\"{account}\"]");
        await menu.ClickAsync();

        // Wait for the billing history to load
        await page.WaitForSelectorAsync("tr.billing-history-row");

        // Download each bill
        var rows = await page.QuerySelectorAllAsync("tr.billing-history-row");
        foreach (var row in rows)
        {
            var cells = await row.QuerySelectorAllAsync("td");
            var date = await cells[0].InnerTextAsync();
            var amount = await cells[1].InnerTextAsync();

            Console.WriteLine($"{account} {date} {amount}");

            await DownloadAlectraBill(page, account, $"{date} {amount}");

            _workbook.AddRow([account, date, amount]);
        }
    }

    private async Task DownloadEnbridgeBillsForAccount(IPage page, string account)
    {
        // Navigate to Billing History
        await page.GotoAsync("https://myaccount.enbridgegas.com/my-account/account-activity?activityType=Bills&periodFilter=PAST1YR");

        // Wait for the bills to load
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Click the account selector dropdown
        await page.Locator("span:nth-child(3)").ClickAsync();

        // Click the account in the dropdown
        await page.Locator("#dynamic-selector").GetByText(account).ClickAsync();

        // Wait for the billing history to load
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Task.Delay(1000);

        // Download each bill
        var dateCells = await page.QuerySelectorAllAsync("p.down-date-list");
        var billLinks = await page.QuerySelectorAllAsync("div.td.download-details-list p a");

        for (int i = 0; i < dateCells.Count && i < billLinks.Count; i++)
        {
            var date = await dateCells[i].InnerTextAsync();
            var billLink = billLinks[i];

            Console.WriteLine($"{account} {date} --");
            await DownloadEnbridgeBill(page, billLink, account, date);
            _workbook.AddRow([account, date, "0"]);
        }
    }

    private async Task DownloadAlectraBill(IPage page, string account, string bill)
    {
        // Open the billing page in a new tab
        var billingPage = await page.RunAndWaitForPopupAsync(async () =>
        {
            await page.GetByRole(AriaRole.Checkbox, new() { Name = bill }).GetByLabel("Navigate to View Bill PDF").ClickAsync();
        });

        // Listen for download events so we can specify the 
        billingPage.Download += async (_, download) =>
        {
            // Ensure the directory exists
            Directory.CreateDirectory(DOWNLOAD_PATH);

            var filePath = Path.Combine(DOWNLOAD_PATH, $"{account} {bill}.pdf");
            await download.SaveAsAsync(filePath);

            await billingPage.CloseAsync();
        };

        // This closes the warning popup that appears when the download is initiated
        async void billingPage_Dialog_EventHandler(object sender, IDialog dialog)
        {
            await dialog.AcceptAsync();
            billingPage.Dialog -= billingPage_Dialog_EventHandler;
        }

        billingPage.Dialog += billingPage_Dialog_EventHandler;

        // Click the download button
        await billingPage.RunAndWaitForDownloadAsync(async () =>
        {
            await billingPage.GetByRole(AriaRole.Img, new() { Name = "Download PDF" }).ClickAsync();
        });
    }

    private async Task DownloadEnbridgeBill(IPage page, IElementHandle link, string account, string date)
    {
        // Open the billing page in a new tab
        var billingPage = await page.RunAndWaitForPopupAsync(async () =>
        {
            await link.ClickAsync();
        });

        // Listen for download events so we can specify the 
        billingPage.Download += async (_, download) =>
        {
            // Ensure the directory exists
            Directory.CreateDirectory(DOWNLOAD_PATH);

            var filePath = Path.Combine(DOWNLOAD_PATH, $"{account} {date}.pdf");
            await download.SaveAsAsync(filePath);

            await billingPage.CloseAsync();
        };

        // This closes the warning popup that appears when the download is initiated
        async void billingPage_Dialog_EventHandler(object sender, IDialog dialog)
        {
            await dialog.AcceptAsync();
            billingPage.Dialog -= billingPage_Dialog_EventHandler;
        }

        billingPage.Dialog += billingPage_Dialog_EventHandler;

        // Click the download button
        await billingPage.RunAndWaitForDownloadAsync(async () =>
        {
            await billingPage.GetByRole(AriaRole.Img, new() { Name = "Download PDF" }).ClickAsync();
        });

        // Wait for the page to close
        await WaitForPageCloseAsync(billingPage);
    }

    public static async Task WaitForPageCloseAsync(IPage page)
    {
        // Use a TaskCompletionSource to wait for the page to close
        var pageClosed = new TaskCompletionSource<bool>();

        // Event handler to set the result when the page closes
        page.Close += (_, _) => pageClosed.SetResult(true);

        // Wait for the TaskCompletionSource to complete
        await pageClosed.Task;
    }

    public void Configure()
    {
        var configuration = Configuration.Load<BillConfiguration>("billing");
        configuration.RunConfiguration("Billing", "Enter credentials for billing providers");
    }
}
