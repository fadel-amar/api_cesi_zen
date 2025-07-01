using Xunit;
using Moq;
using System.ComponentModel.DataAnnotations;
using CesiZen_API.Helper.Atttributes;
using CesiZen_API.Services.Interfaces;
using CesiZen_API.Models;
using System;

namespace CesiZen_API.Tests.Helper.Attributes
{
    public class UniqueIdentifierAttributeTests
    {
        private class DummyModel
        {
            [UniqueIdentifier(ErrorMessage = "Identifiant déjà pris.")]
            public string Identifier { get; set; } = string.Empty;
        }

        [Fact]
        public void IsValid_Should_Return_Success_If_Identifier_Is_Null_Or_Empty()
        {
            var attribute = new UniqueIdentifierAttribute();
            var context = new ValidationContext(new object());

            Assert.Equal(ValidationResult.Success, attribute.GetValidationResult(null, context));
            Assert.Equal(ValidationResult.Success, attribute.GetValidationResult("", context));
            Assert.Equal(ValidationResult.Success, attribute.GetValidationResult("   ", context));
        }

        [Fact]
        public void IsValid_Should_Return_Success_If_Identifier_Not_Found()
        {
            var identifier = "uniqueUser";
            var mockService = new Mock<IUserService>();
            mockService.Setup(s => s.GetUserByIdentifier(identifier))
                       .ReturnsAsync((User?)null);

            var attribute = new UniqueIdentifierAttribute();

            var validationContext = new ValidationContext(new DummyModel { Identifier = identifier });
            validationContext.InitializeServiceProvider(serviceType =>
            {
                return serviceType == typeof(IUserService) ? mockService.Object : null;
            });

            var result = attribute.GetValidationResult(identifier, validationContext);

            Assert.Equal(ValidationResult.Success, result);
        }

        [Fact]
        public void IsValid_Should_Return_Error_If_Identifier_Exists()
        {
            var identifier = "existingUser";
            var mockService = new Mock<IUserService>();
            mockService.Setup(s => s.GetUserByIdentifier(identifier))
                       .ReturnsAsync(new User { Id = 1, Login = identifier });

            var attribute = new UniqueIdentifierAttribute
            {
                ErrorMessage = "Identifiant déjà utilisé."
            };

            var validationContext = new ValidationContext(new DummyModel { Identifier = identifier });
            validationContext.InitializeServiceProvider(serviceType =>
            {
                return serviceType == typeof(IUserService) ? mockService.Object : null;
            });

            var result = attribute.GetValidationResult(identifier, validationContext);

            Assert.NotNull(result);
            Assert.Equal("Identifiant déjà utilisé.", result?.ErrorMessage);
        }
    }
}
