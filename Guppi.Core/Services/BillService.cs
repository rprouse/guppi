using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
    public void InstallPlaywright()
    {
        Program.Main([ "install" ]);
    }

    public async Task DownloadAllBills(int months)
    {
        EnsureConfigured();
        CreateWorkbook("Bills.xlsx");

        await DownloadAlectraBillsInternal(months);
        await DownloadEnbridgeBillsInternal(months);

        _workbook.Save();
    }

    public async Task DownloadAlectraBills(int months)
    {
        EnsureConfigured();
        CreateWorkbook("Alectra.xlsx");

        await DownloadAlectraBillsInternal(months);

        _workbook.Save();
    }

    public async Task DownloadEnbridgeBills(int months)
    {
        EnsureConfigured();
        CreateWorkbook("Enbridge.xlsx");

        await DownloadEnbridgeBillsInternal(months);

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

    private async Task DownloadAlectraBillsInternal(int months)
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

        await DownloadAlectraBillsForAccount(page, months, "8501783878");
        await DownloadAlectraBillsForAccount(page, months, "7030931444");
        await DownloadAlectraBillsForAccount(page, months, "9676981145");
        await DownloadAlectraBillsForAccount(page, months, "7076520332");

        await Task.Delay(5000);

        await browser.CloseAsync();
    }

    private async Task DownloadEnbridgeBillsInternal(int months)
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

        await DownloadEnbridgeBillsForAccount(page, months, "201 VICTORIA AVE N UNIT 1");
        await DownloadEnbridgeBillsForAccount(page, months, "201 VICTORIA AVE N UNIT 2");

        await Task.Delay(5000);

        await browser.CloseAsync();
    }

    private async Task DownloadAlectraBillsForAccount(IPage page, int months, string account)
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
        foreach (var row in rows.Take(months))
        {
            var cells = await row.QuerySelectorAllAsync("td");
            var date = await cells[0].InnerTextAsync();
            var amount = await cells[1].InnerTextAsync();

            var cleanDate =  DateTime.TryParse(date, out DateTime d) ? d.ToString("yyyy-MM-dd") : date;

            Console.WriteLine($"{cleanDate} {account} {amount}");

            await DownloadAlectraBill(page, account, cleanDate, $"{date} {amount}");

            _workbook.AddRow([account, cleanDate, amount.Replace("$", "")]);
        }
    }

    private async Task DownloadEnbridgeBillsForAccount(IPage page, int months, string account)
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

        for (int i = 0; i < months&&  i < dateCells.Count && i < billLinks.Count; i++)
        {
            var date = await dateCells[i].InnerTextAsync();
            var billLink = billLinks[i];

            var cleanDate = DateTime.TryParseExact(date, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime d) ? d.ToString("yyyy-MM-dd") : date;

            Console.WriteLine($"{cleanDate} {account} $--");
            await DownloadEnbridgeBill(page, billLink, account, date);
            _workbook.AddRow([account, cleanDate, "0"]);
        }
    }

    private async Task DownloadAlectraBill(IPage page, string account, string date, string bill)
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

            var filePath = Path.Combine(DOWNLOAD_PATH, $"{date} {account}.pdf");
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

            var filePath = Path.Combine(DOWNLOAD_PATH, $"{account} {date.Replace('/', '-')}.pdf");
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
