using System;
using System.Threading.Tasks;
using SlothEnterprise.External;
using SlothEnterprise.External.V1;
using SlothEnterprise.ProductApplication.Applications;
using SlothEnterprise.ProductApplication.Products;

namespace SlothEnterprise.ProductApplication
{
    public class ProductApplicationService
    {
        private readonly ISelectInvoiceService _selectInvoiceService;
        private readonly IConfidentialInvoiceService _confidentialInvoiceWebService;
        private readonly IBusinessLoansService _businessLoansService;

        public ProductApplicationService(ISelectInvoiceService selectInvoiceService, IConfidentialInvoiceService confidentialInvoiceWebService, IBusinessLoansService businessLoansService)
        {
            _selectInvoiceService = selectInvoiceService;
            _confidentialInvoiceWebService = confidentialInvoiceWebService;
            _businessLoansService = businessLoansService;
        }

        public int SubmitApplicationFor(ISellerApplication application)
        {
            //2.	No null checks for application and the product/company data within being null. 
            if (application == null || application.Product == null || application.CompanyData == null)
                throw new ArgumentNullException();

            try
            {
                var applicationSuccess = 0;
                if (application.Product is SelectiveInvoiceDiscount sid)
                {
                    applicationSuccess = _selectInvoiceService.SubmitApplicationFor(application.CompanyData.Number.ToString(), sid.InvoiceAmount, sid.AdvancePercentage);
                    //3. disposing objects when done using them
                    sid = null;
                    return applicationSuccess;
                }

                if (application.Product is ConfidentialInvoiceDiscount cid)
                {
                    var result = _confidentialInvoiceWebService.SubmitApplicationFor(
                         new CompanyDataRequest
                         {
                             CompanyFounded = application.CompanyData.Founded,
                             CompanyNumber = application.CompanyData.Number,
                             CompanyName = application.CompanyData.Name,
                             DirectorName = application.CompanyData.DirectorName
                         }, cid.TotalLedgerNetworth, cid.AdvancePercentage, cid.VatRate);

                    //4.	The outcome from the services IApplicationResult should be checked for Null before accessing any properties.
                    applicationSuccess = result != null ? (result.Success) ? result.ApplicationId ?? -1 : -1 : -1;
                    //3. disposing objects when done using them
                    cid = null;
                    return applicationSuccess;
                }

                if (application.Product is BusinessLoans loans)
                {
                    var result = _businessLoansService.SubmitApplicationFor(new CompanyDataRequest
                    {
                        CompanyFounded = application.CompanyData.Founded,
                        CompanyNumber = application.CompanyData.Number,
                        CompanyName = application.CompanyData.Name,
                        DirectorName = application.CompanyData.DirectorName
                    }, new LoansRequest
                    {
                        InterestRatePerAnnum = loans.InterestRatePerAnnum,
                        LoanAmount = loans.LoanAmount
                    });

                    //4.	The outcome from the services IApplicationResult should be checked for Null before accessing any properties.
                    applicationSuccess = result != null ? (result.Success) ? result.ApplicationId ?? -1 : -1 : -1;
                    //3. disposing objects when done using them
                    loans = null;
                    return applicationSuccess;
                }
            }
            catch
            {
                //1.	exception handling to gracefully handle any errors from the services being called.
                //2.    also takes care of the scenario where the service variable is null
                throw new Exception("Error while submitting the application");
            }

            throw new InvalidOperationException();
        }
    }
}