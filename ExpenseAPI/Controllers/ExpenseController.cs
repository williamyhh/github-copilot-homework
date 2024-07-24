// 這個 API Controller 用來處理 Expense 相關的 HTTP Request

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseAPI.Models;

namespace ExpenseAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExpenseController : ControllerBase
    {
        private readonly ExpenseContext _context;

        public ExpenseController(ExpenseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Expense>>> GetExpenses()
        {
            return await _context.Expenses.ToListAsync();
        }

        // 有一個Action，其參數接受一個型別為string的參數，用來模糊搜尋Description欄位；兩個參數用來查詢日期區間
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Expense>>> SearchExpenses(string description, DateTime? startDate, DateTime? endDate)
        {
            // 查詢的起訖時間不得超過30天
            if (startDate != null && endDate != null)
            {
                if (endDate.Value.Subtract(startDate.Value).Days > 30)
                {
                    return BadRequest("Date range must not exceed 30 days");
                }
            }

            // 如果沒有傳入日期區間，則預設為所有資料
            if (startDate == null)
            {
                startDate = DateTime.MinValue;
            }
            if (endDate == null)
            {
                endDate = DateTime.MaxValue;
            }

            // 如果沒有傳入description，則預設為空字串
            if (description == null)
            {
                description = "";
            }

            // 透過EF Core的LINQ查詢，找出符合條件的資料
            var expenses = await _context.Expenses
                .Where(e => e.Description.Contains(description) && e.Date >= startDate && e.Date <= endDate)
                .ToListAsync();

            return expenses;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Expense>> GetExpense(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);

            if (expense == null)
            {
                return NotFound();
            }

            return expense;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutExpense(int id, Expense expense)
        {
            if (id != expense.Id)
            {
                return BadRequest();
            }

            _context.Entry(expense).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExpenseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Expense>> PostExpense(Expense expense)
        {
            // Expense 的所有屬性都是必填的
            if (expense.Date == null || expense.Description == null || expense.Amount == 0 || expense.Category == null)
            {
                return BadRequest("All fields are required");
            }

            // Expense 的 Category 只能是以下選項: 食, 衣, 住, 行
            if (expense.Category != "食" && expense.Category != "衣" && expense.Category != "住" && expense.Category != "行")
            {
                return BadRequest("Category must be 食, 衣, 住, 行");
            }

            // Expense 的 Amount 必須大於 0
            if (expense.Amount <= 0)
            {
                return BadRequest("Amount must be greater than 0");
            }

            // Expense 的 Date 不得晚於 2023 年
            if (expense.Date.Year > 2023)
            {
                return BadRequest("Date must not be later than 2023");
            }

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetExpense", new { id = expense.Id }, expense);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
            {
                return NotFound();
            }

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ExpenseExists(int id)
        {
            return _context.Expenses.Any(e => e.Id == id);
        }
    }
}