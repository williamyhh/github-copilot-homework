using ExpenseAPI.Controllers;
using ExpenseAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseAPITest;

public class ExpenseControllerTests
{
    private ExpenseContext _context;

    public ExpenseControllerTests()
    {
        var options = new DbContextOptionsBuilder<ExpenseContext>()
            .UseInMemoryDatabase(databaseName: "ExpenseDatabase")
            .Options;
        _context = new ExpenseContext(options);
    }

    [Fact]
    public async Task GetExpenses_ReturnsExpenses()
    {
        // Arrange
        // 清空資料庫以確保測試的獨立性
        ClearData();
        // 初始化資料
        InitializeData();

        var controller = new ExpenseController(_context);

        // Act
        var result = await controller.GetExpenses();

        // Assert
        var expenses = Assert.IsType<List<Expense>>(result.Value);
        Assert.Equal(4, expenses.Count); // 確保返回了四筆資料
    }

    [Fact]
    public async Task SearchExpenses_ReturnsExpenses()
    {
        // Arrange
        // 清空資料庫以確保測試的獨立性
        ClearData();
        // 初始化資料
        InitializeData();

        var controller = new ExpenseController(_context);

        // Act
        var result = await controller.SearchExpenses("Lunch", DateTime.Parse("2021-01-01"), DateTime.Parse("2021-01-01"));

        // Assert
        var expenses = Assert.IsType<List<Expense>>(result.Value);
        Assert.Single(expenses);
    }

    [Fact]
    public async Task GetExpense_ReturnsExpense()
    {
        // Arrange
        // 清空資料庫以確保測試的獨立性
        ClearData();
        // 初始化資料
        InitializeData();

        var controller = new ExpenseController(_context);

        // Act
        var result = await controller.GetExpense(1);

        // Assert
        var expense = Assert.IsType<Expense>(result.Value);
        Assert.Equal("Lunch", expense.Description);
    }

    // 為 PostExpense 方法撰寫測試，確保新增支出的功能正常運作
    // 要用 AAA 模式（Arrange, Act, Assert）來撰寫測試
    // Arrange：準備測試環境
    // Act：執行要測試的方法
    // Assert：驗證方法的執行結果是否符合預期
    [Fact]
    public async Task PostExpense_AddsExpense()
    {
        // Arrange
        // 清空資料庫以確保測試的獨立性
        ClearData();
        // 初始化資料
        InitializeData();

        var controller = new ExpenseController(_context);
        var newExpense = new Expense
        {
            Category = "Food",
            Date = DateTime.Parse("2021-01-05"),
            Description = "Dinner",
            Amount = 20
        };

        // Act
        await controller.PostExpense(newExpense);

        // Assert
        var result = await controller.GetExpenses();
        var expenses = Assert.IsType<List<Expense>>(result.Value);
        Assert.Equal(5, expenses.Count);
    }

    // 為 PutExpense 方法撰寫測試，確保修改支出的功能正常運作
    [Fact]
    public async Task PutExpense_UpdatesExpense()
    {
        // Arrange
        // 清空資料庫以確保測試的獨立性
        ClearData();
        // 初始化資料
        InitializeData();

        var controller = new ExpenseController(_context);
        var updatedExpense = new Expense
        {
            Id = 1,
            Category = "Food",
            Date = DateTime.Parse("2021-01-01"),
            Description = "Lunch",
            Amount = 15
        };

        // Act
        await controller.PutExpense(1, updatedExpense);

        // Assert
        var result = await controller.GetExpense(1);
        var expense = Assert.IsType<Expense>(result.Value);
        Assert.Equal(15, expense.Amount);
    }

    // 為 DeleteExpense 方法撰寫測試，確保刪除支出的功能正常運作
    [Fact]
    public async Task DeleteExpense_RemovesExpense()
    {
        // Arrange
        // 清空資料庫以確保測試的獨立性
        ClearData();
        // 初始化資料
        InitializeData();

        var controller = new ExpenseController(_context);

        // Act
        await controller.DeleteExpense(1);

        // Assert
        var result = await controller.GetExpenses();
        var expenses = Assert.IsType<List<Expense>>(result.Value);
        Assert.Equal(4, expenses.Count);
    }

    // 建立一個私有方法，用來初始化資料庫
    private void InitializeData()
    {
        // 建立五筆預設資料，Expense類別的所有屬性都要填入值
        var expenses = new Expense[]
        {
            new Expense
            {
                Date = DateTime.Parse("2021-01-01"),
                Description = "Lunch",
                Amount = 10,
                Category = "Food"
            },
            new Expense
            {
                Date = DateTime.Parse("2021-01-02"),
                Description = "Dinner",
                Amount = 20,
                Category = "Food"
            },
            new Expense
            {
                Date = DateTime.Parse("2021-01-03"),
                Description = "Breakfast",
                Amount = 5,
                Category = "Food"
            },
            new Expense
            {
                Date = DateTime.Parse("2021-01-04"),
                Description = "Snack",
                Amount = 3,
                Category = "Food"
            }
        };
        _context.Expenses.AddRange(expenses);

        _context.SaveChanges();
    }

    // 建立一個私有方法，用來清空資料庫
    private void ClearData()
    {
        _context.Expenses.RemoveRange(_context.Expenses);
        _context.SaveChanges();
    }
}