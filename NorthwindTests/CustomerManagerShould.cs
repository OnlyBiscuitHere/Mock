using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using NorthwindData.Services;
using NorthwindData;
using NorthwindBusiness;
using Microsoft.EntityFrameworkCore;

namespace NorthwindTests
{
    public class CustomerManagerShould
    {
        //private CustomerManager sut;
        [Test]
        public void BeAbleToConstructed()
        {
            // Arrange
            var mockCustomerService = new Mock<ICustomerService>();
            // Act
            var sut = new CustomerManager(mockCustomerService.Object);
            // Assert
            Assert.That(sut, Is.InstanceOf<CustomerManager>());
        }
        [Test]
        public void ReturnTrue_WhenUpdateIsCalled_WithValidId()
        {
            var mockCustomerService = new Mock<ICustomerService>();
            var sut = new CustomerManager(mockCustomerService.Object);
            var originalCustomer = new Customer()
            {
                CustomerId = "ROCK",
            };
            mockCustomerService.Setup(cs => cs.GetCustomerById("ROCK")).Returns(originalCustomer);
            sut = new CustomerManager(mockCustomerService.Object);
            var result = sut.Update("ROCK", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            Assert.That(result);
        }
        [Test]
        public void UpdateSelectedCustomer_WhenUpdateIsCalled_WithValidId()
        {
            var mockCustomerService = new Mock<ICustomerService>();
            var sut = new CustomerManager(mockCustomerService.Object);
            var originalCustomer = new Customer()
            {
                CustomerId = "ROCK",
                ContactName = "Rocky Raccoon",
                CompanyName = "Zoo UK",
                City = "Telford"
            };
            mockCustomerService.Setup(cs => cs.GetCustomerById("ROCK")).Returns(originalCustomer);
            sut = new CustomerManager(mockCustomerService.Object);
            var result = sut.Update("ROCK", "Rocky Raccoon", "UK", "Chester", null);
            Assert.That(sut.SelectedCustomer.ContactName, Is.EqualTo("Rocky Raccoon"));
            Assert.That(sut.SelectedCustomer.CompanyName, Is.EqualTo("Zoo UK"));
            Assert.That(sut.SelectedCustomer.Country, Is.EqualTo("UK"));
            Assert.That(sut.SelectedCustomer.City, Is.EqualTo("Chester"));
        }
        [Test]
        public void ReturnsFalse_WhenUpdateIsCalled_WithValidId()
        {
            var mockCustomerService = new Mock<ICustomerService>();
            var sut = new CustomerManager(mockCustomerService.Object);
            mockCustomerService.Setup(cs => cs.GetCustomerById("ROCK")).Returns((Customer)null);
            sut = new CustomerManager(mockCustomerService.Object);
            var result = sut.Update("ROCK", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            Assert.That(result, Is.False);
        }
        [Test]
        public void DoesNotUpdateSelectedCustomer_WhenUpdateIsCalled_WithInvalidId()
        {
            var mockCustomerService = new Mock<ICustomerService>();
            var sut = new CustomerManager(mockCustomerService.Object);
            var originalCustomer = new Customer()
            {
                CustomerId = "ROCK",
                ContactName = "Rocky Raccoon",
                CompanyName = "Zoo UK",
                City = "Telford"
            };
            mockCustomerService.Setup(cs => cs.GetCustomerById("ROCK")).Returns((Customer)null);
            sut = new CustomerManager(mockCustomerService.Object);
            sut.SelectedCustomer = originalCustomer;
            var result = sut.Update("ROCK", "Rocky Raccoon", "UK", "Chester", null);
            Assert.That(sut.SelectedCustomer.ContactName, Is.EqualTo("Rocky Raccoon"));
            Assert.That(sut.SelectedCustomer.CompanyName, Is.EqualTo("Zoo UK"));
            Assert.That(sut.SelectedCustomer.Country, Is.Null);
            Assert.That(sut.SelectedCustomer.City, Is.EqualTo("Telford"));
        }
        // Happy Path
        [Test]
        public void DeletingAUser_RemovesUser_WhenThereIsAValidId()
        {
            var mockCustomerService = new Mock<ICustomerService>();
            var sut = new CustomerManager(mockCustomerService.Object);
            var newCustomer = new Customer() { CustomerId = "ROCK" };
            mockCustomerService.Setup(cs => cs.GetCustomerById("ROCK")).Returns(newCustomer);
            sut = new CustomerManager(mockCustomerService.Object);
            var result = sut.Delete(newCustomer.CustomerId);
            Assert.That(result);
        }
        // Sad Path
        [Test]
        public void DeletingAUser_RemovesUser_WhenThereIsAnInvalidId()
        {
            var mockCustomerService = new Mock<ICustomerService>();
            var sut = new CustomerManager(mockCustomerService.Object);
            mockCustomerService.Setup(cs => cs.GetCustomerById("ROCK")).Returns((Customer)null);
            sut = new CustomerManager(mockCustomerService.Object);
            var result = sut.Delete("ROCK");
            Assert.That(result, Is.False);
        }
        [Test]
        public void ReturnFalse_WhenUpdateIsCalled_AndADatabaseExceptionIsThrown()
        {
            var mockCustomerService = new Mock<ICustomerService>();
            var sut = new CustomerManager(mockCustomerService.Object);
            var originalCustomer = new Customer();
            mockCustomerService.Setup(cs => cs.GetCustomerById(It.IsAny<string>())).Returns(originalCustomer);
            mockCustomerService.Setup(cs => cs.SaveCustomerChanges()).Throws<DbUpdateConcurrencyException>();
            sut = new CustomerManager(mockCustomerService.Object);

            var result = sut.Update("ROCK", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            Assert.That(result, Is.False);
        }
        [Test]
        public void NotChangeTheSelectedCustomer_WhenUpdateIsCalled_AndADatabaseExceptionIsThrown()
        {
            var mockCustomerService = new Mock<ICustomerService>();
            var sut = new CustomerManager(mockCustomerService.Object);
            var originalCustomer = new Customer()
            {
                CustomerId = "ROCK",
                ContactName = "Rocky Raccoon",
                CompanyName = "Zoo UK",
                City = "Telford"
            };
            mockCustomerService.Setup(cs => cs.GetCustomerById("ROCK")).Returns(originalCustomer);
            mockCustomerService.Setup(cs => cs.SaveCustomerChanges()).Throws<DbUpdateConcurrencyException>();

            sut = new CustomerManager(mockCustomerService.Object);
            sut.SelectedCustomer = new Customer()
            {
                CustomerId = "ROCK",
                ContactName = "Rocky Raccoon",
                CompanyName = "Zoo UK",
                City = "Telford"
            }; ;

            var result = sut.Update("ROCK", "Rocky Raccoon", "UK", "Chester", null);
            Assert.That(sut.SelectedCustomer.ContactName, Is.EqualTo("Rocky Raccoon"));
            Assert.That(sut.SelectedCustomer.CompanyName, Is.EqualTo("Zoo UK"));
            Assert.That(sut.SelectedCustomer.Country, Is.Null);
            Assert.That(sut.SelectedCustomer.City, Is.EqualTo("Telford"));
        }
        [Test]
        public void CallSaveCustomerChanges_WhenUpdateIsCalled_WithValidId()
        {
            var mockCustomerService = new Mock<ICustomerService>();
            mockCustomerService.Setup(cs => cs.GetCustomerById("ROCK")).Returns(new Customer());
            var sut = new CustomerManager(mockCustomerService.Object);
            var result = sut.Update("ROCK", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            mockCustomerService.Verify(cs => cs.SaveCustomerChanges(), Times.Once);
        }
        [Test]
        public void LetsSeeWhatHappens_WhenUpdateIsCalled_AndAllInvocationsArentSetup()
        {
            var mockCustomerService = new Mock<ICustomerService>(MockBehavior.Strict);
            mockCustomerService.Setup(cs => cs.GetCustomerById("ROCK")).Returns(new Customer());
            mockCustomerService.Setup(cs => cs.SaveCustomerChanges());
            var sut = new CustomerManager(mockCustomerService.Object);
            var result = sut.Update("ROCK", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            mockCustomerService.Verify(cs => cs.SaveCustomerChanges(), Times.Once);

            Assert.That(result);
        }
    }

}
