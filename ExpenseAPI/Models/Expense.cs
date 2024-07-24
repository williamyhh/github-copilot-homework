// 這是用來代表支出的類別
// 這個類別會對應到資料庫中的一個資料表
// 這個類別會被 EF Core 用來建立資料表
// 這個類別有幾個欄位，分別是 Id, Date, Description, Amount, Category
// 這個類別有一個屬性，叫做 Id，這個屬性是用來代表支出的編號，這個屬性是唯一的，且是自動增加的
// 這個類別有一個屬性，叫做 Date，這個屬性是用來代表支出的日期
// 這個類別有一個屬性，叫做 Description，這個屬性是用來代表支出的描述
// 這個類別有一個屬性，叫做 Amount，這個屬性是用來代表支出的金額
// 這個類別有一個屬性，叫做 Category，這個屬性是用來代表支出的類別

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseAPI.Models
{
    public class Expense
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // 設定為自動增加
        public int Id { get; set; } // 支出的編號
        [Required]
        public DateTime Date { get; set; } // 支出的日期
        [Required]
        public string Description { get; set; } // 支出的描述
        [Required]
        public decimal Amount { get; set; } // 支出的金額
        [Required]
        public string Category { get; set; } // 支出的類別
    }
}

