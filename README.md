# BankID-Implementation
BankID API implementation and live demos written in C# using .NET Framework.

[BankID](https://www.bankid.com/) is a citizen identification solution that allows companies, banks and government agencies to authenticate and conclude agreements with individuals over the Internet. BankID is the most used digital identification solution in Sweden.

The class library (BankID.Client) contains the client class (BankIDClient), which includes all BankID service endpoints, for authenticating, signing, collecting status/information of an identification attempt, and to cancel an active identification attempt. The client class also contains helper-methods to obtain relevant user messages, which are displayed to the user during identification attempts.

This solution includes two demo projects which interacts with the BankID service through the BankID client class. The BankID certificates has to be installed in order to run these demo projects. Install the RP and server certificate to your local key store, and configure the certificate thumbprint to the project App/Web.config.

### BankID.WebDemo
This web demo demonstrates how the BankID client class can be used to manage the mobile BankID authentication flow through a web-app or website. The demo functions as a basic Single Page Application (SPA) - the frontend app communicates with the backend API using JavaScript & jQuery, and uses response values to modify the visual view. The frontend includes mobile app-launch support for an even more seamless user experience (although note that app-launch support is untested for this specific demo). The backend runs on MVC & Web API.

### BankID.ConsoleDemo
This console application interacts directly with the BankID client class to demonstrate the mobile BankID authentication and signing flow, in a GUI-like manner.

### Notes
- Only Swedish user messages are supported
- The demo projects do not include .NET authentication/authorization