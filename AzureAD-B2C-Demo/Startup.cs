using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace AzureAD_B2C_Demo
{
    public class Startup
    {
        private readonly string _signUpPolicyName;
        private readonly string _signInPolicyName;
        private readonly string _signUpSignInPolicyName;
        private readonly string _profileEditingPolicyName;
        private readonly string _tenant;
        private readonly string _applicationId;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            _signUpPolicyName = Configuration["Authentication:AzureAdB2C:SignUpPolicyName"]; // B2C_1_sign_up
            _signInPolicyName = Configuration["Authentication:AzureAdB2C:SignInPolicyName"]; // B2C_1_sign_in
            _signUpSignInPolicyName = Configuration["Authentication:AzureAdB2C:SignUpSignInPolicyName"]; // B2C_1_sign_up_in
            _profileEditingPolicyName = Configuration["Authentication:AzureAdB2C:ProfileEditingPolicyName"]; // B2C_1_edit_profile
            _tenant = Configuration["Authentication:AzureAdB2C:Tenant"];
            _applicationId = Configuration["Authentication:AzureAdB2C:ApplicationId"];
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options => { options.Filters.Add(new RequireHttpsAttribute()); });
            services.AddSingleton<IConfiguration>(Configuration);

            services.AddAuthentication(options =>
            {
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = _signUpSignInPolicyName; // "B2C_1_sign_up_in";
            })
                    .AddOpenIdConnect(_signUpPolicyName, // which also happens to be the name of my policy in Azure AD B2C.
                        options =>
                        {
                            SetOptionsForOpenIdConnectPolicy(_signUpPolicyName, options); // "B2C_1_sign_up"
                        })
                    .AddOpenIdConnect(_signInPolicyName,
                        options =>
                        {
                            SetOptionsForOpenIdConnectPolicy(_signInPolicyName, options); // "B2C_1_sign_in"
                            options.Events = new OpenIdConnectEvents
                            {
                                OnMessageReceived = context =>
                                {
                                    if(!string.IsNullOrEmpty(context.ProtocolMessage.Error) && !string.IsNullOrEmpty(context.ProtocolMessage.ErrorDescription))
                                    {
                                        if(context.ProtocolMessage.ErrorDescription.StartsWith("AADB2C99002")) // User does not exist. Please sign up before you can sign in.
                                        {
                                            context.HandleResponse();
                                            context.Response.Redirect("/auth/signup");
                                        }
                                    }

                                    return Task.FromResult(0);
                                }
                            };
                        })
                    .AddOpenIdConnect(_signUpSignInPolicyName,
                        options => SetOptionsForOpenIdConnectPolicy(_signUpSignInPolicyName, options)) // "B2C_1_sign_up_in"
                    .AddOpenIdConnect(_profileEditingPolicyName,
                        options =>
                        {
                            SetOptionsForOpenIdConnectPolicy(_profileEditingPolicyName, options); // "B2C_1_edit_profile"
                            options.Events = new OpenIdConnectEvents
                            {
                                OnMessageReceived = context =>
                                {
                                    if(!string.IsNullOrEmpty(context.ProtocolMessage.Error) && !string.IsNullOrEmpty(context.ProtocolMessage.ErrorDescription))
                                    {
                                        if(context.ProtocolMessage.ErrorDescription.StartsWith("AADB2C90091")) // cancel profile editing
                                        {
                                            context.HandleResponse();
                                            context.Response.Redirect("/");
                                        }
                                        else if(context.ProtocolMessage.ErrorDescription.StartsWith("AADB2C90118")) // forgot password
                                        {
                                            context.HandleResponse();
                                            context.Response.Redirect("/Account/ResetPassword");
                                        }
                                    }

                                    return Task.FromResult(0);
                                }
                            };
                        })
                    .AddCookie();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        public void SetOptionsForOpenIdConnectPolicy(string policy, OpenIdConnectOptions options)
        {
            options.MetadataAddress = $"https://login.microsoftonline.com/{_tenant}/v2.0/.well-known/openid-configuration?p={policy}";
            options.ClientId = _applicationId; // Azure AD B2C application ID."### ADD APPLICATION ID HERE ###";
            options.ResponseType = OpenIdConnectResponseType.IdToken;
            options.CallbackPath = "/signin/" + policy; // These have to be set in the "Reply URL" of the Azure application in this B2C tanent. 
            options.SignedOutCallbackPath = "/signout/" + policy;
            options.SignedOutRedirectUri = "/";
            options.TokenValidationParameters.NameClaimType = "name";
        }
    }
}
