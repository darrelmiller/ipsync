# SyncIP

This is a simple command line utility to detect the public IP address of a computer and then update a Azure DNS Zone with that IP address.

The IP sync part of this utility is not very interesting. What is a bit more interesting is the fact that it uses MSAL.NET and therefore the Azure AD V2 authentication endpoint to access an Azure ARM API.

Most examples of ARM authentication still use ADAL and the Azure AD V1 endpoint. The examples that use the Azure AD v2 endpoint focus on accessing Microsoft Graph or a custom API.

This example assumes that it will be deployed on a secured server and therefore uses Client Credentials flow with a client secret.  There are several key steps when enabling this scenario:

- Create an application registration using the latest App Registration portal
- Setup a client secret
- Assign the DNS Zone Contributor role to the Service Principal that belongs to the application registration just created in the Subscription blade.
- Fill in the appsettings.json file


