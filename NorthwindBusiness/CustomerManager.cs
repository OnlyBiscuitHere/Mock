using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using NorthwindData;


namespace NorthwindBusiness
{
    public class CustomerManager
    {
        public Customer SelectedCustomer { get; set; }

        public void SetSelectedCustomer(object selectedItem)
        {
            SelectedCustomer = (Customer)selectedItem;
        }

        public List<Customer> RetrieveAll()
        {
            using (var db = new NorthwindContext())
            {
                return db.Customers.ToList();
            }
        }

        public void Create(string customerId, string contactName, string companyName, string city = null)
        {
            var newCust = new Customer() { CustomerId = customerId, ContactName = contactName, CompanyName = companyName };
            using (var db = new NorthwindContext())
            {
                db.Customers.Add(newCust);
                db.SaveChanges();
            }
        }

        public bool Update(string customerId, string contactName, string country, string city, string postcode)
        {
            using (var db = new NorthwindContext())
            {
                var customer = db.Customers.Where(c => c.CustomerId == customerId).FirstOrDefault();
                if (customer == null)
                {
                    Debug.WriteLine($"Customer {customerId} not found");
                    return false;
                }
                customer.ContactName = contactName;
                customer.City = city;
                customer.PostalCode = postcode;
                customer.Country = country;
                // write changes to database
                try
                {
                    db.SaveChanges();
                    SelectedCustomer = customer;
                }
                catch (Exception e) // an exception can be thrown if the database has been updated since last loaded 
                {
                    Debug.WriteLine($"Error updating {customerId}");
                    return false;
                }
            }
            return true;
        }

        public bool Delete(string customerId)
        {
            using (var db = new NorthwindContext())
            {
                var customer = db.Customers.Where(c => c.CustomerId == customerId).FirstOrDefault();
                if (customer == null)
                {
                    Debug.WriteLine($"Customer {customerId} not found");
                    return false;
                }
                db.Customers.Remove(customer);
                SelectedCustomer = null;
                db.SaveChanges();
            }
            return true;
        }
    }
}
