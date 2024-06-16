using System;
using System.Collections.Generic;

internal class Program
{
    private class Customer
    {
        public string Name { get; set; }
        public int LastMonth { get; set; }
        public int ThisMonth { get; set; }
        public string CustomerType { get; set; }
        public int NumberPeople { get; set; }
        public double TotalBill { get; set; }
    }

    private static void Main()
    {
        List<Customer> customers = new List<Customer>();
        while (true)
        {
            string name = PromptUser("Enter customer name (or 'exit' to finish): ");
            if (name.ToLower() == "exit") break;

            int lastMonth = PromptForInt("Enter last month water meter reading: ");
            int thisMonth = PromptForInt("Enter this month water meter reading: ");

            if (thisMonth < lastMonth)
            {
                Console.WriteLine("Error: This month's reading cannot be less than last month's reading.");
                continue;
            }

            string customerType = PromptUser("Enter type of customer (household/administrative/production/business): ").ToLower();
            int numberPeople = 0;

            if (customerType == "household")
            {
                numberPeople = PromptForInt("Enter number of people in the household: ");
            }

            int consumption = thisMonth - lastMonth;
            double totalBill = CalculateWaterBill(customerType, consumption, numberPeople);

            var customer = new Customer
            {
                Name = name,
                LastMonth = lastMonth,
                ThisMonth = thisMonth,
                CustomerType = customerType,
                NumberPeople = numberPeople,
                TotalBill = totalBill
            };

            customers.Add(customer);
            DisplayCustomerInfo(customer, consumption);
        }

        string searchName = PromptUser("Enter the name of the customer to search for: ");
        Customer foundCustomer = customers.Find(c => c.Name.Equals(searchName, StringComparison.OrdinalIgnoreCase));

        if (foundCustomer != null)
        {
            DisplayCustomerInfo(foundCustomer, foundCustomer.ThisMonth - foundCustomer.LastMonth);
        }
        else
        {
            Console.WriteLine("Customer not found.");
        }

        Console.ReadKey();
    }

    private static string PromptUser(string message)
    {
        Console.Write(message);
        return Console.ReadLine();
    }

    private static int PromptForInt(string message)
    {
        int value;
        while (!int.TryParse(PromptUser(message), out value))
        {
            Console.WriteLine("Invalid input. Please enter a valid number.");
        }
        return value;
    }

    private static void DisplayCustomerInfo(Customer customer, int consumption)
    {
        Console.WriteLine("\nCustomer Information:");
        Console.WriteLine($"Name: {customer.Name}");
        Console.WriteLine($"Last Month's Reading: {customer.LastMonth}");
        Console.WriteLine($"This Month's Reading: {customer.ThisMonth}");
        Console.WriteLine($"Consumption: {consumption} m³");
        Console.WriteLine($"Total Bill (without VAT): {customer.TotalBill:N0} VND");
        Console.WriteLine($"Total Bill (with 10% VAT): {customer.TotalBill * 1.1:N0} VND\n");
        Console.ReadKey();
    }

    private static double CalculateWaterBill(string customerType, int consumption, int numberOfPeople)
    {
        double rate = customerType switch
        {
            "household" => CalculateHouseholdRate(consumption, numberOfPeople),
            "administrative" => 9955.0,
            "production" => 11615.0,
            "business" => 22068.0,
            _ => throw new ArgumentException("Invalid customer type.")
        };

        return rate * consumption;
    }

    private static double CalculateHouseholdRate(int consumption, int numberOfPeople)
    {
        double consumptionPerPerson = (double)consumption / numberOfPeople;
        return consumptionPerPerson switch
        {
            <= 10.0 => 5973.0,
            <= 20.0 => 7052.0,
            <= 30.0 => 8699.0,
            _ => 15929.0
        };
    }
}
