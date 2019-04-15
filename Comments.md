1.	There is no exception handling to gracefully handle any errors from the services being called. It needs try-catch block to handle any exceptions and throw 
	appropriate exception or error to the calling services.

2.	There are no null checks for the application and the product/company data within being null. 

3.	The variable declared as part of the If condition are not disposed after their use. -> (if (application.Product is SelectiveInvoiceDiscount sid))

4.	The outcome from the services IApplicationResult should be checked for Null before accessing any properties.

5.	Any new service introduced in the future will mean ProductApplicationService will need to be modified again so in essence it's not following the open-closed principle.
	however due to the nature of each service requiring differnt parameters, at present this issue cannot be fixed. Ideally, each service should be refactored to accept
	parameters of the same type (ISellerApplication) and extract the relevant properties each service needs out of this rather than ProductApplicationService determining 
	it. Each service should have a base interface to implement with the same parameter for SubmitApplicationFor method. ProductApplicationService can then just call the 
	method without worrying about which service to call as the incoming service will know its concrete type.