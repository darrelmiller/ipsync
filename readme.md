# SyncIP

This is a simple command line utility to detect the public IP address of a computer and then update a Azure DNS Zone with that IP address.

The IP sync part of this utility is not very interesting. What is a bit more interesting is the fact that it uses MSAL.NET and therefore the Azure AD V2 authentication endpoint to access an Azure ARM API.

Most examples of ARM authentication still use ADAL and the Azure AD V1 endpoint. The examples that use the Azure AD v2 endpoint focus on accessing Microsoft Graph or a custom API.

This example assumes that it will be deployed on a secured server and therefore uses Client Credentials flow with a client secret.  There are several key steps when enabling this scenario:

- Create an application registration using the latest App Registration portal
- Setup a client secret
- Assign the DNS Zone Contributor role to the Service Principal that belongs to the application registration just created in the Subscription blade.
- Fill in the appsettings.json file

In order to make token acquisition transparent, I pulled in the Microsoft.Graph.Core library and used the GraphClientFactory to create an instance of HttpClient with an authentication provider wired up.  Ideally, I would have used the ClientCredentialProvider in Microsoft.Graph.Auth but sadly that class doesn't currently allow changing the default scope, which for ARM need to be https://management.azure.com/.default  

As an interim workaround, I created a custom IAuthenticationProvider called ArmAuthenticationProvider.  This doesn't do all the error handling the ClientCredentialProvider does, so I will update as soon as I can.
