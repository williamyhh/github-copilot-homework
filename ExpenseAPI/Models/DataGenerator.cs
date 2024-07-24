// 使用 ExpenseContext 產生預設資料

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseAPI.Models
{
    public static class DataGenerator
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ExpenseContext(
                serviceProvider.GetRequiredService<DbContextOptions<ExpenseContext>>()))
            {
                // Look for any expenses.
                if (context.Expenses.Any())
                {
                    return;   // Data was already seeded
                }

                context.Expenses.AddRange(
                    new Expense
                    {
                        Category = "Food",
                        Date = DateTime.Parse("2021-01-01"),
                        Description = "Lunch",
                        Amount = 10
                    },
                    new Expense
                    {
                        Category = "Transportation",
                        Date = DateTime.Parse("2021-01-02"),
                        Description = "Bus",
                        Amount = 5
                    },
                    new Expense
                    {
                        Category = "Food",
                        Date = DateTime.Parse("2021-01-03"),
                        Description = "Dinner",
                        Amount = 15
                    },
                    new Expense
                    {
                        Category = "Transportation",
                        Date = DateTime.Parse("2021-01-04"),
                        Description = "Taxi",
                        Amount = 20
                    }
                );

                context.SaveChanges();
            }
        }
    }
}