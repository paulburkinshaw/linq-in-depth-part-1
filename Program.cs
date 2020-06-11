using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using static System.Console;

namespace linq_in_depth_part1
{
    public class Product
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
    }


    class Program
    {
        static IEnumerable<Product> products;
        static Program()
        {
            var products = new List<Product>
            {
                new Product { Name = "Wrench", Category = "Hand Tools", Price = 6.00m },
                new Product { Name = "Claw Hammer", Category = "Hand Tools", Price = 4.00m },
                new Product { Name = "Drill", Category = "Power Tools", Price = 40.00m },
                new Product { Name = "Jigsaw", Category = "Power Tools", Price = 60.00m },
                new Product { Name = "Padlock", Category = "Ironmongery", Price = 2.50m },
                new Product { Name = "Silicone", Category = "Sealants", Price = 15.00m }
            };

            Program.products = products;
        }


        static void Main(string[] args)
        {

            Setup();

        }

        public static void Setup()
        {
            Console.Clear();

            Console.CursorSize = 1;
            ForegroundColor = ConsoleColor.Blue;
            WriteLine("************************************");
            WriteLine("                                    ");
            WriteLine("       Dot Net eCoremmerce          ");
            WriteLine("                                    ");
            WriteLine("************************************");
            WriteLine("");
            ForegroundColor = ConsoleColor.White;
            WriteLine("Select a feature from the below options:");
            WriteLine("");
            ForegroundColor = ConsoleColor.Green;
            WriteLine("1. Products broken down by category");
            WriteLine("2. Product and Price Breakdown");
            WriteLine("3. Search API");

            ForegroundColor = ConsoleColor.White;
            ConsoleKeyInfo key = ReadKey();

            switch (key.KeyChar)
            {
                case '1':
                    Console.Clear();
                    Feature1();
                    break;
                case '2':
                    Console.Clear();
                    Feature2();
                    break;
                case '3':
                    Console.Clear();
                    WriteLine("Please supply a comma seperated search query eg: Hand Tools,Sealants");
                    WriteLine("");
                    WriteLine("The categories in the system are:");
                    WriteLine("");
                    ForegroundColor = ConsoleColor.Green;
                    WriteLine("Hand Tools");
                    WriteLine("Sealants");
                    WriteLine("Power Tools");
                    WriteLine("Ironmongery");
                    WriteLine("");
                    ForegroundColor = ConsoleColor.Magenta;
                    Write("Search: ");
                    ForegroundColor = ConsoleColor.White;
                    var searchQueryString = ReadLine();

                    string[] categories = searchQueryString.Split(',');

                    Feature3(categories);

                    break;
                default:
                    Console.Clear();
                    ForegroundColor = ConsoleColor.Red;
                    WriteLine("Invalid input");
                    break;
            }
        }

        public static void Feature1()
        {
            // Fluent / Method syntax
            // var result = products.GroupBy(p => p.Category);

            // Query syntax
            var result = from p in products
                         group p by p.Category;

            foreach (var grouping in result)
            {
                Console.WriteLine($"Category: {grouping.Key}");
                foreach (Product product in grouping)
                    Console.WriteLine($" - Product: {product.Name}");
            }
        }

        public static void Feature2()
        {

            /* // Fluent syntax as one expression using Query Operator Chaining
            var result = products.GroupBy(product => product.Category)
                                 .Select(grouping => new
                                 {
                                     Category = grouping.Key,
                                     ProductCount = grouping.Count(),
                                     TotalPrice = grouping.Sum(x => x.Price)
                                 }); */





            /* // Progressive Query Building in Fluent Syntax
             var grouped = products.GroupBy(product => product.Category);

             var result = grouped.Select(grouping => new
             {
                 Category = grouping.Key,
                 ProductCount = grouping.Count(),
                 TotalPrice = grouping.Sum(x => x.Price)
             }); */




            /*  // Progressive Query Building using query syntax using 
             var grouped = from product in products
                           group product by product.Category;

             var result = from grouping in grouped
                          select new
                          {
                              Category = grouping.Key,
                              ProductCount = grouping.Count(),
                              TotalPrice = grouping.Sum(x => x.Price)
                          }; */




            // Query syntax using into chaining strategy
            var result = from product in products
                         group product by product.Category
                         into grouping
                         select new
                         {
                             Category = grouping.Key,
                             ProductCount = grouping.Count(),
                             TotalPrice = grouping.Sum(x => x.Price)
                         };



            foreach (var grouping in result)
            {
                Console.WriteLine($"Category: {grouping.Category}");
                Console.WriteLine($" - No Products: {grouping.ProductCount}");
                Console.WriteLine($" - Total Price: ${grouping.TotalPrice}");
            }

        }

        public static void Feature3(string[] searchQuery = null)
        {

            /* // Single expression query
            var result = (from product in products
                          where searchQuery.Any(c => c == product.Category)
                          group product by 1
                          into filteredGrouping
                          let products = from p in filteredGrouping select p
                          let totalCategories = products.Select(p => p.Category).Distinct().Count()
                          select new
                          {
                              products = products,
                              totals = new
                              {
                                  total_products = products.Count(),
                                  total_categories = totalCategories,
                                  total_price = products.Sum(p => p.Price)
                              }
                          }).FirstOrDefault(); */


            // Progressive Query Building using mixture of fluent and query syntax
            var filteredProducts = from p in products
                                   where searchQuery.Any(c => c == p.Category)
                                   select p;

            var totalProducts = filteredProducts.Count();
            var totalCategories = filteredProducts.Select(p => p.Category).Distinct().Count();
            var totalPrice = filteredProducts.Sum(p => p.Price);

            var result = new
            {
                products = filteredProducts,
                totals = new
                {
                    total_products = totalProducts,
                    total_categories = totalCategories,
                    total_price = totalPrice
                }
            };

            var json = JsonSerializer.Serialize(result, new JsonSerializerOptions() { WriteIndented = true });

            Console.WriteLine(json);

        }
    }
}
