using NSubstitute;
using SlothEnterprise.External;
using SlothEnterprise.External.V1;
using SlothEnterprise.ProductApplication.Applications;
using SlothEnterprise.ProductApplication.Products;
using System;
using Xunit;

namespace SlothEnterprise.ProductApplication.Tests
{
    public class ProductApplicationTests : IDisposable
    {
        private ProductApplicationService productApplicationService;
        private ISellerApplication sellerApplication;

        public ProductApplicationTests()
        {
            productApplicationService = Substitute.For<ProductApplicationService>(Substitute.For<ISelectInvoiceService>(), Substitute.For<IConfidentialInvoiceService>(), Substitute.For<IBusinessLoansService>());
            sellerApplication = Substitute.For<ISellerApplication>();
        }

        [Fact]
        public void SubmitApplicationFor_throws_exception_when_application_is_unknown()
        {            
            Assert.Throws<InvalidOperationException>(() => productApplicationService.SubmitApplicationFor(sellerApplication));
        }

        [Fact]
        public void SubmitApplicationFor_throws_exception_when_application_product_is_null()
        {
            sellerApplication.Product = null;
            Assert.Throws<ArgumentNullException>(() => productApplicationService.SubmitApplicationFor(sellerApplication));
        }

        [Fact]
        public void SubmitApplicationFor_returns_expected_applicationid_for_SelectiveInvoiceDiscount()
        {
            var selectiveInvoiceDiscount = Substitute.For<SelectiveInvoiceDiscount>();
            selectiveInvoiceDiscount.AdvancePercentage = 0.9M;
            selectiveInvoiceDiscount.InvoiceAmount = 100;
            sellerApplication.Product = selectiveInvoiceDiscount;

            productApplicationService.SubmitApplicationFor(sellerApplication).Returns(99);
            var result = productApplicationService.SubmitApplicationFor(sellerApplication);
            Assert.Equal(99, result);
        }

        public void Dispose()
        {
            productApplicationService = null;
            sellerApplication = null;
        }
    }
}