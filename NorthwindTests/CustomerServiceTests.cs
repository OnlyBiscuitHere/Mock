using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using NorthwindData;
using NorthwindData.Services;

namespace NorthwindTests
{
    public class CustomerServiceTests
    {
        private CustomerService _sut;
        private NorthwindContext _context;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var options = new DbContextOptionsBuilder<NorthwindContext>().UseInMemoryDatabase(databaseName:"Example_DB").Options;
            _context = new NorthwindContext(options);
            _sut = new CustomerService(_context);

            // Seed the database
            _sut.CreateCustomer(new Customer { CustomerId = "MAND", ContactName = "Nish Mandal", CompanyName = "SpartaGlobal", City = "Paris" });
            _sut.CreateCustomer(new Customer { CustomerId = "FREN", ContactName = "Cathy French", CompanyName = "SpartaGlobal", City = "Ottawa" });
        }
        [Test]
        public void GivenAValidId_CorrectCustomerIsReturned()
        {
            var result = _sut.GetCustomerById("MAND");
            Assert.That(result, Is.TypeOf<Customer>());
            Assert.That(result.ContactName, Is.EqualTo("Nish Mandal"));
            Assert.That(result.City, Is.EqualTo("Paris"));
            Assert.That(result.CompanyName, Is.EqualTo("SpartaGlobal"));
        }
        [Test]
        public void GivenANewCustomer_CreateCustomerAddsItToTheDatabase()
        {
            // Arrange
            var numberOfCustomersBefore = _context.Customers.Count();
            var newCustomer = new Customer { CustomerId = "BEAR", ContactName = "Martin Beard", CompanyName = "SpartaGlobal", City="Rome" };
            // Act
            _sut.CreateCustomer(newCustomer);
            var numberOfCustomersAfter = _context.Customers.Count();
            var result = _sut.GetCustomerById("BEAR");
            // Assert
            Assert.That(numberOfCustomersBefore + 1, Is.EqualTo(numberOfCustomersAfter));
            Assert.That(result.ContactName, Is.EqualTo("Martin Beard"));
            Assert.That(result.City, Is.EqualTo("Rome"));
            Assert.That(result.CompanyName, Is.EqualTo("SpartaGlobal"));
            // Clean up
            _context.Customers.Remove(newCustomer);
            _context.SaveChanges();
        }
        [Test]
        public void GetCustomerList_ReturnsCorrectCustomerList()
        {
            var expected = _sut.GetCustomerList();
            var actual = _context.Customers.ToList();
            Assert.That(actual, Is.EqualTo(expected));
            Assert.That(actual.Count, Is.EqualTo(expected.Count));
        }
        [Test]
        public void GivenACustomer_CustomerIsRemovedFromTheDatabase()
        {
            var newCustomer = new Customer { CustomerId = "BEAR", ContactName = "Martin Beard", CompanyName = "SpartaGlobal", City = "Rome" };
            _sut.CreateCustomer(newCustomer);
            var numberOfCustomerBefore = _context.Customers.Count();
            _sut.RemoveCustomer(newCustomer);
            var numberOfCustomerAfter = _context.Customers.Count();
            Assert.That(numberOfCustomerBefore - 1, Is.EqualTo(numberOfCustomerAfter));
        }
    }
}
