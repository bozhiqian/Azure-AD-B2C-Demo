# Azure AD B2C Demo ASP.NET Core 2.0 Web App

This Sample ASP.NET Core 2.0 Web app demonstrate to authenticate users using Azure AD B2C with default tenant domain account or other social identities. 

### Steps to implementation:
1. > **Azure Portal**: Create Active Directory B2C tenant
2. > **Azure Portal**: Add new application in AD
3. > **Azure Portal**: Add policiy to "Sign-up or sign-in policies", "Profile editing policies", "Sign-up policies", "Sign-in policies". 
4. > **Azure Portal**: Set reply url for each policy to handle OIDC redirects
5. > **Azure Portal**: Add identity provider such as Google, Microsoft, Facebook and Twitter.
6. > **ASP.NET Core web app**: Add Cookie middleware
7. > **ASP.NET Core web app**: Implement Sign out end point
8. > **ASP.NET Core web app**: Add user profile editing. 


### References
* **[Create an Azure Active Directory B2C tenant in the Azure portal](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-get-started)**

* **[Azure Active Directory B2C: Register your application](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-app-registration)**
* **[Azure Active Directory B2C: Provide sign-up and sign-in to consumers with Google+ accounts](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-setup-goog-app)** 
* **[Azure Active Directory B2C: Provide sign-up and sign-in to consumers with Microsoft accounts](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-setup-msa-app)** 
* **[Azure Active Directory B2C: Provide sign-up and sign-in to consumers with Twitter accounts](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-setup-twitter-app)** 
* **[Azure Active Directory B2C: Provide sign-up and sign-in to consumers with Facebook accounts](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-setup-fb-app)** 
* **[ASP.NET Core 2 Authentication Playbook](https://app.pluralsight.com/library/courses/aspnet-core-identity-management-playbook/table-of-contents)** 