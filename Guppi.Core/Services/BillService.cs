using System;
using System.IO;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Guppi.Core.Configurations;
using Guppi.Core.Exceptions;
using Guppi.Core.Interfaces.Services;
using Microsoft.Playwright;

namespace Guppi.Core.Services;

internal class BillService : IBillService
{
    private readonly BillConfiguration _configuration = Configuration.Load<BillConfiguration>("billing");

    readonly static string _downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "bills");

    int _row = 2;
    IXLWorksheet _worksheet;

    private bool Configured => _configuration.Configured;

    public async Task DownloadAlectraBills()
    {
        if (!Configured)
        {
            throw new UnconfiguredException("Please configure the Billing provider");
        }

        // Create an Excel spreadsheet to store the billing data
        string path = Path.Combine(_downloadsPath, "Bills.xlsx");
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        var workbook = new ClosedXML.Excel.XLWorkbook();
        _worksheet = workbook.Worksheets.Add("Bills");

        _worksheet.Cell(1, 1).Value = "Account";
        _worksheet.Cell(1, 2).Value = "Date";
        _worksheet.Cell(1, 3).Value = "Amount";

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

        await DownloadBillsForAccount(page, "8501783878");
        await DownloadBillsForAccount(page, "7030931444");
        await DownloadBillsForAccount(page, "9676981145");
        await DownloadBillsForAccount(page, "7076520332");

        await Task.Delay(5000);

        workbook.SaveAs(path);
    }

    private async Task DownloadBillsForAccount(IPage page, string account)
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

            if (DateTime.TryParse(date, out DateTime d))
            {
                date = d.ToShortDateString();
            }

            if (double.TryParse(amount.Substring(1), out double a))
            {
                amount = a.ToString("N2");
            }

            _worksheet.Cell(_row, 1).Value = account;
            _worksheet.Cell(_row, 2).Value = date;
            _worksheet.Cell(_row, 3).Value = amount;
            _row++;

            Console.WriteLine($"{account} {date} {amount}");

            await DownloadBill(page, account, $"{date} {amount}");
        }
    }

    private async Task DownloadBill(IPage page, string account, string bill)
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
            Directory.CreateDirectory(_downloadsPath);

            var filePath = Path.Combine(_downloadsPath, $"{account} {bill}.pdf");
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
    public void Configure()
    {
        var configuration = Configuration.Load<BillConfiguration>("billing");
        configuration.RunConfiguration("Billing", "Enter credentials for billing providers");
    }
}
