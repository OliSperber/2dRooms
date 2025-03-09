using Moq;
using System;
using System.Threading.Tasks;
using _2dRooms.Models;
using _2dRooms.Repositories;
using _2dRooms.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using Dapper;

namespace _2dRooms.Tests.Tests.Controllers
{
    [TestClass]
    public class EnvironmentRepositoryTest
    {
        private Environment2D environment = new()
        {
            Id = "FakeId",
            Name = "FakeName",
            MaxHeight = 50,
            MaxWidth = 50,
            UserId = "test"
        };

        private Environment2DRepository repository = new(new ConnectionStringService("FakeConnectionString"));

        [TestMethod]
        [DataRow(10, 50)] // lowest possible height
        [DataRow(50, 20)] // lowest possible width
        [DataRow(100, 50)] // highest possible height
        [DataRow(50, 200)] // highest possible width
        [DataRow(50, 50)] // normal possible input
        public void NoExceptionIf_HeightIs10To100_WidthIs20To200(float height, float width)
        {
            // Arrange
            var environment = this.environment;
            environment.MaxHeight = height;
            environment.MaxWidth = width;

            // Act
            try
            {
                repository.ValidateNameAndSize(environment);
            }
            catch (Exception ex)
            {
                // If an exception is thrown, fail the test
                Assert.Fail($"Expected no exception, but one was thrown.\n{ex.Message}");
            }

            // Assert: If no exception was thrown, the test passes
        }

        [TestMethod]
        public void ThrowExceptionIf_NameIsTooLong()
        {
            // Arrange
            var environment = this.environment;
            environment.Name = new string('A', 26);  // Name too long

            // Act & Assert
            try
            {
                repository.ValidateNameAndSize(environment);
                Assert.Fail("Expected ArgumentException was not thrown.");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("The environment name must be between 1 and 25 characters.", ex.Message);
            }
        }

        [TestMethod]
        [DataRow(1)] // Min length
        [DataRow(25)] // Max length
        [DataRow(10)] // Normal length
        public void NoExceptionExceptionIf_NameIsBetween1And25Length(int nameLength)
        {
            // Arrange
            var environment = this.environment;
            environment.Name = new string('A', nameLength); // Name length

            // Act & Assert
            try
            {
                repository.ValidateNameAndSize(environment);
            }
            catch (ArgumentException ex)
            {
                Assert.Fail($"Expected no exception, but one was thrown.\n{ex.Message}");
            }
        }
    }
}
