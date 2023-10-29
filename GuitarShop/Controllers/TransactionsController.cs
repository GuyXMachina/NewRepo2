using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GuitarShop.Data;
using GuitarShop.Models;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Net;
using System.Net.Mail;

namespace GuitarShop.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly AppDbContext _context;

        public TransactionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Transactions.Include(t => t.Booking);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Transactions == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.Booking)
                .FirstOrDefaultAsync(m => m.TransactionID == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transactions/Create
        public IActionResult Create()
        {
            ViewData["BookingID"] = new SelectList(_context.Bookings, "BookingID", "Status");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransactionID,BookingID,Amount,TransactionDate,PaymentMethod")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                _context.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookingID"] = new SelectList(_context.Bookings, "BookingID", "Status", transaction.BookingID);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Transactions == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            ViewData["BookingID"] = new SelectList(_context.Bookings, "BookingID", "Status", transaction.BookingID);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TransactionID,BookingID,Amount,TransactionDate,PaymentMethod")] Transaction transaction)
        {
            if (id != transaction.TransactionID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.TransactionID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookingID"] = new SelectList(_context.Bookings, "BookingID", "Status", transaction.BookingID);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Transactions == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.Booking)
                .FirstOrDefaultAsync(m => m.TransactionID == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Transactions == null)
            {
                return Problem("Entity set 'AppDbContext.Transactions'  is null.");
            }
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.TransactionID == id);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessTransaction(Transaction transaction)
        {
            await Task.Run(() => GeneratePdf(transaction));
            await SendEmailWithPdfAsync();

            return RedirectToAction("Index");
        }

        public void GeneratePdf(Transaction transaction)
        {
            Document doc = new Document();
            PdfWriter.GetInstance(doc, new FileStream("TransactionReport.pdf", FileMode.Create));
            doc.Open();

            doc.Add(new Paragraph($"Transaction ID: {transaction.TransactionID}"));
            doc.Add(new Paragraph($"Booking ID: {transaction.BookingID}"));
            doc.Add(new Paragraph($"Amount: {transaction.Amount}"));
            doc.Add(new Paragraph($"Transaction Date: {transaction.TransactionDate}"));
            doc.Add(new Paragraph($"Payment Method: {transaction.PaymentMethod}"));

            doc.Close();
        }
        public async Task SendEmailWithPdfAsync()
        {
            string fromMail = "thembi1018@gmail.com";
            string password = "cyis kdhf pmpl ansl";

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(fromMail);
            mail.To.Add(new MailAddress("2018477840@ufs4life.ac.za"));
            mail.Subject = "Transaction Report";
            mail.Body = "Attached is the transaction report.";

            Attachment attachment;
            attachment = new Attachment("TransactionReport.pdf");
            mail.Attachments.Add(attachment);
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, password),
                EnableSsl = true
            };


            await SmtpServer.SendMailAsync(mail);
        }


    }
}
